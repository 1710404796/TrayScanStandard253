using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using TrayScanStandard.Data;
using TrayScanStandard.Data.Models;


namespace TrayScanStandard.ViewModel.CZPallet
{
    public partial class WarningLogViewModel(LinxContext context) : ObservableRecipient
    {
        [ObservableProperty] private ObservableCollection<WarningLog> _mesLogs = [];
        private IEnumerable<WarningLog> _logs = [];

        public DateTime StartTime { get; set; } = DateTime.Today.AddDays(-30);
        public DateTime EndTime { get; set; } = DateTime.Today.AddDays(1);


        public void RefreshContext()
        {
            _logs = context.WarningLogs.AsNoTracking().OrderByDescending(s => s.Id).Take(100);
            Search();
        }

        public bool IsNowLog(WarningLog log)
        {

            return log.WarningTime >= StartTime && log.WarningTime <= EndTime;
        }

        [RelayCommand]
        public void Search()
        {
            var afterFilter = _logs.Where(s => s.WarningTime >= StartTime && s.WarningTime <= EndTime);
            MesLogs = new(
                afterFilter
            );
        }
    }
}