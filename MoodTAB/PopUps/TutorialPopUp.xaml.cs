using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;

namespace MoodTAB.Popups
{
    public partial class TutorialPopup : Popup
    {
        public TutorialPopup()
        {
            InitializeComponent();
            _ = AnimateInAsync();
            TutorialCarousel.ItemsSource = new List<TutorialPage>
            {
                new() { Title = "Registra tus emociones", Description = "Responde cuantas veces quieras,expresate con comodidad." , Imagen="TutorialDiario.gif"},
                new() { Title = "Responde cuestionarios", Description = "Menos Papeles y el en momento que desees." , Imagen="TutorialCuestionario.gif"},
                new() { Title = "Revisa tus respuestas", Description = "Tambien puedes ver tus horas de sueño" , Imagen="TutorialHistorial.gif"},
                new() { Title = "Personaliza tus registros", Description = "Elige en todo momento que compartir con tu Psiquiatra", Imagen="TutorialAjustes.gif" }
            };
        }

        private async Task AnimateInAsync()
        {
            RootGrid.Opacity = 0;
            await RootGrid.FadeTo(1, 300, Easing.CubicIn);
        }

        private async Task AnimateOutAsync()
        {
            await RootGrid.FadeTo(0, 250, Easing.CubicOut);
            Close();
        }

        private void OnSkip(object sender, EventArgs e)
        {
            Preferences.Set("tutorial_shown", true);
            _ = AnimateOutAsync();
        }

        private async void OnNext(object sender, EventArgs e)
        {
            int count = GetItemCount();
            if (count == 0)
            {
                Preferences.Set("tutorial_shown", true);
                await AnimateOutAsync();
                return;
            }

            if (TutorialCarousel.Position < count - 1)
            {
                TutorialCarousel.Position += 1;
            }
            else
            {
                Preferences.Set("tutorial_shown", true);
                await AnimateOutAsync();
            }
        }

        private void OnPositionChanged(object sender, PositionChangedEventArgs e)
        {
            int count = GetItemCount();
            if (count == 0) return;

            if (e.CurrentPosition == count - 1)
                NextButton.Text = "Finalizar";
            else
                NextButton.Text = "Siguiente";
        }
        private int GetItemCount()
        {
            var itemsSource = TutorialCarousel.ItemsSource;

            if (itemsSource == null)
                return 0;

            if (itemsSource is ICollection collection)
                return collection.Count;

            if (itemsSource is IEnumerable enumerable)
            {
                try
                {
                    return enumerable.Cast<object>().Count();
                }
                catch
                {
                    // fallback seguro
                }
            }

            return 0;
        }

        public class TutorialPage
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string Imagen { get; set; }
        }
    }
}
