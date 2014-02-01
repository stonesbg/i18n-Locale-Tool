using i18n.Helper.BingTranslate;
using i18n.LocaleTool.Models;
using JsonFx.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;

namespace i18n.Helper
{
    public enum SaveType
    {
        Json,
        Csv
    }

    public interface ILocalizationHandler
    {
    }

    public class I18NHandler
    {
        readonly BingHandler _bingHandler;

        public I18NHandler()
        {
            _bingHandler = new BingHandler();
        }

        public void CreateDictionaryFile(string filePath, string outputPath, SaveType type, string languageCode = null)
        {
            List<i18nDirectoryFile> dictionaries = null;

            var fileInfo = new FileInfo(filePath);

            if(outputPath.Trim().Length > 0 && languageCode == null)
            {
                // Get the root above the parent Directory
                // The Parent Directory is the language folder at least assumed.
                var dirInfo = new DirectoryInfo(fileInfo.DirectoryName);
                if (dirInfo.Parent != null) outputPath = dirInfo.Parent.Name + "ConvertedCsv" + @"\" + fileInfo.Name;
            }
            else if (outputPath.Trim().Length > 0 && languageCode != null)
            {
                var dirInfo = new DirectoryInfo(fileInfo.DirectoryName);
                if (dirInfo.Parent != null) outputPath = dirInfo.Parent.Name + languageCode + @"\" + fileInfo.Name;
            }

            switch(fileInfo.Extension.ToLower())
            {
                case ".json":
                    dictionaries = LoadJsonFileOrFolder(filePath);
                    break;
                case ".csv":
                    dictionaries = LoadCsvFileOrFolder(filePath);
                    break;
            }

            if (dictionaries != null)
            {
                if (dictionaries.Count > 0)
                {
                    SaveDictionaries(outputPath, dictionaries, type);
                }
            }
        }

        public void TranslateDictionaryFile(string languageCode, string filePath, string outputPath)
        {
            const string sourceLanguage = "en";

            List<i18nDirectoryFile> dictionaries = null;

            var fileInfo = new FileInfo(filePath);

            switch (fileInfo.Extension.ToLower())
            {
                case ".json":
                    dictionaries = LoadJsonFileOrFolder(filePath);
                    break;
            }

            if (dictionaries != null)
            {
                foreach (i18nDirectoryFile dictonary in dictionaries)
                {
                    var keys = new List<string>(dictonary.Dictionary.Keys);
                    foreach(string key in keys)
                    {
                        string untranslatedFile = dictonary.Dictionary[key].ToString();
                        var translationResult = _bingHandler.Translate(untranslatedFile, sourceLanguage, languageCode);

                        dictonary.Dictionary[key] = translationResult;
                    }

                    string json = CreateJsonFile(dictonary.Dictionary);
                    var dirInfo = new DirectoryInfo(outputPath);

                    string outPath = dirInfo.FullName + @"\" + languageCode + @"\" + fileInfo.Name;
                    SaveJson(json, outPath);
                }
            }
        }

        public void GenerateFakeJsonFileForLanguageCode(string languageCode, string filePath, string outputPath)
        {
            var dictionaries = LoadJsonFileOrFolder(filePath);

            if (dictionaries.Count > 0)
            {
                var localizedDictionaries = new List<i18nDirectoryFile>();
                dictionaries.ForEach(
                    d => localizedDictionaries.Add(new i18nDirectoryFile
                    {
                        FileInfo = d.FileInfo,
                        Dictionary = d.Dictionary.ToDictionary(item => item.Key, item => (object)(item.Value + "_" + languageCode))
                    }));

                SaveDictionaries(outputPath,localizedDictionaries, SaveType.Json, languageCode);
            }
        }

        public void GenerateLongestStringsFile(string filePath, string outputPath)
        {
            var dictionaries = LoadJsonFileOrFolder(filePath);

            if (dictionaries.Count > 0)
            {
                var getUniqueKeys = dictionaries.SelectMany(d => d.Dictionary.Keys).Distinct().ToList();

                var longestDictionary = new i18nDirectoryFile()
                {
                    Dictionary = new Dictionary<string, object>(),
                    FileInfo = new FileInfo(filePath),
                };

                getUniqueKeys.ForEach(
                    k =>
                        longestDictionary.Dictionary.Add(k,
                            dictionaries.Select(d => d.Dictionary[k].ToString()).OrderByDescending(s => s.Length).First()));

                SaveDictionaries(outputPath, new List<i18nDirectoryFile>() {longestDictionary}, SaveType.Json, "dev_Longest");
            }
        }

        public void GenerateShortestStringsFile(string filePath, string outputPath)
        {
            var dictionaries = LoadJsonFileOrFolder(filePath);

            if (dictionaries.Count > 0)
            {
                var getUniqueKeys = dictionaries.SelectMany(d => d.Dictionary.Keys).Distinct().ToList();

                var longestDictionary = new i18nDirectoryFile()
                {
                    Dictionary = new Dictionary<string, object>(),
                    FileInfo = new FileInfo(filePath),
                };

                getUniqueKeys.ForEach(
                    k =>
                        longestDictionary.Dictionary.Add(k,
                            dictionaries.Select(d => d.Dictionary[k].ToString()).OrderByDescending(s => s.Length).Last()));

                SaveDictionaries(outputPath, new List<i18nDirectoryFile>() { longestDictionary }, SaveType.Json, "dev_Shortest");
            }
        }

