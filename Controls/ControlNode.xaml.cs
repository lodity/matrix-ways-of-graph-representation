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
        public event RoutedEventHandler? Moved;
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
            ((TranslateTransform)RenderTransform).X = point.X;
            ((TranslateTransform)RenderTransform).Y = point.Y;
        }
        public Point Position
        {
            get => new(((TranslateTransform)RenderTransform).X, ((TranslateTransform)RenderTransform).Y);
        }
        private void NodeBorder_MouseMove(object sender, MouseEventArgs e)
        {
            Vector diff = e.GetPosition(Parent as Window) - CurrentMousePosition;
            if (NodeBorder.IsMouseCaptured)
            {
                ((TranslateTransform)RenderTransform).X += diff.X;
                ((TranslateTransform)RenderTransform).Y += diff.Y;
                CurrentPos.X = ((TranslateTransform)RenderTransform).X;
                CurrentPos.Y = ((TranslateTransform)RenderTransform).Y;
                CurrentMousePosition = e.GetPosition(Parent as Window);
                //Runtime updating arcs
                Moved?.Invoke(sender, e);
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
