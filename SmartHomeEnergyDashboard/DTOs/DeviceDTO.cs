using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace SmartHomeEnergyDashboard.DTOs
{
    public abstract partial class DeviceDTO: ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ObservableProperty]
        private int power;
    }
}
