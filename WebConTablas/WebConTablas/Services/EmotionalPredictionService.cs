using Microsoft.ML;
using WebConTablas.ML;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ML.Data; 
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using System; 
using WebConTablas.Models; // Necesario para AppDbContext y DiarioEmocional

namespace WebConTablas.Services
{
    // Las clases AppDbContext y DiarioEmocional se simulan para que el código compile y sea funcionalmente correcto.
    // En una aplicación real, estas clases deben existir en WebConTablas.Models.

    // ----------------------------------------------------------------------------------
    // SIMULACIÓN DE CLASES DEL MODELO (para propósitos de compilación y contexto)
    // ----------------------------------------------------------------------------------
    namespace WebConTablas.Models
    {
        // Esta clase debe ser una entidad de EF Core
        public class DiarioEmocional
        {
            public int ID_Diario { get; set; }
            public int ID_Paciente { get; set; }
            public DateTime Fecha { get; set; }
            public string Emociones { get; set; } // e.g., "5,5,5,5,5"
            public string Descripcion { get; set; } // e.g., "Zona1 / Zona2 / Zona3"
            public int? Pasos { get; set; }
            public int? Horas_celular { get; set; }
            public int? Horas_redes { get; set; }
            public float? Hora_dormida { get; set; }
            public string Estado { get; set; } // La etiqueta, e.g., "basal"
            public float? HR_RitmoCardiaco { get; set; }
            public float? HRV_VariabilidadFrecuencia { get; set; }
            public float? Coherencia { get; set; }
            public float? Errores_gramaticales { get; set; }
        }

        // Esta clase debe ser el contexto de la base de datos
        public class AppDbContext : IDisposable
        {
            // En una aplicación real, esto sería una propiedad DbSet<DiarioEmocional>
            public List<DiarioEmocional> DiariosEmocionales { get; set; } 

            public AppDbContext()
            {
                // Inicialización simulada de datos para pruebas
                DiariosEmocionales = new List<DiarioEmocional>();
                // Simular un historial de datos personales para el entrenamiento
                for (int i = 0; i < 50; i++)
                {
                    DiariosEmocionales.Add(new DiarioEmocional
                    {
                        ID_Paciente = 1,
                        Estado = i % 3 == 0 ? "inhibido" : (i % 3 == 1 ? "basal" : "exaltado"),
                        Emociones = i % 3 == 0 ? "1,1,1,1,1" : (i % 3 == 1 ? "5,5,5,5,5" : "9,9,9,9,9"),
                        Descripcion = i % 3 == 0 ? "Cama, sin energía / Solo, evito llamadas / Tristeza, vacío" : (i % 3 == 1 ? "Trabajo, caminata / Familia, amigos / Calma, ideas" : "Proyecto, éxito / Fiesta, mucha gente / Euforia, metas"),
                        HR_RitmoCardiaco = i % 3 == 0 ? 55f : (i % 3 == 1 ? 70f : 100f),
                        HRV_VariabilidadFrecuencia = i % 3 == 0 ? 15f : (i % 3 == 1 ? 30f : 50f),
                        Coherencia = i % 3 == 0 ? 0.6f : (i % 3 == 1 ? 0.9f : 0.8f),
                        Errores_gramaticales = i % 3 == 0 ? 2.5f : (i % 3 == 1 ? 0.5f : 0.2f),
                        // Más campos ...
                    });
                }
            }

            public void Dispose() { /* Implementación real de Dispose en EF Core */ }
        }
    }
    
    // ----------------------------------------------------------------------------------
    // CLASE PRINCIPAL: EmotionalPredictionService
    // ----------------------------------------------------------------------------------
    
    public class EmotionalPredictionService
    {
        private readonly MLContext _mlContext;
        private ITransformer _trainedModel; // Cambiado a campo de instancia

        // Se mantienen estáticos: GeneralData (simulados) y la lógica de featurización.
        private static List<EmotionalStateData> GeneralData = LoadGeneralData();
        
        // Se cambia a campo de instancia, se cargará en el constructor.
        private List<EmotionalStateData> PersonalizedData; 

