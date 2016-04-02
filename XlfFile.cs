using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace xlflib
{
    public class XlfFile
    {
        private XElement node;
        private XNamespace ns;

        internal XlfFile(XElement node, XNamespace ns)
        {
            this.node = node;
            this.ns = ns;
            Optional = new Optionals(node);
            if (node.Elements("header").Any())
            {
                Header = new XlfHeader(node.Element("header"));
            }
        }

        public string Original
        {
            get
            {
                return this.node.Attribute("original").Value;
            }
            //private set;
        }

        // xml, html etc.
        public string DataType
        {
            get
            {
                return this.node.Attribute("datatype").Value;
            }
            //private set;
        }

        public string SourceLang { get { return this.node.Attribute("source-language").Value; } }

        public Optionals Optional { get; private set; }

        public XlfHeader Header { get; private set; }

        public IEnumerable<XlfTransUnit> TransUnits
        {
            get
            {
                return this.node.Descendants(this.ns + "trans-unit").Select(t => new XlfTransUnit(t, this.ns));
            }
            //private set;
        }

        public XlfTransUnit AddTransUnit(string id, string source, string target)
        {
            var n = new XElement(this.ns + "trans-unit");
            this.node.Descendants(this.ns + "trans-unit").Last().AddAfterSelf(n);

            return new XlfTransUnit(n, this.ns, id, source, target);
        }

        public void RemoveTransUnitById(string id)
        {
            RemoveTransUnit("id", id);
        }

        public void RemoveTransUnitByResname(string resname)
        {
            RemoveTransUnit("resname", resname);
        }

        private void RemoveTransUnit(string identifierName, string value)
        {
            this.node.Descendants(this.ns + "trans-unit").Where(u =>
            {
                var a = u.Attribute(identifierName);
                return a != null && a.Value == value;
            }).Remove();
        }

        public class Optionals
        {
            private XElement node;

            internal Optionals(XElement node)
            {
                this.node = node;
            }

            public string TargetLang
            {
                get { return GetAttributeIfExists("target-language"); }
            }

            public string ToolId
            {
                get { return GetAttributeIfExists("tool-id"); }
            }

            public string ProductName
            {
                get { return GetAttributeIfExists("product-name"); }
            }

            public string ProductVersion
            {
                get { return GetAttributeIfExists("product-version"); }
            }

            public string BuildNum
            {
                get { return GetAttributeIfExists("build-num"); }
            }

            public string GetAttributeIfExists(string name)
            {
                return XmlUtil.GetAttributeIfExists(node, name);
            }
        }
    }
}