        public bool IsDirectory(string path)
        {
            bool result = true;
            var fileTest = new FileInfo(path);
            if (fileTest.Exists)
            {
                result = false;
            }
            else
            {
                if (fileTest.Extension != "")
                {
                    result = false;
                }
            }
            return result;
        }

        private List<i18nDirectoryFile> LoadJsonFileOrFolder(string filePath)
        {
            var output = new List<i18nDirectoryFile>();
            bool isFile = false;
            if (File.Exists(filePath))
                isFile = true;

            if (isFile)
            {
                var info = new FileInfo(filePath);

                var i18NDictionary = CreateDictionaryFromFile(filePath);
                output.Add(new i18nDirectoryFile { FileInfo = info, Dictionary = i18NDictionary });
            }
            else
            {
                List<string> files = DirSearch(filePath);

                foreach (string file in files)
                {
                    var info = new FileInfo(file);

                    var i18nDictionary = CreateDictionaryFromFile(file);
                    output.Add(new i18nDirectoryFile { FileInfo = info, Dictionary = i18nDictionary });
                }
            }

            return output;
        }

        public List<String> DirSearch(string sDir)
        {
            var dirInfo = new DirectoryInfo(sDir);
            var files = new List<String>();
            foreach (string f in Directory.GetFiles(dirInfo.FullName))
            {
                files.Add(f);
            }
            foreach (string d in Directory.GetDirectories(dirInfo.FullName))
            {
                files.AddRange(DirSearch(d));
            }

            return files;
        }

        private List<i18nDirectoryFile> LoadCsvFileOrFolder(string filePath)
        {
            var output = new List<i18nDirectoryFile>();
            bool isFile = false;
            if (File.Exists(filePath))
                isFile = true;

            if (isFile)
            {
                var info = new FileInfo(filePath);

                using (var reader = new StreamReader(File.OpenRead(filePath)))
                {
                    var i18NDictionary = new Dictionary<string, object>();

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (line != null)
                        {
                            var values = line.Split(',');

                            i18NDictionary.Add(values[0].Trim('"'), values[1].Trim('"'));
                        }
                    }

                    output.Add(new i18nDirectoryFile { FileInfo = info, Dictionary = i18NDictionary });
                }
            }

            return output;
        }

        private void SaveDictionaries(string outputPath, IEnumerable<i18nDirectoryFile> dictionaries, SaveType type, string languageCode = null)
        {
            foreach (i18nDirectoryFile dictionaryItem in dictionaries)
            {
                string outputFullPath = ConstructFilePath(outputPath, dictionaryItem.FileInfo, type, languageCode);

                switch (type)
                {
                    case SaveType.Json:
                        //Convert Dictionary to be json string
                        string json = CreateJsonFile(dictionaryItem.Dictionary);
                        SaveJson(json, outputFullPath);
                        break;
                    case SaveType.Csv:
                        SaveDictionary(dictionaryItem.Dictionary, outputFullPath);
                        break;
                }
            }
        }

        private string CreateJsonFile(Dictionary<string, object> dictionary)
        {
            var parser = new I18NJsonParser(); //using System.Web.Script.Serialization;
            object jsonObject = parser.GenerateJsonObject(dictionary);

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObject, Newtonsoft.Json.Formatting.Indented);

            return json;
        }

        private Dictionary<string, object> CreateDictionaryFromFile(string filePath)
        {
            //Load in File
            string json = File.ReadAllText(filePath);

            return CreateDictionaryFromJson(json);
        }

        private Dictionary<string, object> CreateDictionaryFromJson(string json)
        {
            var i18NParser = new I18NJsonParser();
            var reader = new JsonReader();
            dynamic jsonObject = reader.Read(json);
            var i18NDictionary = new Dictionary<string, object>();

            i18NParser.GenerateDictionary((ExpandoObject)jsonObject, i18NDictionary, "");

            return i18NDictionary;
        }

        private void SaveDictionary(Dictionary<string, object> i18NDictionary, string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);
            if (directory != null) CreateDirectory(new DirectoryInfo(directory));

            File.WriteAllLines(filePath, i18NDictionary.Select(x => "\"" + x.Key + "\",\"" + x.Value + "\""));
        }

        public void CreateDirectory(DirectoryInfo directory)
        {
            if (directory.Parent != null && !directory.Parent.Exists)
                CreateDirectory(directory.Parent);
            directory.Create();
        }

        private void SaveJson(string json, string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);
            if (directory != null) CreateDirectory(new DirectoryInfo(directory));

            File.WriteAllText(filePath, json);
        }

        private string ConstructFilePath(string outDir, FileInfo fileInfo, SaveType type, string languageCode = null)
        {
            var pathItems = new List<string>();

            if (outDir[outDir.Length - 1] == '\\')
            {
                outDir = outDir.Substring(0, outDir.Length - 2);
            }

            pathItems.Add(outDir);

            string parentFolder;
            if (languageCode != null)
            {
                parentFolder = languageCode;
            }
            else
            {
                parentFolder = fileInfo.Directory.Name;
            }

            pathItems.Add(parentFolder);
            
            string nameOfFile = Path.GetFileNameWithoutExtension(fileInfo.Name);
            pathItems.Add(nameOfFile);

            string outputFullPath = string.Join(@"\", pathItems);

            switch (type)
            {
                case SaveType.Json:
                    outputFullPath += ".json";
                    break;
                case SaveType.Csv:
                    outputFullPath += ".csv";
                    break;
            }

            return outputFullPath;
        }
    }
}
