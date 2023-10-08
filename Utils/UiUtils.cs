using System;
using System.Windows.Controls;

namespace CDM_Lab_3._1.Utils
{
    internal class UiUtils
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
    }
}
