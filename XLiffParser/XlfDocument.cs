using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace xlflib
{
    public class XlfDocument
    {
        private XDocument doc;

        public XlfDocument(string fileName)
        {
            FileName = fileName;
            doc = XDocument.Load(FileName);
        }

        public enum SaveMode
        {
            Default, Sorted
        }

        public string FileName
        { get; }

        public string Version
        {
            get { return this.doc.Root.Attribute("version").Value; }
            set { this.doc.Root.SetAttributeValue("version", value); }
        }

        public IEnumerable<XlfFile> Files
        {
            get
            {
                var ns = this.doc.Root.Name.Namespace;
                return this.doc.Descendants(ns + "file").Select(f => new XlfFile(f, ns));
            }
        }

        public XlfFile AddFile(string original, string dataType, string sourceLang)
        {
            var ns = this.doc.Root.Name.Namespace;
            var f = new XElement(ns + "file");
            this.doc.Descendants(ns + "file").Last().AddAfterSelf(f);

            return new XlfFile(f, ns, original, dataType, sourceLang);
        }

        public void RemoveFile(string original)
        {
            var ns = this.doc.Root.Name.Namespace;
            this.doc.Descendants(ns + "file").Where(u =>
            {
                var a = u.Attribute("original");
                return a != null && a.Value == original;
            }).Remove();
        }

        public void Save()
        {
            this.doc.Save(this.FileName);
        }

        public void SaveAsResX(string fileName)
        {
            SaveAsResX(fileName, SaveMode.Default);
        }

        public void SaveAsResX(string fileName, SaveMode mode)
        {
            using (var resx = new ResXResourceWriter(fileName))
            {
                var nodes = new List<ResXDataNode>();
                foreach (var f in Files)
                {
                    foreach (var u in f.TransUnits)
                    {
                        var id = u.Id;
                        if (u.Optional.Resname.Length > 0)
                        {
                            id = u.Optional.Resname;
                        }
                        else if (id.Length > 5 && id.Substring(0, 5).ToUpperInvariant() == "RESX/")
                        {
                            id = id.Substring(5);
                        }

                        var node = new ResXDataNode(id, u.Target.Replace("\n", Environment.NewLine));
                        if (u.Optional.Notes.Count() > 0)
                        {
                            node.Comment = u.Optional.Notes.First().Value.Replace("\n", Environment.NewLine);
                        }
                        nodes.Add(node);
                    }
                }

                if (mode == SaveMode.Sorted)
                {
                    nodes.Sort((x, y) =>
                    {
                        if (x.Name == null && y.Name == null) return 0;
                        else if (x.Name == null) return -1;
                        else if (y.Name == null) return 1;
                        else return x.Name.CompareTo(y.Name);
                    });
                }

                foreach (var node in nodes)
                {
                    resx.AddResource(node);
                }
            }
        }

        /// <summary>
        /// Uses "new" as the state for updated and new strings
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>Return the number of updated/added/removed items</returns>
        public Tuple<int, int, int> UpdateFromResX(string fileName)
        {
            return UpdateFromResX(fileName, "new", "new");
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="updatedResourceStateString"></param>
        /// <param name="addedResourceStateString"></param>
        /// <returns>Return the number of updated/added/removed items</returns>
        public Tuple<int, int, int> UpdateFromResX(string fileName, string updatedResourceStateString, string addedResourceStateString)
        {
            var resxData = new Dictionary<string, Tuple<string, string>>(); // name, data, comment
            using (var resx = new ResXResourceReader(fileName))
            {
                resx.UseResXDataNodes = true;
                var dict = resx.GetEnumerator();
                while (dict.MoveNext())
                {
                    var item = dict.Value as ResXDataNode;
                    var value = item.GetValue((ITypeResolutionService)null) as string;
                    resxData.Add(
                        dict.Key as string,
                        Tuple.Create(value, item.Comment));
                }
            }

            int updatedItems = 0;
            int addedItems = 0;
            int removedItems = 0;
            foreach (var f in Files)
            {
                var removedUnits = new List<XlfTransUnit>();
                foreach (var u in f.TransUnits)
                {
                    var key = u.Optional.Resname.Length > 0 ? u.Optional.Resname : u.Id;
                    if (resxData.ContainsKey(key))
                    {
                        if (XmlUtil.NormalizeLineBreaks(u.Source) != XmlUtil.NormalizeLineBreaks(resxData[key].Item1))
                        {
                            // source text changed
                            u.Source = resxData[key].Item1;
                            u.Optional.TargetState = updatedResourceStateString;
                            u.Optional.SetCommentFromResx(resxData[key].Item2);

                            ++updatedItems;
                        }
                    }
                    else
                    {
                        removedUnits.Add(u);
                    }
                    resxData.Remove(key);
                }

                foreach (var u in removedUnits)
                {
                    if (string.IsNullOrWhiteSpace(u.Optional.Resname))
                    {
                        f.RemoveTransUnitById(u.Id);
                    }
                    else
                    {
                        f.RemoveTransUnitByResname(u.Optional.Resname);
                    }
                    ++removedItems;
                }

                foreach (var d in resxData)
                {
                    var unit = f.AddTransUnit(d.Key, d.Value.Item1, d.Value.Item1);
                    unit.Optional.TargetState = addedResourceStateString;
                    unit.Optional.SetCommentFromResx(d.Value.Item2);

                    ++addedItems;
                }
            }

            return Tuple.Create(updatedItems, addedItems, removedItems);
        }
    }
}