﻿using i18n.Helper.Contracts;
using i18n.LocaleTool.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace i18n.Helper
{
    public class i18nHandler : Ii18nHandler
    {
        private readonly ITranslateHandler _translateHandler;
        private readonly IFileHandler _fileHandler;

        public i18nHandler(ITranslateHandler translateHandler, IFileHandler fileHandler)
        {
            _translateHandler = translateHandler;
            _fileHandler = fileHandler;
        }

        public void TranslateLocalizationFile(string languageCode, string inputPath, string outputPath)
        {
            if (_fileHandler.IsDirectory(inputPath))
            {
                foreach (string file in _fileHandler.DirSearch(inputPath))
                {
                    TranslateDictionaryFile(languageCode, file, outputPath);
                }
            }
            else
            {
                TranslateDictionaryFile(languageCode, inputPath, outputPath);
            }
        }

        public void FlattenLocalizationFile(string inputPath, string outputPath)
        {
            if (_fileHandler.IsDirectory(inputPath))
            {
                foreach (string file in _fileHandler.DirSearch(inputPath))
                {
                    CreateDictionaryFile(file, outputPath, SaveType.Csv);
                }
            }
            else
            {
                CreateDictionaryFile(inputPath, outputPath, SaveType.Csv);
            }
        }

        public void CreateDictionaryFile(string filePath, string outputPath, SaveType type, string languageCode = null)
        {
            List<I18NDirectoryFile> dictionaries = null;

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
                    dictionaries = _fileHandler.LoadJsonFileOrFolder(filePath);
                    break;
                case ".csv":
                    dictionaries = _fileHandler.LoadCsvFileOrFolder(filePath);
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

            List<I18NDirectoryFile> dictionaries = null;

            var fileInfo = new FileInfo(filePath);

            switch (fileInfo.Extension.ToLower())
            {
                case ".json":
                    dictionaries = _fileHandler.LoadJsonFileOrFolder(filePath);
                    break;
            }

            if (dictionaries != null)
            {
                foreach (I18NDirectoryFile dictonary in dictionaries)
                {
                    var keys = new List<string>(dictonary.Dictionary.Keys);
                    foreach(string key in keys)
                    {
                        string untranslatedFile = dictonary.Dictionary[key].ToString();
                        var translationResult = _translateHandler.Translate(untranslatedFile, sourceLanguage, languageCode);

                        dictonary.Dictionary[key] = translationResult;
                    }

                    string json = _fileHandler.CreateJsonFile(dictonary.Dictionary);
                    var dirInfo = new DirectoryInfo(outputPath);

                    string outPath = dirInfo.FullName + @"\" + languageCode + @"\" + fileInfo.Name;
                    _fileHandler.SaveJson(json, outPath);
                }
            }
        }

        public void GenerateFakeLocalizationFile(string languageCode, string filePath, string outputPath)
        {
            var dictionaries = _fileHandler.LoadJsonFileOrFolder(filePath);

            if (dictionaries.Count > 0)
            {
                var localizedDictionaries = new List<I18NDirectoryFile>();
                dictionaries.ForEach(
                    d => localizedDictionaries.Add(new I18NDirectoryFile
                    {
                        FileInfo = d.FileInfo,
                        Dictionary = d.Dictionary.ToDictionary(item => item.Key, item => (object)(item.Value + "_" + languageCode))
                    }));

                SaveDictionaries(outputPath,localizedDictionaries, SaveType.Json, languageCode);
            }
        }

        public void GenerateLongestStringsFile(string filePath, string outputPath)
        {
            var dictionaries = _fileHandler.LoadJsonFileOrFolder(filePath);

            if (dictionaries.Count > 0)
            {
                var getUniqueKeys = dictionaries.SelectMany(d => d.Dictionary.Keys).Distinct().ToList();

                var longestDictionary = new I18NDirectoryFile()
                {
                    Dictionary = new Dictionary<string, object>()
                };

                getUniqueKeys.ForEach(
                    k =>
                        longestDictionary.Dictionary.Add(k,
                            dictionaries.Select(d => d.Dictionary[k].ToString()).OrderByDescending(s => s.Length).First()));

                SaveDictionaries(outputPath, new List<I18NDirectoryFile>() {longestDictionary}, SaveType.Json, "dev_Longest");
            }
        }

        public void GenerateShortestStringsFile(string filePath, string outputPath)
        {
            var dictionaries = _fileHandler.LoadJsonFileOrFolder(filePath);

            if (dictionaries.Count > 0)
            {
                var getUniqueKeys = dictionaries.SelectMany(d => d.Dictionary.Keys).Distinct().ToList();

                var longestDictionary = new I18NDirectoryFile()
                {
                    Dictionary = new Dictionary<string, object>(),
                };

                getUniqueKeys.ForEach(
                    k =>
                        longestDictionary.Dictionary.Add(k,
                            dictionaries.Select(d => d.Dictionary[k].ToString()).OrderByDescending(s => s.Length).Last()));

                SaveDictionaries(outputPath, new List<I18NDirectoryFile>() { longestDictionary }, SaveType.Json, "dev_Shortest");
            }
        }

        public void ConvertFlattenedFile(string inputPath, string outputPath)
        {
            if (_fileHandler.IsDirectory(inputPath))
            {
                foreach (string file in _fileHandler.DirSearch(inputPath))
                {
                    CreateDictionaryFile(file, outputPath, SaveType.Json);
                }
            }
            else
            {
                CreateDictionaryFile(inputPath, outputPath, SaveType.Json);
            }
        }

        private void SaveDictionaries(string outputPath, IEnumerable<I18NDirectoryFile> dictionaries, SaveType type, string languageCode = null)
        {
            foreach (I18NDirectoryFile dictionaryItem in dictionaries)
            {
                string outputFullPath = _fileHandler.ConstructFilePath(outputPath, dictionaryItem.FileInfo, type, languageCode);

                switch (type)
                {
                    case SaveType.Json:
                        //Convert Dictionary to be json string
                        string json = _fileHandler.CreateJsonFile(dictionaryItem.Dictionary);
                        _fileHandler.SaveJson(json, outputFullPath);
                        break;
                    case SaveType.Csv:
                        _fileHandler.SaveDictionary(dictionaryItem.Dictionary, outputFullPath);
                        break;
                }
            }
        }
    }
}
