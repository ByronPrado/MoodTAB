using Microsoft.ML;
using WebConTablas.ML;
using System.Collections.Generic;
using System.Linq;
// using Microsoft.ML.LightGbm; // 👈 Ya no es necesario
// Asegúrate de que tienes todos los usings necesarios para tipos de datos:
using Microsoft.ML.Data; 
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;

namespace WebConTablas.Services
{
    public class EmotionalPredictionService
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _trainedModel;
        // Asumiremos que esta data viene de un repositorio o archivo estático
        private static List<EmotionalStateData> GeneralData = LoadGeneralData();
        private static List<EmotionalStateData> PersonalizedData = LoadPersonalizedData(); 

        // Definiciones de parámetros de adaptación
        private const int X_SEMANAS_INICIALES = 3; 
        private const int SEMANAS_DE_TRANSICION = 3; 

        public EmotionalPredictionService()
        {
            _mlContext = new MLContext(seed: 0);
            
            // 1. **Determinar la semana actual** (SIMULACIÓN: en una app real la obtendrías del usuario)
            // Para simplificar, asumiremos una semana fija o la obtendríamos del contexto del usuario/BD.
            int currentWeek = 6; 
            
            // 2. Entrenar el modelo al inicializar el servicio (esto puede ser lento, idealmente se carga un modelo pre-entrenado)
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

        // --- MÉTODOS DE CÁLCULO DE PESO Y CARGA DE DATOS (Mismos que en Código 2) ---

        private float GetPersonalizedWeight(int currentWeek)
        {
            // ... (Lógica GetPersonalizedWeight idéntica a tu Código 2)
            if (currentWeek <= X_SEMANAS_INICIALES) return 0f;
            if (currentWeek >= X_SEMANAS_INICIALES + SEMANAS_DE_TRANSICION) return 100f;
            int weeksIntoTransition = currentWeek - X_SEMANAS_INICIALES;
            return (float)weeksIntoTransition / SEMANAS_DE_TRANSICION * 100f;
        }
        
        private List<EmotionalStateData> LoadTrainingDataForCurrentWeek(float personalizedWeight)
        {
             // ... (Lógica LoadTrainingDataForCurrentWeek idéntica a tu Código 2)
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

        private static IEstimator<ITransformer> BuildTrainingPipeline(MLContext mlContext)
        {
            // ... (Lógica BuildTrainingPipeline idéntica a tu Código 2)
            var featureNames = new[] { 
                nameof(EmotionalStateData.HR_RitmoCardiaco), nameof(EmotionalStateData.HRV_VariabilidadFrecuencia),
                nameof(EmotionalStateData.PasosDiarios), nameof(EmotionalStateData.HorasSueno),
                nameof(EmotionalStateData.Temperatura_Desviacion), nameof(EmotionalStateData.RedesSociales_Frecuencia),
                nameof(EmotionalStateData.DeteccionEmociones_Subjetivas), nameof(EmotionalStateData.ContenidoSemantico_KeywordsScore),
                nameof(EmotionalStateData.ComplejidadLenguaje_Errores) 
            };

            var dataProcessPipeline = mlContext.Transforms.Conversion.MapValueToKey(
                inputColumnName: nameof(EmotionalStateData.EstadoEmocional),
                outputColumnName: "Label")
                .Append(mlContext.Transforms.NormalizeMeanVariance(
                    featureNames.Select(f => new InputOutputColumnPair(f, f)).ToArray()))
                .Append(mlContext.Transforms.Concatenate("Features", featureNames));

            var trainer = mlContext.MulticlassClassification.Trainers.LightGbm(
                labelColumnName: "Label", 
                featureColumnName: "Features",
                numberOfLeaves: 40,
                minimumExampleCountPerLeaf: 10);

            return dataProcessPipeline
                .Append(trainer)
                .Append(mlContext.Transforms.Conversion.MapKeyToValue(inputColumnName: "PredictedLabel", outputColumnName: "Prediction"));
        }
        
        // **NOTA**: LoadGeneralData() y LoadPersonalizedData() deben ser estáticos o inicializados fuera del constructor,
        // ya que el constructor de `EmotionalPredictionService` se podría llamar muchas veces. 
        // Para simplificar, asumo que se mantuvieron las implementaciones de tu Código 2, pero sin el uso de Console.WriteLine.
        private static List<EmotionalStateData> LoadGeneralData() 
        { 
            // ... (Lógica LoadGeneralData idéntica a tu Código 2, eliminando Console.WriteLine) 
            return new List<EmotionalStateData>(); // Devuelve los datos generados
        }
        private static List<EmotionalStateData> LoadPersonalizedData() 
        { 
            // ... (Lógica LoadPersonalizedData idéntica a tu Código 2, eliminando Console.WriteLine) 
            return new List<EmotionalStateData>(); // Devuelve los datos generados
        }
    }
}