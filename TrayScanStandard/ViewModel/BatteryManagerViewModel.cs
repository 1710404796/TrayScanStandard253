using System.Collections.ObjectModel;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Microsoft.Extensions.Logging;
using TrayScanStandard.Data;
using TrayScanStandard.Data.Models;


namespace TrayScanStandard.ViewModel;

public partial class BatteryManagerViewModel : ObservableRecipient
{
    private readonly ILogger<BatteryManagerViewModel> _logger;
    private readonly LinxContext _context;
    public readonly IMediator Mediator;

    private BatteryTypeInfo[] _batterySource = [];

    [ObservableProperty] ObservableCollection<BatteryTypeInfo> _battery = new();
    [ObservableProperty] private BatteryTypeInfo _selectedBattery;

    public BatteryManagerViewModel(ILogger<BatteryManagerViewModel> logger, LinxContext context, IMediator mediator)
    {
        _logger = logger;
        _context = context;
        Mediator = mediator;
        _batterySource = context.BatteryTypeInfos.ToArray();
        FilterBattery("");

        // foreach (var item in aa)
        // {
        //     _battery.Add(item);
        // }
    }
    

    [RelayCommand]
    public void AddPallet()
    {
        var np = new BatteryTypeInfo();
        // 弹出界面设置托盘信息
        _battery.Add(np);
        _logger.LogInformation($"添加托盘");
        _context.BatteryTypeInfos.Add(np);
    }

    [RelayCommand]
    public void DeletePallet()
    {
        if (_selectedBattery == null)
        {
            MessageBox.Show("请选择要删除的托盘");
            return;
        }

        _context.BatteryTypeInfos.Remove(_selectedBattery);
        _logger.LogInformation($"删除托盘{_selectedBattery.TypeName}");
        _battery.Remove(_selectedBattery);
    }

//public void SelectPallet(Pallet pallet)
//{
//    _selectPallet = pallet;
//}

    [RelayCommand]
    public void SavePallet()
    {
        try
        {
            var cnt = _context.SaveChanges();
            _logger.LogInformation("影响{cnt}行", cnt);
            // 刷新2d的
        }
        catch (Exception exception)
        {
            MessageBox.Show($"保存失败: {exception}");
            // throw;
        }
    }

    public void FilterBattery(string text)
    {
        Battery = new ObservableCollection<BatteryTypeInfo>(
            _batterySource.Where(s => s.TypeName.Contains(text))
        );
    }
}