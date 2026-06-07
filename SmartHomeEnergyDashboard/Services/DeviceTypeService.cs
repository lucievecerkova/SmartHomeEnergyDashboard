using SmartHomeEnergyDashboard.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace SmartHomeEnergyDashboard.Services
{
    public class DeviceTypeService : IDeviceTypeService
    {
        private readonly string _filePath = Path.Combine(FileSystem.AppDataDirectory, "DeviceTypes564364.txt");
        private readonly string _addNewTypeText = "Add new device type...";

        public async Task<List<string>> GetDeviceTypesAsync()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    await File.WriteAllTextAsync(_filePath, string.Empty);
                    return new List<string>();
                }

                var lines = await File.ReadAllLinesAsync(_filePath);
                return lines.Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => l.Trim()).ToList();
            }
            catch (Exception ex) 
            {
                Debug.WriteLine($"Error loading devices: {ex.Message}");
                return new List<string>();
            }
        }

        public async Task GetDeviceTypesAsync(ObservableCollection<string> deviceTypesCollection)
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    await File.WriteAllTextAsync(_filePath, string.Empty);
                    if (!deviceTypesCollection.Contains(_addNewTypeText))
                    {
                        deviceTypesCollection.Add(_addNewTypeText);
                    }
                    return;
                }

                var lines = await File.ReadAllLinesAsync(_filePath);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    deviceTypesCollection.Clear();
                    deviceTypesCollection.Add(_addNewTypeText);
                    foreach (var line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                            deviceTypesCollection.Add(line.Trim());
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading devices: {ex.Message}");
            }
        }

        public async Task SaveDeviceTypesAsync(IEnumerable<string> types)
        {
            try
            {
                await File.WriteAllLinesAsync(_filePath, types, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving devices: {ex.Message}");
            }
        }
    }
}