        private readonly AppDbContext _context; // 🎯 Añadido para inyección de dependencias 🎯
        
        // Definiciones de parámetros de adaptación
        private const int X_SEMANAS_INICIALES = 3;
        private const int SEMANAS_DE_TRANSICION = 3; 

        // 🎯 Nueva constante: Fecha de inicio manual. Se establece 25 días atrás.
        private static readonly DateTime START_DATE = DateTime.Now.AddDays(-25);

        // 🎯 Constructor modificado para recibir AppDbContext 🎯
        public EmotionalPredictionService(AppDbContext context)
        {
            _mlContext = new MLContext(seed: 0);
            _context = context; 
            
            // Llamar al método de carga y entrenamiento después de la inyección
            LoadAndTrainModel();
        }

        // Nuevo método de instancia para la carga y entrenamiento
        private void LoadAndTrainModel()
        {
            // 1. Cargar los datos personalizados de la BD
            PersonalizedData = LoadPersonalizedData();
            
            TimeSpan timeSinceStart = DateTime.Now - START_DATE;

            // La semana 1 es la primera semana. Si han pasado 0-6 días, es la semana 1.
            // Math.Ceiling(días / 7.0) nos da la semana actual.
            int currentWeek = (int)Math.Floor(timeSinceStart.TotalDays / 7.0) + 1;

            // Asegurar que la semana sea al menos 1
            if (currentWeek < 1) currentWeek = 1; 

            Console.WriteLine($"✅ Cálculo de Semana: Fecha Inicio (Manual) = {START_DATE:yyyy-MM-dd}. Semana Actual = {currentWeek}");
            // --------------------------------------------------------
            
            // 3. Entrenar el modelo
            float personalizedWeight = GetPersonalizedWeight(currentWeek);
            var currentTrainingData = LoadTrainingDataForCurrentWeek(personalizedWeight);
            var trainingDataView = _mlContext.Data.LoadFromEnumerable(currentTrainingData);

            var pipeline = BuildTrainingPipeline(_mlContext);
            _trainedModel = pipeline.Fit(trainingDataView);
        }

        public string PredictState(EmotionalStateData data)
        {
            var predictor = _mlContext.Model.CreatePredictionEngine<EmotionalStateData, EmotionalStatePrediction>(_trainedModel);
            var prediction = predictor.Predict(data);
            return prediction.Prediction;
        }

        // --- MÉTODOS DE CÁLCULO DE PESO Y CARGA DE DATOS ---

        private float GetPersonalizedWeight(int currentWeek)
        {
            if (currentWeek <= X_SEMANAS_INICIALES) return 0f;
            if (currentWeek >= X_SEMANAS_INICIALES + SEMANAS_DE_TRANSICION) return 100f;
            int weeksIntoTransition = currentWeek - X_SEMANAS_INICIALES;
            return (float)weeksIntoTransition / SEMANAS_DE_TRANSICION * 100f;
        }
        
        private List<EmotionalStateData> LoadTrainingDataForCurrentWeek(float personalizedWeight)
        {
            var trainingData = new List<EmotionalStateData>();
            float generalWeight = 100f - personalizedWeight;
            
            // Log para mostrar qué datos se están utilizando
            Console.WriteLine("-----------------------------------------------------");
            Console.WriteLine($"⚙️ INICIO DEL ENTRENAMIENTO");
            Console.WriteLine($"Semana de Adaptación: {GetPersonalizedWeight((int)(X_SEMANAS_INICIALES + (personalizedWeight/100 * SEMANAS_DE_TRANSICION)))} (Simulado)");
            Console.WriteLine($"Peso General: {generalWeight:F2}% | Peso Personalizado: {personalizedWeight:F2}%");

            if (generalWeight > 0)
            {
                int numGeneralSamples = (int)System.Math.Round(GeneralData.Count * (generalWeight / 100.0));
                trainingData.AddRange(GeneralData.Take(numGeneralSamples));
                Console.WriteLine($"Se usaron {numGeneralSamples} muestras de datos GENERALES (Total: {GeneralData.Count}).");
            }
            if (personalizedWeight > 0)
            {
                trainingData.AddRange(PersonalizedData); // Usa la lista de instancia cargada
                Console.WriteLine($"Se usaron {PersonalizedData.Count} muestras de datos PERSONALIZADOS (Total: {PersonalizedData.Count}).");
            }
            
            Console.WriteLine($"TOTAL de muestras para entrenamiento: {trainingData.Count}");
            Console.WriteLine("-----------------------------------------------------");

            var random = new System.Random();
            return trainingData.OrderBy(x => random.Next()).ToList();
        }

