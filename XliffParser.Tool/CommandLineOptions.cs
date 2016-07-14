namespace XliffParser.Tool
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using FMDev.ArgsParser;

    public class CommandLineOptions
    {
        public static List<Command> Create()
        {
            return new List<Command>()
            {
                new InfoCommand(),
                new UpdateCommand(),
                new WriteTargetCommand()
            };
        }

        [Description("update XLF translation data from specified .resx input file")]
        public class UpdateCommand : Command
        {
            [CommandArg(HelpText = "dispay additional information")]
            public bool Verbose { get; set; }

            [CommandArg(HelpText = "specify resx source file", IsRequired = true)]
            public string Resx { get; set; }

            [CommandArg(HelpText = "specify xlf file to be updated", IsRequired = true)]
            public string Xlf { get; set; }

            ////[Option('s', "sorted", HelpText = "sort data alphabetically by name")]
            ////public bool IsSorted { get; set; }
        }

        [Description("write XLF translation data to specified .resx output file")]
        public class WriteTargetCommand : Command
        {
            [CommandArg(HelpText = "sort data alphabetically by name")]
            public bool Sorted { get; set; }

            [CommandArg(HelpText = "specify output resx file", IsRequired = true)]
            public string Resx { get; set; }

            [CommandArg(HelpText = "specify xlf input file", IsRequired = true)]
            public string Xlf { get; set; }

            [CommandArg(HelpText = "includes the first note as comment")]
            public bool IncludeComments { get; set; }
        }

        [Description("retrieve xlf file information")]
        public class InfoCommand : Command
        {
            [CommandArg(HelpText = "xlf input file", IsRequired = true)]
            public string Xlf { get; set; }
        }
    }
}