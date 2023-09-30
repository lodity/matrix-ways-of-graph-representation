using System.Windows;
using System.Windows.Controls;

namespace CDM_Lab_3._1.Controls
{
    /// <summary>
    /// Interaction logic for ControlEdge.xaml
    /// </summary>
    public partial class ControlEdge : UserControl
    {
        public ControlNode nodeStart;
        public ControlNode nodeEnd;
        public Point window;
        public bool isLoop;
        public ControlEdge(ControlNode nodeStart, ControlNode nodeEnd, Point window, bool isLoop)
        {
            InitializeComponent();

            nodeStart.Moved += UpdateHandler;
            nodeEnd.Moved += UpdateHandler;

            this.nodeStart = nodeStart;
            this.nodeEnd = nodeEnd;
            this.window = window;
            this.isLoop = isLoop;

            UpdatePosition();
        }
        private void UpdateHandler(object sender, RoutedEventArgs e) => UpdatePosition();
        private void UpdatePosition()
        {
            EdgeStart.StartPoint = new Point(nodeStart.Position.X + window.X / 2 - 10, nodeStart.Position.Y + window.Y / 2 - 10);
            if (isLoop)
            {
                EdgeEnd.Point = new Point(nodeStart.Position.X + window.X / 2 - 5, nodeStart.Position.Y + window.Y / 2 - 5);
                EdgeEnd.Size = new Size(25, 25);
                EdgeEnd.IsLargeArc = true;
            }
            else
            {
                EdgeEnd.Point = new Point(nodeEnd.Position.X + window.X / 2 - 5, nodeEnd.Position.Y + window.Y / 2 - 5);
            }
        }
    }
}
