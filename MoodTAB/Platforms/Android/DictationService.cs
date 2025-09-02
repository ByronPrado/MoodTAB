using Android.Content;
using Android.Speech;
using System.Threading;
using System.Threading.Tasks;
using MoodTAB.Services;

namespace MoodTAB.Platforms.Android
{
    public class DictationService : IDictationService
    {
        // Instancia estática para poder acceder desde MainActivity
        public static DictationService? Current { get; private set; }

        // Inicializamos en null pero siempre se crea en StartDictationAsync
        public TaskCompletionSource<string>? DictationResult { get; private set; }

        public DictationService()
        {
            Current = this;
        }

        public Task<string> StartDictationAsync(CancellationToken cancellationToken = default)
        {
            DictationResult = new TaskCompletionSource<string>();

            var intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            intent.PutExtra(RecognizerIntent.ExtraPrompt, "Habla para dictar tu diario");
            intent.PutExtra(RecognizerIntent.ExtraLanguage, "es-ES");

            // Usamos la instancia de MainActivity que ya guardaste
            var activity = MainActivity.Instance;
            activity?.StartActivityForResult(intent, 10);

            return DictationResult.Task;
        }
    }
}
