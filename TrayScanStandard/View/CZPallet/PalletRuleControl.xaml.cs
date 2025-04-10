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
using TrayScanStandard.Models.CZPallet;

namespace TrayScanStandard.View.CZPallet
{
    /// <summary>
    /// PalletRuleControl.xaml 的交互逻辑
    /// </summary>
    public partial class PalletRuleControl : UserControl
    {
        //public PalletRuleControlViewModel ViewModel { get; set; }
        public Action<PalletRuleControl> OnDelete { get; set; }
        public PalletTypeRule PalletTypeRule { get; }

        public PalletRuleControl(PalletTypeRule palletTypeRule)
        {
            PalletTypeRule = palletTypeRule;
            DataContext = this;
            //ViewModel = palletRuleControlViewModel;

            InitializeComponent();
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            // 思考这个delete
            var res = MessageBox.Show(
                //"确认要删除此记录?"
                Properties.Resources.AreYouSureYouWantToDeleteThisRecord

                ,
                Properties.Resources.DeleteConfirmation
                , MessageBoxButton.YesNo);
            if (res == MessageBoxResult.Yes)
            {
                OnDelete?.Invoke(this);

            }
        }

        private void SetBattery_Click(object sender, RoutedEventArgs e)
        {
            var aa = new BatteryRuleEdit(PalletTypeRule);
            var res = aa.ShowDialog();
        }
    }
}
