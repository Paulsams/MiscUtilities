using System.Text;

namespace Paulsams.MicsUtil
{
    public static class StringBuilderExtensions
    {
        public static void AppendLine(this StringBuilder stringBuilder, string text, int tabIndex)
        {
            AppendTabs(stringBuilder, tabIndex);
            stringBuilder.AppendLine(text);
        }

        public static void AppendOpeningBrace(this StringBuilder stringBuilder, ref int tabIndex)
        {
            AppendLine(stringBuilder, "{", tabIndex);
            ++tabIndex;
        }

        public static void AppendBreakingBrace(this StringBuilder stringBuilder, ref int tabIndex, bool semicolon = false)
        {
            --tabIndex;
            AppendLine(stringBuilder, "}" + (semicolon ? ";" : ""), tabIndex);
        }

        public static void AppendTabs(this StringBuilder stringBuilder, int tabIndex)
        {
            for (int i = 0; i < tabIndex; ++i)
                stringBuilder.Append("    ");
        }
    }
}