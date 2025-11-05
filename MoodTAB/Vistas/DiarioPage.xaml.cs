using Microsoft.Extensions.DependencyInjection;
using MoodTAB.ViewModel;
using MoodTAB.Services;
using MoodTAB.Popups;
using CommunityToolkit.Maui.Views;
#if ANDROID
using Android.Content;
using MoodTAB.Platforms.Android;
using AndroidX.Health.Connect.Client;
using AndroidX.Health.Connect.Client.Records;
using MoodTAB.Platforms.Android.Callbacks;
using AndroidX.Health.Connect.Client.Records.Metadata;
using Java.Time;
using AndroidX.Health.Connect.Client.Permission;
using Java.Util;
using MoodTAB.Platforms.Android.Permissions;
#endif
namespace MoodTAB.Vistas;

public partial class DiarioPage : ContentPage
{
    private DiarioViewModel viewModel;

    public DiarioPage(IStepCounterService stepService, IDictationService dictationService)
    {
        InitializeComponent();
		viewModel = new DiarioViewModel(stepService, dictationService);
		BindingContext = viewModel;
		
		/*if (viewModel.DebeMostrarTutorial)
        {
            this.ShowPopup(new DiarioTutorialPopUp());
        }*/
        this.ShowPopup(new DiarioTutorialPopUp());
    }

	// Constructor sin parámetros para Shell/XAML
	public DiarioPage() : this(
		IPlatformApplication.Current.Services.GetRequiredService<IStepCounterService>(),
		IPlatformApplication.Current.Services.GetRequiredService<IDictationService>())
	{ }
    
	protected override async void OnAppearing()
	{
		base.OnAppearing();
#if ANDROID
		SolicitarPermisosAlIniciar();
		await LoadHealthDataAsync();
#endif
	}
	private async void SolicitarPermisosAlIniciar()
	{
#if ANDROID
		if (!UsageStatsHelper.TienePermisoDeUso())
		{
			bool aceptar = await this.DisplayAlert(
				"Permiso necesario",
				"Para mostrar el tiempo de uso de apps, debes conceder acceso a uso. ¿Deseas abrir la configuración ahora?",
				"Sí", "No");

			if (aceptar)
			{
				UsageStatsHelper.OpenUsageAccessSettings();
			}
		}
#else
		await Task.CompletedTask;
#endif
	}
	private void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
	{
		var slider = (Slider)sender;

		int valor = (int)Math.Round(e.NewValue);

		if (slider.Value != valor)
			slider.Value = valor;

		if (valor <= 3)
		{
			slider.ThumbColor = Colors.Red;
			slider.MinimumTrackColor = Colors.Red;
		}
		else if (valor <= 5)
		{
			slider.ThumbColor = Colors.Orange;
			slider.MinimumTrackColor = Colors.Orange;
		}
		else
		{
			slider.ThumbColor = Colors.Green;
			slider.MinimumTrackColor = Colors.Green;
		}
	}

	//healthconnect:
	#if ANDROID
    private KotlinCallback _healthConnectClient;
           private Instant DateTimeToInstant(DateTime date)
        {
            long unixTimestamp = ((DateTimeOffset)date).ToUnixTimeSeconds();
            return Instant.OfEpochSecond(unixTimestamp);
        }

