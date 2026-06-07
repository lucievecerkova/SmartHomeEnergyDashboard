using SmartHomeEnergyDashboard.ViewModels;

namespace SmartHomeEnergyDashboard.Views;

public partial class SimulatorPage : ContentPage
{
	public SimulatorPage(SimulatorViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }

}