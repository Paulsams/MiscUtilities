using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Paulsams.MicsUtils.CodeGeneration
{
    public abstract class BaseCreatorScriptFromTemplate
    {
        public struct FileCreateInfo
        {
            public readonly string TemplateScript;
            public readonly string FileName;
            public readonly string AbsolutePathToFolder;

            public FileCreateInfo(string baseScript, string fileName, string absolutePathToFolder)
            {
                TemplateScript = baseScript;
                FileName = fileName;
                AbsolutePathToFolder = absolutePathToFolder;
            }
        }

        protected abstract string TextError { get; }

        protected Dictionary<string, string> _keysValues;

        protected BaseCreatorScriptFromTemplate(string scriptNamespace)
        {
            _keysValues = new Dictionary<string, string>()
            {
                ["NamespaceBegin"] = $"namespace {scriptNamespace}\r\n{{",
                ["NamespaceEnd"] = "}",
            };
        }

        public void Create()
        {
            var fileCreateInfos = OnSetPropertiesAndGetFileCreateInfos();
            if (fileCreateInfos.Any())
            {
                foreach (var info in fileCreateInfos)
                {
                    CreateFile(info);
                }
            }
            else
            {
                Debug.LogError(TextError);
            }
        }

        protected abstract IEnumerable<FileCreateInfo> OnSetPropertiesAndGetFileCreateInfos();

        private void CreateFile(FileCreateInfo info)
        {
            var baseScript = info.TemplateScript;
            StringBuilder script = new StringBuilder((int)(baseScript.Length * 1.5f));
            bool isWord = false;
            StringBuilder word = new StringBuilder();
            for (int i = 0; i < baseScript.Length; ++i)
            {
                if (isWord)
                {
                    if (baseScript[i] == '#')
                    {
                        if (_keysValues.TryGetValue(word.ToString(), out string value))
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

            var absolutePathToFolder = info.AbsolutePathToFolder;
            if (Directory.Exists(absolutePathToFolder) == false)
                Directory.CreateDirectory(absolutePathToFolder);

            File.WriteAllText($"{absolutePathToFolder}/{info.FileName}.cs", script.ToString());
            AssetDatabase.Refresh();
        }
    }
}