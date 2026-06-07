using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartHomeEnergyDashboard.DTOs;
using SmartHomeEnergyDashboard.Services;
using System.Collections.ObjectModel;

namespace SmartHomeEnergyDashboard.ViewModels
{
    public partial class ManageProducersViewModel : ObservableObject
    {
        private readonly EnergyDashboardService _producerService;

        public ObservableCollection<ProducerDTO> Devices { get; set; } = new();
        [ObservableProperty]
        private string _inputPower;

        public ManageProducersViewModel(EnergyDashboardService producerService)
        {
            _producerService = producerService;
            LoadData();
        }

        private async void LoadData()
        {
            var devices = await _producerService.GetProducersAsync();
            Devices.Clear();
            foreach (var d in devices) Devices.Add(d);
        }

        [RelayCommand]
        private async Task SaveDevice()
        {
            if (!int.TryParse(InputPower, out int p))
                return;
            var newDevice = new ProducerDTO
            {
                Power = p
            };

            await _producerService.SaveProducerRecordAsync(newDevice);
            Devices.Add(newDevice);

            InputPower = string.Empty;
        }

        [RelayCommand]
        private async Task DeleteDevice(ProducerDTO device)
        {
            if (device == null) return;
            await _producerService.DeleteProducerAsync(device);
            Devices.Remove(device);
        }
    }
}
