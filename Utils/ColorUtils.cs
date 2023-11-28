using System;
using System.Collections.Generic;

namespace CDM_Lab_3._1.Utils
{
    public static class ColorUtils
    {
        public static readonly List<string> colors = new()
        {
            "#FF0000",
            "#00FF00",
            "#0000FF",
            "#FF00FF",
            "#00FFFF",
            "#FFA500",
            "#A52A2A",
            "#008000",
            "#800080",
            "#000080",
            "#FF4500",
            "#008080",
            "#800000",
            "#808080",
            "#008B8B",
            "#FFD700",
            "#8B008B",
            "#DC143C",
            "#556B2F"
        };
        public static string InvertHexColor(string hexColor)
        {
            if (hexColor.StartsWith("#"))
                hexColor = hexColor.Substring(1);

            if (hexColor.Length != 6)
                throw new ArgumentException("Input hex color is not in the correct format.");

            // Convert hex to an integer
            int rgb = int.Parse(hexColor, System.Globalization.NumberStyles.HexNumber);

            // Invert the RGB values (bitwise NOT operation)
            int invertedRgb = ~rgb & 0x00FFFFFF;

            return $"#{invertedRgb:X6}";
        }
    }
}
