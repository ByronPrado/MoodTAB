using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MoodTAB.Models;
using MoodTAB.Vistas;
using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Maui.Controls;


namespace MoodTAB.ViewModel
{
    public partial class DetalleDiarioViewModel : ObservableObject
    {
        [ObservableProperty] public Diario diarioDetallado;
        [ObservableProperty] public string animo;
        [ObservableProperty] public string apetito;
        [ObservableProperty] public string energia;
        [ObservableProperty] public string calidadSueno;
        [ObservableProperty] public string bateriaSocial;
        [ObservableProperty] public string pregunta1;
        [ObservableProperty] public string pregunta2;
        [ObservableProperty] public string pregunta3;
        [ObservableProperty] bool optionSuenoTrue = Globals.OptionSueno;
        [ObservableProperty] bool optionSuenoFalse = !Globals.OptionSueno;
        public string[] listaSliders;
        public string[] preguntas;
        
        public DetalleDiarioViewModel(Diario diario)
        {
            diarioDetallado = diario;
            listaSliders = diario.Emocion_Diaria.Split(',');
            Animo = listaSliders[0];
            Apetito = listaSliders[1];
            Energia = listaSliders[2];
            CalidadSueno = listaSliders[3];
            BateriaSocial = listaSliders[4];

            preguntas = diario.Descripcion.Split('/');
            Pregunta1 = preguntas[0];
            Pregunta2 = preguntas[1];
            Pregunta3 = preguntas[2];
        }
    }
    
    
}