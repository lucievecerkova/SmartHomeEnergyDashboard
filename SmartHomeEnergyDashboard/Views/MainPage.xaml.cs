using SmartHomeEnergyDashboard.Services;

namespace SmartHomeEnergyDashboard
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = new SmartHomeEnergyDashboard.ViewModels.MainViewModel();
        }
    }
}
