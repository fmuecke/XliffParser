using System;
using System.IO;

namespace XliffParser.Test
{
    internal abstract class TestSample : IDisposable
    {
        protected static readonly string RootTestPath = Path.GetTempPath() + "XlifParserTempTestData";

        protected TestSample(string resxContent, string xlfContent)
        {
            SampleGuid = Guid.NewGuid();

            if (!Directory.Exists(RootTestPath))
            {
                Directory.CreateDirectory(RootTestPath);
            }

            ResxFileName = Path.Combine(Path.GetTempPath(), RootTestPath, "Resx_" + SampleGuid);
            File.WriteAllText(ResxFileName, resxContent);
            XlfFileName = Path.Combine(Path.GetTempPath(), RootTestPath, "Xlf_" + SampleGuid);
            File.WriteAllText(XlfFileName, xlfContent);
        }

        ~TestSample()
        {
            Dispose(false);
        }

        //public abstract string ResxContents { get; }
        public string ResxFileName { get; }

        //public abstract string XlfContents { get; }
        public string XlfFileName { get; }

        protected Guid SampleGuid { get; }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }

            try
            {
                if (File.Exists(ResxFileName))
                {
                    File.Delete(ResxFileName);
                }

                if (File.Exists(XlfFileName))
                {
                    File.Delete(XlfFileName);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Exception thrown in TestSample Dispose:\n" + e);
            }
        }
    }
}