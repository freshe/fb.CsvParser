# fb.CsvParser

*A small CSV reader / parser / lexer written in C# for .NET*

A CSV viewer can be tested here:  
https://www.fredrikblank.com/csv-viewer/  

How to use:  

```csharp
//How to use
var csv = "Header 1, Header 2, Header 3\nValue 1, Value 2, Value 3";

var parser = new fb.CsvParser.Parser();
//Or
var parser = new fb.CsvParser.Parser(delimiterChar: ',', quoteChar: '"');

//Get rows with columns
var data = parser.GetRows(csv);
```