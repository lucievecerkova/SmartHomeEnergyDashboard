using SmartHomeEnergyDashboard.ViewModels;

namespace SmartHomeEnergyDashboard.Views;

public partial class ManageProducersPage : ContentPage
{
    public ManageProducersPage(ManageProducersViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}