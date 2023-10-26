using CDM_Lab_3._1.Controls;
using CDM_Lab_3._1.Models;
using CDM_Lab_3._1.Models.Graph;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static CDM_Lab_3._1.MainWindow;
using static CDM_Lab_3._1.Models.Graph.Node;

namespace CDM_Lab_3._1.View
{
    /// <summary>
    /// Interaction logic for GraphWindow.xaml
    /// </summary>
    public partial class GraphWindow : Window
    {
        public event EventHandler<GraphChangedArgs>? GraphChanged;
        public IncidenceAccessType IncedenceAction;
        ControlNode? controlNodeSelected_Add;
        ControlNode? controlNodeSelected_Remove;
        readonly Random randomGlobal = new();
        Graph _graph;
        public GraphType GraphTypeCurrent;
        readonly int horizontalSectorsMax;
        readonly int verticalSectorsMax;
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
        public class GraphChangedArgs : EventArgs
        {
            public int NodeIndexFrom { get; set; }
            public int NodeIndexTo { get; set; }
            public IncidenceAccessType AccessType { get; set; }
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
                controlNode.SelectToAddEdge += ControlNode_SelectedToAddEdge;
                controlNode.SelectToRemoveEdge += ControlNode_SelectedToRemoveEdge;

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
                    bool isLoop = children[j].Item2.Id == _graph.Nodes[controlNodes[i].index].Id;
                    ControlEdge controlEdge = new(controlNodes[i], controlNodes[children[j].Item2.Id], new Point(Field.Width, Field.Height),
                        edgeCount++, edgeOffsetList[j], edgeMultipleOffsetMax, GraphTypeCurrent,
                        isLoop ? EdgeType.Loop : _graph.Nodes[i].Edges[j].Item2);
                    Field.Children.Add(controlEdge);
                }
            }
        }
        private void ControlNode_SelectedToAddEdge(object sender, RoutedEventArgs e)
        {
            ControlNode nodeTo = (ControlNode)((Border)sender).Parent;
            if (controlNodeSelected_Add == null)
            {
                controlNodeSelected_Add = nodeTo;
                controlNodeSelected_Add.SelectAddEdge();
            }
            else
            {
                bool isLoop = controlNodeSelected_Add == nodeTo;
                EdgeType edgeType = GraphTypeCurrent == GraphType.Undirected ? EdgeType.Undirected : EdgeType.Directed;
                _graph.Nodes[controlNodeSelected_Add.index].AddChild(_graph.Nodes[nodeTo.index], new Tuple<int, EdgeType>(edgeCount, isLoop ? EdgeType.Loop : edgeType));
                int edgeOffset = _graph.Nodes[controlNodeSelected_Add.index].Edges.Count;
                int edgeOffsetMax = MatrixAdjacencyTable[nodeTo.index, controlNodeSelected_Add.index];

                ControlEdge controlEdge = new(controlNodeSelected_Add, nodeTo, new Point(Field.Width, Field.Height),
                         edgeCount++, edgeOffset, edgeOffsetMax == 0 ? ++edgeOffsetMax : edgeOffsetMax, GraphTypeCurrent,
                        isLoop ? EdgeType.Loop : _graph.Nodes[controlNodeSelected_Add.index].Edges[^1].Item2);
                Field.Children.Add(controlEdge);
                IncedenceAction = IncidenceAccessType.GraphWindow_EdgeAdded;
                GraphChangedArgs graphChangedArgs = new()
                {
                    AccessType = IncidenceAccessType.GraphWindow_EdgeAdded,
                    NodeIndexFrom = controlNodeSelected_Add.index,
                    NodeIndexTo = nodeTo.index
                };
                GraphChanged?.Invoke(this, graphChangedArgs);
                controlNodeSelected_Add.SelectClear();
                controlNodeSelected_Add = null;
            }
        }
        private void ControlNode_SelectedToRemoveEdge(object sender, RoutedEventArgs e)
        {
            ControlNode nodeTo = (ControlNode)((Border)sender).Parent;
            if (controlNodeSelected_Remove == null)
            {
                controlNodeSelected_Remove = nodeTo;
                controlNodeSelected_Remove.SelectRemoveEdge();
            }
            else
            {

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
                controlNode.SelectToAddEdge += ControlNode_SelectedToAddEdge;
                controlNode.SelectToRemoveEdge += ControlNode_SelectedToRemoveEdge;
                controlNodes.Add(controlNode);
                Field.Children.Add(controlNode);
                IncedenceAction = IncidenceAccessType.GraphWindow_NodeAdded;
                GraphChangedArgs graphChangedArgs = new()
                {
                    AccessType = IncidenceAccessType.GraphWindow_NodeAdded
                };
                GraphChanged?.Invoke(this, graphChangedArgs);
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
