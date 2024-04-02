using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEditor.Compilation;

namespace Paulsams.MicsUtils.CodeGeneration
{
    public abstract class BaseCodeGeneratorFromTemplate
    {
        protected class InserterCode
        {
            public readonly StringBuilder Builder;
            private int _tabIndex;

            public InserterCode() => Builder = new StringBuilder();

            public InserterCode(int capacity) => Builder = new StringBuilder(capacity);
            
            public void IncrementCountTabs() => ++_tabIndex;
            public void DecrementCountTabs() => --_tabIndex;

            public void SkipLine() => Builder.AppendLine();
            public void AppendLine(string text) => Builder.AppendLine(text, _tabIndex);
            public void Append(string text) => Builder.Append(text);

            public void AppendWithTabs(string text) => Builder.Append(text, _tabIndex);
            public void AppendTabs() => Builder.AppendTabs(_tabIndex);

            public void AppendOpeningBrace() => Builder.AppendOpeningBrace(ref _tabIndex);
            public void AppendBreakingBrace(bool semicolon = false) => Builder.AppendBreakingBrace(ref _tabIndex, semicolon);

            public override string ToString() => Builder.ToString();
        }

        public struct FileCreateInfo
        {
            public readonly string TemplateScript;
            public readonly string FileName;
            public readonly string PathToFolder;
            public readonly string Namespace;

            public static FileCreateInfo Create(string templateScript, string fileName,
                string pathToFolder, string @namespace = null) =>
                new FileCreateInfo(templateScript, fileName, pathToFolder, @namespace);

            public static FileCreateInfo CreateWithNamespaceFromPath(string templateScript, string fileName,
                string absolutePathToFolder, string relativePathInUnityProject) =>
                new FileCreateInfo(templateScript, fileName, absolutePathToFolder, 
                    CompilationPipeline.GetAssemblyRootNamespaceFromScriptPath(relativePathInUnityProject));
            
            public static FileCreateInfo CreateWithNamespaceFromPath(string templateScript, string fileName,
                string relativePathInUnityProject) =>
                new FileCreateInfo(templateScript, fileName, relativePathInUnityProject, 
                    CompilationPipeline.GetAssemblyRootNamespaceFromScriptPath(relativePathInUnityProject));

            private FileCreateInfo(string templateScript, string fileName, string absolutePathToFolder, string @namespace)
            {
                TemplateScript = templateScript;
                FileName = fileName;
                PathToFolder = absolutePathToFolder;
                Namespace = @namespace;
            }
        }

        private const string _namespaceBeginKey = "NamespaceBegin";
        private const string _namespaceEndKey = "NamespaceEnd";

        protected abstract string TextError { get; }

        protected readonly Dictionary<string, string> KeyToBlockCode;

        protected BaseCodeGeneratorFromTemplate()
        {
            KeyToBlockCode = new Dictionary<string, string>()
            {
                [_namespaceBeginKey] = "",
                [_namespaceEndKey] = "",
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
            if (info.Namespace != null)
            {
                KeyToBlockCode["NamespaceBegin"] = $"namespace {info.Namespace}{Environment.NewLine}{{";
                KeyToBlockCode["NamespaceEnd"] = "}";
            }

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

            if (info.Namespace != null)
            {
                KeyToBlockCode["NamespaceBegin"] = "";
                KeyToBlockCode["NamespaceEnd"] = "";
            }

            var pathToFolder = info.PathToFolder;
            if (Directory.Exists(pathToFolder) == false)
                Directory.CreateDirectory(pathToFolder);

            File.WriteAllText($"{pathToFolder}/{info.FileName}.cs", script.ToString());
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
                        if (KeyToBlockCode.TryGetValue(word.ToString(), out string value))
                        {
                            if (value.Length >= Environment.NewLine.Length
                                && value.Substring(value.Length - Environment.NewLine.Length) == Environment.NewLine)
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
                                    script.AppendLine(linesInValue[j], tabIndex);

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