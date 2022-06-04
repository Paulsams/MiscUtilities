using System.Collections.Generic;
using System.IO;
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

        protected Dictionary<string, string> _properties;

        protected string this[string key]
        {
            set
            {
                _properties[key] = value;
            }
        }

        protected BaseCreatorScriptFromTemplate(string scriptNamespace)
        {
            _properties = new Dictionary<string, string>()
            {
                ["NamespaceBegin"] = $"namespace {scriptNamespace}\r\n{{",
                ["NamespaceEnd"] = "}",
            };
        }

        public void Create()
        {
            if (ValidCreate())
            {
                foreach (var info in OnSetPropertiesAndGetFileCreateInfos())
                {
                    CreateFile(info);
                }
            }
            else
            {
                Debug.LogError(TextError);
            }
        }

        protected abstract bool ValidCreate();
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
                        if (_properties.TryGetValue(word.ToString(), out string value))
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