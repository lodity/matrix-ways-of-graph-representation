using CDM_Lab_3._1.Controls;
using CDM_Lab_3._1.Models;
using CDM_Lab_3._1.Models.Graph;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CDM_Lab_3._1.View
{
    /// <summary>
    /// Interaction logic for GraphWindow.xaml
    /// </summary>
    public partial class GraphWindow : Window
    {
        public event RoutedEventHandler? GraphChanged;
        ControlNode? controlNodeSelected;
        Random randomGlobal = new();
        Graph _graph;
        public GraphType GraphTypeCurrent;
        int horizontalSectorsMax;
        int verticalSectorsMax;
        int edgeCount;
        bool[,] nodeSectors;
        public short[,] MatrixAdjacencyTable;
        List<ControlNode> controlNodes;
        public GraphWindow(Graph graph, GraphType graphTypeCurrent, ref short[,] matrixAdjacencyTable)
        {
            InitializeComponent();

            _graph = graph;
            GraphTypeCurrent = graphTypeCurrent;
            controlNodes = new List<ControlNode>();
            horizontalSectorsMax = (int)Math.Floor(Field.Width / 40) - 1;
            verticalSectorsMax = (int)Math.Floor(Field.Height / 40) - 1;
            nodeSectors = new bool[horizontalSectorsMax, verticalSectorsMax];
            MatrixAdjacencyTable = matrixAdjacencyTable;

            BuildNodes();
            BuildEdges();
        }
        public Graph Graph
        {
            get => _graph;
            set
            {
                _graph = value;
                Field.Children.Clear();
                controlNodes = new List<ControlNode>();
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
                    horizontalPos = 0;
                    verticalPos = 0;
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
                ControlNode controlNode = new(_graph.Nodes[i], new Point((horizontalPos + 1) * 40 - Width / 2, (verticalPos + 1) * 40 - Height / 2));
                controlNode.Selected += ControlNode_Selected;
                controlNodes.Add(controlNode);

                Field.Children.Add(controlNode);
            }
        }
        private void BuildEdges()
        {
            edgeCount = 0;
            for (int i = 0; i < controlNodes.Count; i++)
            {
                List<Tuple<int, Node>> children = _graph.Nodes[i].Children;
                Dictionary<int, int> edges = new();
                List<int> edgeOffsetList = new();
                int edgeMultipleOffsetMax = 0;
                foreach (var child in children)
                {
                    if (!edges.TryGetValue(child.Item1, out int edgeOffset))
                    {
                        edges.Add(child.Item1, 0);
                        edgeOffset = 0;
                        edgeOffsetList.Add(edgeOffset);
                    }
                    else
                    {
                        edgeOffset = ++edges[child.Item1];
                        edgeOffsetList.Add(edgeOffset);
                    }
                    edgeMultipleOffsetMax++;
                }
                for (int j = 0; j < children.Count; j++)
                {
                    ControlEdge controlEdge = new(controlNodes[i], controlNodes[children[j].Item2.Id], new Point(Field.Width, Field.Height),
                        children[j].Item2.Id == _graph.Nodes[controlNodes[i].index].Id, edgeCount++, edgeOffsetList[j], edgeMultipleOffsetMax, GraphTypeCurrent,
                        _graph.Nodes[i].Edges[j]);
                    Field.Children.Add(controlEdge);
                }
            }
        }
        private void ControlNode_Selected(object sender, RoutedEventArgs e)
        {
            Border borderSender = (Border)sender;
            if (controlNodeSelected == null)
            {
                controlNodeSelected = (ControlNode)borderSender.Parent;
                controlNodeSelected.Select(true);
            }
            else
            {
                _graph.Nodes[controlNodeSelected.index].AddChild(_graph.Nodes[((ControlNode)borderSender.Parent).index], GraphTypeCurrent != GraphType.Undirected);
                int edgeOffset = _graph.Nodes[controlNodeSelected.index].Edges.Count;
                int edgeOffsetMax = MatrixAdjacencyTable[((ControlNode)borderSender.Parent).index, controlNodeSelected.index];
                bool isLoop = controlNodeSelected == (ControlNode)borderSender.Parent;

                ControlEdge controlEdge = new(controlNodeSelected, (ControlNode)borderSender.Parent, new Point(Field.Width, Field.Height),
                        isLoop, edgeCount++, edgeOffset, edgeOffsetMax == 0 ? ++edgeOffsetMax : edgeOffsetMax, GraphTypeCurrent,
                        _graph.Nodes[controlNodeSelected.index].Edges[^1]);
                Field.Children.Add(controlEdge);
                controlNodeSelected.Select(false);
                controlNodeSelected = null;
                GraphChanged?.Invoke(this, new RoutedEventArgs());
            }
        }
        private void Field_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Point spawnPoint = e.GetPosition(this);
            if (spawnPoint != new Point(0, 0) && e.ClickCount == 2 && e.ChangedButton == MouseButton.Left && sender != Field)
            {
                _graph.AddNode();
                short[,] MatrixAdjacencyTableCopy = new short[_graph.Count, _graph.Count];
                for (int i = 0; i < _graph.Count; i++)
                {
                    for (int j = 0; j < _graph.Count; j++)
                    {
                        if (Math.Sqrt(MatrixAdjacencyTable.Length) > i && Math.Sqrt(MatrixAdjacencyTable.Length) > j)
                            MatrixAdjacencyTableCopy[i, j] = MatrixAdjacencyTable[i, j];
                        else MatrixAdjacencyTableCopy[i, j] = 0;
                    }
                }
                MatrixAdjacencyTable = MatrixAdjacencyTableCopy;
                ControlNode controlNode = new(_graph.Nodes[^1], new Point(spawnPoint.X - Width / 2, spawnPoint.Y - Height / 2));
                controlNode.Selected += ControlNode_Selected;
                controlNodes.Add(controlNode);
                Field.Children.Add(controlNode);
                GraphChanged?.Invoke(this, new RoutedEventArgs());
            }
        }

        // Window title bar actions
        private void TopBar_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void WindowMinimaze_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void WindowClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
