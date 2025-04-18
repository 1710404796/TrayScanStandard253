using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TrayScanStandard;
using TrayScanStandard.Attritubes;
using TrayScanStandard.Data;
using TrayScanStandard.Data.Models;
using TrayScanStandard.Mediator.Commands;
using TrayScanStandard.Service;
using TrayScanStandard.ViewModel;


namespace TrayScanStandard.View
{
    /// <summary>
    /// AllBcrListView.xaml 的交互逻辑
    /// </summary>
    [PowerView(PowerEnum.相机列表)]
    public partial class AllBcrListView : Page
    {
        List<BcrBorder> _borderList = [];
        public ScanCameraService CRService { get; }

        private readonly IMediator meditor;
        private readonly ILogger<AllBcrListView> logger;
        private readonly LinxContext linxContext;

        CancellationTokenSource _cts;

        public int DebugExp {  get; set; }

        public AllBcrListView()
        {
            DataContext = this;
            CRService = App.GetService<ScanCameraService>();   
            meditor = App.GetService<IMediator>();
            logger = App.GetService<ILogger<AllBcrListView>>();
            linxContext = App.GetService<LinxContext>();
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateRatio();

            int idx = 0;
            foreach (var item in CRService.BcrBorderViewModels)
            {
                int i = idx;
                var bborder = new BcrBorder(item) { Width = 320, Height = 320, Margin = new Thickness(5) };

                BcrPanel.Children.Add(bborder);
                bborder.MouseDoubleClick += (o, s) => Bborder_MouseDoubleClick(i);
                _borderList.Add(bborder);

                idx++;
            }
        }

        private void UpdateRatio()
        {
            //Ratio.Text = $"{Properties.Resources.NumberOfSuccessfulAttempts}: {MainStorage.Saves.OkCnt} {Properties.Resources.TotalNumberOfTimes}: {MainStorage.Saves.ScanCnt} {Properties.Resources.SuccessRate}: {MainStorage.Saves.OkCnt * 1.0 / MainStorage.Saves.ScanCnt:P}";
        }

        private void Bborder_MouseDoubleClick(int idx)
        {
            //MainWindow.NageTo(new Image2DView(CRService.Image2DViewModels[idx]) );
            //MessageBox.Show("双击");
        }

        private BcrBorder CreateBorder(BcrBorderViewModel viewModel)
        {
            var bborder = new BcrBorder(viewModel) { Width = 200, Height = 200 };

            return bborder;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            //foreach (var bborder in _borderList)
            //{
            //    bborder.MouseDoubleClick -= bborder.MouseDoubleClick;

            //}
            // 是不是要删除事件

            _cts?.Cancel();


        }

        private async void AllCapture_Click(object sender, RoutedEventArgs e)
        {
            //Parallel.ForEach(CRService.Image2DViewModels, vm =>
            //{
            //    vm.Capture();
            //});
            //await Task.Yield();

            //(sender as Button).IsEnabled = false;

            //try
            //{
            //    var res = await meditor.Send(new DelectCCDCommand());
            //    var batterylist = linxContext.BatteryTypeInfos.ToArray();
            //    BatteryInfo batteryInfo = batterylist.FirstOrDefault(s => s.Id == MainStorage.Saves.SelectBattery);

            //    var regions = batteryInfo.Regions.SelectMany(s => s).Select(s => s.ChannelIdx);
            //    var cnt = regions.Count() == 0 ? 0 : regions.Max();
            //    logger.LogInformation("总通道数{cnt}", cnt);
            //    var channels = Enumerable.Range(1, cnt);
            //    var resChannels = res.CodeInfos.Where(s => !string.IsNullOrWhiteSpace(s.Code)).Select(s => s.Channel).ToList();
            //    if (channels.All(s => resChannels.Contains(s)))
            //    {
            //        MainStorage.Saves.OkCnt++;
            //    }

            //    MainStorage.Saves.ScanCnt++;
            //    UpdateRatio();
            //}
            //catch (Exception ex)
            //{
            //    logger.LogError(ex, "error");


            //}
            

            //(sender as Button).IsEnabled = true;

            //Ratio.Text = $"成功次数: {MainStorage.Saves.OkCnt} 总次数: {MainStorage.Saves.ScanCnt} 成功率: {MainStorage.Saves.OkCnt * 1.0 / MainStorage.Saves.ScanCnt:P}%";
            //foreach (var vm in CRService.Image2DViewModels)
            //{
            //    vm.Capture();

            //}
        }

        private async void ManualStart_Click(object sender, RoutedEventArgs e)
        {
            await Task.Yield();
            //await meditor.Send(new StartDelectTaskCommand(true));
        }

        private async void ManualStop_Click(object sender, RoutedEventArgs e)
        {
            //await meditor.Send(new StartDelectTaskCommand(false));

        }
        static object obj = new object ();
        private async void DebugCapture_Click(object sender, RoutedEventArgs e)
        {
           
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                (sender as Button).Content = Properties.Resources.DebuggingQRCodeScanning;

                _cts.Cancel();
            }
            else
            {

                (sender as Button).Content = Properties.Resources.StopDebugging;

                _cts = new CancellationTokenSource();
                while (!_cts.Token.IsCancellationRequested)
                {
                    try
                    {
                        var res = await meditor.Send(new DelectCCDCommand(
                            //DebugExp
                            ));


                        //var batterylist = linxContext.BatteryTypeInfos.ToArray();
                        //BatteryTypeInfo batteryInfo = batterylist.FirstOrDefault(s => s.Id == MainStorage.Saves.SelectBatteryId);

                        //var regions = batteryInfo.Regions.SelectMany(s => s).Select(s => s.ChannelIdx);
                        //var cnt = regions.Count() == 0 ? 0 : regions.Max();


                        //logger.LogInformation("总通道数{cnt}", cnt);
                        //var channels = Enumerable.Range(1, cnt);
                        //var resChannels = res.CodeInfos.Where(s => !string.IsNullOrWhiteSpace(s.Code)).Select(s => s.Channel).ToList();
                        //if (channels.All(s => resChannels.Contains(s)))
                        //{
                        //    MainStorage.Saves.OkCnt++;
                        //}

                        //MainStorage.Saves.ScanCnt++;
                        UpdateRatio();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "error");


                    }
                    finally
                    {
                        await Task.Delay(3000);
                    }
                }
            }
        

        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            //MainStorage.Saves.OkCnt = MainStorage.Saves.ScanCnt = 0;    
            UpdateRatio() ;
        }
    }
}
