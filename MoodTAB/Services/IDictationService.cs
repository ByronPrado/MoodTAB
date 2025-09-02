using System.Threading;
using System.Threading.Tasks;

namespace MoodTAB.Services
{
    public interface IDictationService
    {
        /// <summary>
        /// Inicia el dictado y devuelve el texto reconocido.
        /// </summary>
        /// <param name="cancellationToken">Token para cancelar el dictado.</param>
        /// <returns>Texto reconocido del usuario.</returns>
        Task<string> StartDictationAsync(CancellationToken cancellationToken = default);
    }
}
