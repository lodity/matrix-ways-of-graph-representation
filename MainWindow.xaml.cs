﻿using CDM_Lab_3._1.Models;
using CDM_Lab_3._1.Utils;
using System;
using System.Collections.Generic;
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
        private static void ClearGrid(Grid grid)
        {
            grid.Children.Clear();
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();
        }
        private void ComboBoxGraphType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClearGrid(GridIncidenceTable);
        }
        private void OnlyNumbersValidation_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = TextUtils.IsTextSatisfiesRegex(e.Text);
        }
        private void TextBoxAdjacencyTable_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            short textBoxSenderText = short.Parse(((TextBox)sender).Text);
            if (textBoxSenderText >= 0 && textBoxSenderText < 9)
            {
                ((TextBox)sender).Text = (textBoxSenderText + 1).ToString();
            }
        }
        private void TextBoxAdjacencyTable_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            TextBox textBoxSender = (TextBox)sender;
            short textBoxSenderText = short.Parse(textBoxSender.Text);
            if (((textBoxSenderText > 0 || e.Delta > 0) && textBoxSenderText < 9) || (textBoxSenderText == 9 && e.Delta < 0))
            {
                textBoxSender.Text = (int.Parse(textBoxSender.Text) + (e.Delta > 0 ? 1 : -1)).ToString();
            }
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
            TextBox textBoxSender = ((TextBox)sender);
            int indexX = (int.Parse(textBoxSender.Name.Split('_')[1]) - 1);
            int indexY = (int.Parse(textBoxSender.Name.Split('_')[2]) - 1);
            AdjacencyTable[indexX, indexY] = textBoxSender.Text != "" ? short.Parse(textBoxSender.Text) : (short)0;

            UpdateIncidenceTable();
        }

        private void UpdateIncidenceTable()
        {
            ClearGrid(GridIncidenceTable);

            int nodeCount = GridAdjacencyTable.RowDefinitions.Count - 1;
            short[,] AdjacencyTableCopy = (short[,])AdjacencyTable.Clone();
            edgeCount = 0;
            List<Tuple<int, int>> listOfIndexes = new();

            for (int x = 0; x < nodeCount; x++)
            {
                for (int y = 0; y < nodeCount; y++)
                {
                    if (AdjacencyTable[x, y] >= 1 && ((!listOfIndexes.Contains(new Tuple<int, int>(y, x)) || x == y) || CurrentGraphType == GraphType.Directed))
                    {
                        listOfIndexes.Add(new Tuple<int, int>(x, y));
                        edgeCount++;
                    }
                }
            }

            if (edgeCount == 0)
            {
                return;
            }
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
            bool isNoZeroRemained;
            short count = 0;
            do
            {
                isNoZeroRemained = false;
                count++;
                for (int x = 0; x < nodeCount; x++)
                {
                    for (int y = 0; y < nodeCount; y++)
                    {
                        if (AdjacencyTableCopy[x, y] != 0)
                        {
                            isNoZeroRemained = true;
                        }
                        if (AdjacencyTableCopy[x, y] != 0 && ((!listOfIndexes.Contains(new Tuple<int, int>(y, x)) || x == y) || CurrentGraphType == GraphType.Directed))
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
                                                {
                                                    textBox.Text = "1";
                                                }
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
                        if (AdjacencyTableCopy[x, y] >= 1)
                        {
                            AdjacencyTableCopy[x, y]--;
                        }
                    }
                }

            } while (isNoZeroRemained);
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
            for (short x = 0; x < nodeCount + 1; x++)
            {
                for (short y = 0; y < nodeCount + 1; y++)
                {
                    if (x == 0 && y == 0) continue;
                    if (x == 0 || y == 0)
                    {
                        Label label = new()
                        {
                            Content = $"x{((x == 0) ? y : x)}"
                        };
                        Grid.SetRow(label, x);
                        Grid.SetColumn(label, y);
                        GridAdjacencyTable.Children.Add(label);
                    }
                    else
                    {
                        AdjacencyTable[x - 1, y - 1] = 0;
                        TextBox textBox = new()
                        {
                            Name = $"textBoxAdjacencyTable_{x}_{y}",
                            Text = "0",
                            MaxLength = 1
                        };
                        textBox.PreviewTextInput += OnlyNumbersValidation_PreviewTextInput;
                        textBox.TextChanged += TextBoxAdjacencyTable_TextChanged;
                        textBox.LostFocus += TextBoxAdjacencyTable_LostFocus;
                        textBox.MouseDoubleClick += TextBoxAdjacencyTable_MouseDoubleClick;
                        textBox.MouseWheel += TextBoxAdjacencyTable_MouseWheel;
                        Grid.SetRow(textBox, x);
                        Grid.SetColumn(textBox, y);
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
