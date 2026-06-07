using SmartHomeEnergyDashboard.Views;
namespace SmartHomeEnergyDashboard
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ManageConsumersPage), typeof(ManageConsumersPage));
            Routing.RegisterRoute(nameof(ManageProducersPage), typeof(ManageProducersPage));
            Routing.RegisterRoute(nameof(SimulatorPage), typeof(SimulatorPage));
            Routing.RegisterRoute(nameof(StatisticsPage), typeof(StatisticsPage));
        }
    }
}
