using CommunityToolkit.Maui;
using LiveChartsCore.SkiaSharpView.Maui;
using Microcharts.Maui;
using Microsoft.Extensions.Logging;
using SmartHomeEnergyDashboard.DAL.Entities;
using SmartHomeEnergyDashboard.Services;
using SmartHomeEnergyDashboard.Services.Interfaces;
using SmartHomeEnergyDashboard.ViewModels;
using SmartHomeEnergyDashboard.Views;

namespace SmartHomeEnergyDashboard
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMicrocharts()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<SimulationService>();
            builder.Services.AddSingleton<SimulatorPage>();
            builder.Services.AddSingleton<SimulatorViewModel>();
            builder.Services.AddSingleton<EnergyDashboardService>();
            builder.Services.AddSingleton<IDeviceTypeService, DeviceTypeService>();
            builder.Services.AddSingleton<IAlertService, AlertService>();

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<MainViewModel>();

            builder.Services.AddTransient<ManageConsumersPage>();
            builder.Services.AddTransient<ManageConsumersViewModel>();
            builder.Services.AddTransient<ManageProducersPage>();
            builder.Services.AddTransient<ManageProducersViewModel>();
            builder.Services.AddTransient<StatisticsPage>();
            builder.Services.AddTransient<StatisticsViewModel>();

            return builder.Build();
        }
    }
}
