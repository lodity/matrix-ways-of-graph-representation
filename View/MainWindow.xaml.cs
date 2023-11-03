using CDM_Lab_3._1.Models;
using CDM_Lab_3._1.Models.Graph;
using CDM_Lab_3._1.Utils;
using CDM_Lab_3._1.View;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static CDM_Lab_3._1.Models.Graph.Node;
using static CDM_Lab_3._1.View.GraphWindow;

namespace CDM_Lab_3._1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Settings
        const string DEFAULT_NODES_COUNT = "3";
        const short DEFAULT_ADJACENCY_TABLE_VALUE = 0; // 0 - no edge, 1 - edge
        #endregion

        public enum IncidenceAccessType
        {
            AdjacencyTable,
            GraphWindow_EdgeAdded,
            GraphWindow_EdgeRemoved,
            GraphWindow_NodeAdded,
            Graph
        }

        GraphWindow? graphWindow = null;
        private GraphType GraphTypeCurrent { get => (GraphType)ComboBoxGraphType.SelectedIndex; }
        private int NodeCount = 0;
        private short[,] MatrixAdjacencyTable;
        private TextBox[,] AdjacencyTableTextBox;
        private bool IsTextBoxTextChangedFromUI = true;
        public MainWindow()
        {
            InitializeComponent();
            TextBoxNodesCount.Text = DEFAULT_NODES_COUNT;

            MatrixAdjacencyTable = new short[0, 0];
            AdjacencyTableTextBox = new TextBox[0, 0];
            ButtonApplyNodesCount_Click();
            CreateAdjacencyTable(NodeCount);
            if (DEFAULT_ADJACENCY_TABLE_VALUE != 0)
                UpdateIncidenceTable(IncidenceAccessType.AdjacencyTable);
        }
        private static void ClearGrid(Grid grid)
        {
            grid.Children.Clear();
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();
        }
        private void ComboBoxGraphType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClearGrid(GridIncidenceTable);
            if (MatrixAdjacencyTable != null)
                UpdateIncidenceTable(IncidenceAccessType.AdjacencyTable);
            if (graphWindow != null)
                graphWindow.GraphTypeCurrent = GraphTypeCurrent;
        }
        private void OnlyNumbersValidation_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = TextUtils.IsTextSatisfiesRegex(e.Text);
        }
        private void TextBoxAdjacencyTable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            short textBoxSenderText = short.Parse(((TextBox)sender).Text);
            if (textBoxSenderText >= 0 && textBoxSenderText < 9)
                ((TextBox)sender).Text = (textBoxSenderText + 1).ToString();
        }
        private void TextBoxAdjacencyTable_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            TextBox textBoxSender = (TextBox)sender;
            short textBoxSenderText = short.Parse(textBoxSender.Text);
            if (((textBoxSenderText > 0 || e.Delta > 0) && textBoxSenderText < 9) || (textBoxSenderText == 9 && e.Delta < 0))
                textBoxSender.Text = (textBoxSenderText + (e.Delta > 0 ? 1 : -1)).ToString();
        }
        private void TextBoxAdjacencyTable_LostFocus(object sender, RoutedEventArgs e)
        {
            if (((TextBox)(sender)).Text == "")
                ((TextBox)(sender)).Text = "0";
        }
        private void TextBoxAdjacencyTable_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsTextBoxTextChangedFromUI)
                return;
            TextBox textBoxSender = ((TextBox)sender);
            int indexX = (int.Parse(textBoxSender.Name.Split('_')[1]) - 1);
            int indexY = (int.Parse(textBoxSender.Name.Split('_')[2]) - 1);
            MatrixAdjacencyTable[indexX, indexY] = textBoxSender.Text != "" ? short.Parse(textBoxSender.Text) : (short)0;
            if (GraphTypeCurrent == GraphType.Undirected && indexX != indexY)
                AdjacencyTableTextBox[indexY, indexX].Text = textBoxSender.Text;

            UpdateIncidenceTable(IncidenceAccessType.AdjacencyTable);
        }
        private void UpdateIncidenceTable(IncidenceAccessType accessType, [Optional] GraphChangedArgs args)
        {
            if (accessType == IncidenceAccessType.GraphWindow_NodeAdded)
            {
                if (GridIncidenceTable.ColumnDefinitions.Count < 2)
                    return;
                GridIncidenceTable.RowDefinitions.Add(new RowDefinition { Height = new GridLength(24, GridUnitType.Pixel) });
                Label label = UiUtils.CreateTableLabel($"x{NodeCount - 1}", new Tuple<int, int>(NodeCount, 0));
                GridIncidenceTable.Children.Add(label);

                for (int i = 1; i < GridIncidenceTable.ColumnDefinitions.Count; i++)
                {
                    TextBox textBox = UiUtils.CreateTableTextBox(null, new Tuple<int, int>(NodeCount, i), 0, 2);
                    GridIncidenceTable.Children.Add(textBox);
                }
                return;
            }
            if (accessType == IncidenceAccessType.GraphWindow_EdgeAdded)
            {
                if (GridIncidenceTable.ColumnDefinitions.Count < 2)
                {
                    ClearGrid(GridIncidenceTable);
                    GridIncidenceTable.RowDefinitions.Add(new RowDefinition { Height = new GridLength(24, GridUnitType.Pixel) });
                    GridIncidenceTable.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(32, GridUnitType.Pixel) });
                    for (int i = 1; i < NodeCount + 1; i++)
                    {
                        GridIncidenceTable.RowDefinitions.Add(new RowDefinition { Height = new GridLength(24, GridUnitType.Pixel) });
                        Label labelX = UiUtils.CreateTableLabel($"x{i - 1}", new Tuple<int, int>(i, 0));
                        GridIncidenceTable.Children.Add(labelX);
                    }
                }
                GridIncidenceTable.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(32, GridUnitType.Pixel) });
                Label label = UiUtils.CreateTableLabel($"a{GridIncidenceTable.ColumnDefinitions.Count - 2}", new Tuple<int, int>(0, GridIncidenceTable.ColumnDefinitions.Count - 1));
                GridIncidenceTable.Children.Add(label);
                bool isLoop = false;
                for (int i = 1; i < NodeCount + 1; i++)
                {
                    int textBoxValue = 0;
                    if (args.NodeIndexFrom == args.NodeIndexTo && args.NodeIndexTo == i - 1)
                    {
                        isLoop = true;
                        textBoxValue = 2;
                    }
                    else if (!isLoop && args.NodeIndexFrom == i - 1)
                        textBoxValue = 1;
                    else if (!isLoop && args.NodeIndexTo == i - 1)
                        textBoxValue = GraphTypeCurrent == GraphType.Undirected ? 1 : -1;
                    TextBox textBox = UiUtils.CreateTableTextBox(null, new Tuple<int, int>(i, GridIncidenceTable.ColumnDefinitions.Count - 1), textBoxValue, 2);
                    GridIncidenceTable.Children.Add(textBox);
                }
                return;
            }
            if (accessType == IncidenceAccessType.GraphWindow_EdgeRemoved)
            {
                foreach (ContentControl child in GridIncidenceTable.Children)
                {
                    if (child.Name != null && child.Name == $"a{args.EdgeId}") { }
                    // TODO remove incedence column
                }
            }
            ClearGrid(GridIncidenceTable);

            Graph graph;
            if (accessType == IncidenceAccessType.AdjacencyTable || graphWindow == null)
                graph = CreateGraph_AdjacencyBased();
            else if (accessType == IncidenceAccessType.Graph)
                graph = graphWindow.Graph;
            else graph = CreateGraph_AdjacencyBased();
            NodeCount = graph.Count;

            for (short i = 0; i < NodeCount + 1; i++)
                GridIncidenceTable.RowDefinitions.Add(new RowDefinition { Height = new GridLength(24, GridUnitType.Pixel) });
            GridIncidenceTable.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(32, GridUnitType.Pixel) });

            for (int i = 1; i < NodeCount + 1; i++)
            {
                Label label = UiUtils.CreateTableLabel($"x{i - 1}", new Tuple<int, int>(i, 0));
                GridIncidenceTable.Children.Add(label);
            }
            foreach (Node node in graph)
            {
                for (int i = 0; i < node.Children.Count; i++)
                {
                    bool isLoop = false;
                    GridIncidenceTable.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(32, GridUnitType.Pixel) });
                    int indexLastColumn = GridIncidenceTable.ColumnDefinitions.Count - 1;
                    for (short k = 0; k < NodeCount + 1; k++)
                    {
                        if (k == 0)
                        {
                            Label label = UiUtils.CreateTableLabel($"a{indexLastColumn - 1}", new Tuple<int, int>(k, indexLastColumn));
                            GridIncidenceTable.Children.Add(label);
                        }
                        else
                        {
                            int textBoxValue;
                            if (GraphTypeCurrent == GraphType.Undirected || GraphTypeCurrent == GraphType.Directed)
                            {
                                if (node.Children[i].Item1 == node.Id && node.Id == k - 1)
                                {
                                    textBoxValue = 2;
                                    isLoop = true;
                                }
                                else if (!isLoop && k - 1 == node.Id)
                                    textBoxValue = 1;
                                else if (!isLoop && k - 1 == node.Children[i].Item1)
                                    textBoxValue = (node.Edges[i].Item2 == EdgeType.Directed ? -1 : 1);
                                else
                                    textBoxValue = 0;
                            }
                            else
                            {
                                if (node.Children[i].Item1 == node.Id && node.Id == k - 1)
                                {
                                    textBoxValue = 2;
                                    isLoop = true;
                                }
                                else if (!isLoop && k - 1 == node.Children[i].Item1 && node.Edges[i].Item2 == EdgeType.Directed)
                                    textBoxValue = -1;
                                else if (!isLoop && k - 1 == node.Id || k - 1 == node.Children[i].Item1)
                                    textBoxValue = 1;
                                else
                                    textBoxValue = 0;
                            }
                            TextBox textBox = UiUtils.CreateTableTextBox(null, new Tuple<int, int>(k, indexLastColumn), textBoxValue, 2);
                            GridIncidenceTable.Children.Add(textBox);
                        }
                    }
                }
            }
        }
        private void CreateAdjacencyTable(int nodeCount)
        {
            MatrixAdjacencyTable = new short[nodeCount, nodeCount];
            AdjacencyTableTextBox = new TextBox[nodeCount, nodeCount];
            for (short i = 0; i < nodeCount + 1; i++)
                GridAdjacencyTable.RowDefinitions.Add(new RowDefinition { Height = new GridLength(24, GridUnitType.Pixel) });
            for (short i = 0; i < nodeCount + 1; i++)
                GridAdjacencyTable.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(32, GridUnitType.Pixel) });
            for (short x = 0; x < nodeCount + 1; x++)
            {
                for (short y = 0; y < nodeCount + 1; y++)
                {
                    if (x == 0 && y == 0) continue;
                    if (x == 0 || y == 0)
                    {
                        Label label = UiUtils.CreateTableLabel($"x{((x == 0) ? y - 1 : x - 1)}", new Tuple<int, int>(x, y));
                        GridAdjacencyTable.Children.Add(label);
                    }
                    else
                    {
                        TextBox textBox = UiUtils.CreateTableTextBox("textBoxAdjacencyTable", new Tuple<int, int>(x, y), DEFAULT_ADJACENCY_TABLE_VALUE, 1);
                        textBox.PreviewTextInput += OnlyNumbersValidation_PreviewTextInput;
                        textBox.TextChanged += TextBoxAdjacencyTable_TextChanged;
                        textBox.LostFocus += TextBoxAdjacencyTable_LostFocus;
                        textBox.MouseDoubleClick += TextBoxAdjacencyTable_MouseDoubleClick;
                        textBox.MouseWheel += TextBoxAdjacencyTable_MouseWheel;
                        GridAdjacencyTable.Children.Add(textBox);
                        AdjacencyTableTextBox[x - 1, y - 1] = textBox;
                        MatrixAdjacencyTable[x - 1, y - 1] = DEFAULT_ADJACENCY_TABLE_VALUE;
                    }
                }
            }
        }
        private void ButtonApplyNodesCount_Click([Optional] object sender, [Optional] RoutedEventArgs e)
        {
            ButtonClearTable_Click();
            NodeCount = int.Parse(TextBoxNodesCount.Text);
            CreateAdjacencyTable(NodeCount);
        }
        private void ButtonClearTable_Click([Optional] object sender, [Optional] RoutedEventArgs e)
        {
            ClearGrid(GridIncidenceTable);
            ClearGrid(GridAdjacencyTable);
        }
        private void ButtonBuildGraph_Click(object sender, RoutedEventArgs e)
        {
            if (graphWindow != null && graphWindow.IsVisible)
            {
                graphWindow.Graph = CreateGraph_AdjacencyBased();
                graphWindow.GraphTypeCurrent = GraphTypeCurrent;
                graphWindow.MatrixAdjacencyTable = MatrixAdjacencyTable;
            }
            else
            {
                graphWindow = new(CreateGraph_AdjacencyBased(), GraphTypeCurrent, ref MatrixAdjacencyTable);
                graphWindow.Show();
                graphWindow.GraphChanged += GraphWindow_GraphChanged;
            }
        }

        private void GraphWindow_GraphChanged(object sender, GraphChangedArgs graphChangedArgs)
        {
            GraphWindow graphWindow = ((GraphWindow)sender);
            Graph graph = graphWindow.Graph;
            NodeCount = graph.Count;
            UpdateIncidenceTable(graphChangedArgs.AccessType, graphChangedArgs);
            UpdateAdjacencyTable();
        }

        private Graph CreateGraph_AdjacencyBased()
        {
            Graph graph = new(NodeCount);

            short[,] AdjacencyTableCopy = (short[,])MatrixAdjacencyTable.Clone();
            bool isNoZeroRemained;
            int edgeCount = 0;
            do
            {
                isNoZeroRemained = false;
                for (int x = 0; x < NodeCount; x++)
                {
                    for (int y = 0; y < NodeCount; y++)
                    {
                        if (AdjacencyTableCopy[x, y] > 0 && (GraphTypeCurrent != GraphType.Undirected || x >= y))
                        {
                            EdgeType edgeType = EdgeType.Directed;
                            switch (GraphTypeCurrent)
                            {
                                case GraphType.Undirected: edgeType = EdgeType.Undirected; break;
                                case GraphType.Mixed:
                                    {
                                        if (x == y)
                                        {
                                            AdjacencyTableCopy[x, y]--;
                                            edgeType = EdgeType.Loop;
                                            break;
                                        }
                                        if ((AdjacencyTableCopy[x, y] - AdjacencyTableCopy[y, x]) < 1)
                                        {
                                            AdjacencyTableCopy[x, y]--;
                                            AdjacencyTableCopy[y, x]--;
                                            edgeType = EdgeType.Undirected;
                                        }
                                        else if (AdjacencyTableCopy[x, y] > AdjacencyTableCopy[y, x]) AdjacencyTableCopy[x, y]--;
                                        else AdjacencyTableCopy[y, x]--;
                                    }
                                    break;
                            }
                            graph.Nodes[x].AddChild(graph.Nodes[y], new Tuple<int, EdgeType>(edgeCount++, edgeType));
                        }
                        if (GraphTypeCurrent != GraphType.Mixed)
                            AdjacencyTableCopy[x, y]--;
                        if (AdjacencyTableCopy[x, y] > 0)
                            isNoZeroRemained = true;
                    }
                }
            } while (isNoZeroRemained);
            return graph;
        }
        private void UpdateAdjacencyTable()
        {
            ClearGrid(GridAdjacencyTable);

            Graph graph;
            if (graphWindow != null)
                graph = graphWindow.Graph;
            else
                graph = CreateGraph_AdjacencyBased();

            int nodeCount = graph.Count;

            MatrixAdjacencyTable = new short[nodeCount, nodeCount];
            AdjacencyTableTextBox = new TextBox[nodeCount, nodeCount];
            for (short i = 0; i < nodeCount + 1; i++)
                GridAdjacencyTable.RowDefinitions.Add(new RowDefinition { Height = new GridLength(24, GridUnitType.Pixel) });
            for (short i = 0; i < nodeCount + 1; i++)
                GridAdjacencyTable.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(32, GridUnitType.Pixel) });
            List<Tuple<int, int>> doSymetric = new();
            List<int> doSymetricCount = new();
            for (short x = 0; x < nodeCount + 1; x++)
            {
                for (short y = 0; y < nodeCount + 1; y++)
                {
                    if (x == 0 && y == 0) continue;
                    if (x == 0 || y == 0)
                    {
                        Label label = UiUtils.CreateTableLabel($"x{((x == 0) ? y - 1 : x - 1)}", new Tuple<int, int>(x, y));
                        GridAdjacencyTable.Children.Add(label);
                    }
                    else
                    {
                        int value = 0;
                        if (x != 0 && y != 0)
                            for (int i = 0; i < graph.Nodes[x - 1].Children.Count; i++)
                            {
                                if (graph.Nodes[x - 1].Children[i].Item1 == graph.Nodes[y - 1].Id)
                                {
                                    value++;
                                    if (graph.Nodes[x - 1].Edges[i].Item2 == EdgeType.Undirected && graph.Nodes[x - 1].Id != graph.Nodes[y - 1].Id)
                                    {
                                        doSymetric.Add(new Tuple<int, int>(y - 1, x - 1));
                                        doSymetricCount.Add(x);
                                    }
                                }
                            }
                        TextBox textBox = UiUtils.CreateTableTextBox("textBoxAdjacencyTable", new Tuple<int, int>(x, y), value, 1);
                        textBox.PreviewTextInput += OnlyNumbersValidation_PreviewTextInput;
                        textBox.TextChanged += TextBoxAdjacencyTable_TextChanged;
                        textBox.LostFocus += TextBoxAdjacencyTable_LostFocus;
                        textBox.MouseDoubleClick += TextBoxAdjacencyTable_MouseDoubleClick;
                        textBox.MouseWheel += TextBoxAdjacencyTable_MouseWheel;
                        GridAdjacencyTable.Children.Add(textBox);
                        AdjacencyTableTextBox[x - 1, y - 1] = textBox;
                        MatrixAdjacencyTable[x - 1, y - 1] = (short)value;
                    }
                }
            }
            IsTextBoxTextChangedFromUI = false;
            doSymetric.ForEach(item =>
            {
                AdjacencyTableTextBox[item.Item1, item.Item2].Text = (short.Parse(AdjacencyTableTextBox[item.Item1, item.Item2].Text) + 1).ToString();
                MatrixAdjacencyTable[item.Item1, item.Item2]++;
            });
            IsTextBoxTextChangedFromUI = true;
        }
    }
}
