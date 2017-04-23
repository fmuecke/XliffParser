namespace XliffParser.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class TestXlfExporter : IXlfExporter
    {
        public TestXlfExporter()
        {
        }

        public string File { get; set; }

        public string Lang { get; set; }

        public List<XlfTransUnit> Units { get; set; }

        public void ExportTranslationUnits(string filePath, IEnumerable<XlfTransUnit> units, string targetLanguage)
        {
            File = filePath;
            Lang = targetLanguage;
            Units = units.ToList();
        }
    }
}