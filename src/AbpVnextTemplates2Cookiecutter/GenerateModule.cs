using McMaster.Extensions.CommandLineUtils;
using System.ComponentModel.DataAnnotations;
using System;
using System.IO;

namespace AbpVnextTemplates2Cookiecutter
{
    [Command(Name = "dotnetrsa gen", Description = "Generate xml, pkcs1, pkcs8 keys.")]
    [HelpOption("-h|--help")]
    public class GenerateModule
    {
        [Option("-s|--source <path>", Description = "Required.Source format.The value must be xml, pkcs1,pkcs8.")]
        [Required]
        public string SourcePath { get; }

        [Option("-t|--target <path>", Description =
            "Files output path.If you do not specify it will be output in the current directory.")]
        [Required]
        public string TargetPath { get; } = Environment.CurrentDirectory;


        private int OnExecute(CommandLineApplication app)
        {
            if (app.Options.Count == 1 && app.Options[0].ShortName == "h")
            {
                app.ShowHelp();
                return 0;
            }

            return 0;
        }
    }
}
