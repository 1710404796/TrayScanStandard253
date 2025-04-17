using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TrayScanStandard.Models;

namespace TrayScanStandard.ViewModel
{
    public partial class BcrBorderViewModel: ObservableRecipient
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor("BorderBrush")]
        [NotifyPropertyChangedFor("ConnectText")]
        private bool _isConnect = false;

        public Brush BorderBrush => IsConnect ? Brushes.Green : Brushes.Red;
        public string ConnectText => IsConnect ? Properties.Resources.Connected : Properties.Resources.Disconnect;

        public required BcrInfo BcrInfo { get; set; }

        public int CameraIdx => Image2DViewModel?.CameraIdx ?? 0;

        public Image2DViewModel? Image2DViewModel { get; set; }



    }
}
