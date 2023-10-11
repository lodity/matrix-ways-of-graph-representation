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
        public bool isSeleted;
        public int CountOfLoops;
        public event RoutedEventHandler? Moved;
        public event RoutedEventHandler? Selected;
        private Point CurrentPos;
        private Point CurrentMousePosition;
        public int index;
        public ControlNode(int index, Point point)
        {
            InitializeComponent();
            isSeleted = false;
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
        public void Select(bool isSelected)
        {
            if (isSelected)
            {
                ChangeBorderColor(new BrushConverter().ConvertFrom("#45b500") as SolidColorBrush);
                ChangeBorderThickness(new Thickness(3));
            }
            else
            {
                ChangeBorderColor(Brushes.White);
                ChangeBorderThickness(new Thickness(1));
            }
        }
        private void ChangeBorderColor(SolidColorBrush borderColor)
        {
            NodeBorder.BorderBrush = borderColor;
        }
        private void ChangeBorderThickness(Thickness thickness)
        {
            NodeBorder.BorderThickness = thickness;
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
        private void NodeBorder_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Selected?.Invoke(sender, e);
        }
    }
}
