using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Paulsams.MicsUtils
{
    public static class StringExtensions
    {
        public static string SplitByUpperSymbols(this string text)
        {
            StringBuilder stringBuilder = new StringBuilder(text);
            int indexOffset = 0;
            for (int i = 1; i < text.Length; ++i)
            {
                char currentCharacter = text[i];
                if (currentCharacter.IsUpper())
                    stringBuilder.Insert(i + indexOffset++, ' ');
            }
            return stringBuilder.ToString();
        }

        public static string ClearSpaces(this string text) => text.Replace(" ", "");
    }
}
