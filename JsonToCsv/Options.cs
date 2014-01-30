using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i18n.LocaleTool
{
    public class Options
    {
        [Option('i', "input", Required = true, HelpText = "Input file or folder to read.")]
        public string InputFileFolder { get; set; }

        [Option('o', "output", Required = true, HelpText = "Output file or folder to read.")]
        public string OutputFileFolder { get; set; }

        [Option('t', "targetLanguage", HelpText = "Target Language that file or folder will be translated to")]
        public string TargetLanguage { get; set; }

        [Option('v', null, HelpText = "Print details during execution.")]
        public bool Verbose { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            // this without using CommandLine.Text
            //  or using HelpText.AutoBuild
            var usage = new StringBuilder();
            usage.AppendLine("Quickstart Application 1.0");
            usage.AppendLine("Read user manual for usage instructions...");
            return usage.ToString();
        }
    }

}
