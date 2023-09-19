using CDM_Lab_3._1.Models;
using CDM_Lab_3._1.Utils;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace CDM_Lab_3._1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GraphType CurrentGraphType { get => (GraphType)ComboBoxGraphType.SelectedIndex; }
        private short edgeCount;
        private short[,] AdjacencyTable;
        public MainWindow()
        {
            InitializeComponent();
            edgeCount = 0;
        }
        private void ClearGrid(Grid grid)
        {
            grid.Children.Clear();
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();
        }
        private void ComboBoxGraphType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClearGrid(GridIncidenceTable);
        }
        private void TextBoxNodesCount_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = TextUtils.IsTextSatisfiesRegex(e.Text);
        }
        private void TextBoxAdjacencyTable_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = TextUtils.IsTextSatisfiesRegex(e.Text, new Regex("[^0-1]"));
        }
        private void TextBoxAdjacencyTable_LostFocus(object sender, RoutedEventArgs e)
        {
            if (((TextBox)(sender)).Text == "")
            {
                ((TextBox)(sender)).Text = "0";
            }
        }
        private void TextBoxAdjacencyTable_TextChanged(object sender, TextChangedEventArgs e)
        {
            string senderText = ((TextBox)(sender)).Text;
            UpdateIncidenceTable(sender, senderText != "" ? short.Parse(senderText) : (short)0);
        }

        private void UpdateIncidenceTable(object textBoxSender, short newTableValue)
        {
            ClearGrid(GridIncidenceTable);

            short indexI = (short)(short.Parse(((TextBox)(textBoxSender)).Name.Split('_')[1]) - 1);
            short indexJ = (short)(short.Parse(((TextBox)(textBoxSender)).Name.Split('_')[2]) - 1);

            AdjacencyTable[indexI, indexJ] = newTableValue;
            edgeCount = 0;
            foreach (var item in AdjacencyTable)
            {
                if (item == 1) edgeCount++;
            }
            if (edgeCount == 0)
            {
                return;
            }
            int nodeCount = GridAdjacencyTable.RowDefinitions.Count - 1;
            for (short i = 0; i < nodeCount + 1; i++)
            {
                GridIncidenceTable.RowDefinitions.Add(new RowDefinition { Height = new GridLength(24, GridUnitType.Pixel) });
            }

            GridIncidenceTable.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(32, GridUnitType.Pixel) });

            for (int i = 1; i < nodeCount + 1; i++)
            {
                Label label = new()
                {
                    Content = $"x{i}"
                };
                Grid.SetRow(label, i);
                Grid.SetColumn(label, 0);
                GridIncidenceTable.Children.Add(label);
            }

            for (int x = 0; x < nodeCount; x++)
            {
                for (int y = 0; y < nodeCount; y++)
                {
                    if (AdjacencyTable[x, y] == 1)
                    {
                        GridIncidenceTable.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(32, GridUnitType.Pixel) });
                        int indexLastColumn = GridIncidenceTable.ColumnDefinitions.Count - 1;
                        bool isLoop = false;
                        for (short k = 0; k < nodeCount + 1; k++)
                        {
                            if (k == 0)
                            {
                                Label label = new()
                                {
                                    Content = $"a{indexLastColumn}"
                                };
                                Grid.SetRow(label, k);
                                Grid.SetColumn(label, indexLastColumn);
                                GridIncidenceTable.Children.Add(label);
                            }
                            else
                            {
                                TextBox textBox = new();
                                switch (CurrentGraphType)
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
                                }
                                textBox.MaxLength = 2;
                                Grid.SetRow(textBox, k);
                                Grid.SetColumn(textBox, indexLastColumn);
                                GridIncidenceTable.Children.Add(textBox);
                            }
                        }
                    }
                }
            }
        }

        private void CreateAdjacencyTable(short nodeCount)
        {
            AdjacencyTable = new short[nodeCount, nodeCount];
            for (short i = 0; i < nodeCount + 1; i++)
            {
                GridAdjacencyTable.RowDefinitions.Add(new RowDefinition { Height = new GridLength(24, GridUnitType.Pixel) });
            }
            for (short i = 0; i < nodeCount + 1; i++)
            {
                GridAdjacencyTable.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(32, GridUnitType.Pixel) });
            }
            for (short i = 0; i < nodeCount + 1; i++)
            {
                for (short j = 0; j < nodeCount + 1; j++)
                {
                    if (i == 0 && j == 0) continue;
                    if (i == 0 || j == 0)
                    {
                        Label label = new()
                        {
                            Content = $"x{((i == 0) ? j : i)}"
                        };
                        Grid.SetRow(label, i);
                        Grid.SetColumn(label, j);
                        GridAdjacencyTable.Children.Add(label);
                    }
                    else
                    {
                        AdjacencyTable[i - 1, j - 1] = 0;
                        TextBox textBox = new()
                        {
                            Name = $"textBoxAdjacencyTable_{i}_{j}",
                            Text = "0",
                            MaxLength = 1
                        };
                        textBox.PreviewTextInput += TextBoxAdjacencyTable_PreviewTextInput;
                        textBox.TextChanged += TextBoxAdjacencyTable_TextChanged;
                        textBox.LostFocus += TextBoxAdjacencyTable_LostFocus;
                        Grid.SetRow(textBox, i);
                        Grid.SetColumn(textBox, j);
                        GridAdjacencyTable.Children.Add(textBox);
                    }
                }
            }
        }

        private void ButtonApplyNodesCount_Click(object sender, RoutedEventArgs e)
        {
            ButtonClearTable_Click(sender, e);
            short nodeCount = short.Parse(TextBoxNodesCount.Text);
            CreateAdjacencyTable(nodeCount);
        }

        private void ButtonClearTable_Click(object sender, RoutedEventArgs e)
        {
            ClearGrid(GridIncidenceTable);
            ClearGrid(GridAdjacencyTable);
        }
    }
}
