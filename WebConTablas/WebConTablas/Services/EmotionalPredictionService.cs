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

        // --- PIPELINE DE ENTRENAMIENTO CORREGIDO (Restaurando FeatureBuffer) ---
        
        private static IEstimator<ITransformer> BuildTrainingPipeline(MLContext mlContext)
        {
            // --- PASO 1: PARSEAR LA CADENA CSV ---
            // 🚨 CORRECCIÓN CLAVE: El CustomMapping Action debe usar FeatureBuffer como tipo de salida
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
            
            // Creamos una CustomMappingEstimator con el CustomMappingTransformer
            // 🚨 CORRECCIÓN CLAVE: Especificamos EmotionalStateData (Input) y FeatureBuffer (Output)
            var customMapping = mlContext.Transforms.CustomMapping(customAction, contractName: "CsvParser");
            
            // Los nombres de las características para la concatenación
            var newFeatureNames = new[] { 
                nameof(EmotionalStateData.HR_RitmoCardiaco), 
                nameof(EmotionalStateData.HRV_VariabilidadFrecuencia),
                nameof(EmotionalStateData.PasosDiarios), 
                nameof(EmotionalStateData.Hora_dormida), 
                // ✅ Nuevos campos independientes
                nameof(EmotionalStateData.Horas_celular), 
                nameof(EmotionalStateData.Horas_redes), 
                // 🚨 Los 5 campos generados por el CustomMapping (propiedades de FeatureBuffer)
                nameof(FeatureBuffer.Animo), nameof(FeatureBuffer.Apetito), 
                nameof(FeatureBuffer.Energia), nameof(FeatureBuffer.Sueno), 
                nameof(FeatureBuffer.BateriaSocial), 
                nameof(EmotionalStateData.Coherencia),
                nameof(EmotionalStateData.Errores_gramaticales) 
            };

            var dataProcessPipeline = mlContext.Transforms.Conversion.MapValueToKey(
                    inputColumnName: nameof(EmotionalStateData.EstadoEmocional),
                    outputColumnName: "Label")
                // 🚨 Insertamos el CustomMapping
                .Append(customMapping) 
                .Append(mlContext.Transforms.NormalizeMeanVariance(
                    newFeatureNames.Select(f => new InputOutputColumnPair(f, f)).ToArray()))
                .Append(mlContext.Transforms.Concatenate("Features", newFeatureNames));

            var trainer = mlContext.MulticlassClassification.Trainers.LightGbm(
                labelColumnName: "Label", 
                featureColumnName: "Features",
                numberOfLeaves: 40,
                minimumExampleCountPerLeaf: 10);

            return dataProcessPipeline
                .Append(trainer)
                .Append(mlContext.Transforms.Conversion.MapKeyToValue(inputColumnName: "PredictedLabel", outputColumnName: "Prediction"));
        }

        // --- CLASE AUXILIAR NECESARIA PARA LA TRANSFORMACIÓN CUSTOM MAPPING ---
        public class FeatureBuffer
        {
            public float Animo { get; set; }
            public float Apetito { get; set; }
            public float Energia { get; set; }
            public float Sueno { get; set; }
            public float BateriaSocial { get; set; }
        }

        // --- MÉTODOS DE CARGA DE DATOS CORREGIDOS (Ajuste de Datos Simulados) ---
        
        private static List<EmotionalStateData> LoadGeneralData()
        {
            var data = new List<EmotionalStateData>();
            var random = new Random(0); 

            // Generar 100 ejemplos para el estado "basal" (Valores entre 4-6)
            for (int i = 0; i < 100; i++)
            {
                data.Add(new EmotionalStateData
                {
                    HR_RitmoCardiaco = random.Next(60, 75) + (float)random.NextDouble(),
                    HRV_VariabilidadFrecuencia = random.Next(70, 100) + (float)random.NextDouble(),
                    PasosDiarios = random.Next(4000, 6500),
                    Hora_dormida = random.Next(7, 9) + (float)random.NextDouble(),
                    // ✅ Nuevos campos de uso de redes
                    Horas_celular = random.Next(2, 4),
                    Horas_redes = random.Next(1, 3), 

                    Emociones = $"{random.Next(4, 7)},{random.Next(4, 7)},{random.Next(4, 7)},{random.Next(4, 7)},{random.Next(4, 7)}",
                    Coherencia = random.Next(2, 4),
                    Errores_gramaticales = (float)(random.NextDouble() * 1.5 + 0.5),
                    EstadoEmocional = "basal"
                });
            }

            // Generar 100 ejemplos para el estado "exaltado" (Valores entre 8-10)
            for (int i = 0; i < 100; i++)
            {
                data.Add(new EmotionalStateData
                {
                    HR_RitmoCardiaco = random.Next(90, 120) + (float)random.NextDouble(),
                    HRV_VariabilidadFrecuencia = random.Next(20, 45) + (float)random.NextDouble(),
                    PasosDiarios = random.Next(10000, 20000),
                    Hora_dormida = random.Next(3, 6) + (float)random.NextDouble(),
                    // ✅ Nuevos campos de uso de redes
                    Horas_celular = random.Next(5, 10),
                    Horas_redes = random.Next(3, 7),

                    Emociones = $"{random.Next(8, 11)},{random.Next(8, 11)},{random.Next(8, 11)},{random.Next(8, 11)},{random.Next(8, 11)}",
                    Coherencia = random.Next(6, 10),
                    Errores_gramaticales = (float)(random.NextDouble() * 0.5),
                    EstadoEmocional = "exaltado"
                });
            }

            // Generar 100 ejemplos para el estado "inhibido" (Valores entre 0-3)
            for (int i = 0; i < 100; i++)
            {
                data.Add(new EmotionalStateData
                {
                    HR_RitmoCardiaco = random.Next(50, 60) + (float)random.NextDouble(),
                    HRV_VariabilidadFrecuencia = random.Next(90, 120) + (float)random.NextDouble(),
                    PasosDiarios = random.Next(500, 3000),
                    Hora_dormida = random.Next(8, 11) + (float)random.NextDouble(),
                    // ✅ Nuevos campos de uso de redes
                    Horas_celular = random.Next(0, 2),
                    Horas_redes = random.Next(0, 1),

                    Emociones = $"{random.Next(0, 4)},{random.Next(0, 4)},{random.Next(0, 4)},{random.Next(0, 4)},{random.Next(0, 4)}",
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