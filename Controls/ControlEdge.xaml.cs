using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        double EdgeEndRadius;
        readonly double EdgeMultipleOffset;
        readonly int EdgeMultipleOffsetMax;
        readonly double HalfOfWindowWidth;
        readonly double HalfOfWindowHeight;

        double NodeStartPosX;
        double NodeStartPosY;
        double NodeEndPosX;
        double NodeEndPosY;

        public ControlEdge(ControlNode nodeStart, ControlNode nodeEnd, Point window, bool isLoop, int number, double edgeOffsetList, int edgeMultipleOffsetMax)
        {
            InitializeComponent();

            HalfOfWindowWidth = window.X / 2;
            HalfOfWindowHeight = window.Y / 2;
            EdgeMultipleOffset = edgeOffsetList;

            EdgeEndRadius = 700 + EdgeMultipleOffset / edgeMultipleOffsetMax * 1200;
            EdgeEnd.Size = new Size(EdgeEndRadius, EdgeEndRadius);
            EdgeName.Text = $"{number}";
            nodeStart.Moved += UpdateHandler;
            nodeEnd.Moved += UpdateHandler;

            NodeStart = nodeStart;
            NodeEnd = nodeEnd;
            IsLoop = isLoop;
            EdgeMultipleOffsetMax = edgeMultipleOffsetMax;

            UpdatePosition();
        }
        private void UpdateHandler(object sender, RoutedEventArgs e) => UpdatePosition();
        private void UpdatePosition()
        {
            NodeStartPosX = NodeStart.Position.X;
            NodeStartPosY = NodeStart.Position.Y;
            NodeEndPosX = NodeEnd.Position.X;
            NodeEndPosY = NodeEnd.Position.Y;

            EdgeStart.StartPoint = new Point(NodeStartPosX + HalfOfWindowWidth, NodeStartPosY + HalfOfWindowHeight);
            if (IsLoop)
            {
                double EdgeEndSizeCoeff = EdgeMultipleOffset * 1.8;
                double EdgeTextCoeff = EdgeMultipleOffset / EdgeMultipleOffsetMax;
                EdgeEnd.IsLargeArc = true;
                EdgeEnd.Point = new(NodeStartPosX + HalfOfWindowWidth + 5, NodeStartPosY + HalfOfWindowHeight + 5);
                EdgeEnd.Size = new(18 + EdgeEndSizeCoeff, 18 + EdgeEndSizeCoeff);
                SetTextPoint(new Point(NodeStartPosX + 30 + EdgeTextCoeff * 45, NodeStartPosY + 25 - EdgeTextCoeff * 37.5));
            }
            else
            {
                EdgeEnd.Point = new(NodeEndPosX + HalfOfWindowWidth, NodeEndPosY + HalfOfWindowHeight);

                double halfOfStraightLength =
                    Math.Sqrt(Math.Pow(NodeStartPosX - NodeEndPosX, 2) + Math.Pow(NodeStartPosY - NodeEndPosY, 2)) / 2;
                double offset = EdgeEndRadius - Math.Sqrt(Math.Pow(EdgeEndRadius, 2) - Math.Pow(halfOfStraightLength, 2));

                Vector ortogonal = new(-NodeEndPosY + NodeStartPosY, NodeEndPosX - NodeStartPosX);
                ortogonal.Normalize();
                ortogonal.Negate();
                // TODO textPos depends on EdgeMultipleOffset
                Point textPos = new(((NodeEndPosX + NodeStartPosX) / 2) + ortogonal.X * offset,
                                            ((NodeEndPosY + NodeStartPosY) / 2) + ortogonal.Y * offset);
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
        private void EdgeName_MouseEnter(object sender, MouseEventArgs e) { Panel.SetZIndex(this, 999); }
        private void EdgeName_MouseLeave(object sender, MouseEventArgs e) { Panel.SetZIndex(this, 1); }
    }
}
