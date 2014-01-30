using JsonFx.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Services.Client;
using Microsoft;

namespace JsonToCsv
{
    public class Program
    {
        static void Main(string[] args)
        {
            i18nHandler handler = new i18nHandler();

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

            string filePath;
            string outputPath;
            string languageCode;

            do 
            {
            cki = Console.ReadKey(true);
            switch (cki.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    Console.WriteLine("Option 1 Selected");

                    filePath = WaitForUserInput("Path to Folder of Json Files or File:");
                    outputPath = WaitForUserInput("Folder where file(s) to be saved or leave empty:");

                    if (handler.IsDirectory(filePath))
                    {
                        foreach (string file in handler.DirSearch(filePath))
                        {
                            handler.CreateDictionaryFile(file, outputPath, SaveType.Csv);
                        }
                    }
                    else
                    {
                        handler.CreateDictionaryFile(filePath, outputPath, SaveType.Csv);
                    }
                    
                    Console.WriteLine("File Created");
                    break;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    Console.WriteLine("Option 2 Selected");
                    filePath = WaitForUserInput("Path to Folder of Json Files or File:");
                    outputPath = WaitForUserInput("Folder where file(s) to be saved:");

                    if (handler.IsDirectory(filePath))
                    {
                        foreach (string file in handler.DirSearch(filePath))
                        {
                            handler.CreateDictionaryFile(file, outputPath, SaveType.Json);
                        }
                    }
                    else
                    {
                        handler.CreateDictionaryFile(filePath, outputPath, SaveType.Json);
                    }
                    
                    Console.WriteLine("File Created");
                    break;
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    languageCode = WaitForUserInput("Enter Language code (i.e. de-DE):");
                    Console.WriteLine("Generating File with the code: " + languageCode);
                    filePath = CheckFilePathExists("Path to Folder of Json Files or File:");
                    outputPath = CheckFilePathExists("Folder where file(s) to be saved:", false);

                    handler.GenerateFakeJsonFileForLanguageCode(languageCode, filePath, outputPath);

                    Console.WriteLine("Complete");
                    break;
                case ConsoleKey.D4:
                case ConsoleKey.NumPad4:
                    Console.Write("4");
                    Console.WriteLine();

                    break;
                case ConsoleKey.D5:
                case ConsoleKey.NumPad5:
                    Console.Write("5");
                    Console.WriteLine();
                    break;
                case ConsoleKey.D6:
                case ConsoleKey.NumPad6:
                    languageCode = WaitForUserInput("Enter Language code (i.e. de-DE):");
                    Console.WriteLine("Generating File with the code: " + languageCode);

                    filePath = WaitForUserInput("Path to Folder of Json Files or File:");
                    outputPath = WaitForUserInput("Folder where file(s) to be saved:");

                    if (handler.IsDirectory(filePath))
                    {
                        foreach (string file in handler.DirSearch(filePath))
                        {
                            handler.TranslateDictionaryFile(languageCode, file, outputPath);
                        }
                    }
                    else
                    {
                        handler.TranslateDictionaryFile(languageCode, filePath, outputPath);
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
