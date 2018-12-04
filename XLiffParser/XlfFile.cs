namespace XliffParser
{
    using System;
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
        private const string AttributeId = "id";
        private const string AttributeResname = "resname";
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

        public enum AddMode
        {
            SkipExisting = 0,
            UpdateExisting = 1,
            FailIfExists = 2
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

        // Add a new or updates an existing translation unit
        public XlfTransUnit AddOrUpdateTransUnit(string id, string source, string target, XlfDialect dialect)
        {
            return AddTransUnit(id, source, target, AddMode.UpdateExisting, dialect);
        }

        public XlfTransUnit AddTransUnit(string id, string source, string target, AddMode addMode, XlfDialect dialect)
        {
            if (TryGetTransUnit(id, dialect, out XlfTransUnit resultUnit))
            {
                switch (addMode)
                {
                    case AddMode.FailIfExists:
                        throw new InvalidOperationException($"There is already a trans-unit with id={id}");

                    case AddMode.SkipExisting:
                        return resultUnit;

                    default:
                    case AddMode.UpdateExisting:
                        resultUnit.Source = source;

                        // only update target value if there is already a target element present
                        if (resultUnit.Target != null)
                        {
                            resultUnit.Target = target;
                        }

                        return resultUnit;
                }
            }

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
            else if (dialect == XlfDialect.MultilingualAppToolkit)
            {
                if (!id.StartsWith(XlfTransUnit.ResxPrefix, StringComparison.InvariantCultureIgnoreCase))
                {
                    return new XlfTransUnit(n, this.ns, XlfTransUnit.ResxPrefix + id, source, target);
                }
            }

            return new XlfTransUnit(n, this.ns, id, source, target);
        }

        public XlfTransUnit GetTransUnit(string id, XlfDialect dialect)
        {
            return TransUnits.First(u => u.GetId(dialect) == id);
        }

        public bool TryGetTransUnit(string id, XlfDialect dialect, out XlfTransUnit unit)
        {
            try
            {
                unit = GetTransUnit(id, dialect);
                return true;
            }
            catch (InvalidOperationException)
            {
                unit = null;
                return false;
            }
            catch (NullReferenceException)
            {
                unit = null;
                return false;
            }
        }

        public void RemoveTransUnit(string id, XlfDialect dialect)
        {
            switch (dialect)
            {
                case XlfDialect.RCWinTrans11:
                    RemoveTransUnit(AttributeResname, id);
                    break;

                case XlfDialect.MultilingualAppToolkit:
                    RemoveTransUnit(AttributeId, XlfTransUnit.ResxPrefix + id);
                    break;

                default:
                    RemoveTransUnit(AttributeId, id);
                    break;
            }
        }

        public void RemoveTransUnit(string identifierName, string identifierValue)
        {
            this.node.Descendants(this.ns + ElementTransUnit).Where(u =>
            {
                var a = u.Attribute(identifierName);
                return a != null && a.Value == identifierValue;
            }).Remove();
        }

        public void Export(string outputFilePath, IXlfExporter handler, List<string> stateFilter, List<string> restTypeFilter, XlfDialect dialect)
        {
            var units = stateFilter != null && stateFilter.Any() ?
                TransUnits.Where(u => stateFilter.Contains(u.Optional.TargetState)) :
                TransUnits;

            units = restTypeFilter != null && restTypeFilter.Any() ?
                units.Where(u => restTypeFilter.Contains(u.Optional.Restype)) :
                units;

            handler.ExportTranslationUnits(outputFilePath, units, Optional.TargetLang, dialect);
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