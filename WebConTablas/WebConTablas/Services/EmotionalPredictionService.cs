using Microsoft.ML;
using WebConTablas.ML;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ML.Data; 
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using System; 

namespace WebConTablas.Services
{
    // NOTE: This assumes EmotionalStateData has been updated in WebConTablas.ML/EmotionalStateData.cs
    // to include:
    // public string Zona1_Actividades { get; set; }
    // public string Zona2_Personas { get; set; }
    // public string Zona3_Pensamientos { get; set; }
    
    public class EmotionalPredictionService
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _trainedModel;
        
        private static List<EmotionalStateData> GeneralData = LoadGeneralData();
        private static List<EmotionalStateData> PersonalizedData = LoadPersonalizedData(); 

        // Definiciones de parámetros de adaptación
        private const int X_SEMANAS_INICIALES = 3; 
        private const int SEMANAS_DE_TRANSICION = 3; 

        public EmotionalPredictionService()
        {
            _mlContext = new MLContext(seed: 0);
            
            // 1. **Determinar la semana actual** (SIMULACIÓN)
            int currentWeek = 1; 
            
            // 2. Entrenar el modelo
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
            if (generalWeight > 0)
            {
                int numGeneralSamples = (int)System.Math.Round(GeneralData.Count * (generalWeight / 100.0));
                trainingData.AddRange(GeneralData.Take(numGeneralSamples));
            }
            if (personalizedWeight > 0)
            {
                trainingData.AddRange(PersonalizedData);
            }
            var random = new System.Random();
            return trainingData.OrderBy(x => random.Next()).ToList();
        }

        // ----------------------------------------------------------------------------------
        // --- PIPELINE DE ENTRENAMIENTO MODIFICADO: Agrega FeaturizeText para las 3 zonas ---
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
                "Zone1_Features", "Zone2_Features", "Zone3_Features" // <-- Nuevas características de texto
            }).ToArray();

            var dataProcessPipeline = mlContext.Transforms.Conversion.MapValueToKey(
                    inputColumnName: nameof(EmotionalStateData.EstadoEmocional),
                    outputColumnName: "Label")
                // 🚨 Insertamos el CustomMapping para las 5 emociones
                .Append(customMapping) 
                // 🚀 Insertamos el procesamiento de texto para las 3 zonas
                .Append(textFeaturizationPipeline) 
                .Append(mlContext.Transforms.NormalizeMeanVariance(
                    numericAndCustomFeatures.Select(f => new InputOutputColumnPair(f, f)).ToArray()))
                .Append(mlContext.Transforms.Concatenate("Features", allFeatureNames)); // Concatenamos todas

            var trainer = mlContext.MulticlassClassification.Trainers.LightGbm(
                labelColumnName: "Label", 
                featureColumnName: "Features",
                numberOfLeaves: 40,
                minimumExampleCountPerLeaf: 10);

            return dataProcessPipeline
                .Append(trainer)
                .Append(mlContext.Transforms.Conversion.MapKeyToValue(inputColumnName: "PredictedLabel", outputColumnName: "Prediction"));
        }

        // --- CLASE AUXILIAR NECESARIA PARA LA TRANSFORMACIÓN CUSTOM MAPPING (Sin cambios) ---
        public class FeatureBuffer
        {
            public float Animo { get; set; }
            public float Apetito { get; set; }
            public float Energia { get; set; }
            public float Sueno { get; set; }
            public float BateriaSocial { get; set; }
        }

        // ----------------------------------------------------------------------------------
        // --- MÉTODOS DE CARGA DE DATOS MODIFICADOS: Incluyen datos simulados para las 3 zonas ---
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
                    HRV_VariabilidadFrecuencia = random.Next(70, 100) + (float)random.NextDouble(),
                    PasosDiarios = random.Next(4000, 6500),
                    Hora_dormida = random.Next(7, 9) + (float)random.NextDouble(),
                    Horas_celular = random.Next(2, 4),
                    Horas_redes = random.Next(1, 3), 

                    Emociones = $"{random.Next(4, 7)},{random.Next(4, 7)},{random.Next(4, 7)},{random.Next(4, 7)},{random.Next(4, 7)}",
                    
                    // 🚀 Nuevos campos de texto
                    Zona1_Actividades = text.Item1,
                    Zona2_Personas = text.Item2,
                    Zona3_Pensamientos = text.Item3,

                    Coherencia = random.Next(2, 4),
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
                    HRV_VariabilidadFrecuencia = random.Next(20, 45) + (float)random.NextDouble(),
                    PasosDiarios = random.Next(10000, 20000),
                    Hora_dormida = random.Next(3, 6) + (float)random.NextDouble(),
                    Horas_celular = random.Next(5, 10),
                    Horas_redes = random.Next(3, 7),

                    Emociones = $"{random.Next(8, 11)},{random.Next(8, 11)},{random.Next(8, 11)},{random.Next(8, 11)},{random.Next(8, 11)}",
                    
                    // 🚀 Nuevos campos de texto
                    Zona1_Actividades = text.Item1,
                    Zona2_Personas = text.Item2,
                    Zona3_Pensamientos = text.Item3,

                    Coherencia = random.Next(6, 10),
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
                    HRV_VariabilidadFrecuencia = random.Next(90, 120) + (float)random.NextDouble(),
                    PasosDiarios = random.Next(500, 3000),
                    Hora_dormida = random.Next(8, 11) + (float)random.NextDouble(),
                    Horas_celular = random.Next(0, 2),
                    Horas_redes = random.Next(0, 1),

                    Emociones = $"{random.Next(0, 4)},{random.Next(0, 4)},{random.Next(0, 4)},{random.Next(0, 4)},{random.Next(0, 4)}",
                    
                    // 🚀 Nuevos campos de texto
                    Zona1_Actividades = text.Item1,
                    Zona2_Personas = text.Item2,
                    Zona3_Pensamientos = text.Item3,

                    Coherencia = random.Next(0, 2),
                    Errores_gramaticales = (float)(random.NextDouble() * 3.0 + 2.0),
                    EstadoEmocional = "inhibido"
                });
            }

            return data;
        }
        
        private static List<EmotionalStateData> LoadPersonalizedData() 
        { 
            return new List<EmotionalStateData>(); 
        }
    }
}