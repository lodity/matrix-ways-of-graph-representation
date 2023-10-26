using CDM_Lab_3._1.Models.Graph;
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
        public event RoutedEventHandler? SelectToAddEdge;
        public event RoutedEventHandler? SelectToRemoveEdge;
        private Point CurrentPos;
        private Point CurrentMousePosition;
        public int index;
        public Node node;
        public ControlNode(Node node, Point point)
        {
            InitializeComponent();
            isSeleted = false;
            Panel.SetZIndex(this, 2);
            this.node = node;
            this.index = node.Id;
            Text.Text = $"x{index}";
            RenderTransform = new TranslateTransform();
            ((TranslateTransform)RenderTransform).X = point.X;
            ((TranslateTransform)RenderTransform).Y = point.Y;
        }
        public Point Position
        {
            get => new(((TranslateTransform)RenderTransform).X, ((TranslateTransform)RenderTransform).Y);
        }
        public enum NodeSelectType
        {
            Add, Remove
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
        public void SelectAddEdge()
        {
            ChangeBorderColor(new BrushConverter().ConvertFrom("#45b500") as SolidColorBrush);
            ChangeBorderThickness(new Thickness(3));
        }
        public void SelectRemoveEdge()
        {
            ChangeBorderColor(new BrushConverter().ConvertFrom("#b50000") as SolidColorBrush);
            ChangeBorderThickness(new Thickness(3));
        }
        public void SelectClear()
        {
            ChangeBorderColor(Brushes.White);
            ChangeBorderThickness(new Thickness(1));
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
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                SelectToRemoveEdge?.Invoke(sender, e);
                return;
            }
            SelectToAddEdge?.Invoke(sender, e);
        }
    }
}
