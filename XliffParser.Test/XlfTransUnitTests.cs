namespace XliffParser.Test
{
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using XliffParser;

    [TestClass]
    public class XlfTransUnitTests
    {
        private static string xlf11doc =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
            "<xliff version=\"1.1\" xmlns:rwt=\"http://www.schaudin.com/xmlns/rwt11\">" +
            "  <file original=\"Strings.resx\" source-language=\"en\" datatype=\"resx\" tool=\"RC-WinTrans\" target-language=\"en-US\">" +
            "    <header>" +
            "      <rwt:crc>0</rwt:crc>" +
            "    </header>" +
            "    <body>" +
            "      <group id=\"Strings.resx\" restype=\"x-stringtable\">" +
            "        <trans-unit resname=\"RememberLogonInformation\" id=\"none\" restype=\"x-text\">" +
            "          <source>_Save logon information</source>" +
            "          <target state=\"translated\">_Save logon information</target>" +
            "        </trans-unit>" +
            "        <trans-unit resname=\"BMC_Title\" id=\"none\" restype=\"x-text\">" +
            "          <source>muitool Management Center</source>" +
            "          <target state=\"final\">muitool Management Center</target>" +
            "        </trans-unit>" +
            "        <trans-unit resname=\"Button_AssignJobs\" id=\"none\" restype=\"x-text\">" +
            "          <source>Assign Jobs</source>" +
            "          <target state=\"translated\">Assign jobs</target>" +
            "        </trans-unit>" +
            "        <trans-unit resname=\"Button_RevokeLicense\" id=\"none\" restype=\"x-text\">" +
            "          <source>Revoke license</source>" +
            "          <target state=\"translated\">Revoke license</target>" +
            "        </trans-unit>" +
            "        <trans-unit resname=\"Button_Cancel\" id=\"none\" restype=\"x-text\">" +
            "          <source>Cancel</source>" +
            "          <target state=\"final\">Cancel</target>" +
            "        </trans-unit>" +
            "        <trans-unit resname=\"Button_CloseApplication\" id=\"none\" restype=\"x-text\">" +
            "          <source>Close Application</source>" +
            "          <target state=\"translated\">Close application</target>" +
            "        </trans-unit>" +
            "        <trans-unit resname=\"Button_Connect\" id=\"none\" restype=\"x-text\">" +
            "          <source>_Connect</source>" +
            "          <target state=\"final\">_Connect</target>" +
            "        </trans-unit>" +
            "        <trans-unit resname=\"Button_Continue\" id=\"none\" restype=\"x-text\">" +
            "          <source>Continue</source>" +
            "          <target state=\"final\">Continue</target>" +
            "        </trans-unit>" +
            "        <trans-unit resname=\"Button_ContinueTest\" id=\"none\" restype=\"x-text\">" +
            "          <source>Continue<span>Test</span></source>" +
            "          <target state=\"final\">Continue</target>" +
            "        </trans-unit>" +
            "	  </group>" +
            "    </body>" +
            "  </file>" +
            "</xliff>";

        private static string xlf12doc =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
            "<xliff version=\"1.2\" xmlns=\"urn:oasis:names:tc:xliff:document:1.2\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"urn:oasis:names:tc:xliff:document:1.2 xliff-core-1.2-transitional.xsd\">" +
            "  <file datatype=\"xml\" source-language=\"en\" target-language=\"de\" original=\"Strings.resx\">" +
            "    <body>" +
            "      <group id=\"Resx\">" +
            "        <trans-unit id=\"Resx/State_new\" translate=\"yes\" xml:space=\"preserve\">" +
            "          <source>Indicates that the item is new. For example, translation units that were not in a previous version of the document.</source>" +
            "          <target state=\"new\"></target>" +
            "        </trans-unit>" +
            "        <trans-unit id=\"Resx/State_needs-review-translation\" translate=\"yes\" xml:space=\"preserve\">" +
            "          <source>Indicates that only the text of the item needs to be reviewed.</source>" +
            "          <target state=\"needs-review-translation\"></target>" +
            "        </trans-unit>" +
            "        <trans-unit id=\"Resx/State_final\" translate=\"yes\" xml:space=\"preserve\">" +
            "          <source>Indicates the terminating state.</source>" +
            "          <target state=\"final\"></target>" +
            "        </trans-unit>" +
            "        <trans-unit id=\"Resx/State_translated\" translate=\"yes\" xml:space=\"preserve\">" +
            "          <source>Indicates that the item has been translated.</source>" +
            "          <target state=\"translated\"></target>" +
            "        </trans-unit>		" +
            "        <trans-unit id=\"Resx/State_signed-off\" translate=\"yes\" xml:space=\"preserve\">" +
            "          <source>Indicates that changes are reviewed and approved.</source>" +
            "          <target state=\"signed-off\"></target>" +
            "        </trans-unit>		" +
            "        <trans-unit id=\"Resx/State_translated_withNotes\" translate=\"yes\" xml:space=\"preserve\">" +
            "          <source>Indicates that the item has been translated.</source>" +
            "          <target state=\"translated\"></target>" +
            "          <note from=\"MultilingualEditor\" annotates=\"source\" priority=\"4\">Kommentar</note>" +
            "          <note from=\"MultilingualEditor\" annotates=\"source\" priority=\"4\">k2</note>" +
            "          <note from=\"MultilingualBuild\" annotates=\"source\" priority=\"2\">Icon column</note>" +
            "        </trans-unit>" +
            "        <trans-unit id=\"Resx/NotTranslatable\" translate=\"no\" xml:space=\"preserve\">" +
            "          <source>XLIFF</source>" +
            "          <target state=\"signed-off\">XLIFF</target>" +
            "        </trans-unit>" +
            "        <trans-unit id=\"Resx/State_translated_withXML\" translate=\"no\" xml:space=\"preserve\">" +
            "          <source>XLIFF<span>Test</span></source>" +
            "          <target state=\"signed-off\">XLIFF<span>Test</span></target>" +
            "        </trans-unit>" +
            "      </group>" +
            "    </body>" +
            "  </file>" +
            "</xliff>";

        [TestMethod]
        public void SourceWithXMLShouldReturnSubNodes()
        {
            var doc = XDocument.Parse(xlf12doc);
            var ns = doc.Root.Name.Namespace;
            var node = doc.Descendants(ns + "trans-unit")
                .Where(t =>
                {
                    return t.Attribute(XName.Get("id")).Value == "Resx/State_translated_withXML";
                })
                .Single();

            var unit = new XlfTransUnit(node, ns);

            Assert.AreEqual("XLIFF<span>Test</span>", unit.Source);

            doc = XDocument.Parse(xlf11doc);
            ns = doc.Root.Name.Namespace;
            node = doc.Descendants(ns + "trans-unit")
                .Where(t =>
                {
                    return t.Attribute(XName.Get("resname")).Value == "Button_ContinueTest";
                })
                .Single();

            unit = new XlfTransUnit(node, ns);

            Assert.AreEqual("Continue<span>Test</span>", unit.Source);
        }

        [TestMethod]
        public void AddNoteTest()
        {
            var doc = XDocument.Parse(xlf12doc);
            var ns = doc.Root.Name.Namespace;
            var unit = new XlfTransUnit(doc.Descendants(ns + "trans-unit").First(), ns);
            unit.Optional.AddNote("Valar morghulis!", "XliffParserTest");
            var isWithNote = doc.ToString().Replace(" ", string.Empty).Contains("</target><notefrom=\"XliffParserTest\">Valarmorghulis!</note></trans-unit>\r\n<trans-unit");
            Assert.IsTrue(isWithNote);

            doc = XDocument.Parse(xlf11doc);
            ns = doc.Root.Name.Namespace;
            unit = new XlfTransUnit(doc.Descendants(ns + "trans-unit").First(), ns);
            unit.Optional.AddNote("Valar morghulis!", "XliffParserTest");
            isWithNote = doc.ToString().Replace(" ", string.Empty).Contains("</target>\r\n<notefrom=\"XliffParserTest\">Valarmorghulis!</note>\r\n</trans-unit>\r\n<trans-unit");
            Assert.IsTrue(isWithNote);
        }

        [TestMethod]
        public void RemoveNoteTest()
        {
            var doc = XDocument.Parse(xlf12doc);
            var ns = doc.Root.Name.Namespace;
            var unit = new XlfTransUnit(doc.Descendants(ns + "trans-unit").Where(d => d.Attribute("id").Value == "Resx/State_translated_withNotes").First(), ns);
            var noteCountBefore = unit.Optional.Notes.Count(n => n.Optional.From == "MultilingualEditor");
            Assert.AreNotEqual(0, noteCountBefore, "There must be at least one note of type 'MultilingualEditor' for this test to work");
            unit.Optional.RemoveNotes("from", "MultilingualEditor");
            var noteCountAfter = unit.Optional.Notes.Count(n => n.Optional.From == "MultilingualEditor");
            Assert.AreEqual(0, noteCountAfter, "There must not be any notes from 'MultilingualEditor' after removal");
        }
    }
}