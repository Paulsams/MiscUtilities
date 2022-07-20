using System;
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
            var linesInBaseScript = baseScript.Split(Environment.NewLine);
            StringBuilder word = new StringBuilder();
            foreach (var lineInBaseScript in linesInBaseScript)
            {
                foreach (char charInBaseScript in lineInBaseScript)
                {
                    if (isWord)
                    {
                        if (charInBaseScript == '#')
                        {
                            if (_keysValues.TryGetValue(word.ToString(), out string value))
                            {
                                if (value.Substring(value.Length - Environment.NewLine.Length) == Environment.NewLine)
                                    value = value.Remove(value.Length - Environment.NewLine.Length);

                                int tabIndex = lineInBaseScript.IndexOf(word[0]) / 4;
                                var linesInValue = value.Split(Environment.NewLine);
                                for (int j = 0; j < linesInValue.Length - 1; ++j)
                                {
                                    script.AppendLine(linesInValue[j], tabIndex);
                                }
                                script.Append(linesInValue[linesInValue.Length - 1]);
                            }

                            word.Clear();
                            isWord = false;
                            continue;
                        }

                        word.Append(charInBaseScript);
                    }
                    else
                    {
                        if (charInBaseScript == '#')
                        {
                            isWord = true;
                            continue;
                        }

                        script.Append(charInBaseScript);
                    }
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