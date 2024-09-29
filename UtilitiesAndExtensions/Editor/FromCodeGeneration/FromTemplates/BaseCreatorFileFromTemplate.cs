using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEditor.Compilation;

namespace Paulsams.MicsUtils.CodeGeneration
{
    /// <summary>
    /// Base class for generating C# scripts based on template script.
    /// <para> That is, you can create your own successor somewhere from the outside and call the method: <see cref="Create"/>. </para>
    /// <example>
    /// I’ll say in advance that the example is a little artificial, since it’s always better to put general logic into templates,
    /// but I just wanted an extensive example.
    /// Example template script:
    /// <code>
    /// using UnityEngine;
    /// 
    /// &#35;NamespaceBegin&#35;
    /// public abstract class &#35;ClassName&#35; : MonoBehaviour
    /// {
    ///     private void Awake()
    ///     {
    ///         Debug.Log(&#35;CustomMethod&#35;());
    ///     }
    /// 
    ///     &#35;CustomMethod&#35;
    /// }
    /// &#35;NamespaceEnd&#35;
    /// </code>
    /// 
    /// Example implementation of generator:
    /// <code>
    /// public class ExampleGenerator : BaseCodeGeneratorFromTemplate
    /// {
    ///     private const string _pathToTemplate = "Assets/ExampleGenerator.txt";
    /// 
    ///     private const string _classNameKey = "ClassName";
    ///     private const string _customMethodNameKey = "CustomMethodName";
    ///     private const string _customMethodKey = "CustomMethod";
    /// 
    ///     protected override string TextError => $"{nameof(ExampleGenerator)} failed to generate code.";
    /// 
    ///     private readonly string _templateScriptText = AssetDatabase.LoadAssetAtPath&lt;TextAsset&gt;(_pathToTemplate).text;
    /// 
    ///     protected override IEnumerable&lt;FileCreateInfo&gt; OnSetPropertiesAndGetFileCreateInfos()
    ///     {
    ///         var inserter = new InserterCode();
    ///         yield return CreateFileInfo(inserter, "Test1", "MyCustomMethod1",
    ///             new[] { "Test1_1, Test1_2" });
    ///         inserter.Clear();
    ///         yield return CreateFileInfo(inserter, "Test2", "MyCustomMethod2",
    ///             new[] { "Test2_1, Test2_2", "Test2_3" });
    ///     }
    /// 
    ///     private FileCreateInfo CreateFileInfo(InserterCode inserter, string fileName,
    ///         string methodName, string[] randomizeLogs)
    ///     {
    ///         KeyToBlockCode[_classNameKey] = fileName;
    ///         KeyToBlockCode[_customMethodNameKey] = methodName;
    ///         inserter.AppendLine($"private string {methodName}()");
    ///         inserter.AppendOpeningBrace();
    ///         {
    ///             inserter.AppendLine($"return Random.Range(0, {randomizeLogs.Length}) switch");
    ///             inserter.AppendOpeningBrace();
    ///             {
    ///                 for (int i = 0; i &lt; randomizeLogs.Length; ++i)
    ///                     inserter.AppendLine($"{i} => \"{randomizeLogs[i]}\",");
    ///             }
    ///             inserter.AppendBreakingBrace(true);
    ///         }
    ///         inserter.AppendBreakingBrace();
    /// 
    ///         KeyToBlockCode[_customMethodKey] = inserter.ToString();
    ///         return FileCreateInfo.Create(_templateScriptText, fileName, "Assets", "TestNamespace");
    ///     }
    /// }
    /// </code>
    /// </example>
    /// </summary>
    public abstract class BaseCodeGeneratorFromTemplate
    {
        /// <summary>
        /// Class that provides an abstraction over <see cref="System.Text.StringBuilder"/> with a focus on C# code generation.
        /// </summary>
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

            public void AppendBreakingBrace(bool semicolon = false) =>
                Builder.AppendBreakingBrace(ref _tabIndex, semicolon);

            public void Clear() => Builder.Clear();

            public override string ToString() => Builder.ToString();
        }

        /// <summary>
        /// Description of the file that will be generated when invoke <see cref="M:Paulsams.MicsUtils.CodeGeneration.BaseCodeGeneratorFromTemplate.Create"/>.
        /// </summary>
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

            private FileCreateInfo(string templateScript, string fileName, string absolutePathToFolder,
                string @namespace)
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

        /// <summary>
        /// Dictionary of keys that the parser will access if it finds such a pattern <c>#ExampleKey#</c>.
        /// </summary>
        protected readonly Dictionary<string, string> KeyToBlockCode = new()
        {
            [_namespaceBeginKey] = "",
            [_namespaceEndKey] = "",
        };

        /// <summary>
        /// Invoke code generation.
        /// </summary>
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

        /// <summary>
        /// It should return a collection of information about the files that will be filled with code.
        /// And it’s best to do it through iterator, since you can change <see cref="KeyToBlockCode"/> before each new file.
        /// </summary>
        /// <returns></returns>
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