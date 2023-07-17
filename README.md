# fb.CsvParser

*A small CSV reader / parser / lexer written in C# for .NET*

This was mostly just a fun exercise.  

A CSV viewer can be tested here:  
https://www.fredrikblank.com/csv-viewer/  

How to use:  

```csharp
var parser = new fb.CsvParser.Parser(delimiterChar: ',', quoteChar: '"');
var data = parser.GetRows(text);
```