# fb.CsvParser

*A small CSV reader / parser / lexer for .NET*

This was mostly just a fun exercise.  

A CSV viewer can be tested here:  
https://www.fredrikblank.com/csv-viewer/  

How to use:  

```csharp
var parser = new fb.CsvParser.Parser(delimiterChar: ',', escapeChar: '"');
var data = parser.GetRows(text);
```