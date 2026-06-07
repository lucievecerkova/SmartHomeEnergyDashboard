using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartHomeEnergyDashboard.DTOs;
using SmartHomeEnergyDashboard.Services;
using SmartHomeEnergyDashboard.Services.Interfaces;
using System.Collections.ObjectModel;

namespace SmartHomeEnergyDashboard.ViewModels
{
    public partial class ManageConsumersViewModel : ObservableObject
    {
        private readonly EnergyDashboardService _consumerService;
        private readonly IDeviceTypeService _deviceTypeService;
        private readonly IAlertService _alertService;
        private readonly string _addNewTypeText = "Add new device type...";

        public ObservableCollection<string> DeviceTypes { get; set; } = new();
        public ObservableCollection<ConsumerDTO> Devices { get; set; } = new();
        [ObservableProperty]
        private string _selectedType;
        [ObservableProperty]
        private bool _isCustomTypeVisible;
        [ObservableProperty]
        private string _customTypeName;
        [ObservableProperty]
        private string _inputName;
        [ObservableProperty]
        private string _inputPower;
        [ObservableProperty]
        private ConsumerDTO _selectedDevice;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SubmitButtonText))]
        private ConsumerDTO _editingDevice;
        public string SubmitButtonText => EditingDevice == null ? "Add Device" : "Edit Device";

        public ManageConsumersViewModel(EnergyDashboardService consumerService, IDeviceTypeService deviceTypeService, 
                                        IAlertService alertService)
        {
            _consumerService = consumerService;
            _deviceTypeService = deviceTypeService;
            _alertService = alertService;
            LoadData();
        }

        private async void LoadData()
        {
            var types = await _deviceTypeService.GetDeviceTypesAsync();
            MainThread.BeginInvokeOnMainThread(() => {
                DeviceTypes.Clear();
                DeviceTypes.Add(_addNewTypeText);
                foreach (var t in types) 
                    DeviceTypes.Add(t);
            });

            var devices = await _consumerService.GetConsumersAsync();
            MainThread.BeginInvokeOnMainThread(() => {
                Devices.Clear();
                foreach (var d in devices) 
                    Devices.Add(d);
            });
        }

        [RelayCommand]
        private async Task SaveDeviceTypes()
        {
            var typesToSave = DeviceTypes
                            .Where(t => t != _addNewTypeText)
                            .ToList();
            await _deviceTypeService.SaveDeviceTypesAsync(typesToSave);
        }

        partial void OnSelectedTypeChanged(string value)
        {
            IsCustomTypeVisible = value == _addNewTypeText;
        }

        [RelayCommand]
        private async Task DeleteDeviceType(string typeToDelete)
        {
            if (string.IsNullOrEmpty(typeToDelete) || typeToDelete == _addNewTypeText)
            {  
                return; 
            }

            if (DeviceTypes.Contains(typeToDelete))
            {
                DeviceTypes.Remove(typeToDelete);
                if (SelectedType == typeToDelete)
                {
                    SelectedType = null;
                }
                await SaveDeviceTypes();
                await _alertService.ShowAlertAsync("Success", "Device type deleted!");
            }
        }

        [RelayCommand]
        private async Task CreateDeviceType()
        {
            if (!string.IsNullOrWhiteSpace(CustomTypeName) && !DeviceTypes.Contains(CustomTypeName))
            {
                DeviceTypes.Add(CustomTypeName);
                CustomTypeName = string.Empty;
                await SaveDeviceTypes();
            }
        }

        [RelayCommand]
        private async Task SaveDevice()
        {
            string finalType = SelectedType;
            bool newDeviceTypeCreated = false;

            if (IsCustomTypeVisible)
            {
                finalType = CustomTypeName.Trim();
                newDeviceTypeCreated = true;
            }
            if (!await ValidateInput(finalType))
            {
                return;
            }
            if (newDeviceTypeCreated)
            {
                await CreateDeviceType();
            }
            int.TryParse(InputPower, out int p);
            if (EditingDevice == null)
            {
                var newDevice = new ConsumerDTO
                {
                    Name = InputName,
                    Type = finalType,
                    Power = p
                };

                await _consumerService.SaveConsumerRecordAsync(newDevice);
                Devices.Add(newDevice);
            }
            else
            {
                EditingDevice.Name = InputName;
                EditingDevice.Type = SelectedType;
                EditingDevice.Power = p;

                await _consumerService.SaveConsumerRecordAsync(EditingDevice);

                int index = Devices.IndexOf(EditingDevice);
                Devices[index] = EditingDevice;

                EditingDevice = null;
            }

            InputName = string.Empty;
            InputPower = string.Empty;
            SelectedType = null;
        }

        private async Task<bool> ValidateInput(string finalType)
        {
            if (string.IsNullOrWhiteSpace(finalType))
            {
                await _alertService.ShowAlertAsync("Error", "Choose device type!");
            }
            else if (string.IsNullOrWhiteSpace(InputName))
            {
                await _alertService.ShowAlertAsync("Error", "Name must not be empty!");
            }
            else if (!int.TryParse(InputPower, out int p))
            {
                await _alertService.ShowAlertAsync("Error", "Power must be a number!");
            } 
            else
            {
                return true;
            }
            return false;
        }

        [RelayCommand]
        private async Task DeleteDevice(ConsumerDTO device)
        {
            if (device == null) return;
            await _consumerService.DeleteConsumerAsync(device);
            Devices.Remove(device);
        }

        [RelayCommand]
        private async Task EditDevice(ConsumerDTO device)
        {
            if (device == null) return;

            EditingDevice = device;
            InputName = device.Name;
            InputPower = device.Power.ToString();
            SelectedType = device.Type;
        }
    }
}
