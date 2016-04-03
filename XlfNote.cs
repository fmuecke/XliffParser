using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace xlflib
{
    /// <summary>
    ///  The <note> element is used to add localization-related comments to the XLIFF document. The content of <note>
    ///  may be instructions from developers about how to handle the <source>, comments from the translator about the
    ///  translation, or any comment from anyone involved in processing the XLIFF file. The optional xml:lang attribute
    ///  specifies the language of the note content. The optional from attribute indicates who entered the note.
    ///  The optional priority attribute allows a priority from 1 (high) to 10 (low) to be assigned to the note.
    ///  The optional annotates attribute indicates if the note is a general note or, in the case of a <trans-unit>,
    ///  pertains specifically to the <source> or the <target> element.
    /// </summary>
    public class XlfNote
    {
        private XElement node;

        public XlfNote(XElement node)
        {
            this.node = node;
            Optional = new Optionals(this.node);
        }

        public string Value
        {
            get { return this.node.Value; }
            set { this.node.Value = value; }
        }

        public Optionals Optional { get; }

        public XElement GetNode()
        {
            return this.node;
        }

        public class Optionals
        {
            private XElement node;

            internal Optionals(XElement node)
            {
                this.node = node;
            }

            /// <summary>
            /// Specifies the language of the note content.
            /// </summary>
            public string Lang
            {
                get { return XmlUtil.GetAttributeIfExists(this.node, "xml:lang"); }
                set { this.node.SetAttributeValue("xml:lang", value); }
            }

            /// <summary>
            /// Indicates who entered the note.
            /// </summary>
            public string From
            {
                get { return XmlUtil.GetAttributeIfExists(this.node, "from"); }
                set { this.node.SetAttributeValue("from", value); }
            }

            /// <summary>
            /// Allows a priority from 1 (high) to 10 (low) to be assigned to the note.
            /// </summary>
            public int Priority
            {
                get { return XmlUtil.GetIntAttributeIfExists(this.node, "priority"); }
                set { this.node.SetAttributeValue("priority", value); }
            }

            /// <summary>
            /// Indicates if the note is a general note or, in the case of a <trans-unit>,
            /// pertains specifically to the <source> or the <target> element.
            /// </summary>
            public string Annotates
            {
                get { return XmlUtil.GetAttributeIfExists(this.node, "annotates"); }
                set { this.node.SetAttributeValue("annotates", value); }
            }
        }
    }
}