        private async Task LoadHealthDataAsync()
        {
            try
            {
                Android.Util.Log.Info("v0", "OnLoadDataClicked iniciado");
                StatusLabel.Text = "Verificando disponibilidad...";
                
                string providerPackageName = "com.google.android.apps.healthdata";
                int availabilityStatus = HealthConnectClient.GetSdkStatus(Platform.CurrentActivity, providerPackageName);

                Android.Util.Log.Info("v0", $"SDK Status: {availabilityStatus}");

                if (availabilityStatus == HealthConnectClient.SdkUnavailable)
                {
                    await DisplayAlert("No soportado", "Health Connect no está disponible en este dispositivo.", "OK");
                    StatusLabel.Text = "Health Connect no disponible";
                    return;
                }

                if (availabilityStatus == HealthConnectClient.SdkUnavailableProviderUpdateRequired)
                {
                    OpenProviderInstallOrWeb(providerPackageName);
                    StatusLabel.Text = "Se requiere actualización";
                    return;
                }

                if (OperatingSystem.IsAndroidVersionAtLeast(26) && availabilityStatus == HealthConnectClient.SdkAvailable)
                {
                    // Inicializar cliente
                    if (_healthConnectClient == null)
                    {
                        var client = HealthConnectClient.GetOrCreate(Android.App.Application.Context);
                        _healthConnectClient = new KotlinCallback(client);
                        Android.Util.Log.Info("v0", "HealthConnectClient inicializado");
                    }

                    StatusLabel.Text = "Verificando permisos...";

                    var permissionsGranted = await RequestAllPermissions();

                    if (permissionsGranted)
                    {
                        StatusLabel.Text = "Cargando datos...";
                        await LoadAllHealthData();
                        StatusLabel.Text = "Datos actualizados correctamente";
                    }
                    else
                    {
                        StatusLabel.Text = "Permisos denegados. Ve a Configuración > Aplicaciones > Health > Permisos para concederlos manualmente.";
                    }
                }
            }
            catch (Exception ex)
            {
                Android.Util.Log.Error("v0", $"Error en OnLoadDataClicked: {ex.Message}");
                Android.Util.Log.Error("v0", $"StackTrace: {ex.StackTrace}");
                await DisplayAlert("Error", $"Error: {ex.Message}", "OK");
                StatusLabel.Text = $"Error: {ex.Message}";
            }
        }

        private async Task<bool> RequestAllPermissions()
        {
            try
            {
                var stepsRecord = new StepsRecord(
                    DateTimeToInstant(DateTime.Now.AddMinutes(-1)),
                    ZoneOffset.OfHours(0),
                    DateTimeToInstant(DateTime.Now),
                    ZoneOffset.OfHours(0),
                    1,
                    new Metadata()
                );

                var sleepRecord = new SleepSessionRecord(
                    DateTimeToInstant(DateTime.Now.AddHours(-8)),
                    ZoneOffset.OfHours(0),
                    DateTimeToInstant(DateTime.Now.AddHours(-1)),
                    ZoneOffset.OfHours(0),
                    null,
                    null,
                    new List<SleepSessionRecord.Stage>(),
                    new Metadata()
                );

                var heartRateRecord = new HeartRateRecord(
                    DateTimeToInstant(DateTime.Now.AddMinutes(-1)),
                    ZoneOffset.OfHours(0),
                    DateTimeToInstant(DateTime.Now),
                    ZoneOffset.OfHours(0),
                    new List<HeartRateRecord.Sample>(),
                    new Metadata()
                );

                var distanceRecord = new DistanceRecord(
                    DateTimeToInstant(DateTime.Now.AddMinutes(-1)),
                    ZoneOffset.OfHours(0),
                    DateTimeToInstant(DateTime.Now),
                    ZoneOffset.OfHours(0),
                    AndroidX.Health.Connect.Client.Units.Length.InvokeMeters(1),
                    new Metadata()
                );
                var hrvClass = Java.Lang.Class.ForName("androidx.health.connect.client.records.HeartRateVariabilityRmssdRecord");

                var permissionsToGrant = new Java.Util.HashSet();
                permissionsToGrant.Add(HealthPermission.GetReadPermission(Kotlin.Jvm.Internal.Reflection.GetOrCreateKotlinClass(stepsRecord.Class)));
                permissionsToGrant.Add(HealthPermission.GetReadPermission(Kotlin.Jvm.Internal.Reflection.GetOrCreateKotlinClass(sleepRecord.Class)));
                permissionsToGrant.Add(HealthPermission.GetReadPermission(Kotlin.Jvm.Internal.Reflection.GetOrCreateKotlinClass(heartRateRecord.Class)));
                permissionsToGrant.Add(HealthPermission.GetReadPermission(Kotlin.Jvm.Internal.Reflection.GetOrCreateKotlinClass(distanceRecord.Class)));
                permissionsToGrant.Add(HealthPermission.GetReadPermission(Kotlin.Jvm.Internal.Reflection.GetOrCreateKotlinClass(hrvClass)));

                Android.Util.Log.Info("v0", $"Permisos a solicitar: {permissionsToGrant.Size()}");

                var grantedPermissions = await _healthConnectClient.GetGrantedPermissions();
                Android.Util.Log.Info("v0", $"Permisos ya concedidos: {grantedPermissions?.Count ?? 0}");

                bool needsPermissions = false;
                if (grantedPermissions == null || grantedPermissions.Count == 0)
                {
                    needsPermissions = true;
                }
                else
                {
                    var iterator = permissionsToGrant.Iterator();
                    while (iterator.HasNext)
                    {
                        var permission = iterator.Next()?.ToString();
                        if (permission != null && !grantedPermissions.Contains(permission))
                        {
                            needsPermissions = true;
                            break;
                        }
                    }
                }

                Android.Util.Log.Info("v0", $"Necesita solicitar permisos: {needsPermissions}");

                if (needsPermissions)
                {
                    var result = await PermissionHandler.Request(permissionsToGrant);
                    
                    if (result != null && result.Count > 0)
                    {
                        Android.Util.Log.Info("v0", $"Permisos concedidos: {result.Count}");
                        grantedPermissions = result;
                    }
                    else
                    {
                        Android.Util.Log.Warn("v0", "No se concedieron permisos");
                        return false;
                    }
                }

                bool allGranted = true;
                if (grantedPermissions != null)
                {
                    var iterator = permissionsToGrant.Iterator();
                    while (iterator.HasNext)
                    {
                        var permission = iterator.Next()?.ToString();
                        if (permission != null && !grantedPermissions.Contains(permission))
                        {
                            allGranted = false;
                            break;
                        }
                    }
                }
                else
                {
                    allGranted = false;
                }

                Android.Util.Log.Info("v0", $"Todos los permisos concedidos: {allGranted}");
                
                return allGranted;
            }
            catch (Exception ex)
            {
                Android.Util.Log.Error("v0", $"Error solicitando permisos: {ex.Message}");
                Android.Util.Log.Error("v0", $"StackTrace: {ex.StackTrace}");
                return false;
            }
        }

