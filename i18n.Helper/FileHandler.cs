using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using i18n.Helper.Contracts;
using i18n.LocaleTool.Models;
using JsonFx.Json;

namespace i18n.Helper
{
    public interface IFileHandler
    {
        Dictionary<string, object> ReadJsonFile(string filePath);

        void SaveJson(string json, string filePath);

        void SaveDictionary(Dictionary<string, object> i18NDictionary, string filePath);

        bool IsDirectory(string path);

        List<String> DirSearch(string sDir);

        List<I18NDirectoryFile> LoadJsonFileOrFolder(string filePath);

        List<I18NDirectoryFile> LoadCsvFileOrFolder(string filePath);

        string ConstructFilePath(string outDir, FileInfo fileInfo, SaveType type, string languageCode);
    }

    public class FileHandler : IFileHandler
    {
        public Dictionary<string, object> ReadJsonFile(string filePath)
        {
            //Load in File
            string json = File.ReadAllText(filePath);

            return CreateDictionaryFromJson(json);
        }

        private Dictionary<string, object> CreateDictionaryFromJson(string json)
        {
            var i18NParser = new i18nJsonParser();
            var reader = new JsonReader();
            dynamic jsonObject = reader.Read(json);
            var i18NDictionary = new Dictionary<string, object>();

            i18NParser.GenerateDictionary((ExpandoObject)jsonObject, i18NDictionary, "");

            return i18NDictionary;
        }

        public void SaveJson(string json, string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);
            if (directory != null) CreateDirectory(new DirectoryInfo(directory));

            File.WriteAllText(filePath, json);
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

        public List<I18NDirectoryFile> LoadCsvFileOrFolder(string filePath)
        {
            var output = new List<I18NDirectoryFile>();
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

                    output.Add(new I18NDirectoryFile { FileInfo = info, Dictionary = i18NDictionary });
                }
            }

            return output;
        }

        public List<I18NDirectoryFile> LoadJsonFileOrFolder(string filePath)
        {
            var output = new List<I18NDirectoryFile>();
            bool isFile = false;
            if (File.Exists(filePath))
                isFile = true;

            if (isFile)
            {
                var info = new FileInfo(filePath);

                var i18NDictionary = ReadJsonFile(filePath);
                output.Add(new I18NDirectoryFile { FileInfo = info, Dictionary = i18NDictionary });
            }
            else
            {
                List<string> files = DirSearch(filePath);

                foreach (string file in files)
                {
                    var info = new FileInfo(file);

                    var i18nDictionary = ReadJsonFile(file);
                    output.Add(new I18NDirectoryFile { FileInfo = info, Dictionary = i18nDictionary });
                }
            }

            return output;
        }

        private void CreateDirectory(DirectoryInfo directory)
        {
            if (directory.Parent != null && !directory.Parent.Exists)
                CreateDirectory(directory.Parent);
            directory.Create();
        }

        public void SaveDictionary(Dictionary<string, object> i18NDictionary, string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);
            if (directory != null) CreateDirectory(new DirectoryInfo(directory));

            File.WriteAllLines(filePath, i18NDictionary.Select(x => "\"" + x.Key + "\",\"" + x.Value + "\""));
        }

        public string ConstructFilePath(string outDir, FileInfo fileInfo, SaveType type, string languageCode)
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
