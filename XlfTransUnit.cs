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

        public XlfTransUnit(XElement node)
        {
            this.node = node;
            ns = node.Document.Root.Name.Namespace;

            Optional = new Optionals(this.node);
        }

        public XlfTransUnit(XElement node, string id, string source, string target)
        {
            this.node = node;
            ns = node.Document.Root.Name.Namespace;

            Id = id;
            Source = source;
            Target = target;

            Optional = new Optionals(this.node);
        }

        public string Id
        {
            get { return this.node.Attribute("id").Value; }
            private set { this.node.SetAttributeValue("id", value); }
        }

        public string Source
        {
            get { return this.node.Element(ns + "source").Value; }
            set { this.node.SetElementValue(ns + "source", value); }
        }
        public string Target
        {
            get { return this.node.Element(ns + "target").Value; }
            set { this.node.SetElementValue(ns + "target", value); }
        }

        public Optionals Optional { get; }

        public class Optionals
        {
            private XElement node;

            internal Optionals(XElement node)
            {
                this.node = node;
            }

            /// <summary>
            /// Indicates whether a translation is final or has passed its final review.
            /// </summary>
            public string Approved { get { return XmlUtil.GetAttributeIfExists(this.node, "approved"); } }

            /// <summary>
            /// Indicates whether or not the text referred to should be translated.
            /// </summary>
            public string Translate { get { return XmlUtil.GetAttributeIfExists(this.node, "translate"); } }

            /// <summary>
            /// The status of a particular translation in a <target> or <bin-target> element.
            /// <see cref="http://docs.oasis-open.org/xliff/v1.2/os/xliff-core.html#state"/>
            /// </summary>
            public string TargetState
            {
                get
                {
                    var ns = this.node.Document.Root.Name.Namespace;
                    return XmlUtil.GetAttributeIfExists(this.node.Element(ns + "target"), "state");
                }
                set
                {
                    var ns = this.node.Document.Root.Name.Namespace;
                    this.node.Element(ns + "target").SetAttributeValue("state", value);
                }
            } // TODO later: use XlfState

            /// <summary>
            /// The datatype attribute specifies the kind of text contained in the element. Depending on that type, you may
            /// apply different processes to the data. For example: datatype="winres" specifies that the content is Windows
            /// resources which would allow using the Win32 API in rendering the content.
            /// </summary>
            public string DataType { get { return XmlUtil.GetAttributeIfExists(this.node, "datatype"); } } // TODO later: use XlfDataType

            /// <summary>
            ///  Indicates the resource type of the container element.
            /// </summary>
            public string Restype {
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

            /// <summary>
            ///  The <note> element is used to add localization-related comments to the XLIFF document. The content of <note> 
            ///  may be instructions from developers about how to handle the <source>, comments from the translator about the
            ///  translation, or any comment from anyone involved in processing the XLIFF file. The optional xml:lang attribute
            ///  specifies the language of the note content. The optional from attribute indicates who entered the note. 
            ///  The optional priority attribute allows a priority from 1 (high) to 10 (low) to be assigned to the note. 
            ///  The optional annotates attribute indicates if the note is a general note or, in the case of a <trans-unit>, 
            ///  pertains specifically to the <source> or the <target> element.
            /// </summary>
            public List<string> Notes
            {
                get
                {
                    var ns = this.node.Document.Root.Name.Namespace;
                    return new List<string>(this.node.Elements(ns + "note").Select(n => n.Value));
                }
            }

            public void AddNote(string comment)
            {
                var ns = this.node.Document.Root.Name.Namespace;
                var n = new XElement(ns + "note", comment);
                this.node.DescendantNodes().Last().AddAfterSelf(n);
            }
        }
    }
}