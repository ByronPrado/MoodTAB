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
        if (viewModel.DebeMostrarTutorial)
        {   await Task.Delay(300);
            this.ShowPopup(new DiarioTutorialPopUp());
        }
        //this.ShowPopup(new DiarioTutorialPopUp());
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
        // Restar 10 minutos
            DateTime tenMinutesAgo = now.AddMinutes(-10);
            DateTime startOfToday = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Local);
            DateTime startOfMonth = startOfToday.AddDays(-30);
            
            Instant startTimeMonth = DateTimeToInstant(startOfMonth);
            Instant startTimeTenMinAgo = DateTimeToInstant(tenMinutesAgo);
            Instant endTimeNow = DateTimeToInstant(now);
            Instant startTimeToday = DateTimeToInstant(startOfToday);

            Android.Util.Log.Info("v0", $"📊 CARGANDO DATOS HEALTH");
            Android.Util.Log.Info("v0", $"   Rango mes: {startOfMonth:yyyy-MM-dd HH:mm} → {now:yyyy-MM-dd HH:mm}");
            Android.Util.Log.Info("v0", $"   Rango hoy: {startOfToday:yyyy-MM-dd HH:mm} → {now:yyyy-MM-dd HH:mm}");

            var stepsTask = LoadStepsData(startTimeToday, endTimeNow);
            var sleepTask = LoadSleepData(startTimeToday, endTimeNow);
            var heartRateTask = LoadHeartRateData(startTimeTenMinAgo, endTimeNow);
            var distanceTask = LoadDistanceData(startTimeToday, endTimeNow);
            var hrvTask = LoadHeartRateVariabilityData(startTimeToday, endTimeNow);

            await Task.WhenAll(stepsTask, sleepTask, heartRateTask, distanceTask, hrvTask);
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
                var startDt = InstantToDateTime(startTime);
                var endDt = InstantToDateTime(endTime);
                Android.Util.Log.Info("v0", $"📅 Rango de búsqueda: {startDt:yyyy-MM-dd HH:mm:ss} → {endDt:yyyy-MM-dd HH:mm:ss}");
                
                var records = await _healthConnectClient.ReadSleepRecords(startTime, endTime);
                Android.Util.Log.Info("v0", $"📊 Registros de sueño encontrados: {records?.Count ?? 0}");
                
                double totalHours = 0;
                
                if (records != null && records.Count > 0)
                {
                    Android.Util.Log.Info("v0", "📋 TODOS LOS REGISTROS ENCONTRADOS:");
                    for (int i = 0; i < records.Count; i++)
                    {
                        var record = records[i];
                        var recordStart = InstantToDateTime(record.StartTime);
                        var recordEnd = InstantToDateTime(record.EndTime);
                        
                        var duration = Java.Time.Duration.Between(record.StartTime, record.EndTime);
                        double recordHours = duration.ToMinutes() / 60.0;
                        
                        Android.Util.Log.Info("v0", $"  [{i}] {recordStart:yyyy-MM-dd HH:mm} → {recordEnd:yyyy-MM-dd HH:mm} ({recordHours:F2}h) | EndTime.EpochSecond: {record.EndTime.EpochSecond}");
                    }
                    
                    var sortedRecords = records.OrderByDescending(r => r.EndTime.EpochSecond).ToList();
                    Android.Util.Log.Info("v0", "⬇️ REGISTROS ORDENADOS (más reciente primero):");
                    for (int i = 0; i < sortedRecords.Count; i++)
                    {
                        var record = sortedRecords[i];
                        var recordStart = InstantToDateTime(record.StartTime);
                        var recordEnd = InstantToDateTime(record.EndTime);
                        var duration = Java.Time.Duration.Between(record.StartTime, record.EndTime);
                        double recordHours = duration.ToMinutes() / 60.0;
                        
                        Android.Util.Log.Info("v0", $"  [{i}] END: {recordEnd:yyyy-MM-dd HH:mm} ({recordHours:F2}h)");
                    }
                    
                    var latestRecord = sortedRecords.FirstOrDefault();
                    
                    if (latestRecord != null)
                    {
                        var startInstant = latestRecord.StartTime;
                        var endInstant = latestRecord.EndTime;
                        
                        Android.Util.Log.Info("v0", $"🔍 CALCULANDO DURACIÓN:");
                        Android.Util.Log.Info("v0", $"   StartTime.EpochSecond: {startInstant.EpochSecond}");
                        Android.Util.Log.Info("v0", $"   EndTime.EpochSecond: {endInstant.EpochSecond}");
                        Android.Util.Log.Info("v0", $"   Diferencia de segundos: {endInstant.EpochSecond - startInstant.EpochSecond}");
                        
                        var duration = Java.Time.Duration.Between(startInstant, endInstant);
                        
                        long minutes = duration.ToMinutes();
                        long seconds = duration.Seconds;
                        
                        Android.Util.Log.Info("v0", $"   Duration.ToMinutes(): {minutes}");
                        Android.Util.Log.Info("v0", $"   Duration.Seconds: {seconds}");
                        
                        totalHours = minutes / 60.0;
                        
                        Android.Util.Log.Info("v0", $"   totalHours (minutos/60.0): {totalHours}");
                        
                        var startDate = InstantToDateTime(latestRecord.StartTime);
                        var endDate = InstantToDateTime(latestRecord.EndTime);
                        
                        Android.Util.Log.Info("v0", $"✅ REGISTRO SELECCIONADO: {startDate:yyyy-MM-dd HH:mm:ss} → {endDate:yyyy-MM-dd HH:mm:ss}");
                        Android.Util.Log.Info("v0", $"   Duración FINAL mostrada: {totalHours}");
                        Android.Util.Log.Info("v0", $"   Origen: {latestRecord.Metadata?.DataOrigin?.PackageName ?? "desconocido"}");
                    }
                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    EntryHorasSueno.Text = totalHours > 0 ? $"{totalHours}" : "0";
                    Android.Util.Log.Info("v0", $"📱 VALOR EN UI: {EntryHorasSueno.Text}");
                });
            }
            catch (Exception ex)
            {
                Android.Util.Log.Error("v0", $"❌ Error cargando sueño: {ex.Message}");
                Android.Util.Log.Error("v0", $"   StackTrace: {ex.StackTrace}");
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
                
                double totalBpm = 0;
                int totalSamples = 0;

                if (records != null && records.Count > 0)
                {
                    foreach (var record in records)
                    {
                        Android.Util.Log.Info("v0", $"📍 Ritmo cardíaco - Origen: {record.Metadata?.DataOrigin?.PackageName ?? "desconocido"}");
                        
                        if (record.Samples == null)
                        {
                            Android.Util.Log.Warn("v0", "   ⚠️ record.Samples es null");
                            continue;
                        }

                        try
                        {
                            if (record.Samples is Java.Util.IList javaList)
                            {
                                int sampleCount = javaList.Size();
                                Android.Util.Log.Debug("v0", $"   📊 Samples encontrados: {sampleCount}");
                                
                                for (int i = 0; i < sampleCount; i++)
                                {
                                    var item = javaList.Get(i);
                                    
                                    if (item is HeartRateRecord.Sample sample)
                                    {
                                        long bpm = sample.BeatsPerMinute;
                                        var sampleTime = InstantToDateTime(sample.Time);
                                        Android.Util.Log.Debug("v0", $"      BPM: {bpm} @ {sampleTime:HH:mm:ss}");
                                        
                                        totalBpm += bpm;
                                        totalSamples++;
                                    }
                                    else
                                    {
                                        Android.Util.Log.Debug("v0", $"      ⚠️ Item {i} no es HeartRateRecord.Sample, es: {item?.GetType().Name}");
                                    }
                                }
                            }
                            else if (record.Samples is System.Collections.IEnumerable enumerable)
                            {
                                Android.Util.Log.Debug("v0", $"   Samples es IEnumerable (no IList)");
                                foreach (var item in enumerable)
                                {
                                    if (item is HeartRateRecord.Sample sample)
                                    {
                                        totalBpm += sample.BeatsPerMinute;
                                        totalSamples++;
                                    }
                                }
                            }
                            else
                            {
                                Android.Util.Log.Warn("v0", $"   ⚠️ Samples es tipo desconocido: {record.Samples?.GetType().Name}");
                            }
                        }
                        catch (Exception sampleEx)
                        {
                            Android.Util.Log.Error("v0", $"   ❌ Error procesando samples: {sampleEx.Message}");
                        }
                    }
                }

                double averageBpm = totalSamples > 0 ? totalBpm / totalSamples : 0;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    EntryHR.Text = averageBpm > 0 ? $"{averageBpm:F0}" : "--";
                    Android.Util.Log.Info("v0", $"✅ Promedio de BPM: {averageBpm:F0} ({totalSamples} muestras)");
                });
            }
            catch (Exception ex)
            {
                Android.Util.Log.Error("v0", $"❌ Error cargando ritmo cardíaco: {ex.Message}");
                Android.Util.Log.Error("v0", $"   StackTrace: {ex.StackTrace}");
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
        
        private DateTime InstantToDateTime(Java.Time.Instant instant)
        {
            long unixTimestamp = instant.EpochSecond;
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTimestamp).ToLocalTime();
        }
    #endif

}
