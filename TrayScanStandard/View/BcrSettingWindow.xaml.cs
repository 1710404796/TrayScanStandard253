using MediatR;
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
using System.Windows.Shapes;
using TrayScanStandard.Mediator.Commands;
using TrayScanStandard.Models;
using TrayScanStandard.Service;

namespace TrayScanStandard.View
{
    /// <summary>
    /// BcrSettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BcrSettingWindow : Window
    {
        //public BcrSettingWindow()
        //{
        //    InitializeComponent();
        //}

        public BcrInfo Setting { get; set; }
        public readonly IMediator mediator;
        public BcrSettingWindow(BcrInfo setting)
        {
            var cc = App.GetService<CodeReaderService>();
            mediator=App.GetService<IMediator>();
            Setting = setting;
            InitializeComponent();
            CamKey.ItemsSource = cc.CamCodes;
            CamKey.Text = Setting.Key;
            gammaPattern.Text = Setting.Gamma.ToString();
            gainPattern.Text = Setting.Gain.ToString();
            ExpPattern.Text = string.Join("\n", Setting.Exposure);

            deviceCode.Text = Setting.DeviceCode;
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string logmessage = $"序列号{Setting.Key}修改为{CamKey.Text}";
                await mediator.Send(new OperatorLogCommand(logmessage));
                Setting.Key = CamKey.Text;

                string logmessage1 = $"曝光{Setting.Exposure}修改为{ExpPattern.Text}";
                await mediator.Send(new OperatorLogCommand(logmessage1));
                Setting.Exposure = ExpPattern.Text.Trim().Split("\n").Select(int.Parse).ToList();

                string logmessage2 = $"伽马{Setting.Gamma}修改为{float.Parse(gammaPattern.Text.Trim())}";
                await mediator.Send(new OperatorLogCommand(logmessage2));
                Setting.Gamma = float.Parse(gammaPattern.Text.Trim());

                string logmessage3 = $"增益{Setting.DeviceCode}修改为{deviceCode.Text}";
                await mediator.Send(new OperatorLogCommand(logmessage3));
                Setting.DeviceCode = deviceCode.Text;

                string logmessage4 = $"设备编号{Setting.Gain}修改为{float.Parse(gainPattern.Text.Trim())}";
                await mediator.Send(new OperatorLogCommand(logmessage4));
                Setting.Gain = float.Parse(gainPattern.Text.Trim());

                DialogResult = true;
                Close();
            }
            catch (Exception ex )
            {
                MessageBox.Show("有不合法数值，修改失败!");
            }
          
        }
    }
}
