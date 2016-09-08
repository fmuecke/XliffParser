namespace XliffParser
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    public class XlfFile
    {
        private const string ElementHeader = "header";
        private const string AttributeDataType = "datatype";
        private const string AttributeOriginal = "original";
        private const string AttributeSourceLanguage = "source-language";
        private const string ElementTransUnit = "trans-unit";
        private const string ElementBody = "body";
        private const string IdNone = "none";
        private XElement node;
        private XNamespace ns;

        internal XlfFile(XElement node, XNamespace ns)
        {
            this.node = node;
            this.ns = ns;
            Optional = new Optionals(node);
            if (node.Elements(ns + ElementHeader).Any())
            {
                Header = new XlfHeader(node.Element(ns + ElementHeader));
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
            get { return this.node.Attribute(AttributeDataType).Value; }
            private set { this.node.SetAttributeValue(AttributeDataType, value); }
        }

        public XlfHeader Header { get; private set; }

        public Optionals Optional { get; private set; }

        public string Original
        {
            get { return this.node.Attribute(AttributeOriginal).Value; }
            private set { this.node.SetAttributeValue(AttributeOriginal, value); }
        }

        public string SourceLang
        {
            get { return this.node.Attribute(AttributeSourceLanguage).Value; }
            private set { this.node.SetAttributeValue(AttributeSourceLanguage, value); }
        }

        public IEnumerable<XlfTransUnit> TransUnits
        {
            get
            {
                return this.node.Descendants(this.ns + ElementTransUnit).Select(t => new XlfTransUnit(t, this.ns));
            }
        }

        public XlfTransUnit AddTransUnit(string id, string source, string target)
        {
            return AddTransUnit(id, source, target, XlfDialect.Standard);
        }

        public XlfTransUnit AddTransUnit(string id, string source, string target, XlfDialect dialect)
        {
            var n = new XElement(this.ns + ElementTransUnit);
            var transUnits = this.node.Descendants(this.ns + ElementTransUnit).ToList();

            if (transUnits.Any())
            {
                transUnits.Last().AddAfterSelf(n);
            }
            else
            {
                var bodyElements = this.node.Descendants(this.ns + ElementBody).ToList();

                XElement body;

                if (bodyElements.Any())
                {
                    body = bodyElements.First();
                }
                else
                {
                    body = new XElement(this.ns + ElementBody);
                    this.node.Add(body);
                }

                body.Add(n);
            }

            if (dialect == XlfDialect.RCWinTrans11)
            {
                var unit = new XlfTransUnit(n, this.ns, IdNone, source, target);
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
            this.node.Descendants(this.ns + ElementTransUnit).Where(u =>
            {
                var a = u.Attribute(identifierName);
                return a != null && a.Value == value;
            }).Remove();
        }

        public class Optionals
        {
            private const string AttributeBuildNum = "build-num";
            private const string AttributeProductName = "product-name";
            private const string AttributeProductVersion = "product-version";
            private const string AttributeTargetLanguage = "target-language";
            private const string AttributeToolId = "tool-id";
            private XElement node;

            internal Optionals(XElement node)
            {
                this.node = node;
            }

            public string BuildNum
            {
                get { return GetAttributeIfExists(AttributeBuildNum); }
                set { this.node.SetAttributeValue(AttributeBuildNum, value); }
            }

            public string ProductName
            {
                get { return GetAttributeIfExists(AttributeProductName); }
                set { this.node.SetAttributeValue(AttributeProductName, value); }
            }

            public string ProductVersion
            {
                get { return GetAttributeIfExists(AttributeProductVersion); }
                set { this.node.SetAttributeValue(AttributeProductVersion, value); }
            }

            public string TargetLang
            {
                get { return GetAttributeIfExists(AttributeTargetLanguage); }
                set { this.node.SetAttributeValue(AttributeTargetLanguage, value); }
            }

            public string ToolId
            {
                get { return GetAttributeIfExists(AttributeToolId); }
                set { this.node.SetAttributeValue(AttributeToolId, value); }
            }

            public string GetAttributeIfExists(string name)
            {
                return XmlUtil.GetAttributeIfExists(node, name);
            }
        }
    }
}