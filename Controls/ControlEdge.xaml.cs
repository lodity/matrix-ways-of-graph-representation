using System.Windows;
using System.Windows.Controls;

namespace CDM_Lab_3._1.Controls
{
    /// <summary>
    /// Interaction logic for ControlEdge.xaml
    /// </summary>
    public partial class ControlEdge : UserControl
    {
        public ControlEdge(Point posStart, Point posEnd, Point window)
        {
            InitializeComponent();

            EdgeStart.StartPoint = new Point(posStart.X + window.X / 2, posStart.Y + window.Y / 2);
            EdgeEnd.Point = new Point(posEnd.X + window.X / 2, posEnd.Y + window.Y / 2);
        }
    }
}
