using CDM_Lab_3._1.Controls;
using System.Collections.Generic;
using System.Windows;

namespace CDM_Lab_3._1.View
{
    /// <summary>
    /// Interaction logic for SetWeightsWindow.xaml
    /// </summary>
    public partial class SetWeightsWindow : Window
    {
        readonly List<ControlEdge> controlEdges;
        public SetWeightsWindow(List<ControlNode> controlNodes)
        {
            InitializeComponent();

            controlEdges = new();

            foreach (ControlNode node in controlNodes)
                foreach (ControlEdge edge in node.ControlEdges)
                    if (!controlEdges.Contains(edge))
                        controlEdges.Add(edge);

            foreach (ControlEdge edge in controlEdges)
            {
                Field.Children.Add(new SetWeight(edge));
            }
        }
    }
}
