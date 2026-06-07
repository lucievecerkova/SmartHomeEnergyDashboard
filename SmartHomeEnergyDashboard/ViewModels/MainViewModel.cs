using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartHomeEnergyDashboard.Views;

namespace SmartHomeEnergyDashboard.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [RelayCommand]
        private async Task NavigateToManageConsumers()
        {
            await Shell.Current.GoToAsync(nameof(ManageConsumersPage));
        }

        [RelayCommand]
        private async Task NavigateToManageProducers()
        {
            await Shell.Current.GoToAsync(nameof(ManageProducersPage));
        }

        [RelayCommand]
        private async Task NavigateToSimulation()
        {
            await Shell.Current.GoToAsync(nameof(SimulatorPage));
        }

        [RelayCommand]
        private async Task NavigateToStatistics()
        {
            await Shell.Current.GoToAsync(nameof(StatisticsPage));
        }
    }
}
