using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkiaSharp;
using SmartHomeEnergyDashboard.DTOs;
using SmartHomeEnergyDashboard.Services;
using SmartHomeEnergyDashboard.Services.Interfaces;
using System.Linq;

namespace SmartHomeEnergyDashboard.ViewModels
{
    public partial class StatisticsViewModel : ObservableObject
    {
        private readonly SimulationService _simulationService;
        private readonly IAlertService _alertService;

        [ObservableProperty]
        private Microcharts.Chart _productionChart;
        [ObservableProperty]
        private Microcharts.Chart _consumptionChart;
        [ObservableProperty]
        private Microcharts.Chart _balanceChart;
        private List<EnergyLogDTO> _history;

        public StatisticsViewModel(SimulationService simulationService, IAlertService alertService)
        {
            _simulationService = simulationService;
            _alertService = alertService;
        }

        [RelayCommand]
        private async Task ImportData()
        {
            _history = await _simulationService.ImportFromJsonAsync();

            if (_history != null && _history.Any())
            {
                await LoadChartData();
                await _alertService.ShowAlertAsync("Done", $"{_history.Count} records loaded successfully.");
            }
            else
            {
                await _alertService.ShowAlertAsync("Error", "File is empty");
            }
        }

        [RelayCommand]
        public async Task SetActualData()
        {
            _history = await _simulationService.GetRecordsAsync();

            if (_history != null && _history.Any())
            {
                await LoadChartData();
                await _alertService.ShowAlertAsync("Done", $"{_history.Count} records loaded successfully.");
            }
            else
            {
                await _alertService.ShowAlertAsync("No Data", "Please start a simulation first.");
            }
        }

        public async Task LoadChartData()
        {
            ProductionChart = CreateChart(_history, h => (float)h.Production, SKColors.Green);
            ConsumptionChart = CreateChart(_history, h => (float) h.Consumption, SKColors.Red);
            BalanceChart = CreateChart(_history, h => (float)h.Balance, SKColors.Blue);
        }

        private Microcharts.LineChart CreateChart(List<EnergyLogDTO> history, Func<EnergyLogDTO, float> valueSelector, 
                                                SkiaSharp.SKColor color)
        {
            var entries = history.Select(h => new Microcharts.ChartEntry(valueSelector(h))
            {
                Label = $"{h.SimulatedHour}:00",
                ValueLabel = $"{valueSelector(h):N1} W",
                Color = color.WithAlpha(200)
            }).ToArray();

            return new Microcharts.LineChart
            {
                Entries = entries,
                LineMode = Microcharts.LineMode.Spline,
                LineSize = 8,
                PointMode = Microcharts.PointMode.Circle,
                PointSize = 10,
                LabelTextSize = 24
            };
        }
    }
}
