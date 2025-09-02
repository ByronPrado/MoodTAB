namespace MoodTAB.Vistas;

using MoodTAB.ViewModel;
using MoodTAB.ViewModels;
using MoodTAB.Services;
#if ANDROID
using Android;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using MoodTAB.Platforms.Android;
using Java.Sql;
#endif
public partial class UsuarioExternoPage : ContentPage
{

	INotificationManagerService notificationManager;
	IDictationService dictationService;
	private ViewModel.UsuarioExternoViewModel viewModel;
	public UsuarioExternoPage(INotificationManagerService manager, IDictationService dictation)
	{
		InitializeComponent();
		viewModel = new UsuarioExternoViewModel(manager, dictation);
		//viewModel.Navigation = this.Navigation;
		BindingContext = viewModel;

		notificationManager = manager;
		dictationService = dictation;
	}
		private async void OnLogoutClicked(object sender, EventArgs e)
		{
			var loginVM = new LoginViewModel();
			await loginVM.Logout();
		}
	/*
		void NotificationClick(object sender, EventArgs e)
		{
			//viewModel.ActualizarDatosUsuario();

			string title = $"Notificación de prueba";
			string message = Globals.cuestionario_pendiente.ToString();
			notificationManager.SendNotification(title, message, DateTime.Now.AddSeconds(1), 1);
		}
	*/


}