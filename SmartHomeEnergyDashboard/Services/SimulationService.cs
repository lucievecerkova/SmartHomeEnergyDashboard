using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using SmartHomeEnergyDashboard.DTOs;
using SmartHomeEnergyDashboard.Services.Interfaces;
using System.Text;

namespace SmartHomeEnergyDashboard.Services
{
    public partial class SimulationService : ObservableObject
    {
        private EnergyDashboardService _dbService;
        private CancellationTokenSource _cts;
        private readonly IAlertService _alertService;
        public bool IsRunning => _cts != null;
        [ObservableProperty]
        private EnergyLogDTO _currentStatus = new();
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNight))]
        private int _simulatedHour = 0;
        [ObservableProperty]
        private int _maxBatteryCapacity = 5000;
        private double _currentBatteryStorage = 0;
        [ObservableProperty]
        private double _batteryPercentage;
        public bool IsNight => (SimulatedHour <= 6 || SimulatedHour >= 18);
        private bool _isCleared = false;

        private int _historyCount;
        public int HistoryCount
        {
            get => _historyCount;
            set
            {
                _historyCount = value;
                OnPropertyChanged(nameof(HistoryCount));
            }
        }

        public SimulationService(EnergyDashboardService dbService, IAlertService alertService)
        {
            _dbService = dbService;
            _alertService = alertService;
        }

        public void Start()
        {
            if (_cts != null) return;
            _cts = new CancellationTokenSource();
            Task.Run(() => RunSimulation(_cts.Token));
        }

        public void Stop()
        {
            _cts?.Cancel();
            _cts = null;
        }

        public async Task RunSimulation(CancellationToken ct)
        {
            if (!_isCleared)
            {
                await ClearHistory();
                _isCleared = true;
            }

            Random rnd = new Random();
            var producers = await _dbService.GetProducersAsync();
            var consumers = await _dbService.GetConsumersAsync();

            while (!ct.IsCancellationRequested)
            {
                // production magic :)
                double sunFactor = Math.Max(0, Math.Sin((SimulatedHour - 6) * Math.PI / 12));
                double currentProduction = producers.Sum(p => p.Power) * sunFactor;
                currentProduction *= (0.9 + rnd.NextDouble() * 0.2);

                // consumption magic: peak 7-9h and 18-22h + something random
                double activityFactor = (SimulatedHour >= 7 && SimulatedHour <= 9) || (SimulatedHour >= 18 && SimulatedHour <= 22) ? 1.2 : 0.4;
                double currentConsumption = consumers.Sum(c => c.Power) * activityFactor;
                currentConsumption *= (0.85 + rnd.NextDouble() * 0.3);

                double balance = Math.Round(currentProduction - currentConsumption, 2);
                _currentBatteryStorage += balance;
                _currentBatteryStorage = Math.Clamp(_currentBatteryStorage, 0, MaxBatteryCapacity);
                BatteryPercentage = _currentBatteryStorage / MaxBatteryCapacity;

                var log = new EnergyLogDTO
                {
                    Timestamp = DateTime.Now,
                    SimulatedHour = SimulatedHour,
                    Production = Math.Round(currentProduction, 2),
                    Consumption = Math.Round(currentConsumption, 2),
                    Balance = balance,
                    BatteryLevel = BatteryPercentage
                };

                await _dbService.AddLogAsync(log);
                HistoryCount++;
                SimulatedHour = (SimulatedHour + 1) % 24;

                MainThread.BeginInvokeOnMainThread(() => {
                    CurrentStatus = log;
                });

                await Task.Delay(500, ct);
            }
        }

        public async Task ClearHistory()
        {
            SimulatedHour = 0;
            await _dbService.ClearAllLogsAsync();
            HistoryCount = 0;
        }

        public async Task<List<EnergyLogDTO>> GetRecordsAsync()
        {
            if (!_isCleared)
            {
                await ClearHistory();
                _isCleared = true;
            }
            return await _dbService.GetLogsAsync();
        }

        public async Task ExportToJsonAsync()
        {
            try
            {
                var history = await GetRecordsAsync();
                if (history == null || !history.Any())
                {
                    await _alertService.ShowAlertAsync("Export", "No data to save.");
                    return;
                }

                string json = JsonConvert.SerializeObject(history, Formatting.Indented);
                using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

                var fileSaveResult = await FileSaver.Default.SaveAsync("EnergyHistory.json", stream, default);

                if (fileSaveResult.IsSuccessful)
                {
                    await _alertService.ShowAlertAsync("Success", $"File was saved.");
                }
            }
            catch (Exception ex)
            {
                await _alertService.ShowAlertAsync("Error", $"Export failed: {ex.Message}");
            }
        }

        public async Task<List<EnergyLogDTO>> ImportFromJsonAsync()
        {
            try
            {
                var options = new PickOptions
                {
                    PickerTitle = "Choose simulation history file:",
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.WinUI, new[] { ".json" } }
                    })
                };

                var result = await FilePicker.Default.PickAsync(options);
                if (result == null)
                    return null;

                using var stream = await result.OpenReadAsync();
                using var reader = new StreamReader(stream);
                string json = await reader.ReadToEndAsync();

                var importedLogs = JsonConvert.DeserializeObject<List<EnergyLogDTO>>(json);

                return importedLogs;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Import Error: {ex.Message}");
                await _alertService.ShowAlertAsync("Error", "Couldn't load file: " + ex.Message);
                return null;
            }
        }
    }
}
