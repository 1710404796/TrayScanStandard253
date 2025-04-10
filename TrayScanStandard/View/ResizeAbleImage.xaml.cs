
using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;
using System.Windows.Media;


namespace TrayScanStandard.View
{
    /// <summary>
    /// ResizeAbleImage.xaml 的交互逻辑
    /// </summary>
    public partial class ResizeAbleImage : UserControl
    {



        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(ResizeAbleImage), new PropertyMetadata(null, OnImageSourceChanged));

        private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ResizeAbleImage)d;
            control.img.Source = (ImageSource)e.NewValue;
        }
        public ResizeAbleImage()
        {
            InitializeComponent();
        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed || ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control))
            {
                if (e.Delta > 0)
                {
                    GTran.ScaleX *= 1.05;
                    GTran.ScaleY *= 1.05;
                }
                else
                {
                    GTran.ScaleX /= 1.05;
                    GTran.ScaleY /= 1.05;
                }
            }
        }
    }
}
