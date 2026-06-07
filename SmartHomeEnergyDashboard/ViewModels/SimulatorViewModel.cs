using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartHomeEnergyDashboard.Services;
using SmartHomeEnergyDashboard.Views;

namespace SmartHomeEnergyDashboard.ViewModels
{
    public partial class SimulatorViewModel : ObservableObject
    {
        [ObservableProperty]
        private SimulationService _simulationService;
        public string SimulationButtonColor => IsSimulationRunning ? "Red" : "Green";
        public string SimulationButtonText => IsSimulationRunning ? "Stop Simulation" : "Start Simulation";
        public string BgColor => GetBgColorForHour(_simulationService.SimulatedHour);
        private string GetBgColorForHour(int hour)
        {
            return hour switch
            {
                >= 0 and < 2 => "#010f1e",
                >= 2 and < 4 => "#03162b",
                >= 4 and < 6 => "#0d2b45", 
                >= 6 and < 8 => "#203c56",
                >= 8 and < 10 => "#527598", 
                >= 10 and < 12 => "#8cb5db",
                >= 12 and < 14 => "#BBDAFC", 
                >= 14 and < 16 => "#99c4f2",
                >= 16 and < 18 => "#6b96c2", 
                >= 18 and < 20 => "#36597d", 
                >= 20 and < 22 => "#163352",
                _ => "#032345" 
            };
        }
        public Color BatteryColor => Color.FromArgb(!_simulationService.IsNight ? "#032345" : "#BBDAFC");
        public string Picture => _simulationService.IsNight ? "moon.png" : "sun.png";
        public string SaveColor => IsSimulationRunning ? "Grey" : "#053970";
        private bool _lastState;
        public int HistoryCountDisplay => _simulationService.HistoryCount;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SimulationButtonColor))]
        [NotifyPropertyChangedFor(nameof(SimulationButtonText))]
        [NotifyPropertyChangedFor(nameof(SaveColor))]
        private bool _isSimulationRunning;

        public double BatteryPercentage => _simulationService.BatteryPercentage;
        public int DynamicMaxBattery
        {
            get => _simulationService.MaxBatteryCapacity;
            set
            {
                _simulationService.MaxBatteryCapacity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(BatteryPercentage));
            }
        }


        public SimulatorViewModel(SimulationService simulationService)
        {
            _simulationService = simulationService;
            IsSimulationRunning = _simulationService.IsRunning;
            _lastState = SimulationService.IsNight;

            // check time and if it's day or night
            // check number of records
            _simulationService.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(SimulationService.SimulatedHour))
                {
                    bool newState = _simulationService.IsNight;
                    // so it won't glitch often
                    if (newState != _lastState)
                    {
                        _lastState = newState;
                        OnPropertyChanged(nameof(Picture));
                        OnPropertyChanged(nameof(BatteryColor));
                    }
                    if (_simulationService.SimulatedHour % 2 == 0)
                    {
                        OnPropertyChanged(nameof(BgColor));
                    }
                }
                if (e.PropertyName == nameof(SimulationService.HistoryCount))
                {
                    OnPropertyChanged(nameof(HistoryCountDisplay));
                }
                if (e.PropertyName == nameof(SimulationService.BatteryPercentage))
                {
                    OnPropertyChanged(nameof(BatteryPercentage));
                }
            };
        }

        [RelayCommand]
        private void ToggleSimulation()
        {
            if (IsSimulationRunning)
            {
                _simulationService.Stop();
                IsSimulationRunning = false;
            }
            else
            {
                _simulationService.Start();
                IsSimulationRunning = true;
            }
        }

        [RelayCommand]
        private async Task NavigateToStatistics()
        {
            if (IsSimulationRunning)
            {
                _simulationService.Stop();
                IsSimulationRunning = false;
            }
            await Shell.Current.GoToAsync(nameof(StatisticsPage));
        }

        [RelayCommand]
        private async Task GoBack()
        {
            if (IsSimulationRunning)
            {
                _simulationService.Stop();
                IsSimulationRunning = false;
            }
            await Shell.Current.GoToAsync($"///{nameof(MainPage)}");
        }

        [RelayCommand]
        private async Task Reset()
        {
            await _simulationService.ClearHistory();
        }

        [RelayCommand]
        private async Task ExportToJson()
        {
            if (IsSimulationRunning)
            {
                return;
            }
            await _simulationService.ExportToJsonAsync();
        }
    }
}
