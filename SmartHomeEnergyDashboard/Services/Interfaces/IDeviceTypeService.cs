using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SmartHomeEnergyDashboard.Services.Interfaces
{
    public interface IDeviceTypeService
    {
        Task GetDeviceTypesAsync(ObservableCollection<string> collection);
        Task<List<string>> GetDeviceTypesAsync();
        Task SaveDeviceTypesAsync(IEnumerable<string> types);
    }
}
