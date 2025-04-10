using System.Windows.Controls;
using TrayScanStandard.Models;

namespace TrayScanStandard.View.CZPallet
{
    /// <summary>
    /// GongWeiView.xaml 的交互逻辑
    /// </summary>
    public partial class GongWeiView : UserControl
    {
        //public GongWeiViewModel ViewModel { get; init; }

        public XYLStation Station { get; set; }


        public string Title { get; set; }
        public GongWeiView(XYLStation station)
        {
            Station = station;
            DataContext = this;
            //ViewModel = App.GetService<GongWeiViewModel>();
            Title = $"{Properties.Resources.DispatchNumber} ：{Station.InsideNum} {Station.Name} {Properties.Resources.WorkstationNumber}{Station.OutsideStationNum}";
            InitializeComponent();
            Pallet1.Station = Station;

        }
    }
}
