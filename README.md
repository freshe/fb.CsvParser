# fb.CsvParser

*A small CSV reader / parser / lexer with async support — written in C# for .NET*

A CSV viewer can be tested here:  
https://www.fredrikblank.com/csv-viewer/

Basic example:
```csharp
var csv = "Header 1,Header 2,Header 3\nValue 1,Value 2,Value 3";
var parser = new fb.CsvParser.Parser();

var rows = parser.GetRows(csv);

foreach (var row in rows)
{
    foreach (var column in row)
    {
        ...
    }
}
```

Async example:
```csharp
using var file = new FileStream("test.csv", FileMode.Open);
using var reader = new StreamReader(file, Encoding.UTF8);

var parser = new fb.CsvParser.Parser();
var rows = parser.GetRowsAsync(reader);

await foreach (var row in rows)
{
    foreach (var column in row)
    {
        ...
    }
}
```