        // ----------------------------------------------------------------------------------
        // --- PIPELINE DE ENTRENAMIENTO (Mantenido sin cambios) ---
        // ----------------------------------------------------------------------------------
        
        private static IEstimator<ITransformer> BuildTrainingPipeline(MLContext mlContext)
        {
            // --- PASO 1: PARSEAR LA CADENA CSV DE EMOCIONES (CustomMapping) ---
            Action<EmotionalStateData, FeatureBuffer> customAction = (input, output) =>
            {
                var values = input.Emociones.Split(',');
                if (values.Length == 5)
                {
                    output.Animo = float.Parse(values[0]);
                    output.Apetito = float.Parse(values[1]);
                    output.Energia = float.Parse(values[2]);
                    output.Sueno = float.Parse(values[3]);
                    output.BateriaSocial = float.Parse(values[4]);
                }
            };
            
            var customMapping = mlContext.Transforms.CustomMapping(customAction, contractName: "CsvParser");
            
            // --- PASO 2: PROCESAMIENTO DE TEXTO (FeaturizeText) ---
            var textFeaturizationPipeline = mlContext.Transforms.Text
                .FeaturizeText(outputColumnName: "Zone1_Features", inputColumnName: nameof(EmotionalStateData.Zona1_Actividades))
                .Append(mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Zone2_Features", inputColumnName: nameof(EmotionalStateData.Zona2_Personas)))
                .Append(mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Zone3_Features", inputColumnName: nameof(EmotionalStateData.Zona3_Pensamientos)));
            
            // Nombres de las características NUMÉRICAS y del CustomMapping
            var numericAndCustomFeatures = new[] { 
                nameof(EmotionalStateData.HR_RitmoCardiaco), 
                nameof(EmotionalStateData.HRV_VariabilidadFrecuencia),
                nameof(EmotionalStateData.PasosDiarios), 
                nameof(EmotionalStateData.Hora_dormida), 
                nameof(EmotionalStateData.Horas_celular), 
                nameof(EmotionalStateData.Horas_redes), 
                nameof(FeatureBuffer.Animo), nameof(FeatureBuffer.Apetito), 
                nameof(FeatureBuffer.Energia), nameof(FeatureBuffer.Sueno), 
                nameof(FeatureBuffer.BateriaSocial), 
                nameof(EmotionalStateData.Coherencia),
                nameof(EmotionalStateData.Errores_gramaticales) 
            };
            
            // Nombres de TODAS las características, incluyendo las generadas por FeaturizeText
            var allFeatureNames = numericAndCustomFeatures.Concat(new[] { 
                "Zone1_Features", "Zone2_Features", "Zone3_Features"
            }).ToArray();

            var dataProcessPipeline = mlContext.Transforms.Conversion.MapValueToKey(
                        inputColumnName: nameof(EmotionalStateData.EstadoEmocional),
                        outputColumnName: "Label")
                    .Append(customMapping) 
                    .Append(textFeaturizationPipeline) 
                    .Append(mlContext.Transforms.NormalizeMeanVariance(
                        numericAndCustomFeatures.Select(f => new InputOutputColumnPair(f, f)).ToArray()))
                    .Append(mlContext.Transforms.Concatenate("Features", allFeatureNames));

            var trainer = mlContext.MulticlassClassification.Trainers.LightGbm(
                labelColumnName: "Label", 
                featureColumnName: "Features",
                numberOfLeaves: 40,
                minimumExampleCountPerLeaf: 10);

            return dataProcessPipeline
                .Append(trainer)
                .Append(mlContext.Transforms.Conversion.MapKeyToValue(inputColumnName: "PredictedLabel", outputColumnName: "Prediction"));
        }

        // --- CLASE AUXILIAR NECESARIA PARA LA TRANSFORMACIÓN CUSTOM MAPPING (Mantenido sin cambios) ---
        public class FeatureBuffer
        {
            public float Animo { get; set; }
            public float Apetito { get; set; }
            public float Energia { get; set; }
            public float Sueno { get; set; }
            public float BateriaSocial { get; set; }
        }

        // ----------------------------------------------------------------------------------
        // --- MÉTODO LoadPersonalizedData() ACTUALIZADO: Carga datos desde AppDbContext ---
        // ----------------------------------------------------------------------------------
        
        // 🎯 Convertido a método de instancia, que usa _context 🎯
        private List<EmotionalStateData> LoadPersonalizedData() 
        { 
            // 1. Obtener los DiariosEmocionales de la tabla (simulación de acceso a EF Core).
            // NOTA: Se asume ID_Paciente = 1 para el modelo personalizado en este ejemplo.
            var diarios = _context.DiariosEmocionales
                .Where(d => d.ID_Paciente == 1) 
                .ToList();

            // 2. Mapear los objetos DiarioEmocional a EmotionalStateData.
            var personalizedData = diarios.Select(d => new EmotionalStateData
            {
                // Mapeo de campos numéricos y de IA
                HR_RitmoCardiaco = d.HR_RitmoCardiaco.GetValueOrDefault(0f),
                HRV_VariabilidadFrecuencia = d.HRV_VariabilidadFrecuencia.GetValueOrDefault(0f), 
                PasosDiarios = d.Pasos.GetValueOrDefault(0),
                Hora_dormida = d.Hora_dormida.GetValueOrDefault(7.0f), 
                Horas_celular = d.Horas_celular.GetValueOrDefault(0),
                Horas_redes = d.Horas_redes.GetValueOrDefault(0),
                Coherencia = d.Coherencia.GetValueOrDefault(1.0f),
                Errores_gramaticales = d.Errores_gramaticales.GetValueOrDefault(0f),

                // Mapeo de texto y emociones
                Emociones = d.Emociones ?? "5,5,5,5,5",
                EstadoEmocional = d.Estado, // La etiqueta para el entrenamiento

                // Mapeo y división de las tres zonas de texto (separadas por '/')
                Zona1_Actividades = d.Descripcion?
                                    .Split(new char[] { '/' }, 3, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(p => p.Trim()).ElementAtOrDefault(0) ?? string.Empty,
                Zona2_Personas = d.Descripcion?
                                    .Split(new char[] { '/' }, 3, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(p => p.Trim()).ElementAtOrDefault(1) ?? string.Empty,
                Zona3_Pensamientos = d.Descripcion?
                                    .Split(new char[] { '/' }, 3, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(p => p.Trim()).ElementAtOrDefault(2) ?? string.Empty
            }).ToList();

            return personalizedData; 
        }

        // ----------------------------------------------------------------------------------
        // --- LoadGeneralData (Mantenido sin cambios) ---
        // ----------------------------------------------------------------------------------
        
        private static List<EmotionalStateData> LoadGeneralData()
        {
            var data = new List<EmotionalStateData>();
            var random = new Random(0); 
            
            // Datos de texto simulados para cada estado
            var basalText = new[] { 
                ("Trabajo normal, gimnasio, serie", "Familia, amigos, llamadas", "Calma, planes futuros, ideas"),
                ("Casa, caminata, estudiar", "Colegas, vecino, sin reuniones", "Concentración, rutina, comida")
            };
            var exaltadoText = new[] { 
                ("Mucho trabajo, éxito, proyecto nuevo", "Conflicto, pareja, fiesta con mucha gente", "Ansiedad, ideas rápidas, euforia, metas"),
                ("Viaje, poco sueño, deporte extremo", "Discusión, pasión, debate", "Acelerado, sin tiempo, mucho por hacer")
            };
            var inhibidoText = new[] { 
                ("Sin salir, solo en casa, cama", "Nadie, aislamiento, tristeza", "Pensamientos lentos, desesperanza, apatía"),
                ("Pocas actividades, sin energía", "Llamadas evitadas, miedo social", "Auto-crítica, problemas sin solución, tristeza")
            };

            // Generar 100 ejemplos para el estado "basal" (Valores entre 4-6)
            for (int i = 0; i < 100; i++)
            {
                var text = basalText[random.Next(basalText.Length)];
                data.Add(new EmotionalStateData
                {
                    HR_RitmoCardiaco = random.Next(60, 75) + (float)random.NextDouble(),
                    HRV_VariabilidadFrecuencia = random.Next(20, 40) + (float)random.NextDouble(),
                    PasosDiarios = random.Next(4000, 10000),
                    Hora_dormida = random.Next(5, 9) + (float)random.NextDouble(),
                    Horas_celular = random.Next(0, 5),
                    Horas_redes = random.Next(0, 3),

                    Emociones = $"{random.Next(4, 7)},{random.Next(4, 7)},{random.Next(4, 7)},{random.Next(4, 7)},{random.Next(4, 7)}",
                    
                    Zona1_Actividades = text.Item1,
                    Zona2_Personas = text.Item2,
                    Zona3_Pensamientos = text.Item3,

                    Coherencia = random.Next(1, 2),
                    Errores_gramaticales = (float)(random.NextDouble() * 1.5 + 0.5),
                    EstadoEmocional = "basal"
                });
            }

            // Generar 100 ejemplos para el estado "exaltado" (Valores entre 8-10)
            for (int i = 0; i < 100; i++)
            {
                var text = exaltadoText[random.Next(exaltadoText.Length)];
                data.Add(new EmotionalStateData
                {
                    HR_RitmoCardiaco = random.Next(90, 120) + (float)random.NextDouble(),
                    HRV_VariabilidadFrecuencia = random.Next(45, 60) + (float)random.NextDouble(),
                    PasosDiarios = random.Next(10001, 50000),
                    Hora_dormida = random.Next(0, 4) + (float)random.NextDouble(),
                    Horas_celular = random.Next(5, 12),
                    Horas_redes = random.Next(4, 10),

                    Emociones = $"{random.Next(8, 11)},{random.Next(8, 11)},{random.Next(8, 11)},{random.Next(8, 11)},{random.Next(8, 11)}",
                    
                    Zona1_Actividades = text.Item1,
                    Zona2_Personas = text.Item2,
                    Zona3_Pensamientos = text.Item3,

                    Coherencia = random.Next(0, 1),
                    Errores_gramaticales = (float)(random.NextDouble() * 0.5),
                    EstadoEmocional = "exaltado"
                });
            }

            // Generar 100 ejemplos para el estado "inhibido" (Valores entre 0-3)
            for (int i = 0; i < 100; i++)
            {
                var text = inhibidoText[random.Next(inhibidoText.Length)];
                data.Add(new EmotionalStateData
                {
                    HR_RitmoCardiaco = random.Next(50, 60) + (float)random.NextDouble(),
                    HRV_VariabilidadFrecuencia = random.Next(10,20) + (float)random.NextDouble(),
                    PasosDiarios = random.Next(0, 4000),
                    Hora_dormida = random.Next(8, 11) + (float)random.NextDouble(),
                    Horas_celular = random.Next(5, 9),
                    Horas_redes = random.Next(4, 8),

                    Emociones = $"{random.Next(0, 4)},{random.Next(0, 4)},{random.Next(0, 4)},{random.Next(0, 4)},{random.Next(0, 4)}",
                    
                    Zona1_Actividades = text.Item1,
                    Zona2_Personas = text.Item2,
                    Zona3_Pensamientos = text.Item3,

                    Coherencia = random.Next(0, 1),
                    Errores_gramaticales = (float)(random.NextDouble() * 3.0 + 2.0),
                    EstadoEmocional = "inhibido"
                });
            }

            return data;
        }
    }
}