using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace SmartHomeEnergyDashboard.DTOs
{
    public partial class EnergyLogDTO : ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public System.DateTime Timestamp { get; set; }
        public int SimulatedHour { get; set; }
        public double Production { get; set; }
        public double Consumption { get; set; }
        public double Balance { get; set; }
        public double BatteryLevel { get; set; }
    }
}
