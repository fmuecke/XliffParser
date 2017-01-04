namespace XliffParser.Test
{
    using System;
    using XliffParser.Test;

    internal class ResxWithNeutralLangXlf : TestSample
    {
        private static readonly string ResxContent =
        @"<?xml version=""1.0"" encoding=""utf-8""?>
<root>
  <resheader name=""resmimetype"">
    <value>text/microsoft-resx</value>
  </resheader>
  <resheader name=""version"">
    <value>2.0</value>
  </resheader>
  <resheader name=""reader"">
    <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <resheader name=""writer"">
    <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <data name=""AddnlInfoMessagesBanner"" xml:space=""preserve"">
    <value>Additional Information Messages:</value>
  </data>
  <data name=""AppContainerTestPrerequisiteFail"" xml:space=""preserve"">
    <value>Could not start test run for unit tests for Windows Store app: {0}.</value>
  </data>
</root>";

        private static readonly string XlfContent =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<xliff xmlns=""urn:oasis:names:tc:xliff:document:1.2"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" version=""1.2"" xsi:schemaLocation=""urn:oasis:names:tc:xliff:document:1.2 xliff-core-1.2-transitional.xsd"">
  <file datatype=""xml"" source-language=""en"" original=""Resources.resx"">
    <body>
      <trans-unit id=""AppContainerTestPrerequisiteFail"">
        <source>Could not start test run for unit tests for Windows Store app: {0}.</source>
        <note></note>
      </trans-unit>
      <trans-unit id=""AddnlInfoMessagesBanner"">
        <source>Additional Information Messages:</source>
        <note></note>
      </trans-unit>
    </body>
  </file>
</xliff>";

        public ResxWithNeutralLangXlf()
            : base(ResxContent, XlfContent)
        {
        }
    }
}