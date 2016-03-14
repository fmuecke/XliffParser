using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xlflib
{
    /// <summary>
    /// see http://docs.oasis-open.org/xliff/v1.2/os/xliff-core.html#datatype
    /// 
    /// Userdefined datatypes are allowed - but have to begin with "x-".
    /// </summary>
    enum XlfDataType
    {
        asp,  //     Indicates Active Server Page data.
        c,  // Indicates C source file data.
        cdf,  //     Indicates Channel Definition Format (CDF) data.
        cfm,  //     Indicates ColdFusion data.
        cpp,  //     Indicates C++ source file data.
        csharp,  //Indicates C-Sharp data.
        cstring,  //     Indicates strings from C, ASM, and driver files data.
        csv,  //     Indicates comma-separated values data.
        database,  //    Indicates database data.
        documentfooter,  //Indicates portions of document that follows data and contains metadata.
        documentheader,  //Indicates portions of document that precedes data and contains metadata.
        filedialog,  //Indicates data from standard UI file operations dialogs (e.g., Open, Save, Save As, Export, Import).
        form,  //    Indicates standard user input screen data.
        html,  //    Indicates HyperText Markup Language (HTML) data - document instance.
        htmlbody,  //    Indicates content within an HTML document's <body> element.
        ini,  //     Indicates Windows INI file data.
        interleaf,  // Indicates Interleaf data.
        javaclass,  // Indicates Java source file data (extension '.java').
        javapropertyresourcebundle,  //Indicates Java property resource bundle data.
        javalistresourcebundle,  //Indicates Java list resource bundle data.
        javascript,  //Indicates JavaScript source file data.
        jscript,  //     Indicates JScript source file data.
        layout,  //Indicates information relating to formatting.
        lisp,  //    Indicates LISP source file data.
        margin,  //Indicates information relating to margin formats.
        menufile,  //    Indicates a file containing menu.
        messagefile,  //     Indicates numerically identified string table.
        mif,  //     Indicates Maker Interchange Format (MIF) data.
        mimetype,  //    Indicates that the datatype attribute value is a MIME Type value and is defined in the mime-type attribute.
        mo,  //Indicates GNU Machine Object data.
        msglib,  //Indicates Message Librarian strings created by Novell's Message Librarian Tool.
        pagefooter,  //Indicates information to be displayed at the bottom of each page of a document.
        pageheader,  //Indicates information to be displayed at the top of each page of a document.
        parameters,  //Indicates a list of property values (e.g., settings within INI files or preferences dialog).
        pascal,  //Indicates Pascal source file data.
        php,  //     Indicates Hypertext Preprocessor data.
        plaintext,  // Indicates plain text file (no formatting other than, possibly, wrapping).
        po,  //Indicates GNU Portable Object file.
        report,  //Indicates dynamically generated user defined document. e.g. Oracle Report, Crystal Report, etc.
        resources,  // Indicates Windows .NET binary resources.
        resx,  //    Indicates Windows .NET Resources.
        rtf,  //     Indicates Rich Text Format (RTF) data.
        sgml,  //    Indicates Standard Generalized Markup Language (SGML) data - document instance.
        sgmldtd,  //     Indicates Standard Generalized Markup Language (SGML) data - Document Type Definition (DTD).
        svg,  //     Indicates Scalable Vector Graphic (SVG) data.
        vbscript,  //    Indicates VisualBasic Script source file.
        warning,  //     Indicates warning message.
        winres,  //Indicates Windows (Win32) resources (i.e. resources extracted from an RC script, a message file, or a compiled file).
        xhtml,  // Indicates Extensible HyperText Markup Language (XHTML) data - document instance.
        xml,  //     Indicates Extensible Markup Language (XML) data - document instance.
        xmldtd,  //Indicates Extensible Markup Language (XML) data - Document Type Definition (DTD).
        xsl,  //     Indicates Extensible Stylesheet Language (XSL) data.
        xul,  //     Indicates XUL elements.
    }
}
