using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace xlflib
{
    public class XlfTransUnit
    {
        private XElement node;
        private XNamespace ns;

        public XlfTransUnit(XElement node, XNamespace ns)
        {
            this.node = node;
            this.ns = ns;

            Optional = new Optionals(this.node, this.ns);
        }

        public XlfTransUnit(XElement node, XNamespace ns, string id, string source, string target)
            : this(node, ns)
        {
            Id = id;
            Source = source;
            Target = target;
        }

        public string Id
        {
            get { return this.node.Attribute("id").Value; }
            private set { this.node.SetAttributeValue("id", value); }
        }

        public string Source
        {
            get { return this.node.Element(this.ns + "source").Value; }
            set { this.node.SetElementValue(this.ns + "source", value); }
        }

        public string Target
        {
            get { return this.node.Element(this.ns + "target").Value; }
            set { this.node.SetElementValue(this.ns + "target", value); }
        }

        public Optionals Optional { get; }

        public class Optionals
        {
            private XElement node;
            private XNamespace ns;

            internal Optionals(XElement node, XNamespace ns)
            {
                this.node = node;
                this.ns = ns;
            }

            /// <summary>
            /// Indicates whether a translation is final or has passed its final review.
            /// </summary>
            public string Approved
            {
                get { return XmlUtil.GetAttributeIfExists(this.node, "approved"); }
                set { this.node.SetAttributeValue("approved", value); }
            }

            /// <summary>
            /// Indicates whether or not the text referred to should be translated.
            /// </summary>
            public string Translate
            {
                get { return XmlUtil.GetAttributeIfExists(this.node, "translate"); }
                set { this.node.SetAttributeValue("translate", value); }
            }

            /// <summary>
            /// The status of a particular translation in a <target> or <bin-target> element.
            /// <see cref="http://docs.oasis-open.org/xliff/v1.2/os/xliff-core.html#state"/>
            /// </summary>
            public string TargetState
            {
                get
                {
                    return XmlUtil.GetAttributeIfExists(this.node.Element(this.ns + "target"), "state");
                }
                set
                {
                    this.node.Element(this.ns + "target").SetAttributeValue("state", value);
                }
            } // TODO later: use XlfState

            /// <summary>
            /// The datatype attribute specifies the kind of text contained in the element. Depending on that type, you may
            /// apply different processes to the data. For example: datatype="winres" specifies that the content is Windows
            /// resources which would allow using the Win32 API in rendering the content.
            /// </summary>
            public string DataType
            {
                get { return XmlUtil.GetAttributeIfExists(this.node, "datatype"); }
                set { this.node.SetAttributeValue("datatype", value); }
            } // TODO later: use XlfDataType

            /// <summary>
            ///  Indicates the resource type of the container element.
            /// </summary>
            public string Restype
            {
                get { return XmlUtil.GetAttributeIfExists(this.node, "restype"); }
                set { this.node.SetAttributeValue("restype", value); }
            }

            /// <summary>
            /// Resource name or identifier of a item. For example: the key in the key/value pair in a Java properties file,
            /// the ID of a string in a Windows string table, the index value of an entry in a database table, etc.
            /// </summary>
            public string Resname
            {
                get { return XmlUtil.GetAttributeIfExists(this.node, "resname"); }
                set { this.node.SetAttributeValue("resname", value); }
            }

            public IEnumerable<XlfNote> Notes
            {
                get
                {
                    return this.node.Descendants(this.ns + "note").Select(t => new XlfNote(t));
                }
            }

            public void AddNote(string comment, string from)
            {
                var note = new XlfNote(new XElement(this.ns + "note", comment));
                if (!string.IsNullOrWhiteSpace(from))
                {
                    note.Optional.From = from;
                }
                this.node.Add(note.GetNode());
            }

            public void AddNote(string comment)
            {
                AddNote(comment, string.Empty);
            }

            public void SetCommentFromResx(string comment)
            {
                if (Notes.Any())
                {
                    Notes.First().Value = comment;
                }
                else
                {
                    AddNote(comment);
                }
            }

            private void RemoveNote(string attributeName, string value)
            {
                this.node.Descendants(this.ns + "note").Where(u =>
                {
                    var a = u.Attribute(attributeName);
                    return a != null && a.Value == value;
                }).Remove();
            }
        }
    }
}