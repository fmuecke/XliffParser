# XliffParser

[![NuGet version](https://badge.fury.io/nu/fmdev.XliffParser.svg)](https://badge.fury.io/nu/fmdev.XliffParser)

This library is intended as a thin layer above the original markup to help access the contents of XLIFF (XML Localization Interchange File Format) files.

Its main purposes are:
- reading values from a XLIFF file
- changing values inside a XLIFF file
- exporting translation data to C# resource files (resx)
- updating XLIFF file from C# resource files (resx)

More information on XLIFF can be found here:
- [OASIS XLIFF v1.1](http://www.oasis-open.org/committees/xliff/documents/xliff-specification.htm)
- [OASIS XLIFF v1.2](http://docs.oasis-open.org/xliff/v1.2/os/xliff-core.html)
- [OASIS XLIFF v2.0](http://docs.oasis-open.org/xliff/xliff-core/v2.0/xliff-core-v2.0.html) *(Not yet supported!)*

## Available on [nuget.org](https://www.nuget.org/packages/fmdev.XliffParser/)

To install XliffParser, run the following command in the Package Manager Console

    Install-Package fmdev.XliffParser
    
## Examples
### Reading values from a XLIFF file
```c#
var doc = new XliffParser.XlfDocument(fileName);
var xlfFile = doc.Files.Single();
foreach (var u in xlfFile.TransUnits)
{
    // id | source | target | target state
    Console.WriteLine("{0}|{1}|{2}", u.Id, u.Source, u.Target, u.Optional.TargetState);
}
```

### Changing values inside a XLIFF file
```c#
var unit = xlfFile.AddTransUnit("MyResourceId", "some resouce string", "my awesome translation");
unit.Optional.TargetState = "translated";
unit.Optional.AddNote("No comment!", "XliffParser");
doc.Save();
```
### Exporting translation data to C# resource files (resx)
```c#
doc.SaveAsResX();
```

### Updating XLIFF file from C# resource files (resx)

```c#
var result = doc.UpdateFromSource();
Console.WriteLine("updated: {0}, added: {1}, removed: {2}",
    result.UpdatedItems.Count(), result.AddedItems.Count(), result.RemovedItems.Count());
```