        private async Task LoadAllHealthData()
        {
            DateTime now = DateTime.Now;
            DateTime startOfToday = new DateTime(now.Year, now.Month, now.Day-1, 0, 0, 0, DateTimeKind.Local);
            DateTime startOfMonth = startOfToday.AddDays(-30);
            
            Instant startTimeMonth = DateTimeToInstant(startOfMonth);
            Instant endTimeNow = DateTimeToInstant(now);
            Instant startTimeToday = DateTimeToInstant(startOfToday);

            Android.Util.Log.Info("v0", $"Cargando datos desde {startOfMonth} hasta {now}");
            Android.Util.Log.Info("v0", $"StartTimeToday: {startTimeToday}, EndTimeNow: {endTimeNow}");

            var stepsTask = LoadStepsData(startTimeToday, endTimeNow);
            var sleepTask = LoadSleepData(startTimeMonth, endTimeNow);
            var heartRateTask = LoadHeartRateData(startTimeMonth, endTimeNow);
            var distanceTask = LoadDistanceData(startTimeToday, endTimeNow);
            var hrvTask = LoadHeartRateVariabilityData(startTimeToday,endTimeNow);

            await Task.WhenAll(stepsTask, sleepTask, heartRateTask, distanceTask,hrvTask);
        }

        private async Task LoadStepsData(Instant startTime, Instant endTime)
        {
            try
            {
                Android.Util.Log.Info("v0", "=== Iniciando carga de pasos ===");
                var records = await _healthConnectClient.ReadStepsRecords(startTime, endTime);
                Android.Util.Log.Info("v0", $"Registros de pasos encontrados: {records?.Count ?? 0}");
                
                long totalSteps = 0;
                if (records != null)
                {
                    foreach (var record in records)
                    {
                        Android.Util.Log.Info("v0", $"Pasos: {record.Count} - Origen: {record.Metadata?.DataOrigin?.PackageName ?? "desconocido"}");
                        totalSteps += record.Count;
                    }
                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    EntryPasos.Text = totalSteps > 0 ? $"{totalSteps:N0}" : "0";
					viewModel.CantidadPasos = (int)totalSteps;
                    Android.Util.Log.Info("v0", $"Total de pasos mostrado: {totalSteps}");
                });
            }
            catch (Exception ex)
            {
                Android.Util.Log.Error("v0", $"Error cargando pasos: {ex.Message}");
                Android.Util.Log.Error("v0", $"StackTrace: {ex.StackTrace}");
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    EntryPasos.Text = "Error";
                });
            }
        }

        private async Task LoadSleepData(Instant startTime, Instant endTime)
        {
            try
            {
                Android.Util.Log.Info("v0", "=== Iniciando carga de sueño ===");
                var records = await _healthConnectClient.ReadSleepRecords(startTime, endTime);
                Android.Util.Log.Info("v0", $"Registros de sueño encontrados: {records?.Count ?? 0}");
                
                double totalHours = 0;
                if (records != null && records.Count > 0)
                {
                    foreach (var record in records)
                    {
                        var duration = Java.Time.Duration.Between(record.StartTime, record.EndTime);
                        double hours = duration.ToMinutes() / 60.0;
                        Android.Util.Log.Info("v0", $"Sueño: {hours:F2}h - Origen: {record.Metadata?.DataOrigin?.PackageName ?? "desconocido"}");
                        totalHours += hours;
                    }
                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    EntryHorasSueno.Text = totalHours > 0 ? $"{totalHours:F1}h" : "0h";
                    Android.Util.Log.Info("v0", $"Total de sueño mostrado: {totalHours:F1}h");
                });
            }
            catch (Exception ex)
            {
                Android.Util.Log.Error("v0", $"Error cargando sueño: {ex.Message}");
                Android.Util.Log.Error("v0", $"StackTrace: {ex.StackTrace}");
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    EntryHorasSueno.Text = "Sin datos";
                });
            }
        }

        private async Task LoadHeartRateData(Instant startTime, Instant endTime)
        {
            try
            {
                Android.Util.Log.Info("v0", "=== Iniciando carga de ritmo cardíaco ===");
                var records = await _healthConnectClient.ReadHeartRateRecords(startTime, endTime);
                Android.Util.Log.Info("v0", $"Registros de ritmo cardíaco encontrados: {records?.Count ?? 0}");
                
                double averageBpm = 0;
                int totalSamples = 0;

                if (records != null && records.Count > 0)
                {
                    foreach (var record in records)
                    {
                        Android.Util.Log.Info("v0", $"Ritmo cardíaco - Origen: {record.Metadata?.DataOrigin?.PackageName ?? "desconocido"}");
                        if (record.Samples != null)
                        {
                            var samples = record.Samples;
                            if (samples is Java.Util.IList javaList)
                            {
                                Android.Util.Log.Info("v0", $"Samples encontrados: {javaList.Size()}");
                                for (int i = 0; i < javaList.Size(); i++)
                                {
                                    if (javaList.Get(i) is HeartRateRecord.Sample sample)
                                    {
                                        Android.Util.Log.Debug("v0", $"BPM: {sample.BeatsPerMinute}");
                                        averageBpm += sample.BeatsPerMinute;
                                        totalSamples++;
                                    }
                                }
                            }
                        }
                    }

                    if (totalSamples > 0)
                    {
                        averageBpm /= totalSamples;
                    }
                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    EntryHR.Text = averageBpm > 0 ? $"{averageBpm:F0}" : "--";
                    Android.Util.Log.Info("v0", $"Promedio de BPM mostrado: {averageBpm:F0}");
                });
            }
            catch (Exception ex)
            {
                Android.Util.Log.Error("v0", $"Error cargando ritmo cardíaco: {ex.Message}");
                Android.Util.Log.Error("v0", $"StackTrace: {ex.StackTrace}");
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    EntryHR.Text = "Sin datos";
                });
            }
        }

        private async Task LoadDistanceData(Instant startTime, Instant endTime)
        {
            try
            {
                Android.Util.Log.Info("v0", "=== Iniciando carga de distancia ===");
                var records = await _healthConnectClient.ReadDistanceRecords(startTime, endTime);
                Android.Util.Log.Info("v0", $"Registros de distancia encontrados: {records?.Count ?? 0}");
                
                double totalMeters = 0;
                if (records != null && records.Count > 0)
                {
                    foreach (var record in records)
                    {
                        double meters = record.Distance.Meters;
                        Android.Util.Log.Info("v0", $"Distancia: {meters}m - Origen: {record.Metadata?.DataOrigin?.PackageName ?? "desconocido"}");
                        totalMeters += meters;
                    }
                }

                double totalKm = totalMeters / 1000.0;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    //DistanceLabel.Text = totalKm > 0 ? $"{totalKm:F2}" : "0.00";
                    Android.Util.Log.Info("v0", $"Total de distancia mostrado: {totalKm:F2}km");
                });
            }
            catch (Exception ex)
            {
                Android.Util.Log.Error("v0", $"Error cargando distancia: {ex.Message}");
                Android.Util.Log.Error("v0", $"StackTrace: {ex.StackTrace}");

            }
        }

        private async Task LoadHeartRateVariabilityData(Instant startTime, Instant endTime)
        {
            try
            {
                Android.Util.Log.Info("v0", "=== Iniciando carga de HRV (RMSSD) ===");
                var records = await _healthConnectClient.ReadHeartRateVariabilityRecords(startTime, endTime);
                Android.Util.Log.Info("v0", $"Registros HRV encontrados: {records?.Count ?? 0}");
                Android.Util.Log.Info("v0", $"Propiedades: {records}");
                
                double totalRmssd = 0;
                int count = 0;

                if (records != null && records.Count > 0)
                {
                    foreach (var record in records)
                    {
                        Android.Util.Log.Info("v0", $"Propiedades: {record}");
                        //var registro = record.getHeartRateVariabilityMillis();
                        /*
                        totalRmssd += rmssd;
                        count++;*/
                    }
                }
                EntryHRV.Text = "Sin datos";
                /*
                double averageRmssd = (count > 0) ? totalRmssd / count : 0;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    HrvLabel.Text = averageRmssd > 0 ? $"{averageRmssd:F2} ms" : "0.00 ms";
                    Android.Util.Log.Info("v0", $"HRV promedio mostrado: {averageRmssd:F2}ms");
                });*/
            }
            catch (Exception ex)
            {
                Android.Util.Log.Error("v0", $"Error cargando HRV: {ex.Message}");
                Android.Util.Log.Error("v0", $"StackTrace: {ex.StackTrace}");
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    EntryHRV.Text = "Sin datos";
                });
            }
        }

        private void OpenProviderInstallOrWeb(string providerPackageName)
        {
            var activity = Platform.CurrentActivity;
            var pm = activity.PackageManager;

            var marketUri = Android.Net.Uri.Parse($"market://details?id={providerPackageName}&url=healthconnect%3A%2F%2Fonboarding");
            var marketIntent = new Intent(Intent.ActionView, marketUri)
                .SetPackage("com.android.vending")
                .PutExtra("overlay", true)
                .PutExtra("callerId", activity.PackageName);

            var handlers = pm.QueryIntentActivities(marketIntent, 0);
            if (handlers != null && handlers.Count > 0)
            {
                try
                {
                    activity.StartActivity(marketIntent);
                    return;
                }
                catch (ActivityNotFoundException)
                {
                    // fallback abajo
                }
            }

            var webUri = Android.Net.Uri.Parse($"https://play.google.com/store/apps/details?id={providerPackageName}&url=healthconnect%3A%2F%2Fonboarding");
            var webIntent = new Intent(Intent.ActionView, webUri);
            activity.StartActivity(webIntent);
        }
    #endif


}
