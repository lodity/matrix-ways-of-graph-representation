using CDM_Lab_3._1.Controls;
using CDM_Lab_3._1.Models.Graph;
using System;
using System.Collections.Generic;
using System.Windows;

namespace CDM_Lab_3._1.View
{
    /// <summary>
    /// Interaction logic for GraphWindow.xaml
    /// </summary>
    public partial class GraphWindow : Window
    {
        Random randomGlobal = new();
        Graph _graph;
        int horizontalSectorsMax;
        int verticalSectorsMax;
        bool[,] nodeSectors;
        ControlNode[] controlNodes;
        public GraphWindow(Graph graph)
        {
            InitializeComponent();

            _graph = graph;
            controlNodes = new ControlNode[_graph.Count];
            horizontalSectorsMax = (int)Math.Floor(Width / 40) - 1;
            verticalSectorsMax = (int)Math.Floor(Height / 40) - 1;
            nodeSectors = new bool[horizontalSectorsMax, verticalSectorsMax];

            BuildNodes();
            for (int i = 0; i < controlNodes.Length; i++)
            {
                Dictionary<int, Node> children = _graph.Nodes[controlNodes[i].index].Children;
                foreach (var key in children.Keys)
                {
                    ControlEdge controlEdge = new(controlNodes[i], controlNodes[children[key].Id], new Point(Width, Height), children[key].Id == _graph.Nodes[controlNodes[i].index].Id);
                    Field.Children.Add(controlEdge);
                }
            }
        }
        private int RandomPosition(int posMax)
        {
            int num = randomGlobal.Next(posMax);
            return num;
        }
        private void BuildNodes()
        {
            for (int i = 0; i < _graph.Count; i++)
            {
                int horizontalPos = RandomPosition(horizontalSectorsMax);
                int verticalPos = RandomPosition(verticalSectorsMax);

                if (!nodeSectors[horizontalPos, verticalPos])
                    nodeSectors[horizontalPos, verticalPos] = true;
                else
                    for (int x = horizontalPos; x < horizontalSectorsMax; x++)
                        for (int y = verticalPos; y < verticalSectorsMax; y++)
                            if (!nodeSectors[x, y])
                            {
                                horizontalPos = x;
                                verticalPos = y;
                                nodeSectors[horizontalPos, verticalPos] = true;
                                break;
                            }
                ControlNode controlNode = new(i, new Point((horizontalPos + 1) * 40 - Width / 2, (verticalPos + 1) * 40 - Height / 2));
                controlNodes[i] = controlNode;
                Field.Children.Add(controlNode);
            }
        }
    }
}
