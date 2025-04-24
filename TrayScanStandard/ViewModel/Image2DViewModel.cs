
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using LinxUniverse.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Brushes1 = System.Drawing.Brushes;
using Bitmap = System.Drawing.Bitmap;
using Image = System.Drawing.Image;
using Graphics = System.Drawing.Graphics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using TrayScanStandard.Models;
using TrayScanStandard.Data.Models;
using TrayScanStandard.Data;
using MediatR;
using TrayScanStandard.Mediator.Commands;
using TrayScanStandard.Service;
using LinxUniverse.Algo.Common;
using Microsoft.Extensions.Logging;


namespace TrayScanStandard.ViewModel
{
    public partial class Image2DViewModel : ObservableRecipient
    {
        [ObservableProperty] string _errorText = String.Empty;
        [ObservableProperty]
        string _resultImg = Environment.CurrentDirectory + @"/Images/black.jpg";
        //string _resultImg = @"D:\wcsrepo\XCCCD2024\XCCCD2024\bin\Debug\net8.0-windows\Images\mio.jpg";

        [ObservableProperty]
        int _selectIdx = 0;
        public Brush[] Colors { get; set; } = Enumerable.Repeat(Brushes.Red, 128).ToArray();
        public string[] Codes { get; set; } = Enumerable.Repeat("", 128).ToArray();
        //public ObservableCollection<Brush> Colors { get; set; } = new(new Brush[128]);

        public event Action ColorUpdate;


        public int DebugExpoure
        {
            get; set;
        } = 5000;

        public int CameraIdx { get; set; } = 0;

        public CameraSetting CameraSetting
        {
            get; set;
        }


        //public BatteryInfo? SelectBattery => SelectIdx < 0 ? null : BatteryInfos[SelectIdx];

        [ObservableProperty]
        BarCodeRegionInfo? _selectBarCodeRegionInfo;

        public BatteryTypeInfo[] BatteryInfos { get; set; } = [];

        //public HuaRuiBCR HuaRuiBCR
        //{
        //    get; set;
        //}
        public required
            ScanCameraService
            Service { get; init; }


        public Image2DViewModel()
        {
            _mediator = App.GetService<IMediator>();
            _logger = App.GetService<ILogger<Image2DViewModel>>();
        }
        public LinxContext LinxContext;
        public void RefreshBatteryInfos()
        {
            LinxContext = App.GetService<LinxContext>();
            BatteryInfos = LinxContext.BatteryTypeInfos.ToArray();
            SelectIdx = System.Array.FindIndex(BatteryInfos, s => s.Id == MainStorage.Saves.SelectBatteryId);
        }

        partial void OnSelectIdxChanged(int oldValue, int newValue)
        {
            MainStorage.Saves.SelectBatteryId = newValue < 0 ? 0 : BatteryInfos[newValue].Id;

            //MainStorage.SaveManager.Save();
        }

        public void RefreshBarCodeRegionData()
        {
            OnPropertyChanged(nameof(SelectBarCodeRegionInfo));
        }
        [ObservableProperty]
        private string _ratioText = string.Empty;

        public void Update() => ColorUpdate?.Invoke();

        public CancellationTokenSource Cts
        {
            get; set;
        }
        public BatteryTypeInfo SelectBattery { get;  set; }
        public IMediator _mediator { get; }

        private ILogger<Image2DViewModel> _logger;

        [RelayCommand]
        public async void DebugCapture()
        {

            if (SelectBattery == null)
            {
                MessageBox.Show("未选择托盘类型");
                return;
            }

            if (Cts != null && !Cts.IsCancellationRequested)
            {
                Cts.Cancel();
            }
            else
            {
                Cts = new();

                while (!Cts.Token.IsCancellationRequested)
                {

                    
                }

                // 撕烤一下更新
            }

            Update();
        }

        public void UpdateRatio()
        {
            //var s = MainStorage.Saves.ScanRatios[CameraIdx - 1];
            //RatioText = $"{Properties.Resources.NumberOfSuccessfulAttempts}{s.OkCnt},{Properties.Resources.TotalNumberOfTimes}{s.ScanCnt},{Properties.Resources.SuccessRate}{s.OkCnt * 1.0 / s.ScanCnt:P}";
        }

        
        [RelayCommand]
        public async Task Detect()
        {
            var data = await _mediator.Send(new DetectCodeCommand([new DetectParam(tempImg, [])]));
            data.Match(
                Right: r =>
                {
                    // 绘制到图上？

                    _logger.LogInformation(r.ToArr().ToString());


                },
                Left: l =>
                {
                }
                );
        }
        byte[] tempImg = [];

