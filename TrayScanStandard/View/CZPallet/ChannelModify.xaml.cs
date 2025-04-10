using System.Windows;
using TrayScanStandard.Models;


namespace TrayScanStandard.View.CZPallet
{
    /// <summary>
    /// ChannelModify.xaml 的交互逻辑
    /// </summary>
    public partial class ChannelModify : Window
    {
        private readonly int id;

        public string Title { get; set; } = "修改通道信息";

        /// <summary>
        /// 用于修改code
        /// </summary>
        public ChannelModify(XYLStation diaoDuStage, int id)
        {
            DataContext = this;
            DiaoDuStage = diaoDuStage;
            this.id = id;
            StageChannel = diaoDuStage.Channels[id];

            InitializeComponent();
        }
        public int Tid => StageChannel.Location + 1;
        public StationChannel StageChannel { get; }
        public XYLStation DiaoDuStage { get; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Ti.Text = $"{Properties.Resources.DispatchNumber}：{DiaoDuStage.InsideNum} {DiaoDuStage.Name} {Properties.Resources.Channel}：{Tid}";

            //DiaoDuStage.OnNG += ShowMessage;
            //DiaoDuStage.OnWarnning += ShowMessage;
        }
        /// <summary>
        /// 保存结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            // 强制修改条码限制取消
            if (await DiaoDuStage.UpdateBattery(id, CodeBox.Text, true))
            {
                MessageBox.Show($"{Properties.Resources.ModificationSuccessful}!");
                Close();
            }
            else
            {
                MessageBox.Show($"{Properties.Resources.ModificationFailed}!");


            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {

            //DiaoDuStage.OnNG -= ShowMessage;
            //DiaoDuStage.OnWarnning -= ShowMessage;

        }

        void ShowMessage(object sender, string message)
        {
            MessageBox.Show(message);
        }
    }
}
