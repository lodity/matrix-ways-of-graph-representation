using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
        int EdgeOffset;
        readonly double HalfOfWindowWidth;
        readonly double HalfOfWindowHeight;
        public ControlEdge(ControlNode nodeStart, ControlNode nodeEnd, Point window, bool isLoop, int number, int edgeOffset)
        {
            InitializeComponent();

            HalfOfWindowWidth = window.X / 2;
            HalfOfWindowHeight = window.Y / 2;

            EdgeEnd.Size = new Size(800 + edgeOffset * 200, 800 + edgeOffset * 200);
            EdgeName.Text = $"{number}";
            nodeStart.Moved += UpdateHandler;
            nodeEnd.Moved += UpdateHandler;

            EdgeOffset = edgeOffset;
            this.nodeStart = nodeStart;
            this.nodeEnd = nodeEnd;
            this.window = window;
            this.isLoop = isLoop;

            UpdatePosition();
        }
        private void UpdateHandler(object sender, RoutedEventArgs e) => UpdatePosition();
        private void UpdatePosition()
        {
            EdgeStart.StartPoint = new Point(nodeStart.Position.X + HalfOfWindowWidth - 10, nodeStart.Position.Y + HalfOfWindowHeight - 10);
            if (isLoop)
            {
                EdgeEnd.Point = new Point(nodeStart.Position.X + HalfOfWindowWidth - 5, nodeStart.Position.Y + HalfOfWindowHeight - 5);
                EdgeEnd.Size = new Size(25, 25);
                EdgeEnd.IsLargeArc = true;
                SetTextPoint(new Point(nodeStart.Position.X + 25, nodeStart.Position.Y + 35));
            }
            else
            {
                EdgeEnd.Point = new Point(nodeEnd.Position.X + HalfOfWindowWidth - 5, nodeEnd.Position.Y + HalfOfWindowHeight - 5);

                double halfOfStraightLength =
                Math.Sqrt(Math.Pow(nodeStart.Position.X - nodeEnd.Position.X, 2) + Math.Pow(nodeStart.Position.Y - nodeEnd.Position.Y, 2)) / 2;
                double offset = (800 + EdgeOffset * 200) - Math.Sqrt(Math.Pow((800 + EdgeOffset * 200), 2) - Math.Pow(halfOfStraightLength, 2));

                Vector ortogonal = new Vector(-nodeEnd.Position.Y + nodeStart.Position.Y, nodeEnd.Position.X - nodeStart.Position.X);
                ortogonal.Normalize();
                ortogonal.Negate();
                Point textPos = new Point(((nodeEnd.Position.X + nodeStart.Position.X) / 2) + ortogonal.X * offset + 5,
                                            ((nodeEnd.Position.Y + nodeStart.Position.Y) / 2) + ortogonal.Y * offset + 5);
                SetTextPoint(textPos);
            }
        }
        private void SetTextPoint(Point pos)
        {
            EdgeName.RenderTransform = new TranslateTransform
            {
                X = pos.X,
                Y = pos.Y
            };
        }
    }
}
