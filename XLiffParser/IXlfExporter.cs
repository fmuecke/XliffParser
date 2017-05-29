namespace XliffParser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IXlfExporter
    {
        void ExportTranslationUnits(string filePath, IEnumerable<XlfTransUnit> units, string targetLanguage, XlfDialect dialect);
    }
}
