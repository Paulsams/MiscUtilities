using System.Text;

namespace Paulsams.MicsUtils.CodeGeneration
{
    public static class ExtensionsFromCodeGeneration
    {
        public static void Append(this StringBuilder stringBuilder, string text, int tabIndex)
        {
            AppendTabs(stringBuilder, tabIndex);
            stringBuilder.Append(text);
        }

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