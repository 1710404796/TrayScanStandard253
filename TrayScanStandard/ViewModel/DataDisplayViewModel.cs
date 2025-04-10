using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using TrayScanStandard.Data;
using TrayScanStandard.Data.Models;


namespace TrayScanStandard.ViewModel
{
    //[ObservableRecipient]
    public partial class DataDisplayViewModel(LinxContext context) : ObservableRecipient
    {
        [ObservableProperty] private ObservableCollection<OKNGCnt> _okNGLogs = [];

        private IEnumerable<OKNGCnt> _logs = [];
        public DateTime StartTime { get; set; } = DateTime.Today.AddDays(-30);
        public DateTime EndTime { get; set; } = DateTime.Today.AddDays(1);


        public void RefreshContext()
        {
            _logs = context.OKNGCnts.AsNoTracking().OrderByDescending(s => s.Id).Take(100);
            Search();
        }

        public bool IsNowLog(WarningLog log)
        {

            return log.WarningTime >= StartTime && log.WarningTime <= EndTime;
        }

        [RelayCommand]
        public void Search()
        {
            var afterFilter = _logs.Where(s => s.EndTime >= StartTime && s.EndTime <= EndTime);
            OkNGLogs = new(
                afterFilter
            );
        }
        public int OkCnt => MainStorage.Saves.OKNGCnt.OKCnt;
        public int NGCnt => MainStorage.Saves.OKNGCnt.NG1Cnt;
        public int NG2Cnt => MainStorage.Saves.OKNGCnt.NG2Cnt;

        public void Update()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                //OnPropertyChanged(nameof(PalletCnt));



                OnPropertyChanged(nameof(OkCnt));
                OnPropertyChanged(nameof(NGCnt));
                OnPropertyChanged(nameof(NG2Cnt));
            });

        }

        public void SaveOkNGCnt()
        {

            MainStorage.Saves.OKNGCnt.EndTime = DateTime.Now;
            context.OKNGCnts.Add(MainStorage.Saves.OKNGCnt);
            var cc = context.SaveChanges();

            MainStorage.Saves.OKNGCnt = new();
            RefreshContext();
            Update();
        }

    }
}
