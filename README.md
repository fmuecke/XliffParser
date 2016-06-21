# XliffParser

[![NuGet version](https://badge.fury.io/nu/fmdev.XliffParser.svg)](https://badge.fury.io/nu/fmdev.XliffParser)

This library is intended as a thin layer above the original markup to help access the contents of XLIFF (XML Localization Interchange File Format) files.

Its main purpose is:
- reading values from a XLIFF file
- changing values inside a XLIFF file
- exporting translation data to C# resource files (resx)
- update XLIFF file from C# resource files (resx)

More information on XLIFF can be found here:
- [OASIS XLIFF v1.1](http://www.oasis-open.org/committees/xliff/documents/xliff-specification.htm)
- [OASIS XLIFF v1.2](docs.oasis-open.org/xliff/v1.2/os/xliff-core.html)
- [OASIS XLIFF v2.0](http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html) *(Not yet supported!)*

## Available on [nuget.org](https://www.nuget.org/packages/fmdev.XliffParser/)

To install XliffParser, run the following command in the Package Manager Console

    Install-Package fmdev.XliffParser


