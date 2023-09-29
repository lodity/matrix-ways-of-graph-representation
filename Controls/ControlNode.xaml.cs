using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CDM_Lab_3._1.Controls
{
    /// <summary>
    /// Interaction logic for ControlNode.xaml
    /// </summary>
    public partial class ControlNode : UserControl
    {
        private Point CurrentPos;
        private Point CurrentMousePosition;
        public int index;
        public ControlNode(int index, Point point)
        {
            InitializeComponent();
            Panel.SetZIndex(this, 2);
            this.index = index;
            Text.Text = $"x{index}";
            RenderTransform = new TranslateTransform();
            (RenderTransform as TranslateTransform).X = point.X;
            (RenderTransform as TranslateTransform).Y = point.Y;
        }
        public Point Position
        {
            get
            {
                Point res = new Point();
                res = new Point((RenderTransform as TranslateTransform).X, (RenderTransform as TranslateTransform).Y);
                return res;
            }
        }
        private void NodeBorder_MouseMove(object sender, MouseEventArgs e)
        {
            Vector diff = e.GetPosition(Parent as Window) - CurrentMousePosition;
            if (NodeBorder.IsMouseCaptured)
            {
                (RenderTransform as TranslateTransform).X += diff.X;
                (RenderTransform as TranslateTransform).Y += diff.Y;
                CurrentPos.X = (RenderTransform as TranslateTransform).X;
                CurrentPos.Y = (RenderTransform as TranslateTransform).Y;
                CurrentMousePosition = e.GetPosition(Parent as Window);
                // Runtime updating arcs
                //Moved?.Invoke(sender, e);
            }
        }

        private void NodeBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CurrentMousePosition = e.GetPosition(Parent as Window);
            NodeBorder.CaptureMouse();
        }
        private void NodeBorder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (NodeBorder.IsMouseCaptured)
                NodeBorder.ReleaseMouseCapture();
        }
    }
}
