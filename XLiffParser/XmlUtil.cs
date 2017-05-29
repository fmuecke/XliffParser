namespace XliffParser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    public static class XmlUtil
    {
        public static string GetAttributeIfExists(XElement node, string name)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return node.Attributes(name).Any() ? node.Attribute(name).Value : string.Empty;
        }

        public static int GetIntAttributeIfExists(XElement node, string name)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return node.Attributes(name).Any() ? int.Parse(node.Attribute(name).Value) : 0;
        }

        public static string NormalizeLineBreaks(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return string.Empty;
            }

            return s.Replace("\r", string.Empty);
        }

        public static string DeNormalizeLineBreaks(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return string.Empty;
            }

            return NormalizeLineBreaks(s).Replace("\r", "\r\n");
        }
    }
}