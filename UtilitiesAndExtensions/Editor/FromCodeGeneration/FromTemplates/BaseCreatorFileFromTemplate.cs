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
        protected class InsertWord
        {
            public readonly StringBuilder Builder;
            private int _tabIndex;

            public InsertWord()
            {
                Builder = new StringBuilder();
            }

            public InsertWord(int capacity)
            {
                Builder = new StringBuilder(capacity);
            }

            public void AppendLine(string text)
            {
                Builder.AppendLine(text, _tabIndex);
            }

            public void Append(string text)
            {
                Builder.Append(text);
            }

            public void AppendWithTabs(string text)
            {
                Builder.Append(text, _tabIndex);
            }

            public void AppendTabs()
            {
                Builder.AppendTabs(_tabIndex);
            }

            public void AppendOpeningBrace()
            {
                Builder.AppendOpeningBrace(ref _tabIndex);
            }

            public void AppendBreakingBrace(bool semicolon = false)
            {
                Builder.AppendBreakingBrace(ref _tabIndex, semicolon);
            }

            public override string ToString()
            {
                return Builder.ToString();
            }
        }

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
                ["NamespaceBegin"] = $"namespace {scriptNamespace}{Environment.NewLine}{{",
                ["NamespaceEnd"] = "}",
            };
        }

        public void Create()
        {
            var fileCreateInfos = OnSetPropertiesAndGetFileCreateInfos();
            bool isAnyFile = false;
            foreach (var info in fileCreateInfos)
            {
                isAnyFile = true;
                CreateFile(info);
            }

            if (isAnyFile == false)
                Debug.LogError(TextError);
        }

        protected abstract IEnumerable<FileCreateInfo> OnSetPropertiesAndGetFileCreateInfos();

        private void CreateFile(FileCreateInfo info)
        {
            var baseScript = info.TemplateScript;
            StringBuilder script = new StringBuilder((int)(baseScript.Length * 1.5f));
            var linesInBaseScript = baseScript.Split(Environment.NewLine);
            StringBuilder word = new StringBuilder();
            for (int i = 0; i < linesInBaseScript.Length - 1; i++)
            {
                string lineInBaseScript = linesInBaseScript[i];
                IteratorInLine(script, word, lineInBaseScript);

                script.AppendLine();
            }

            IteratorInLine(script, word, linesInBaseScript[linesInBaseScript.Length - 1]);

            var absolutePathToFolder = info.AbsolutePathToFolder;
            if (Directory.Exists(absolutePathToFolder) == false)
                Directory.CreateDirectory(absolutePathToFolder);

            File.WriteAllText($"{absolutePathToFolder}/{info.FileName}.cs", script.ToString());
            AssetDatabase.Refresh();
        }

        private void IteratorInLine(StringBuilder script, StringBuilder word, string lineInBaseScript)
        {
            bool isWord = false;
            foreach (char charInBaseScript in lineInBaseScript)
            {
                if (isWord)
                {
                    if (charInBaseScript == '#')
                    {
                        if (_keysValues.TryGetValue(word.ToString(), out string value))
                        {
                            if (value.Length >= Environment.NewLine.Length && value.Substring(value.Length - Environment.NewLine.Length) == Environment.NewLine)
                                value = value.Remove(value.Length - Environment.NewLine.Length);

                            int tabIndex = lineInBaseScript.IndexOf(word[0]) / 4;
                            var linesInValue = value.Split(Environment.NewLine);
                            if (linesInValue.Length == 1)
                            {
                                script.Append(linesInValue[0]);
                            }
                            else
                            {
                                script.AppendLine(linesInValue[0]);

                                for (int j = 1; j < linesInValue.Length - 1; ++j)
                                {
                                    script.AppendLine(linesInValue[j], tabIndex);
                                }

                                script.Append(linesInValue[linesInValue.Length - 1], tabIndex);
                            }
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
    }
}