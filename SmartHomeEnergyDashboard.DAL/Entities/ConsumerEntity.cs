namespace SmartHomeEnergyDashboard.DAL.Entities
{
    public class ConsumerEntity : DeviceEntity
    {
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
