namespace XliffParser
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

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

        internal XlfFile(XElement node, XNamespace ns, string original, string dataType, string sourceLang)
            : this(node, ns)
        {
            Original = original;
            DataType = dataType;
            SourceLang = sourceLang;
        }

        // xml, html etc.
        public string DataType
        {
            get { return this.node.Attribute("datatype").Value; }
            private set { this.node.SetAttributeValue("datatype", value); }
        }

        public XlfHeader Header { get; private set; }

        public Optionals Optional { get; private set; }

        public string Original
        {
            get { return this.node.Attribute("original").Value; }
            private set { this.node.SetAttributeValue("original", value); }
        }

        public string SourceLang
        {
            get { return this.node.Attribute("source-language").Value; }
            private set { this.node.SetAttributeValue("source-language", value); }
        }

        public IEnumerable<XlfTransUnit> TransUnits
        {
            get
            {
                return this.node.Descendants(this.ns + "trans-unit").Select(t => new XlfTransUnit(t, this.ns));
            }
        }

        public XlfTransUnit AddTransUnit(string id, string source, string target)
        {
            return AddTransUnit(id, source, target, XlfDialect.Standard);
        }

        public XlfTransUnit AddTransUnit(string id, string source, string target, XlfDialect dialect)
        {
            var n = new XElement(this.ns + "trans-unit");
            var transUnits = this.node.Descendants(this.ns + "trans-unit").ToList();

            if (transUnits.Any())
            {
                transUnits.Last().AddAfterSelf(n);
            }
            else
            {
                var bodyElements = this.node.Descendants(this.ns + "body").ToList();

                XElement body;

                if (bodyElements.Any())
                {
                    body = bodyElements.First();
                }
                else
                {
                    body = new XElement(this.ns + "body");
                    this.node.Add(body);
                }

                body.Add(n);
            }

            if (dialect == XlfDialect.RCWinTrans11)
            {
                var unit = new XlfTransUnit(n, this.ns, "none", source, target);
                unit.Optional.Resname = id;
                return unit;
            }

            return new XlfTransUnit(n, this.ns, id, source, target);
        }

        public XlfTransUnit GetTransUnit(string id)
        {
            return GetTransUnit(id, XlfDialect.Standard);
        }

        public XlfTransUnit GetTransUnit(string id, XlfDialect dialect)
        {
            if (dialect == XlfDialect.RCWinTrans11)
            {
                return TransUnits.First(u => u.Optional.Resname == id);
            }

            return TransUnits.First(u => u.Id == id);
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

            public string BuildNum
            {
                get { return GetAttributeIfExists("build-num"); }
                set { this.node.SetAttributeValue("build-num", value); }
            }

            public string ProductName
            {
                get { return GetAttributeIfExists("product-name"); }
                set { this.node.SetAttributeValue("product-name", value); }
            }

            public string ProductVersion
            {
                get { return GetAttributeIfExists("product-version"); }
                set { this.node.SetAttributeValue("product-version", value); }
            }

            public string TargetLang
            {
                get { return GetAttributeIfExists("target-language"); }
                set { this.node.SetAttributeValue("target-language", value); }
            }

            public string ToolId
            {
                get { return GetAttributeIfExists("tool-id"); }
                set { this.node.SetAttributeValue("tool-id", value); }
            }

            public string GetAttributeIfExists(string name)
            {
                return XmlUtil.GetAttributeIfExists(node, name);
            }
        }
    }
}