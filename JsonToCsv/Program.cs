using System;
using System.IO;
using CommandLine;
using i18n.Helper;

namespace i18n.LocaleTool
{
    public class Program
    {
        static void Main(string[] args)
        {
            var handler = new I18NHandler();

            var options = new Options();
            var parser = new Parser();
            if (parser.ParseArguments(args, options))
            {
                // consume Options type properties
                if (options.Verbose)
                {
                    Console.WriteLine("Input File/Folder: " + options.InputFileFolder);
                    Console.WriteLine("Output File/Folder: " + options.OutputFileFolder);
                    Console.WriteLine("Target Language: " + options.TargetLanguage);
                }
                else
                    Console.WriteLine("working ...");
            }

            ConsoleKeyInfo cki;
            // Prevent example from ending if CTL+C is pressed.
            //Console.TreatControlCAsInput = true;

            Console.WriteLine("Please enter the number for the action you would like to perform:");
            Console.WriteLine("1: Create CSV of Localized Values");
            Console.WriteLine("2: Convert CSV back to JSON File");
            Console.WriteLine("3: Create Fake JSON File for Particular Localization");
            Console.WriteLine("4: Create Longest Strings Json File");
            Console.WriteLine("5: Create Shortest Strings Json File");
            Console.WriteLine("6: Translate JSON Files to Language");
            Console.WriteLine();
            Console.Out.Flush();
            Console.WriteLine("Press ESC to exit application");
            do 
            {
                cki = Console.ReadKey(true);

                if (String.IsNullOrEmpty(options.InputFileFolder))
                {
                    options.InputFileFolder = CheckFilePathExists("Path to Folder of Json Files or File:");
                }

                if (String.IsNullOrEmpty(options.OutputFileFolder))
                {
                    options.OutputFileFolder = CheckFilePathExists("Folder where file(s) to be saved:", false);
                }

                switch (cki.Key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        Console.WriteLine("Option 1 Selected");

                        if (handler.IsDirectory(options.InputFileFolder))
                        {
                            foreach (string file in handler.DirSearch(options.InputFileFolder))
                            {
                                handler.CreateDictionaryFile(file, options.OutputFileFolder, SaveType.Csv);
                            }
                        }
                        else
                        {
                            handler.CreateDictionaryFile(options.InputFileFolder, options.OutputFileFolder, SaveType.Csv);
                        }

                        Console.WriteLine("Complete");
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        Console.WriteLine("Option 2 Selected");

                        if (handler.IsDirectory(options.InputFileFolder))
                        {
                            foreach (string file in handler.DirSearch(options.InputFileFolder))
                            {
                                handler.CreateDictionaryFile(file, options.OutputFileFolder, SaveType.Json);
                            }
                        }
                        else
                        {
                            handler.CreateDictionaryFile(options.InputFileFolder, options.OutputFileFolder, SaveType.Json);
                        }

                        Console.WriteLine("Complete");
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        Console.WriteLine("Option 3 Selected");

                        if (String.IsNullOrEmpty(options.TargetLanguage))
                        {
                            options.TargetLanguage = WaitForUserInput("Enter Language code (i.e. de-DE):");
                            Console.WriteLine("Generating File with the code: " + options.TargetLanguage);
                        }

                        handler.GenerateFakeJsonFileForLanguageCode(options.TargetLanguage, options.InputFileFolder, options.OutputFileFolder);

                        Console.WriteLine("Complete");
                        break;
                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        Console.WriteLine("Option 4 Selected");

                        handler.GenerateLongestStringsFile(options.InputFileFolder, options.OutputFileFolder);

                        Console.WriteLine("Complete");
                        break;
                    case ConsoleKey.D5:
                    case ConsoleKey.NumPad5:
                        Console.WriteLine("Option 5 Selected");

                        handler.GenerateShortestStringsFile(options.InputFileFolder, options.OutputFileFolder);

                        Console.WriteLine("Complete");
                        break;
                    case ConsoleKey.D6:
                    case ConsoleKey.NumPad6:
                        Console.WriteLine("Option 6 Selected");

                        if (String.IsNullOrEmpty(options.TargetLanguage))
                        {
                            options.TargetLanguage = WaitForUserInput("Enter Language code (i.e. de-DE):");
                            Console.WriteLine("Generating File with the code: " + options.TargetLanguage);
                        }

                        if (handler.IsDirectory(options.InputFileFolder))
                        {
                            foreach (string file in handler.DirSearch(options.InputFileFolder))
                            {
                                handler.TranslateDictionaryFile(options.TargetLanguage, file, options.OutputFileFolder);
                            }
                        }
                        else
                        {
                            handler.TranslateDictionaryFile(options.TargetLanguage, options.InputFileFolder, options.OutputFileFolder);
                        }

                        Console.WriteLine("Complete");
                        break;
                }
            } while (cki.Key != ConsoleKey.Escape);
        }

        private static string CheckFilePathExists(string message, bool checkIfExists = true)
        {
            string filePath;
            bool foundPath = false;
            if (checkIfExists)
                foundPath = true;
            do
            {
                filePath = WaitForUserInput(message);
                if (File.Exists(filePath) || Directory.Exists(filePath))
                {
                    foundPath = true;
                }
                else
                {
                    Console.WriteLine("File/Path Doesn't exist");
                }
            } while (!foundPath);

            return filePath;
        }

        private static string WaitForUserInput(string message)
        {
            string userInput = null;
            Console.WriteLine(message);
            do
            {
                userInput = Console.ReadLine();
            } while (string.IsNullOrEmpty(userInput));


            //Console.WriteLine("Input [" + userInput + "]");
            return userInput;
        }
    }
}
