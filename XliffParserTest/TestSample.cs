using System;
using System.IO;

namespace XliffParser.Test
{
    internal abstract class TestSample: IDisposable
    {
        protected static readonly string RootTestPath = Path.GetTempPath() +  "XlifParserTempTestData";

        protected Guid SampleGuid { get; }

        public abstract string ResxContents { get; }

        public abstract string XlfContents { get; }

        public string ResxFileName { get; }

        public string XlfFileName { get; }

        protected TestSample()
        {
            SampleGuid = Guid.NewGuid();

            if (!Directory.Exists(RootTestPath))
            {
                Directory.CreateDirectory(RootTestPath);
            }

            ResxFileName = Path.Combine(Path.GetTempPath(), RootTestPath, "Resx_" + SampleGuid);
            File.WriteAllText(ResxFileName, ResxContents);

            XlfFileName = Path.Combine(Path.GetTempPath(), RootTestPath, "Xlf_" + SampleGuid);
            File.WriteAllText(XlfFileName, XlfContents);
        }

        ~TestSample() { Dispose(false); }

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