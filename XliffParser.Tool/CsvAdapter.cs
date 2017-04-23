namespace XliffParser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CsvAdapter : IXlfExporter
    {
        public CsvAdapter()
        {
            this.IsLangColumnRequired = false;
            this.IsCsvHeaderRequired = true;
        }

        public string CustomIdColumn { get; set; }

        public bool IsLangColumnRequired { get; set; }

        public bool IsCsvHeaderRequired { get; internal set; }

        public void ExportTranslationUnits(string filePath, IEnumerable<XlfTransUnit> units, string language)
        {
            using (var textWriter = new System.IO.StreamWriter(filePath, false, Encoding.UTF8))
            {
                var csv = new CsvHelper.CsvWriter(textWriter);
                csv.Configuration.Encoding = Encoding.UTF8;
                csv.Configuration.HasHeaderRecord = false;

                if (IsCsvHeaderRequired)
                {
                    WriteHeader(csv);
                }

                foreach (var t in units)
                {
                    if (!string.IsNullOrEmpty(CustomIdColumn))
                    {
                        csv.WriteField(CustomIdColumn);
                    }

                    csv.WriteField(t.Id);
                    csv.WriteField(t.Source);
                    csv.WriteField(t.Target);
                    csv.WriteField(t.Optional.TargetState);

                    if (this.IsLangColumnRequired)
                    {
                        csv.WriteField(language);
                    }

                    csv.NextRecord();
                }
            }
        }

        private void WriteHeader(CsvHelper.CsvWriter csv)
        {
            if (!string.IsNullOrEmpty(CustomIdColumn))
            {
                csv.WriteField("Custom Id");
            }

            csv.WriteField("Id");
            csv.WriteField("Source");
            csv.WriteField("Target");
            csv.WriteField("State");

            if (IsLangColumnRequired)
            {
                csv.WriteField("Language");
            }

            csv.NextRecord();
        }
    }
}
