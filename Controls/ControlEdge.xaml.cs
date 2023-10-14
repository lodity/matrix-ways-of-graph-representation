using CDM_Lab_3._1.Models;
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
        GraphType GraphTypeCurrent;
        public bool IsLoop;
        private bool IsSingleOriented;
        double EdgeOffset;
        readonly int EdgeOffsetMax;
        readonly double HalfOfWindowWidth;
        readonly double HalfOfWindowHeight;

        double NodeStartPosX;
        double NodeStartPosY;
        double NodeEndPosX;
        double NodeEndPosY;
        readonly Vector vectorX = new(1, 0);


        public ControlEdge(ControlNode nodeStart, ControlNode nodeEnd, Point window, bool isLoop, int number, double edgeOffset, int edgeOffsetMax, GraphType graphTypeCurrent, bool isSingleOriented)
        {
            InitializeComponent();

            HalfOfWindowWidth = window.X / 2;
            HalfOfWindowHeight = window.Y / 2;
            EdgeOffset = edgeOffset;
            EdgeName.Text = $"{number}";
            nodeStart.Moved += UpdateHandler;
            nodeEnd.Moved += UpdateHandler;

            GraphTypeCurrent = graphTypeCurrent;
            IsSingleOriented = isSingleOriented;
            NodeStart = nodeStart;
            NodeEnd = nodeEnd;
            IsLoop = isLoop;
            EdgeOffsetMax = edgeOffsetMax;


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
                if (EdgeOffset == -1)
                {
                    EdgeOffset = EdgeOffsetMax;
                }
                double EdgeEndSizeCoeff = EdgeOffset * 1.8;
                EdgeEnd.IsLargeArc = true;
                EdgeEnd.Point = new(NodeStartPosX + HalfOfWindowWidth + 5, NodeStartPosY + HalfOfWindowHeight + 5);
                double radius = 25 + EdgeEndSizeCoeff;
                EdgeEnd.Size = new(radius, radius);
                SetTextPoint(new Point(NodeStartPosX + 30 + EdgeOffset * 6, NodeStartPosY + 25 - EdgeOffset * 5));
                // Arrow pos
                //TODO fix govnocode
                double angleInRadians = Math.Asin(10 / (radius / 2) / 2);
                if (EdgeEndSizeCoeff == 0)
                    angleInRadians *= 1.15;
                double x = Math.Cos(angleInRadians + 145 / (180 / Math.PI));
                double y = Math.Sin(angleInRadians + 145 / (180 / Math.PI));
                Vector vectorArrow = new(x, y);
                vectorArrow = new(-vectorArrow.Y, vectorArrow.X);
                vectorArrow.Normalize();

                SetArrowPoint(NodeEnd.Position + vectorArrow * 27, NodeEnd.Position + vectorArrow * 26);
            }
            else
            {
                double edgeEndRadius;
                if (EdgeOffset == -1)
                {
                    edgeEndRadius = 500 + (EdgeOffsetMax) * 300;
                }
                else
                {
                    edgeEndRadius = 700 + EdgeOffset / EdgeOffsetMax * 1200;
                }
                EdgeEnd.Size = new Size(edgeEndRadius, edgeEndRadius);

                EdgeEnd.Point = new(NodeEndPosX + HalfOfWindowWidth, NodeEndPosY + HalfOfWindowHeight);

                double straightLength =
                    Math.Sqrt(Math.Pow(NodeStartPosX - NodeEndPosX, 2) + Math.Pow(NodeStartPosY - NodeEndPosY, 2));
                double halfOfStraightLength = straightLength / 2;
                double offsetFromCenter = Math.Sqrt(Math.Pow(edgeEndRadius, 2) - Math.Pow(halfOfStraightLength, 2));
                double offset = double.IsNaN(offsetFromCenter) ? straightLength / 2 : edgeEndRadius - offsetFromCenter;

                Vector ortogonal = new(-NodeEndPosY + NodeStartPosY, NodeEndPosX - NodeStartPosX);
                ortogonal.Normalize();
                ortogonal.Negate();
                // TODO textPos depends on EdgeMultipleOffset
                Point textPos = new(((NodeEndPosX + NodeStartPosX) / 2) + ortogonal.X * offset,
                                            ((NodeEndPosY + NodeStartPosY) / 2) + ortogonal.Y * offset);
                SetTextPoint(textPos);


                if (IsSingleOriented)
                {
                    Vector straightVector = new(NodeEndPosX - NodeStartPosX, NodeEndPosY - NodeStartPosY);
                    double angleToRadius = Math.Atan2(offsetFromCenter, halfOfStraightLength);
                    if (double.IsNaN(angleToRadius)) angleToRadius = 0;
                    double angleOfArc = Vector.AngleBetween(straightVector, vectorX) * Math.PI / 180.0;
                    double angle = angleToRadius + angleOfArc;
                    Vector arrow = new(Math.Sin(angle), Math.Cos(angle));
                    Point ArrowEndPoint = new(
                        NodeEnd.Position.X + arrow.X * -27,
                        NodeEnd.Position.Y + arrow.Y * -27
                        );
                    Point ArrowStartPoint = new(
                        NodeEnd.Position.X + arrow.X * -28,
                        NodeEnd.Position.Y + arrow.Y * -28
                        );

                    SetArrowPoint(ArrowStartPoint, ArrowEndPoint);
                }
                else
                    Arrow.Visibility = Visibility.Collapsed;
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
        private void SetArrowPoint(Point start, Point end)
        {
            ArrowStart.StartPoint = new(start.X + HalfOfWindowWidth, start.Y + HalfOfWindowHeight);
            ArrowEnd.Point = new(end.X + HalfOfWindowWidth, end.Y + HalfOfWindowHeight);
        }
        private void EdgeName_MouseEnter(object sender, MouseEventArgs e) { Panel.SetZIndex(this, 999); }
        private void EdgeName_MouseLeave(object sender, MouseEventArgs e) { Panel.SetZIndex(this, 1); }
    }
}
