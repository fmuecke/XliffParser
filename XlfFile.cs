﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace xlflib
{
    public class XlfFile
    {
        private XElement node;

        public class Optionals
        {
            private XElement node;

            internal Optionals(XElement node)
            {
                this.node = node;
            }

            public string TargetLang
            {
                get { return XmlUtil.GetAttributeIfExists(node, "target-language"); }
            }
            public string ToolId
            {
                get { return XmlUtil.GetAttributeIfExists(node, "tool-id"); }
            }

            public string ProductName
            {
                get { return XmlUtil.GetAttributeIfExists(node, "product-name"); }
            }
            public string ProductVersion
            {
                get { return XmlUtil.GetAttributeIfExists(node, "product-version"); }
            }
            public string BuildNum
            {
                get { return XmlUtil.GetAttributeIfExists(node, "build-num"); }
            }
        }

        internal XlfFile(XElement node)
        {
            this.node = node;
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
        public List<XlfTransUnit> TransUnits
        {
            get
            {
                var ns = this.node.Document.Root.Name.Namespace;
                return new List<XlfTransUnit>(this.node.Descendants(ns + "trans-unit").Select(t => new XlfTransUnit(t)));
            }
            //private set;
        }

        public XlfTransUnit AddTransUnit(string id, string source, string target)
        {
            var ns = this.node.Document.Root.Name.Namespace;
            var n = new XElement(ns + "trans-unit");
            this.node.Descendants(ns + "trans-unit").Last().AddAfterSelf(n);

            return new XlfTransUnit(n, id, source, target);
        }
    }
}