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
            BuildEdges();
        }
        public Graph Graph
        {
            set
            {
                _graph = value;
                Field.Children.Clear();
                controlNodes = new ControlNode[_graph.Count];
                nodeSectors = new bool[horizontalSectorsMax, verticalSectorsMax];

                BuildNodes();
                BuildEdges();
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
                // FOR TEST
                if (i == 0)
                {
                    horizontalPos = 3;
                    verticalPos = 5;
                }
                else if (i == 1)
                {
                    horizontalPos = 4;
                    verticalPos = 8;
                }
                else if (i == 2)
                {
                    horizontalPos = 10;
                    verticalPos = 6;
                }

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
        private void BuildEdges()
        {
            short edgeCount = 0;
            for (int i = 0; i < controlNodes.Length; i++)
            {
                List<Tuple<int, Node>> children = _graph.Nodes[controlNodes[i].index].Children;
                Dictionary<int, int> edges = new();
                int edgeOffset;
                foreach (var child in children)
                {
                    if (!edges.TryGetValue(child.Item1, out int offsetCoeff))
                    {
                        edges.Add(child.Item1, 0);
                        edgeOffset = 0;
                    }
                    else
                        edgeOffset = ++edges[child.Item1];

                    ControlEdge controlEdge = new(controlNodes[i], controlNodes[child.Item2.Id], new Point(Width, Height),
                        child.Item2.Id == _graph.Nodes[controlNodes[i].index].Id, edgeCount++, edgeOffset);
                    Field.Children.Add(controlEdge);
                }
            }
        }
    }
}
