namespace XliffParser
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.IO;
    using System.Linq;
    using System.Resources;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using fmdev.ResX;

    public enum XlfDialect
    {
        Standard = 0,
        RCWinTrans11 = 1,
        MultilingualAppToolkit = 2
    }

    public class XlfDocument
    {
        private const string AttributeOriginal = "original";
        private const string ElementFile = "file";
        private const string AttributeVersion = "version";
        private const string ResxPrefix = "Resx/";
        private XDocument doc;

        public XlfDocument(string fileName)
        {
            FileName = fileName;
            doc = XDocument.Load(FileName);
            Dialect = DetermineDialect();
        }

        [Flags]
        public enum ResXSaveOption
        {
            None = 0,
            SortEntries = 1,
            IncludeComments = 2
        }

        public string FileName
        { get; }

        public IEnumerable<XlfFile> Files
        {
            get
            {
                var ns = this.doc.Root.Name.Namespace;
                return this.doc.Descendants(ns + ElementFile).Select(f => new XlfFile(f, ns));
            }
        }

        public string Version
        {
            get { return this.doc.Root.Attribute(AttributeVersion).Value; }
            set { this.doc.Root.SetAttributeValue(AttributeVersion, value); }
        }

        public XlfDialect Dialect
        {
            get; private set;
        }

        public XlfFile AddFile(string original, string dataType, string sourceLang)
        {
            var ns = this.doc.Root.Name.Namespace;
            var f = new XElement(ns + ElementFile);
            this.doc.Descendants(ns + ElementFile).Last().AddAfterSelf(f);

            return new XlfFile(f, ns, original, dataType, sourceLang);
        }

        public void RemoveFile(string original)
        {
            var ns = this.doc.Root.Name.Namespace;
            this.doc.Descendants(ns + ElementFile).Where(u =>
            {
                var a = u.Attribute(AttributeOriginal);
                return a != null && a.Value == original;
            }).Remove();
        }

        public void Save()
        {
            this.doc.Save(this.FileName);
        }

        public void SaveAsResX(string fileName)
        {
            SaveAsResX(fileName, ResXSaveOption.None);
        }

        public void SaveAsResX(string fileName, ResXSaveOption options)
        {
            var entries = new List<ResXEntry>();
            foreach (var f in Files)
            {
                foreach (var u in f.TransUnits)
                {
                    var entry = new ResXEntry() { Id = u.Id, Value = u.Target };

                    if (u.Optional.Resname.Length > 0)
                    {
                        entry.Id = u.Optional.Resname;
                    }
                    else
                    {
                        if (entry.Id.Length > 5 && string.Compare(entry.Id.Substring(0, 5), ResxPrefix, true) == 0)
                        {
                            entry.Id = entry.Id.Substring(5);
                        }
                    }

                    if (u.Optional.Notes.Count() > 0 && options.HasFlag(ResXSaveOption.IncludeComments))
                    {
                        entry.Comment = u.Optional.Notes.First().Value;
                    }

                    entries.Add(entry);
                }
            }

            if (options.HasFlag(ResXSaveOption.SortEntries))
            {
                entries.Sort();
            }

            ResXParser.Write(fileName, entries);
        }

        public UpdateResult UpdateFromSource()
        {
            switch (Version)
            {
                default:
                case "1.1":
                case "1.2":
                    return UpdateFromSource("new", "new");

                case "2.0":
                    return UpdateFromSource("initial", "initial");
            }
        }

        public UpdateResult UpdateFromSource(string updatedResourceStateString, string addedResourceStateString)
        {
            var sourceFile = Path.Combine(Path.GetDirectoryName(this.FileName), Files.Single().Original);
            return Update(sourceFile, updatedResourceStateString, addedResourceStateString);
        }

        /// <summary>
        /// Updates the xlf data from the provided resx source file.
        /// </summary>
        /// <param name="sourceFile">The source file to be processed.</param>
        /// <param name="updatedResourceStateString">The state string that should be used for updated items.</param>
        /// <param name="addedResourceStateString">The state string that should be used for added items.</param>
        /// <returns>Return the ids of the updated/added/removed items in separate lists.</returns>
        public UpdateResult Update(string sourceFile, string updatedResourceStateString, string addedResourceStateString)
        {
            var resxData = new Dictionary<string, ResXEntry>(); // id, value, comment

            foreach (var entry in ResXParser.Read(sourceFile))
            {
                var key = Dialect == XlfDialect.MultilingualAppToolkit ? ResxPrefix + entry.Id : entry.Id;
                resxData.Add(key, entry);
            }

            var updatedItems = new List<string>();
            var addedItems = new List<string>();
            var removedItems = new List<string>();
            foreach (var f in Files)
            {
                foreach (var u in f.TransUnits)
                {
                    var key = u.Optional.Resname.Length > 0 ? u.Optional.Resname : u.Id;
                    if (resxData.ContainsKey(key))
                    {
                        if (XmlUtil.NormalizeLineBreaks(u.Source) != XmlUtil.NormalizeLineBreaks(resxData[key].Value))
                        {
                            // source text changed
                            u.Source = resxData[key].Value;
                            u.Optional.TargetState = updatedResourceStateString;
                            u.Optional.SetCommentFromResx(resxData[key].Comment);

                            updatedItems.Add(key);
                        }
                    }
                    else
                    {
                        removedItems.Add(key);
                    }

                    resxData.Remove(key);
                }

                foreach (var id in removedItems)
                {
                    if (Version == "1.1")
                    {
                        // RC-Wintrans
                        f.RemoveTransUnitByResname(id);
                    }

                    // all others
                    f.RemoveTransUnitById(id);
                }

                foreach (var d in resxData)
                {
                    var unit = f.AddTransUnit(d.Key, d.Value.Value, d.Value.Value);
                    unit.Optional.TargetState = addedResourceStateString;
                    unit.Optional.SetCommentFromResx(d.Value.Comment);

                    addedItems.Add(d.Key);
                }
            }

            return new UpdateResult(addedItems, removedItems, updatedItems);
        }

        private XlfDialect DetermineDialect()
        {
            if (this.Files.First().Optional.ToolId == "MultilingualAppToolkit")
            {
                return XlfDialect.MultilingualAppToolkit;
            }

            if (doc.Root.GetNamespaceOfPrefix("rwt") == "http://www.schaudin.com/xmlns/rwt11")
            {
                return XlfDialect.RCWinTrans11;
            }

            return XlfDialect.Standard;
        }
    }
}