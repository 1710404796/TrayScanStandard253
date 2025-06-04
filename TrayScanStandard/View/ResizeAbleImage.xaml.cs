using System.Linq;
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
        private bool _isDragging = false;
        private Point _lastPosition;

        public Canvas BorderCanvas => borderCanvas;
        
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
            GTran.ScaleX = 0.5;
            GTran.ScaleY = 0.5;
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

        private void Grid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            if (grid != null)
            {
                _isDragging = true;
                _lastPosition = e.GetPosition(this);
                grid.CaptureMouse();
                grid.Cursor = Cursors.Hand;
            }
        }

        private void Grid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            if (grid != null && _isDragging)
            {
                _isDragging = false;
                grid.ReleaseMouseCapture();
                grid.Cursor = Cursors.Arrow;
            }
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && e.RightButton == MouseButtonState.Pressed)
            {
                var currentPosition = e.GetPosition(this);
                var deltaX = currentPosition.X - _lastPosition.X;
                var deltaY = currentPosition.Y - _lastPosition.Y;

                // 获取当前的变换
                var transform = BImage.RenderTransform as TransformGroup;
                if (transform == null)
                {
                    transform = new TransformGroup();
                    // 保留原有的旋转变换
                    transform.Children.Add(new RotateTransform());
                    BImage.RenderTransform = transform;
                }

                var translateTransform = transform.Children.OfType<TranslateTransform>().FirstOrDefault();
                if (translateTransform == null)
                {
                    translateTransform = new TranslateTransform();
                    transform.Children.Add(translateTransform);
                }

                translateTransform.X += deltaX;
                translateTransform.Y += deltaY;

                _lastPosition = currentPosition;
            }
        }
    }
}
