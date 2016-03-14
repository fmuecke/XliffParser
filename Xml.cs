using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace xlflib
{
    public class Xml
    {
        public static string GetAttributeIfExists(XElement node, string name)
        {
            return node.Attributes(name).Any() ? node.Attribute(name).Value : string.Empty;
        }

    }
}
