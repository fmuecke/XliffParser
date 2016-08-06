namespace XliffParser.Tool
{
    using System;
    using System.IO;
    using System.Linq;
    using fmdev.ArgsParser;

    internal class XlfTool
    {
        private static int Main(string[] args)
        {
            try
            {
                var parser = new ArgsParser(CommandLineOptions.Create());
                if (parser.Parse(args))
                {
                    if (parser.Result is CommandLineOptions.InfoCommand)
                    {
                        return RunDefault(parser.Result as CommandLineOptions.InfoCommand);
                    }

                    if (parser.Result is CommandLineOptions.UpdateCommand)
                    {
                        return RunUpdate(parser.Result as CommandLineOptions.UpdateCommand);
                    }

                    if (parser.Result is CommandLineOptions.WriteTargetCommand)
                    {
                        return RunWrite(parser.Result as CommandLineOptions.WriteTargetCommand);
                    }
                    else
                    {
                        parser.PrintUsage();
                    }

                    return 0;
                }
            }
            catch (Exception e)
            {
                // ignore:
                // - file not found
                // - invalid resource file
                // - etc.
                Console.Error.WriteLine("ERROR: " + e.Message + "\n");
            }

            return 1;
        }

        private static int RunDefault(CommandLineOptions.InfoCommand cmd)
        {
            var xlfFile = cmd.Xlf;
            var doc = new XliffParser.XlfDocument(xlfFile);
            Console.WriteLine("file: " + doc.FileName);
            Console.WriteLine("XLIFF version: " + doc.Version);
            Console.WriteLine("file sections: " + doc.Files.Count());
            var n = 1;
            foreach (var f in doc.Files)
            {
                Console.WriteLine(">" + n + " data type: " + f.DataType);
                Console.WriteLine(">" + n + " original: " + f.Original);
                Console.WriteLine(">" + n + " source language: " + f.SourceLang);
                Console.WriteLine(">" + n + " target language: " + f.Optional.TargetLang);
                Console.WriteLine(">" + n + " tool id: " + f.Optional.ToolId);
                Console.WriteLine(">" + n + " translation units: " + f.TransUnits.Count());
            }

            return 0;
        }

        private static int RunUpdate(CommandLineOptions.UpdateCommand cmd)
        {
            var xlfFile = cmd.Xlf;
            var resxFile = cmd.Resx;

            if (!File.Exists(xlfFile))
            {
                Console.Error.WriteLine("error, file not found: " + xlfFile);
                return 2;
            }

            var doc = new XliffParser.XlfDocument(xlfFile);
            if (string.IsNullOrWhiteSpace(resxFile))
            {
                resxFile = Path.Combine(Path.GetDirectoryName(xlfFile), doc.Files.First().Original);
            }

            if (!File.Exists(resxFile))
            {
                Console.Error.WriteLine("error, file not found: " + resxFile);
                return 2;
            }

            var result = doc.UpdateFromSource("new", "new");
            if (cmd.Verbose)
            {
                var msg = string.Empty;
                if (!result.Any())
                {
                    msg = "already up-to-date";
                }
                else
                {
                    var updated = result.UpdatedItems.Count();
                    var added = result.AddedItems.Count();
                    var removed = result.RemovedItems.Count();

                    if (updated > 0)
                    {
                        msg += updated == 1 ? "1 item updated" : $"{updated} items updated";
                    }

                    if (added > 0)
                    {
                        if (msg.Length > 0)
                        {
                            msg += "/";
                        }

                        msg += added == 1 ? "1 item added" : $"{added} items added";
                    }

                    if (removed > 0)
                    {
                        if (msg.Length > 0)
                        {
                            msg += "/";
                        }

                        msg += removed == 1 ? "1 item removed" : $"{removed} items removed";
                    }
                }

                Console.WriteLine(msg);
            }

            doc.Save();
            return 0;
        }

        private static int RunWrite(CommandLineOptions.WriteTargetCommand cmd)
        {
            var doc = new XliffParser.XlfDocument(cmd.Xlf);
            doc.SaveAsResX(cmd.Resx, new XlfDocument.ResXSaveMode() { DoSort = cmd.Sorted, DoIncludeComments = cmd.IncludeComments });
            return 0;
        }
    }
}