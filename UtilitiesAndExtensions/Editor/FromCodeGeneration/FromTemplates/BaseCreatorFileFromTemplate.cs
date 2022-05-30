using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;

namespace Paulsams.MicsUtils
{
    public abstract class BaseCreatorScriptFromTemplate
    {
        protected Dictionary<string, string> _keyToValues;

        protected BaseCreatorScriptFromTemplate(string scriptNamespace)
        {
            _keyToValues = new Dictionary<string, string>()
            {
                ["NamespaceBegin"] = $"namespace {scriptNamespace}\r\n{{",
                ["NamespaceEnd"] = "}",
            };
        }

        public abstract void Create();

        protected void CreateFile(string baseScript, string fileName, string absolutePathToFolder)
        {
            StringBuilder script = new StringBuilder((int)(baseScript.Length * 1.5f));
            bool isWord = false;
            StringBuilder word = new StringBuilder();
            for (int i = 0; i < baseScript.Length; ++i)
            {
                if (isWord)
                {
                    if (baseScript[i] == '#')
                    {
                        if (_keyToValues.TryGetValue(word.ToString(), out string value))
                            script.Append(value);

                        word.Clear();
                        isWord = false;
                        continue;
                    }

                    word.Append(baseScript[i]);
                }
                else
                {
                    if (baseScript[i] == '#')
                    {
                        isWord = true;
                        continue;
                    }

                    script.Append(baseScript[i]);
                }
            }

            if (Directory.Exists(absolutePathToFolder) == false)
                Directory.CreateDirectory(absolutePathToFolder);

            File.WriteAllText($"{absolutePathToFolder}/{fileName}.cs", script.ToString());
            AssetDatabase.Refresh();
        }
    }
}