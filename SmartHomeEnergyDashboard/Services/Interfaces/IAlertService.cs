using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHomeEnergyDashboard.Services.Interfaces
{
    public interface IAlertService
    {
        Task ShowAlertAsync(string title, string message, string cancel = "OK");
        Task<bool> ShowConfirmationAsync(string title, string message, string accept = "Yes", string cancel = "No");
    }
}
