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
        short edgeCount = 0;
        short[,] AdjacencyTable;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void ClearGrid(Grid grid)
        {
            grid.Children.Clear();
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();
        }
        private void TextBoxNodesCount_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = TextUtils.IsTextSatisfiesRegex(e.Text);
        }
        private void TextBoxAdjacencyTable_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = TextUtils.IsTextSatisfiesRegex(e.Text, new Regex("[^0-1]"));
            UpdateIncidenceTable(sender, short.Parse(e.Text));
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

            for (int i = 0; i < nodeCount; i++)
            {
                for (int j = 0; j < nodeCount; j++)
                {
                    if (AdjacencyTable[i, j] == 1)
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
                                if (i == j && i == k)
                                {
                                    textBox.Text = "2";
                                    isLoop = true;
                                }
                                else if (!isLoop && (k - 1 == i || k - 1 == j))
                                    textBox.Text = "1";
                                else
                                    textBox.Text = "0";

                                textBox.MaxLength = 1;
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
