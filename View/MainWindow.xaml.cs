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

namespace CDM_Lab_3._1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Settings
        const string DEFAULT_NODES_COUNT = "2";
        const short DEFAULT_ADJACENCY_TABLE_VALUE = 0; // 0 - no edge, 1 - edge
        #endregion

        GraphWindow? graphWindow;
        private GraphType GraphTypeCurrent { get => (GraphType)ComboBoxGraphType.SelectedIndex; }
        private short edgeCount;
        private int NodeCount = 0;
        private short[,] MatrixAdjacencyTable;
        private TextBox[,] AdjacencyTableTextBox;
        public MainWindow()
        {
            InitializeComponent();
            TextBoxNodesCount.Text = DEFAULT_NODES_COUNT;

            graphWindow = null;
            edgeCount = 0;
            ButtonApplyNodesCount_Click();
            Tuple<short[,], TextBox[,]> tuple = CreateAdjacencyTable(NodeCount);
            MatrixAdjacencyTable = tuple.Item1;
            AdjacencyTableTextBox = tuple.Item2;
            UpdateIncidenceTable();
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
                UpdateIncidenceTable();
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
            TextBox textBoxSender = ((TextBox)sender);
            int indexX = (int.Parse(textBoxSender.Name.Split('_')[1]) - 1);
            int indexY = (int.Parse(textBoxSender.Name.Split('_')[2]) - 1);
            MatrixAdjacencyTable[indexX, indexY] = textBoxSender.Text != "" ? short.Parse(textBoxSender.Text) : (short)0;
            if (GraphTypeCurrent == GraphType.Undirected && indexX != indexY)
                AdjacencyTableTextBox[indexY, indexX].Text = textBoxSender.Text;

            UpdateIncidenceTable();
        }
        private void UpdateIncidenceTable()
        {
            ClearGrid(GridIncidenceTable);

            short[,] AdjacencyTableCopy = (short[,])MatrixAdjacencyTable.Clone();
            edgeCount = 0;
            List<Tuple<int, int>> listOfIndexes = new();

            for (int x = 0; x < NodeCount; x++)
            {
                for (int y = 0; y < NodeCount; y++)
                {
                    if (MatrixAdjacencyTable[x, y] >= 1 && ((!listOfIndexes.Contains(new Tuple<int, int>(y, x)) || x == y) || GraphTypeCurrent == GraphType.Directed))
                    {
                        listOfIndexes.Add(new Tuple<int, int>(x, y));
                        edgeCount++;
                    }
                }
            }

            if (edgeCount == 0) return;
            short[,] MatrixIncidenceTable = new short[edgeCount, NodeCount];
            for (short i = 0; i < NodeCount + 1; i++)
                GridIncidenceTable.RowDefinitions.Add(new RowDefinition { Height = new GridLength(24, GridUnitType.Pixel) });
            GridIncidenceTable.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(32, GridUnitType.Pixel) });

            for (int i = 1; i < NodeCount + 1; i++)
            {
                Label label = new()
                {
                    Content = $"x{i - 1}"
                };
                Grid.SetRow(label, i);
                Grid.SetColumn(label, 0);
                GridIncidenceTable.Children.Add(label);
            }
            bool isNoZeroRemained;
            short whileCount = 0;
            short incedenceEdgeCount = 0;
            do
            {
                isNoZeroRemained = false;
                for (int x = 0; x < NodeCount; x++)
                {
                    for (int y = 0; y < NodeCount; y++)
                    {
                        if (AdjacencyTableCopy[x, y] != 0) isNoZeroRemained = true;
                        if (AdjacencyTableCopy[x, y] != 0 && ((!listOfIndexes.Contains(new Tuple<int, int>(y, x)) || x == y) || GraphTypeCurrent == GraphType.Directed))
                        {
                            GridIncidenceTable.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(32, GridUnitType.Pixel) });
                            int indexLastColumn = GridIncidenceTable.ColumnDefinitions.Count - 1;
                            bool isLoop = false;
                            incedenceEdgeCount++;
                            for (short k = 0; k < NodeCount + 1; k++)
                            {
                                if (k == 0)
                                {
                                    Label label = UiUtils.CreateTableLabel($"a{indexLastColumn - 1}", new Tuple<int, int>(k, indexLastColumn));
                                    GridIncidenceTable.Children.Add(label);
                                }
                                else
                                {
                                    TextBox textBox = new();
                                    switch (GraphTypeCurrent)
                                    {
                                        case GraphType.Undirected:
                                            {
                                                if (x == y && x == k - 1)
                                                {
                                                    textBox.Text = "2";
                                                    isLoop = true;
                                                }
                                                else if (!isLoop && (k - 1 == x || k - 1 == y))
                                                    textBox.Text = "1";
                                                else
                                                    textBox.Text = "0";
                                            }
                                            break;
                                        case GraphType.Directed:
                                            {
                                                if (x == y && x == k - 1)
                                                {
                                                    textBox.Text = "2";
                                                    isLoop = true;
                                                }
                                                else if (!isLoop && k - 1 == x)
                                                    textBox.Text = "1";
                                                else if (!isLoop && k - 1 == y)
                                                    textBox.Text = "-1";
                                                else
                                                    textBox.Text = "0";
                                            }
                                            break;
                                        case GraphType.Mixed:
                                            {
                                                if (x == y && x == k - 1)
                                                {
                                                    textBox.Text = "2";
                                                    isLoop = true;
                                                }
                                                else if (!isLoop && k - 1 == y && AdjacencyTableCopy[y, x] == 0)
                                                    textBox.Text = "-1";
                                                else if (!isLoop && k - 1 == x || k - 1 == y)
                                                    textBox.Text = "1";
                                                else
                                                    textBox.Text = "0";
                                            }
                                            break;
                                    }
                                    if (whileCount == 0)
                                        MatrixIncidenceTable[incedenceEdgeCount - 1, k - 1] = short.Parse(textBox.Text);
                                    textBox.MaxLength = 2;
                                    Grid.SetRow(textBox, k);
                                    Grid.SetColumn(textBox, indexLastColumn);
                                    GridIncidenceTable.Children.Add(textBox);
                                }
                            }
                        }
                        if (AdjacencyTableCopy[x, y] >= 1)
                            AdjacencyTableCopy[x, y]--;
                    }
                }
                whileCount++;
            } while (isNoZeroRemained);
        }
        private Tuple<short[,], TextBox[,]> CreateAdjacencyTable(int nodeCount)
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
            return new Tuple<short[,], TextBox[,]>(MatrixAdjacencyTable, AdjacencyTableTextBox);
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
                graphWindow.Graph = CreateGraph();
            else
            {
                graphWindow = new(CreateGraph(), GraphTypeCurrent);
                graphWindow.Show();
            }
        }
        private Graph CreateGraph()
        {
            Graph graph = new(NodeCount);

            short[,] AdjacencyTableCopy = (short[,])MatrixAdjacencyTable.Clone();
            bool isNoZeroRemained;
            short whileCount = 0;
            do
            {
                isNoZeroRemained = false;
                for (int x = 0; x < NodeCount; x++)
                {
                    for (int y = 0; y < NodeCount; y++)
                    {
                        if (AdjacencyTableCopy[x, y] > 0 && (GraphTypeCurrent != GraphType.Undirected || x >= y))
                        {
                            bool isSingleOriented = false;
                            switch (GraphTypeCurrent)
                            {
                                case GraphType.Directed: isSingleOriented = true; break;
                                case GraphType.Mixed:
                                    {
                                        if (x == y)
                                        {
                                            AdjacencyTableCopy[x, y]--;
                                            break;
                                        }
                                        isSingleOriented = (AdjacencyTableCopy[x, y] - AdjacencyTableCopy[y, x]) > 0;
                                        if (!isSingleOriented)
                                        {
                                            AdjacencyTableCopy[x, y]--;
                                            AdjacencyTableCopy[y, x]--;
                                        }
                                        else if (AdjacencyTableCopy[x, y] > AdjacencyTableCopy[y, x]) AdjacencyTableCopy[x, y]--;
                                        else AdjacencyTableCopy[y, x]--;
                                    }
                                    break;
                            }
                            graph.Nodes[x].AddChild(graph.Nodes[y], isSingleOriented);
                        }
                        if (GraphTypeCurrent != GraphType.Mixed)
                            AdjacencyTableCopy[x, y]--;
                        if (AdjacencyTableCopy[x, y] > 0)
                            isNoZeroRemained = true;
                    }
                }
                whileCount++;
            } while (isNoZeroRemained);
            return graph;
        }
    }
}
