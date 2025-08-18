using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Models;
using MoodTAB.Vistas;
using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Net.Http;

namespace MoodTAB.ViewModel
{
    public partial class DetalleDiarioViewModel : ObservableObject
    {
        [ObservableProperty]
        public Diario diarioDetallado;
        [ObservableProperty]
        public ObservableCollection<EmocionItem> listaEmociones;
        public DetalleDiarioViewModel(Diario diario)
        {
            diarioDetallado = diario;
            listaEmociones = new ObservableCollection<EmocionItem>();
            var colores = new Dictionary<string, string>
            {
                { "Feliz", "#FEF9C3"},
                { "Emocionado", "#FFEDD5" },
                { "Cansado", "#F3E8FF" },
                { "Triste", "#DBEAFE" },
                { "Frustrado", "#FEE2E2" },
                { "Enojado", "#FEE2E2" },
                { "Neutro", "#F3F4F6" },
                { "Angustia", "#E0E7FF" },
                { "Ansioso", "#CCFBF1" },
            };
            var bordes = new Dictionary<string, string>
            {
                { "Feliz", "#FEF4A3"},
                { "Emocionado", "#FEDAB0" },
                { "Cansado", "#EDDDFF" },
                { "Triste", "#BFDBFE" },
                { "Frustrado", "#FECACA" },
                { "Enojado", "#FED5D5" },
                { "Neutro", "#EBEDF0" },
                { "Angustia", "#CCD6FE" },
                { "Ansioso", "#99F6E4" },
            };
            var emoticonos = new Dictionary<string, string>
            {
                { "Feliz", "😊"},
                { "Emocionado", "😃" },
                { "Cansado", "😪" },
                { "Triste", "😢" },
                { "Frustrado", "😖" },
                { "Enojado", "😠" },
                { "Neutro", "😑" },
                { "Angustia", "😰" },
                { "Ansioso", "🫨" },
            };
            foreach (var emocion in (diario.Emocion_Diaria ?? string.Empty).Split(','))
            {
                var texto = emocion.Trim();
                var color = colores.ContainsKey(texto) ? colores[texto] : "#FFE3FF67";
                var colorborde = colores.ContainsKey(texto) ? bordes[texto] : "#FFE3FF67";
                var emoji = colores.ContainsKey(texto) ? emoticonos[texto] : "🤡";
                var textoConEmoticono = $"{emoji} {texto}";
                listaEmociones.Add(new EmocionItem(textoConEmoticono, color, colorborde));
            }
        }
    }
    
    
}