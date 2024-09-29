using System.Text;

namespace Paulsams.MicsUtils
{
    /// <summary>
    /// String-related utilities.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Finds large letters in a line and puts spaces between them.
        /// </summary>
        /// <example> <code> "TestSplitOne".SplitByUpperSymbols() == "Test Split One" </code> </example>
        public static string SplitByUpperSymbols(this string text)
        {
            StringBuilder stringBuilder = new StringBuilder(text);
            int indexOffset = 0;
            for (int i = 1; i < text.Length; ++i)
            {
                char currentCharacter = text[i];
                if (char.IsUpper(currentCharacter))
                    stringBuilder.Insert(i + indexOffset++, ' ');
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Removes all spaces.
        /// </summary>
        public static string ClearSpaces(this string text) => text.Replace(" ", "");
    }
}
