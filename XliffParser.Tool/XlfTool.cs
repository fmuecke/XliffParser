namespace XliffParser.Tool
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using fmdev.ArgsParser;

    internal class XlfTool
    {
        private static int Main(string[] args)
        {
            try
            {
                var parser = new ArgsParser(typeof(CommandArgs));
                if (parser.Parse(args))
                {
                    if (parser.Result is CommandArgs.InfoCommand)
                    {
                        return RunDefault(parser.Result as CommandArgs.InfoCommand);
                    }

                    if (parser.Result is CommandArgs.UpdateCommand)
                    {
                        return RunUpdate(parser.Result as CommandArgs.UpdateCommand);
                    }

                    if (parser.Result is CommandArgs.ExportCsvCommand)
                    {
                        return RunExportCsv(parser.Result as CommandArgs.ExportCsvCommand);
                    }

                    if (parser.Result is CommandArgs.WriteTargetCommand)
                    {
                        return RunWrite(parser.Result as CommandArgs.WriteTargetCommand);
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

        private static int RunExportCsv(CommandArgs.ExportCsvCommand cmd)
        {
            var doc = new XliffParser.XlfDocument(cmd.Xlf);
            var csv = new CsvAdapter()
            {
                CustomIdColumn = cmd.CustomIdColumn,
                IsCsvHeaderRequired = !cmd.NoHeader,
                IsLangColumnRequired = cmd.WithLanguage
            };

            var stateFilter = string.IsNullOrWhiteSpace(cmd.Filter) ? null : cmd.Filter.Split(';').ToList();
            doc.Files.First().Export(cmd.Out, csv, stateFilter);

            return 0;
        }

        private static int RunDefault(CommandArgs.InfoCommand cmd)
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

        private static int RunUpdate(CommandArgs.UpdateCommand cmd)
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

        private static int RunWrite(CommandArgs.WriteTargetCommand cmd)
        {
            var doc = new XliffParser.XlfDocument(cmd.Xlf);
            var saveOptions = XlfDocument.ResXSaveOption.None;

            if (cmd.Sorted)
            {
                saveOptions |= XlfDocument.ResXSaveOption.SortEntries;
            }

            if (cmd.IncludeComments)
            {
                saveOptions |= XlfDocument.ResXSaveOption.IncludeComments;
            }

            doc.SaveAsResX(cmd.Resx, saveOptions);

            return 0;
        }
    }
}