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
        public ControlNode NodeStart;
        public ControlNode NodeEnd;
        public bool IsLoop;
        int EdgeEndRadius;
        readonly double HalfOfWindowWidth;
        readonly double HalfOfWindowHeight;
        public ControlEdge(ControlNode nodeStart, ControlNode nodeEnd, Point window, bool isLoop, int number, int edgeMultipleOffset)
        {
            InitializeComponent();

            HalfOfWindowWidth = window.X / 2;
            HalfOfWindowHeight = window.Y / 2;

            EdgeEndRadius = 800 + edgeMultipleOffset * 200;
            EdgeEnd.Size = new Size(EdgeEndRadius, EdgeEndRadius);
            EdgeName.Text = $"{number}";
            nodeStart.Moved += UpdateHandler;
            nodeEnd.Moved += UpdateHandler;

            NodeStart = nodeStart;
            NodeEnd = nodeEnd;
            IsLoop = isLoop;

            UpdatePosition();
        }
        private void UpdateHandler(object sender, RoutedEventArgs e) => UpdatePosition();
        private void UpdatePosition()
        {
            EdgeStart.StartPoint = new(NodeStart.Position.X + HalfOfWindowWidth - 10, NodeStart.Position.Y + HalfOfWindowHeight - 10);
            if (IsLoop)
            {
                EdgeEnd.Point = new(NodeStart.Position.X + HalfOfWindowWidth - 5, NodeStart.Position.Y + HalfOfWindowHeight - 5);
                EdgeEnd.Size = new(25, 25);
                EdgeEnd.IsLargeArc = true;
                SetTextPoint(new Point(NodeStart.Position.X + 25, NodeStart.Position.Y + 35));
            }
            else
            {
                EdgeEnd.Point = new(NodeEnd.Position.X + HalfOfWindowWidth - 5, NodeEnd.Position.Y + HalfOfWindowHeight - 5);

                double halfOfStraightLength =
                Math.Sqrt(Math.Pow(NodeStart.Position.X - NodeEnd.Position.X, 2) + Math.Pow(NodeStart.Position.Y - NodeEnd.Position.Y, 2)) / 2;
                double offset = EdgeEndRadius - Math.Sqrt(Math.Pow(EdgeEndRadius, 2) - Math.Pow(halfOfStraightLength, 2));

                Vector ortogonal = new(-NodeEnd.Position.Y + NodeStart.Position.Y, NodeEnd.Position.X - NodeStart.Position.X);
                ortogonal.Normalize();
                ortogonal.Negate();
                Point textPos = new(((NodeEnd.Position.X + NodeStart.Position.X) / 2) + ortogonal.X * offset + 5,
                                            ((NodeEnd.Position.Y + NodeStart.Position.Y) / 2) + ortogonal.Y * offset + 5);
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
