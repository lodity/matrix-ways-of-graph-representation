using CDM_Lab_3._1.Models;
using CDM_Lab_3._1.Models.Graph;
using CDM_Lab_3._1.Services;
using CDM_Lab_3._1.Utils;
using CDM_Lab_3._1.View;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static CDM_Lab_3._1.Models.Graph.Node;

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

        GraphWindow? graphWindow = null;
        private GraphType GraphTypeCurrent { get => (GraphType)ComboBoxGraphType.SelectedIndex; }
        private int NodeCount = 0;
        private short[,] MatrixAdjacencyTable;
        private TextBox[,] AdjacencyTableTextBox;
        private bool IsTextBoxTextChangedFromUI = true;
        private readonly Button ButtonAddNode;
        private readonly Button ButtonAddEdge;
        private Button? ButtonAddEdgeSelected;

        public enum IncidenceAccess
        {
            Adjacency,
            Graph
        }

        public MainWindow()
        {
            InitializeComponent();
            TextBoxNodesCount.Text = DEFAULT_NODES_COUNT;

            MatrixAdjacencyTable = new short[0, 0];
            AdjacencyTableTextBox = new TextBox[0, 0];
            CreateAdjacencyTable(NodeCount);
            ButtonAddEdgeSelected = null;
            ButtonAddNode = new()
            {
                Content = "+",
                Background = new SolidColorBrush(Color.FromRgb(4, 194, 55)),
                FontSize = 24,
                Padding = new Thickness(0, -8, 0, 0),
                Visibility = Visibility.Hidden
            };
            ButtonAddNode.Click += ButtonAddNode_Click;
            ButtonAddEdge = new()
            {
                Content = "+",
                Background = new SolidColorBrush(Color.FromRgb(4, 194, 55)),
                FontSize = 24,
                Padding = new Thickness(0, -8, 0, 0),
                Visibility = Visibility.Hidden
            };
            ButtonAddEdge.Click += ButtonAddEdge_Click;
            Grid.SetColumn(ButtonAddNode, 0);
            Grid.SetRow(ButtonAddEdge, 0);
            GridIncidenceTable.Children.Add(ButtonAddNode);
            GridIncidenceTable.Children.Add(ButtonAddEdge);
            ButtonApplyNodesCount_Click();
            if (DEFAULT_ADJACENCY_TABLE_VALUE != 0)
                UpdateIncidenceTable(IncidenceAccess.Adjacency);
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
                UpdateIncidenceTable(IncidenceAccess.Adjacency);
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

            UpdateIncidenceTable(IncidenceAccess.Adjacency);
        }
        private void UpdateIncidenceTable(IncidenceAccess accessType)
        {
            ClearGrid(GridIncidenceTable);

            Graph graph;
            if (graphWindow != null && graphWindow.Visibility == Visibility.Visible && accessType == IncidenceAccess.Graph)
                graph = graphWindow.Graph;
            else
                graph = GraphActions.CreateGraph_AdjacencyBased(MatrixAdjacencyTable, GraphTypeCurrent);

            NodeCount = graph.Count;
            IncedenceCreateBase();
            int edgeCount = 0;
            foreach (Node node in graph)
            {
                for (int i = 0; i < node.Children.Count; i++)
                {
                    bool isLoop = false;
                    GridIncidenceTable.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(32, GridUnitType.Pixel) });
                    int indexLastColumn = GridIncidenceTable.ColumnDefinitions.Count - 2;
                    for (short k = 0; k < NodeCount + 1; k++)
                    {
                        if (k == 0)
                        {
                            Label label = UiUtils.CreateTableLabel($"a{edgeCount++}", new Tuple<int, int>(k, indexLastColumn));
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
            Grid.SetRow(ButtonAddNode, NodeCount + 2);
            Grid.SetColumn(ButtonAddEdge, edgeCount + 2);
        }
        private void ButtonAddNode_Click(object sender, RoutedEventArgs e) => IncedenceAddNode();
        private void ButtonAddEdge_Click(object sender, RoutedEventArgs e)
        {
            UiUtils.AddColumnDefenitions(GridIncidenceTable, 1);
            Grid.SetColumn(ButtonAddEdge, GridIncidenceTable.ColumnDefinitions.Count - 1);

            int edgeIndex = GridIncidenceTable.ColumnDefinitions.Count - 3;
            Label label = UiUtils.CreateTableLabel($"a{edgeIndex}", new Tuple<int, int>(0, edgeIndex + 1));
            GridIncidenceTable.Children.Add(label);
            for (int i = 1; i < GridIncidenceTable.RowDefinitions.Count - 1; i++)
            {
                Button button = UiUtils.CreateTableButton($"x{i - 1}", new SolidColorBrush(Color.FromRgb(135, 135, 135)), 24, true, new Tuple<int, int>(i, edgeIndex + 1));
                button.Click += ButtonAddEdgeSelect_Click;
                GridIncidenceTable.Children.Add(button);
            }
        }

        private void ButtonAddEdgeSelect_Click(object sender, RoutedEventArgs e)
        {
            if (ButtonAddEdgeSelected == null)
            {
                ButtonAddEdgeSelected = (Button)sender;
                ButtonAddEdgeSelected.Background = new SolidColorBrush(Color.FromRgb(66, 66, 66));
                return;
            }
            ButtonAddEdgeSelected.Background = new SolidColorBrush(Color.FromRgb(135, 135, 135));

            List<Button> buttonsList = new();
            for (int i = GridIncidenceTable.RowDefinitions.Count - 2; i > 0; i--)
            {
                buttonsList.Add((Button)GridIncidenceTable.Children[^i]);
            }

            for (int i = 1; i < GridIncidenceTable.RowDefinitions.Count - 1; i++)
            {
                int textBoxValue;
                if (ButtonAddEdgeSelected == (Button)sender && (Button)sender == buttonsList[i - 1])
                    textBoxValue = 2;
                else if (ButtonAddEdgeSelected == buttonsList[i - 1])
                    textBoxValue = 1;
                else if (buttonsList[i - 1] == (Button)sender)
                    textBoxValue = GraphTypeCurrent == GraphType.Undirected ? 1 : -1;
                else textBoxValue = 0;

                TextBox textBox = UiUtils.CreateTableTextBox(null, new Tuple<int, int>(i, GridIncidenceTable.ColumnDefinitions.Count - 2), textBoxValue, 2);
                GridIncidenceTable.Children.Remove(buttonsList[i - 1]);
                GridIncidenceTable.Children.Add(textBox);
            }
            GraphActions.GraphAddEdge(ref MatrixAdjacencyTable, int.Parse(ButtonAddEdgeSelected.Content.ToString()[1..]),
                int.Parse(((Button)sender).Content.ToString()[1..]), GraphTypeCurrent);
            UpdateAdjacencyTable(true);
            ButtonAddEdgeSelected = null;
        }

        private void IncedenceCreateBase()
        {
            UiUtils.AddRowDefenitions(GridIncidenceTable, NodeCount + 2);
            UiUtils.AddColumnDefenitions(GridIncidenceTable, 2);

            for (int i = 1; i < NodeCount + 1; i++)
            {
                Label label = UiUtils.CreateTableLabel($"x{i - 1}", new Tuple<int, int>(i, 0));
                GridIncidenceTable.Children.Add(label);
            }
            ButtonAddNode.Visibility = Visibility.Visible;
            ButtonAddEdge.Visibility = Visibility.Visible;
            if (GridIncidenceTable.ColumnDefinitions.Count < 3)
            {
                Grid.SetRow(ButtonAddNode, NodeCount + 2);
                Grid.SetColumn(ButtonAddEdge, 1);
                GridIncidenceTable.Children.Add(ButtonAddNode);
                GridIncidenceTable.Children.Add(ButtonAddEdge);
            }
        }
        public void IncedenceAddNode([Optional] short[,] matrixAdjacencyTable)
        {
            NodeCount++;
            if (matrixAdjacencyTable == null)
                GraphActions.GraphAddNode(ref MatrixAdjacencyTable);
            else
                MatrixAdjacencyTable = matrixAdjacencyTable;
            UpdateAdjacencyTable(true);

            if (GridIncidenceTable.ColumnDefinitions.Count < 2)
                return;
            UiUtils.AddRowDefenitions(GridIncidenceTable, 1);
            Label label = UiUtils.CreateTableLabel($"x{NodeCount - 1}", new Tuple<int, int>(NodeCount, 0));
            GridIncidenceTable.Children.Add(label);

            for (int i = 1; i < GridIncidenceTable.ColumnDefinitions.Count - 1; i++)
            {
                TextBox textBox = UiUtils.CreateTableTextBox(null, new Tuple<int, int>(NodeCount, i), 0, 2);
                GridIncidenceTable.Children.Add(textBox);
            }
            Grid.SetRow(ButtonAddNode, NodeCount + 2);
            ButtonAddNode.Visibility = Visibility.Visible;
            ButtonAddEdge.Visibility = Visibility.Visible;
        }
        public void IncedenceAddEdge(int NodeIndexFrom, int NodeIndexTo)
        {
            GraphActions.GraphAddEdge(ref MatrixAdjacencyTable, NodeIndexFrom, NodeIndexTo, GraphTypeCurrent);

            UpdateAdjacencyTable();
            if (GridIncidenceTable.ColumnDefinitions.Count < 2)
            {
                UiUtils.AddRowDefenitions(GridIncidenceTable, 1);
                UiUtils.AddColumnDefenitions(GridIncidenceTable, 2);
                for (int i = 1; i < NodeCount + 1; i++)
                {
                    UiUtils.AddRowDefenitions(GridIncidenceTable, 1);
                    Label labelX = UiUtils.CreateTableLabel($"x{i - 1}", new Tuple<int, int>(i, 0));
                    GridIncidenceTable.Children.Add(labelX);
                }
            }
            UiUtils.AddColumnDefenitions(GridIncidenceTable, 1);
            Label label = UiUtils.CreateTableLabel($"a{GridIncidenceTable.ColumnDefinitions.Count - 3}", new Tuple<int, int>(0, GridIncidenceTable.ColumnDefinitions.Count - 2));
            GridIncidenceTable.Children.Add(label);
            bool isLoop = false;
            for (int i = 1; i < NodeCount + 1; i++)
            {
                int textBoxValue = 0;
                if (NodeIndexFrom == NodeIndexTo && NodeIndexTo == i - 1)
                {
                    isLoop = true;
                    textBoxValue = 2;
                }
                else if (!isLoop && NodeIndexFrom == i - 1)
                    textBoxValue = 1;
                else if (!isLoop && NodeIndexTo == i - 1)
                    textBoxValue = GraphTypeCurrent == GraphType.Undirected ? 1 : -1;
                TextBox textBox = UiUtils.CreateTableTextBox(null, new Tuple<int, int>(i, GridIncidenceTable.ColumnDefinitions.Count - 2), textBoxValue, 2);
                GridIncidenceTable.Children.Add(textBox);
            }
            Grid.SetColumn(ButtonAddEdge, GridIncidenceTable.ColumnDefinitions.Count - 1);
        }
        private void CreateAdjacencyTable(int nodeCount)
        {
            MatrixAdjacencyTable = new short[nodeCount, nodeCount];
            AdjacencyTableTextBox = new TextBox[nodeCount, nodeCount];
            UiUtils.AddRowDefenitions(GridAdjacencyTable, nodeCount + 1);
            UiUtils.AddColumnDefenitions(GridAdjacencyTable, nodeCount + 1);
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
            IncedenceCreateBase();
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
                graphWindow.Graph = GraphActions.CreateGraph_AdjacencyBased(MatrixAdjacencyTable, GraphTypeCurrent);
                graphWindow.GraphTypeCurrent = GraphTypeCurrent;
                graphWindow.MatrixAdjacencyTable = MatrixAdjacencyTable;
            }
            else
            {
                graphWindow = new(this, GraphActions.CreateGraph_AdjacencyBased(MatrixAdjacencyTable, GraphTypeCurrent), GraphTypeCurrent, ref MatrixAdjacencyTable);
                graphWindow.Show();
                graphWindow.GraphChanged += GraphWindow_GraphChanged;
            }
        }

        private void GraphWindow_GraphChanged(object sender, EventArgs e)
        {
            GraphWindow graphWindow = ((GraphWindow)sender);
            Graph graph = graphWindow.Graph;
            NodeCount = graph.Count;
            UpdateIncidenceTable(IncidenceAccess.Graph);
            UpdateAdjacencyTable();
        }

        private void UpdateAdjacencyTable([Optional] bool isGraphRebuild)
        {
            ClearGrid(GridAdjacencyTable);

            Graph graph;
            if (isGraphRebuild || graphWindow == null)
                graph = GraphActions.CreateGraph_AdjacencyBased(MatrixAdjacencyTable, GraphTypeCurrent);
            else
                graph = graphWindow.Graph;

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
