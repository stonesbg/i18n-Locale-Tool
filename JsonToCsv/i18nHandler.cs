using i18n.Helper;
using i18n.Helper.BingTranslate;
using i18n.LocaleTool.Models;
using JsonFx.Json;
using Microsoft;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JsonToCsv
{
    public enum SaveType
    {
        Json,
        Csv
    }

    public class i18nHandler
    {
        BingHandler _bingHandler;

        public i18nHandler()
        {
            _bingHandler = new BingHandler();
        }

        public void CreateDictionaryFile(string filePath, string outputPath, SaveType type, string languageCode = null)
        {
            List<i18nDirectoryFile> dictionaries = null;

            FileInfo fileInfo = new FileInfo(filePath);

            if(outputPath.Trim().Length > 0 && languageCode == null)
            {
                // Get the root above the parent Directory
                // The Parent Directory is the language folder at least assumed.
                DirectoryInfo dirInfo = new DirectoryInfo(fileInfo.DirectoryName);
                outputPath = dirInfo.Parent.Name + "ConvertedCsv" + @"\" + fileInfo.Name;
            }
            else if (outputPath.Trim().Length > 0 && languageCode == null)
            {
                // Get the root above the parent Directory
                // The Parent Directory is the language folder at least assumed.
                DirectoryInfo dirInfo = new DirectoryInfo(fileInfo.DirectoryName);
                outputPath = dirInfo.Parent.Name + languageCode + @"\" + fileInfo.Name;
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
            string sourceLanguage = "en";

            List<i18nDirectoryFile> dictionaries = null;

            FileInfo fileInfo = new FileInfo(filePath);

            switch (fileInfo.Extension.ToLower())
            {
                case ".json":
                    dictionaries = LoadJsonFileOrFolder(filePath);
                    break;
            }

            var newI18nDirectoryFile = new List<i18nDirectoryFile>();

            if (dictionaries != null)
            {
                foreach (i18nDirectoryFile dictonary in dictionaries)
                {
                    List<string> keys = new List<string>(dictonary.Dictionary.Keys);
                    foreach(string key in keys)
                    {
                        string untranslatedFile = dictonary.Dictionary[key].ToString();
                        var translationResult = _bingHandler.Translate(untranslatedFile, sourceLanguage, languageCode);

                        dictonary.Dictionary[key] = translationResult;
                    }

                    string json = CreateJsonFile(dictonary.Dictionary);
                    DirectoryInfo dirInfo = new DirectoryInfo(outputPath);

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
                    d => localizedDictionaries.Add(new i18nDirectoryFile()
                    {
                        FileInfo = d.FileInfo,
                        Dictionary = d.Dictionary.ToDictionary(item => item.Key, item => (object)(item.Value + "_" + languageCode))
                    }));

                SaveDictionaries(outputPath,localizedDictionaries, SaveType.Json, languageCode);
            }
        }

        public bool IsDirectory(string path)
        {
            bool result = true;
            System.IO.FileInfo fileTest = new System.IO.FileInfo(path);
            if (fileTest.Exists == true)
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

                var i18nDictionary = CreateDictionaryFromFile(filePath);
                output.Add(new i18nDirectoryFile() { FileInfo = info, Dictionary = i18nDictionary });
            }
            else
            {
                List<string> files = DirSearch(filePath);

                foreach (string file in files)
                {
                    var info = new FileInfo(file);

                    var i18nDictionary = CreateDictionaryFromFile(file);
                    output.Add(new i18nDirectoryFile() { FileInfo = info, Dictionary = i18nDictionary });
                }
            }

            return output;
        }

        public List<String> DirSearch(string sDir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(sDir);
            List<String> files = new List<String>();
            try
            {
                foreach (string f in Directory.GetFiles(dirInfo.FullName))
                {
                    files.Add(f);
                }
                foreach (string d in Directory.GetDirectories(dirInfo.FullName))
                {
                    files.AddRange(DirSearch(d));
                }
            }
            catch (System.Exception)
            {
                throw;
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
                    Dictionary<string, object> i18nDictionary = new Dictionary<string, object>();

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');

                        i18nDictionary.Add(values[0].Trim('"'), values[1].Trim('"'));
                    }

                    output.Add(new i18nDirectoryFile() { FileInfo = info, Dictionary = i18nDictionary });
                }
            }

            return output;
        }

        private void SaveDictionaries(string outputPath,List<i18nDirectoryFile> dictionaries, SaveType type, string languageCode = null)
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
            var parser = new I18nJsonParser(); //using System.Web.Script.Serialization;
            object jsonObject = parser.GenerateJsonObject(dictionary, "");

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
            I18nJsonParser i18nParser = new I18nJsonParser();
            var reader = new JsonReader();
            dynamic jsonObject = reader.Read(json);
            Dictionary<string, object> i18nDictionary = new Dictionary<string, object>();

            i18nParser.GenerateDictionary((ExpandoObject)jsonObject, i18nDictionary, "");

            return i18nDictionary;
        }

        private void SaveDictionary(Dictionary<string, object> i18NDictionary, string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);
            CreateDirectory(new DirectoryInfo(directory));

            File.WriteAllLines(filePath, i18NDictionary.Select(x => "\"" + x.Key + "\",\"" + x.Value + "\""));
        }

        public void CreateDirectory(DirectoryInfo directory)
        {
            if (!directory.Parent.Exists)
                CreateDirectory(directory.Parent);
            directory.Create();
        }

        private void SaveJson(string json, string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);
            CreateDirectory(new DirectoryInfo(directory));

            File.WriteAllText(filePath, json);
        }

        private Dictionary<string, object> AppendStringToLocaleValue(Dictionary<string, object> i18nDictionary, string postFix)
        {
            var localizedDictionary = i18nDictionary.ToDictionary(entry => entry.Key, entry => entry.Value);

            foreach (KeyValuePair<string, object> entry in i18nDictionary)
            {
                localizedDictionary[entry.Key] = entry.Value + "_" + postFix;
            }

            return localizedDictionary;
        }

        private string ConstructFilePath(string outDir, FileInfo fileInfo, SaveType type, string languageCode = null)
        {
            List<string> pathItems = new List<string>();

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
