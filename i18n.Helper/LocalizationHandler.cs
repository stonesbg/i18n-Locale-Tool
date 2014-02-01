using i18n.Helper.Contracts;

namespace i18n.Helper
{
    public class LocalizationHandler : ILocalizationHandler
    {
        private readonly Ii18nHandler _handler;
        private readonly IFileHandler _fileHandler;
        public LocalizationHandler(Ii18nHandler handler, IFileHandler fileHandler)
        {
            _handler = handler;
            _fileHandler = fileHandler;
        }

        public void FlattenLocalizationFile(string inputPath, string outputPath)
        {
            if (_fileHandler.IsDirectory(inputPath))
            {
                foreach (string file in _fileHandler.DirSearch(inputPath))
                {
                    _handler.CreateDictionaryFile(file, outputPath, SaveType.Csv);
                }
            }
            else
            {
                _handler.CreateDictionaryFile(inputPath, outputPath, SaveType.Csv);
            }
        }

        public void ConvertFlattenedFile(string inputPath, string outputPath)
        {
            if (_fileHandler.IsDirectory(inputPath))
            {
                foreach (string file in _fileHandler.DirSearch(inputPath))
                {
                    _handler.CreateDictionaryFile(file, outputPath, SaveType.Json);
                }
            }
            else
            {
                _handler.CreateDictionaryFile(inputPath, outputPath, SaveType.Json);
            }
        }

        public void GenerateFakeLocalizationFile(string languageCode, string inputPath, string outputPath)
        {
            _handler.GenerateFakeJsonFileForLanguageCode(languageCode, inputPath, outputPath);
        }

        public void GenerateLongestStringsFile(string inputPath, string outputPath)
        {
            _handler.GenerateLongestStringsFile(inputPath, outputPath);
        }

        public void GenerateShortestStringsFile(string inputPath, string outputPath)
        {
            _handler.GenerateShortestStringsFile(inputPath, outputPath);
        }


        public void TranslateLocalizationFile(string languageCode, string inputPath, string outputPath)
        {
            if (_fileHandler.IsDirectory(inputPath))
            {
                foreach (string file in _fileHandler.DirSearch(inputPath))
                {
                    _handler.TranslateDictionaryFile(languageCode, file, outputPath);
                }
            }
            else
            {
                _handler.TranslateDictionaryFile(languageCode, inputPath, outputPath);
            }
        }
    }
}