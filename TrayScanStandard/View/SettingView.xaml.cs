using CommunityToolkit.Mvvm.ComponentModel;
using LinxUniverse.PLC.Meditor.Commands;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TrayScanStandard.PLC;
using TrayScanStandard.Attritubes;
using TrayScanStandard.View.CZPallet;
using TrayScanStandard.View.User;
using TrayScanStandard.ViewModel;

namespace TrayScanStandard.View
{
    /// <summary>
    /// SettingView.xaml 的交互逻辑
    /// </summary>
    [PowerView(PowerEnum.程序参数设定)]
    public partial class SettingView : Page
    {

        public SettingViewModel ViewModel
        {
            get;
        }

        /*
        private readonly DispatcherTimer _plcStatusTimer = new()
        {
            Interval = TimeSpan.FromMilliseconds(500)
        };
        private bool _isRefreshingPlcStatus;
        private bool _isDB500Dirty;
        private bool _isDB501Dirty;
        */

        public SettingView()
        {
            DataContext = this;
            ViewModel = App.GetService<SettingViewModel>();
            InitializeComponent();
            // _plcStatusTimer.Tick += PlcStatusTimer_Tick;
            // Loaded += SettingView_Loaded;
            // Unloaded += SettingView_Unloaded;
        }

        /*
        private void SettingView_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshPlcStatus();
            _plcStatusTimer.Start();
        }

        private void SettingView_Unloaded(object sender, RoutedEventArgs e)
        {
            _plcStatusTimer.Stop();
        }

        private void PlcStatusTimer_Tick(object? sender, EventArgs e)
        {
            RefreshPlcStatus();
        }

        private void RefreshPlcStatus()
        {
            var isConnected = MainStorage.IsPlcConnected;
            DB500WriteButton.IsEnabled = isConnected;
            DB501WriteButton.IsEnabled = isConnected;

            if (!isConnected)
            {
                DB500TaskNameTextBlock.Text = "PLC未连接";
                SetTextIfNotEdited(DB500ActionTextBox, "PLC未连接", _isDB500Dirty);
                SetTextIfNotEdited(DB501FeedbackTextBox, "PLC未连接", _isDB501Dirty);
                return;
            }

            var context = App.GetService<CCDContext>();
            var action = context.DB500.Action;
            var taskName = action switch
            {
                0 => "空闲",
                1 => "扫码任务",
                2 => "托盘离开",
                _ => "未知任务"
            };

            DB500TaskNameTextBlock.Text = taskName;
            SetTextIfNotEdited(DB500ActionTextBox, action.ToString(), _isDB500Dirty);
            SetTextIfNotEdited(DB501FeedbackTextBox, context.DB501.ToFeedback.ToString(), _isDB501Dirty);
        }

        private void SetTextIfNotEdited(TextBox textBox, string value, bool isDirty)
        {
            if (isDirty || textBox.IsKeyboardFocusWithin)
                return;

            _isRefreshingPlcStatus = true;
            try
            {
                textBox.Text = value;
            }
            finally
            {
                _isRefreshingPlcStatus = false;
            }
        }

        private void DbValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isRefreshingPlcStatus)
                return;

            if (sender == DB500ActionTextBox)
                _isDB500Dirty = true;
            else if (sender == DB501FeedbackTextBox)
                _isDB501Dirty = true;
        }

        private async void WriteDB500_Click(object sender, RoutedEventArgs e)
        {
            await WriteDbByteAsync(false, DB500ActionTextBox);
        }

        private async void WriteDB501_Click(object sender, RoutedEventArgs e)
        {
            await WriteDbByteAsync(true, DB501FeedbackTextBox);
        }

        private async Task WriteDbByteAsync(bool isFeedback, TextBox textBox)
        {
            if (!byte.TryParse(textBox.Text, out var value))
            {
                MessageBox.Show("请输入 0 到 255 之间的整数。", "输入无效", MessageBoxButton.OK, MessageBoxImage.Warning);
                textBox.Focus();
                textBox.SelectAll();
                return;
            }

            if (!MainStorage.IsPlcConnected)
            {
                MessageBox.Show("PLC未连接，无法写入。", "写入失败", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!int.TryParse(PlcWriteDbBaseTextBox.Text, out var dbBase)
                || dbBase < 1
                || dbBase > 65534
                || !int.TryParse(PlcWriteByteOffsetTextBox.Text, out var offset)
                || offset < 0
                || offset > 65535)
            {
                MessageBox.Show("DB块请输入 1 到 65534 的整数，字节偏移请输入 0 到 65535 的整数。", "地址无效", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var db = isFeedback ? dbBase + 1 : dbBase;


            var signalName = isFeedback
                ? $"DB{db}.DBB{offset} ToFeedback"
                : $"DB{db}.DBB{offset} Action";
            var taskName = value switch
            {
                0 => "空闲",
                1 => "扫码任务",
                2 => "托盘离开",
                _ => "未知任务"
            };
            var message = !isFeedback
                ? $"即将向PLC写入 {signalName}={value}（{taskName}）。动作1或2可能直接触发软件任务，是否继续？"
                : $"即将向PLC写入 {signalName}={value}，是否继续？";

            if (MessageBox.Show(message, "确认写入PLC", MessageBoxButton.OKCancel, MessageBoxImage.Warning) != MessageBoxResult.OK)
                return;

            var context = App.GetService<CCDContext>();
            ViewModel.Saves.PlcWriteDbBase = dbBase;
            ViewModel.Saves.PlcWriteByteOffset = offset;
            if (string.IsNullOrWhiteSpace(context.PlcGuid))
            {
                MessageBox.Show("PLC连接尚未就绪，无法写入。", "写入失败", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var written = await App.GetService<IMediator>().Send(
                    new S7WriteDataBlockBytesCommand(context.PlcGuid, db, offset, value));
                if (!written)
                {
                    MessageBox.Show($"写入 {signalName} 失败。", "写入失败", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!isFeedback && db == 500 && offset == 1)
                {
                    context.DB500.Action = value;
                    _isDB500Dirty = false;
                }
                else if (isFeedback && db == 501 && offset == 1)
                {
                    context.DB501.ToFeedback = value;
                    _isDB501Dirty = false;
                }
                else if (isFeedback)
                    _isDB501Dirty = false;
                else
                    _isDB500Dirty = false;

                RefreshPlcStatus();
                MessageBox.Show($"已写入 {signalName}={value}。", "写入成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"写入 {signalName} 时发生错误：{ex.Message}", "写入失败", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        */
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(PlcWriteDbBaseTextBox.Text, out var dbBase)
                || dbBase < 1
                || dbBase > 65534
                || !int.TryParse(PlcWriteByteOffsetTextBox.Text, out var offset)
                || offset < 0
                || offset > 65535)
            {
                MessageBox.Show("DB块请输入 1 到 65534 的整数，字节偏移请输入 0 到 65535 的整数。", "地址无效", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ViewModel.Saves.PlcWriteDbBase = dbBase;
            ViewModel.Saves.PlcWriteByteOffset = offset;
            MainStorage.SaveManager.Save();

            SaveButton.Content = "√ 保存成功";
            SaveButton.IsEnabled = false;
            await Task.Delay(1000);
            SaveButton.Content = "保存设定";
            SaveButton.IsEnabled = true;
        }

        /// <summary>
        /// 权限配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetPower_Click(object sender, RoutedEventArgs e)
        {
            new PowerSettingWindows().ShowDialog();
        }

    }
}
