namespace XliffParser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /*
     <header>
      <tool tool-id="MultilingualAppToolkit" tool-name="Multilingual App Toolkit" tool-version="3.1.1250.0" tool-company="Microsoft" />
     </header>
    */

    public class XlfTool
    {
        public string Company { get; }

        public string Id { get; }

        public string Name { get; }

        public string Version { get; }
    }
}