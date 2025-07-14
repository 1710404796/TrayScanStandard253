using LanguageExt.Pipes;
using LinxUniverse.DI;
using MediatR;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TrayScanStandard.Attritubes;
using TrayScanStandard.Mediator.Queries;
using TrayScanStandard.View;
using TrayScanStandard.View.CZPallet;
using TrayScanStandard.ViewModel;
using VMWebAIClient;

namespace TrayScanStandard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel
        {
            get; set;
        }
        public IVMWebAIClient VMClient { get; }

        public MainWindow()
        {
            MainStorage.SaveManager.Load();
            ChangeLanguage();

            DataContext = this;
            ViewModel = App.GetService<MainViewModel>();
            VMClient = App.GetService<IVMWebAIClient>();
            InitializeComponent();
        }

        private static void ChangeLanguage()
        {
            switch (MainStorage.Saves.Lang)
            {
                case 0:
                    Properties.Resources.Culture = new System.Globalization.CultureInfo("zh-CN");
                    break;
                case 1:
                    Properties.Resources.Culture = new System.Globalization.CultureInfo("en-US");
                    break;
                case 2:
                    Properties.Resources.Culture = new System.Globalization.CultureInfo("hu");
                    break;
                default:
                    break;
            }
        }

        //public static void NageTo<T>() where T : FrameworkElement
        //{
        //    (App.Current.MainWindow as MainWindow)!.ContentFrame.Content = App.GetService<T>();
        //}
        public static async void NageTo(FrameworkElement frameworkElement)
        {
            var mediator1 = App.GetService<IMediator>();

            var tt = frameworkElement.GetType().GetCustomAttribute<PowerViewAttribute>();
            if (tt != null)
            {
                if (await mediator1.Send(new CheckPowerQuery(tt.Power)))
                {

                }
                else
                {
                    MessageBox.Show("当前没有权限访问此页面!");
                    (App.Current.MainWindow as MainWindow)!.ContentFrame.Content = null;
                    return;
                }
            }
                (App.Current.MainWindow as MainWindow)!.ContentFrame.Content = frameworkElement;
        }

        public static async void NageTo<T>() where T : FrameworkElement
        {
            var mediator1 = App.GetService<IMediator>();

            var tt = typeof(T).GetCustomAttribute<PowerViewAttribute>();
            if (tt != null)
            {
                if (await mediator1.Send(new CheckPowerQuery(tt.Power)))
                {

                }
                else
                {
                    MessageBox.Show("当前没有权限访问此页面!");
                    (App.Current.MainWindow as MainWindow)!.ContentFrame.Content = null;
                    return;
                }
            }
            if ((App.Current.MainWindow as MainWindow)!.ContentFrame.Content is not T)
                (App.Current.MainWindow as MainWindow)!.ContentFrame.Content = App.GetService<T>();
        }



        private void LogDash_Click(object sender, RoutedEventArgs e)
        {
            NageTo<LogDashBoardView>();
        }

        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            NageTo<SettingView>();

        }

        private void PowerManager_Click(object sender, RoutedEventArgs e)
        {
            NageTo<UserManagerView>();

        }
        private void Window_Closed(object sender, EventArgs e)
        {
            ViewModel.CacheService.Cancel();

            MainStorage.SaveManager.Save();
        }

        private void SelectLangBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainStorage.Saves.Lang == SelectLangBox.SelectedIndex)
            {
                return;
            }



            MainStorage.Saves.Lang = SelectLangBox.SelectedIndex;
            MainStorage.SaveManager.Save();

            switch (SelectLangBox.SelectedIndex)
            {
                case 0:
                    Properties.Resources.Culture = new System.Globalization.CultureInfo("zh-CN");
                    break;
                case 1:
                    Properties.Resources.Culture = new System.Globalization.CultureInfo("en-US");
                    break;
                case 2:
                    Properties.Resources.Culture = new System.Globalization.CultureInfo("hu");
                    break;
                default:
                    break;
            }

            MessageBox.Show(Properties.Resources.PleaseRestartTheSoftwareForTheChangesToTakeEffect, Properties.Resources.Prompt, MessageBoxButton.OK, MessageBoxImage.Information);

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SelectLangBox.SelectedIndex = MainStorage.Saves.Lang;
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.GetService<CacheService>().Cancel();

            var mediator = App.GetService<IMediator>();
            var cc = MessageBox.Show("确定要关闭吗", "关闭确认", MessageBoxButton.OKCancel);
            if (cc == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
                return;
            }
            if (await mediator.Send(new CheckPowerQuery(PowerEnum.关闭软件)))
            {
                await VMClient.Exit();
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
                MessageBox.Show("您当前没有权限关闭软件");
            }
        }

        private void StationView_Click(object sender, RoutedEventArgs e)
        {
            NageTo<YWStageView>();

        }

        private void LightBtn_Click(object sender, RoutedEventArgs e)
        {
            NageTo<LightManagerView>();

        }

        private void CameraList_Click(object sender, RoutedEventArgs e)
        {
            NageTo<AllBcrListView>();

        }

        private void BatteryManager_Click(object sender, RoutedEventArgs e)
        {
            NageTo<BatteryManager>();

        }

        private void ZPALgo_Click(object sender, RoutedEventArgs e)
        {
            NageTo<PalletLogView>();
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            NageTo<ImageDisplayView>();

        }
    }
}