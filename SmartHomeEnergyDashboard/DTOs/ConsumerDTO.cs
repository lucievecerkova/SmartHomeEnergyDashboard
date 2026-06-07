using CommunityToolkit.Mvvm.ComponentModel;

namespace SmartHomeEnergyDashboard.DTOs
{
    public partial class ConsumerDTO : DeviceDTO
    {
        [ObservableProperty]
        private string type;

        [ObservableProperty]
        private string name;
    }
}
