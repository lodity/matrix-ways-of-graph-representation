using System;
using System.Windows;
using System.Windows.Controls;
using static CDM_Lab_3._1.Models.Graph.Node;

namespace CDM_Lab_3._1.Controls
{
    /// <summary>
    /// Interaction logic for SetWeight.xaml
    /// </summary>
    public partial class SetWeight : UserControl
    {
        readonly ControlEdge controlEdge;

        public SetWeight(ControlEdge controlEdge)
        {
            InitializeComponent();
            this.controlEdge = controlEdge;
            NodeFrom.Text = $"x{controlEdge.NodeStart.index}";
            NodeTo.Text = $"x{controlEdge.NodeEnd.index}";
            Edge.Text = $"a{controlEdge.Id}";
            Weight.Text = controlEdge.Weight.ToString();
        }

        private void Increment_Click(object sender, RoutedEventArgs e)
        {
            foreach (Tuple<int, EdgeType, int> edge in controlEdge.NodeStart.node.Edges)
            {
                if (controlEdge.NodeStart.node.HasChild(edge.Item1))
                {
                    controlEdge.Weight++;
                    Weight.Text = controlEdge.Weight.ToString();
                    break;
                }
            }
        }
        private void Decrement_Click(object sender, RoutedEventArgs e)
        {
            if (controlEdge.Weight == 0) return;
            foreach (Tuple<int, EdgeType, int> edge in controlEdge.NodeStart.node.Edges)
            {
                if (controlEdge.NodeStart.node.HasChild(edge.Item1))
                {
                    controlEdge.Weight--;
                    Weight.Text = controlEdge.Weight.ToString();
                    break;
                }
            }
        }
    }
}
