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
            
            foreach (var emocion in (diario.Emocion_Diaria ?? string.Empty).Split(','))
            {
                var texto = emocion.Trim();
                var color = Globals.colores.ContainsKey(texto) ? Globals.colores[texto] : "#FFE3FF67";
                var colorborde = Globals.bordes.ContainsKey(texto) ? Globals.bordes[texto] : "#FFE3FF67";
                var emoji = Globals.emoticonos.ContainsKey(texto) ? Globals.emoticonos[texto] : "🤡";
                listaEmociones.Add(new EmocionItem(texto, emoji, color, colorborde));
            }
        }
    }
    
    
}