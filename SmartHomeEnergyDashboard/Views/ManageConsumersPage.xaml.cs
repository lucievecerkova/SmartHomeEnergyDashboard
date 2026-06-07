using SmartHomeEnergyDashboard.ViewModels;

namespace SmartHomeEnergyDashboard.Views;

public partial class ManageConsumersPage : ContentPage
{
	public ManageConsumersPage(ManageConsumersViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}