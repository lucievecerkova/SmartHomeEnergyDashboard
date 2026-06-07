using SmartHomeEnergyDashboard.ViewModels;

namespace SmartHomeEnergyDashboard.Views;

public partial class StatisticsPage : ContentPage
{
	public StatisticsPage(StatisticsViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is StatisticsViewModel vm)
        {
            await vm.SetActualData();
        }
    }
}