using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace CDM_Lab_3._1.Utils
{
    internal class TextUtils
    {
        private static readonly Regex numberRegex = new("[^0-9]");
        /// <summary>
        /// Tests whether the text satisfies a regex
        /// By default the regex is only numbers ([^0-9])
        /// </summary>
        public static bool IsTextSatisfiesRegex(string text, [Optional] Regex regex)
        {
            regex ??= numberRegex;
            return regex.IsMatch(text);
        }
    }
}
