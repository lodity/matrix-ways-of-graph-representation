using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CDM_Lab_3._1.Utils
{
    internal static class UiUtils
    {
        public static TextBox CreateTableTextBox(string? name, Tuple<int, int> coordinates, int value, int maxLength)
        {
            TextBox textBox = new()
            {
                Text = $"{value}",
                MaxLength = maxLength
            };
            if (name != null)
                textBox.Name = $"{name}_{coordinates.Item1}_{coordinates.Item2}";
            Grid.SetRow(textBox, coordinates.Item1);
            Grid.SetColumn(textBox, coordinates.Item2);
            return textBox;
        }
        public static Label CreateTableLabel(string content, Tuple<int, int> coordinates)
        {
            Label label = new()
            {
                Content = content
            };
            Grid.SetRow(label, coordinates.Item1);
            Grid.SetColumn(label, coordinates.Item2);
            return label;
        }
        public static Button CreateTableButton(string content, SolidColorBrush color, int fontSize, bool isVisible, Tuple<int, int> coordinates)
        {
            Button button = new()
            {
                Content = content,
                Background = color,
                FontSize = fontSize,
                Padding = new Thickness(0, -8, 0, 0),
                Visibility = isVisible ? Visibility.Visible : Visibility.Hidden
            };
            Grid.SetRow(button, coordinates.Item1);
            Grid.SetColumn(button, coordinates.Item2);
            return button;
        }
        public static void AddColumnDefenitions(Grid grid, int count)
        {
            for (int i = 0; i < count; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(32, GridUnitType.Pixel) });
        }
        public static void AddRowDefenitions(Grid grid, int count)
        {
            for (int i = 0; i < count; i++)
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(24, GridUnitType.Pixel) });
        }
    }
}