        [RelayCommand]
        public async Task Capture()
        {
            var data = await _mediator.Send(new CamCaptureCommand(
                [
                    //Service.GetMugen(CameraIdx).Map(s => new CaptureInfo(s, CameraSetting.Exposure))
                    Service.GetMugen(CameraIdx).Map(s => new CaptureInfo(s, [DebugExpoure]))
                ] 
                ));


            data.Match(
                Right: r =>
                {
                    var img = tempImg  = r.First().First().Data; // 对结果要验证一下 加个验证器
                    var name = $"Data2d/single-{CameraIdx}-{FilenameHelper.FileName}.jpg";
                    File.WriteAllBytes(name, img);
                    ResultImg = FilenameHelper.AppPath + "/" + name;
                },
                Left: l =>
                {

                }

                );

            //if (SelectBattery == null)
            //{
            //    MessageBox.Show("未选择托盘类型");
            //    return;
            //}
            //CodeReaderDelectResult? res = null;
            //if (MainStorage.Saves.TestMode)
            //{
            //    res = new BCR.Common.CodeReaderDelectResult
            //    {
            //        CodeDatas = [new CodeData { Code = "1234567890", rect = [
            //            new (100, 200),
            //            new (100, 300),
            //            new (200, 200),
            //            new (200, 300),
            //        ] }],
            //    };
            //}
            //else
            //{
            //    HuaRuiBCR.StartStream();

            //    for (int i = 0; i < BcrInfo.Exposure.Count; i++)
            //    {
            //        HuaRuiBCR.SetControl(new ExposureControl { ExposureTime = BcrInfo.Exposure[i] });

            //        var singleRes = HuaRuiBCR?.GetOneFrameV2();
            //        if (res == null)
            //        {
            //            res = singleRes;
            //        }
            //        else
            //        {
            //            res.CodeDatas.AddRange(singleRes.CodeDatas);
            //            res.Image = singleRes.Image;
            //        }
            //    }

            //    HuaRuiBCR.StopStream();

            //    if (res != null)
            //    {
            //        res.CodeDatas = res.CodeDatas.DistinctBy(s => s.Code).ToList();

            //    }


            //}
            //if (res == null)
            //{

            //    MessageBox.Show("failed"); return;
            //}
            //var name = $"Data2d/single-{CameraIdx}-{FilenameHelper.FileName}.jpg";
            //if (!MainStorage.Saves.TestMode)
            //{
            //    File.WriteAllBytes(name, res.Image);
            //    ResultImg = FilenameHelper.AppPath + "/" + name;
            //}

            //var coderes = ScanUtils.GetCodeChannel(SelectBattery.Regions[CameraIdx - 1], res);

            //for (int i = 0; i < Colors.Length; ++i)
            //{
            //    Colors[i] = Brushes.Red;
            //}


            //foreach (var code in coderes)
            //{
            //    if (string.IsNullOrEmpty(code.Code))
            //    {
            //        //Colors[code.Channel] = Brushes.Red;
            //    }
            //    else
            //    {
            //        Colors[code.Channel] = Brushes.Green;
            //        Codes[code.Channel] = code.Code;
            //    }
            //}

            // 撕烤一下更新

            Update();
        }

        public void DrawAImage()
        {

            //if (SelectBattery == null)
            //{
            //    MessageBox.Show("未选择托盘类型");
            //    return;
            //}
            //using var ms = new MemoryStream(File.ReadAllBytes(ResultImg));
            //using var nImage = Image.FromStream(ms);
            //using var g = Graphics.FromImage(nImage);

            //System.Drawing.Font font = new("Aria", 100);
            //System.Drawing.Font font1 = new("Aria", 30);
            //foreach (var region in SelectBattery.Regions[CameraIdx - 1])
            //{
            //    var c = Colors[region.ChannelIdx] == Brushes.Red ? Brushes1.Red : Brushes1.Green;
            //    g.DrawRectangle(new System.Drawing.Pen(c, 10), new System.Drawing.Rectangle(region.Left, region.Top, region.Width, region.Height));
            //    g.DrawString(region.ChannelIdx.ToString(), font, c, new System.Drawing.Point(region.Left, region.Top + region.Height / 2 - 70));

            //    if (Codes[region.ChannelIdx].Length > 14)
            //    {
            //        g.DrawString(Codes[region.ChannelIdx][..14], font1, Brushes1.LightGreen, new System.Drawing.Point(region.Left - 10, region.Top + region.Height + 10));
            //        g.DrawString(Codes[region.ChannelIdx][14..], font1, Brushes1.LightGreen, new System.Drawing.Point(region.Left - 10, region.Top + region.Height + 50));
            //    }


            //}
            //nImage.Save(ResultImg.Replace(".jpg", "-Result.jpg"));
        }
    }
}
