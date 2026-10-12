using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LinxUniverse.Utils;
using Microsoft.EntityFrameworkCore;
using TrayScanStandard.Data;
using TrayScanStandard.Data.Models;
using TrayScanStandard.Models.CZPallet;
using TrayScanStandard.ViewModel.CZPallet;

namespace TrayScanStandard.ViewModel
{
    public partial class PalletLogViewModel(LinxContext context) : ObservableRecipient
    {
        //public ObservableCollection<PalletLogViewModel> Log { get; set; } = [];
        [ObservableProperty] private ObservableCollection<PalletLogExt> _palletLogs = [];
        private IQueryable<PalletLog>? _query;
        public const int PageSize = 100;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PageSummary))]
        [NotifyCanExecuteChangedFor(nameof(PreviousPageCommand))]
        [NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
        private int _currentPage = 1;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TotalPages))]
        [NotifyPropertyChangedFor(nameof(PageSummary))]
        [NotifyCanExecuteChangedFor(nameof(PreviousPageCommand))]
        [NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
        private int _totalCount;

        public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
        public string PageSummary => $"第 {CurrentPage} / {TotalPages} 页，共 {TotalCount} 条，每页 {PageSize} 条";
        public DateTime StartTime { get; set; } = DateTime.Today.AddDays(-30);
        public DateTime EndTime { get; set; } = DateTime.Today.AddDays(1);

        public string Code { get; set; } = string.Empty;


        public void RefreshContext()
        {
            Search();
        }

        public bool IsNowLog(WarningLog log)
        {

            return log.WarningTime >= StartTime && log.WarningTime <= EndTime;
        }

        [RelayCommand]
        public void ExportAll()
        {
            StringBuilder sb = new(1000);
            string[] titles = ["托盘编号", "组盘时间", "电池数量", "电池条码"];

            sb.AppendLine(string.Join(",", titles));

            PalletLog[] logs;
            lock (context)
            {
                logs = (_query ?? CreateQuery()).OrderByDescending(s => s.Id).ToArray();
            }
            foreach (var log in logs)
            {
                //sb.AppendLine($"{log.PalletCode},{log.ZuPanTime},{log.ChannelCount},[{string.Join("|", log.BatteryInfo.Select(s => s.BatteryCode))}]");
                sb.AppendLine($"{log.PalletCode},{log.ZuPanTime.ToString("yyyy-MM-dd HH:mm:ss")},{log.ChannelCount},[{string.Join("|", log.BatteryInfo.Select(s => s.BatteryCode))}]");

            }

            string fileName = $"InsertLog/exportall-{FilenameHelper.FileName}.csv";
            System.IO.File.WriteAllBytes(fileName, Encoding.GetEncoding("gb2312").GetBytes(sb.ToString()));
            MessageBox.Show($"export to {fileName}");

        }
        [RelayCommand]
        public void Export()
        {
            StringBuilder sb = new(1000);
            string[] titles = ["托盘编号", "组盘时间", "电池数量", "电池条码"];

            sb.AppendLine(string.Join(",", titles));

            foreach (var log in PalletLogs.Where(s => s.IsSelect))
            {
                //sb.AppendLine($"{log.PalletCode},{log.ZuPanTime},{log.ChannelCount},[{string.Join("|", log.BatteryInfo.Select(s => s.BatteryCode))}]");
                sb.AppendLine($"{log.PalletCode},{log.ZuPanTime.ToString("yyyy-MM-dd HH:mm:ss")},{log.ChannelCount},[{string.Join("|", log.BatteryInfo.Select(s => s.BatteryCode))}]");

            }

            string fileName = $"InsertLog/export-{FilenameHelper.FileName}.csv";
            System.IO.File.WriteAllBytes(fileName, Encoding.GetEncoding("gb2312").GetBytes(sb.ToString()));
            MessageBox.Show($"export to {fileName}");

        }
        [RelayCommand]
        public void Search()
        {
            lock (context)
            {
                _query = CreateQuery();
                LoadPage(1);
            }
        }

        private IQueryable<PalletLog> CreateQuery()
        {
            var startTime = StartTime;
            var endTime = EndTime;
            var query = context.PalletLogs.AsNoTracking()
                .Where(s => s.PalletType == PalletType.组盘
                    && s.ZuPanTime >= startTime && s.ZuPanTime <= endTime);
            if (!string.IsNullOrEmpty(Code))
            {
                var code = Code.ToUpperInvariant();
                query = query.Where(s => s.PalletCode.ToUpper().Contains(code));
            }
            return query;
        }

        private bool CanPreviousPage() => CurrentPage > 1;
        private bool CanNextPage() => CurrentPage < TotalPages;

        [RelayCommand(CanExecute = nameof(CanPreviousPage))]
        private void PreviousPage() => LoadPage(CurrentPage - 1);

        [RelayCommand(CanExecute = nameof(CanNextPage))]
        private void NextPage() => LoadPage(CurrentPage + 1);

        private void LoadPage(int page)
        {
            lock (context)
            {
                _query ??= CreateQuery();
                TotalCount = _query.Count();
                CurrentPage = Math.Clamp(page, 1, TotalPages);
                var logs = _query.OrderByDescending(s => s.Id)
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize).ToArray();
                PalletLogs = new(logs.Select(s => new PalletLogExt(s)));
            }
        }
    }
}
