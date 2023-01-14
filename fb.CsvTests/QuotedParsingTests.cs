using System.Text;
using fb.CsvParser;

namespace fb.CsvTests;

public class QuotedParsingTests
{
    private const char DelimiterChar = ',';
    private const char EscapeChar = '"';
    
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestParseQuotedHeaders()
    {
        var csv = "\"a\",\"b\",\"c\"";
        var parser = new Parser(DelimiterChar, EscapeChar);
        var rows = parser.GetRows(csv).ToList();
        
        Assert.That(rows.Count, Is.EqualTo(1));
        Assert.That(rows[0].Length, Is.EqualTo(3));
        Assert.That(rows[0][0], Is.EqualTo("a"));
        Assert.That(rows[0][1], Is.EqualTo("b"));
        Assert.That(rows[0][2], Is.EqualTo("c"));
    }
    
    [Test]
    public void TestParseQuotedHeadersAndNonQuotedValues()
    {
        var csv = "\"a\",\"b\",\"c\"\n1,2,3";
        var parser = new Parser(DelimiterChar, EscapeChar);
        var rows = parser.GetRows(csv).ToList();
        
        Assert.That(rows.Count, Is.EqualTo(2));
        Assert.That(rows[0].Length, Is.EqualTo(3));
        Assert.That(rows[0][0], Is.EqualTo("a"));
        Assert.That(rows[0][1], Is.EqualTo("b"));
        Assert.That(rows[0][2], Is.EqualTo("c"));
        
        Assert.That(rows[1].Length, Is.EqualTo(3));
        Assert.That(rows[1][0], Is.EqualTo("1"));
        Assert.That(rows[1][1], Is.EqualTo("2"));
        Assert.That(rows[1][2], Is.EqualTo("3"));
    }
    
    [Test]
    public void TestParseQuotedValuesWithEscapedValues()
    {
        var csv = 
                "Header 1,\"Header 2\",Header 3\n" +
                "\"Value 1 with, comma\",Value 2,\"Value 3 \"";
        
        var parser = new Parser(DelimiterChar, EscapeChar);
        var rows = parser.GetRows(csv).ToList();
        
        Assert.That(rows.Count, Is.EqualTo(2));
        Assert.That(rows[0].Length, Is.EqualTo(3));
        Assert.That(rows[0][0], Is.EqualTo("Header 1"));
        Assert.That(rows[0][1], Is.EqualTo("Header 2"));
        Assert.That(rows[0][2], Is.EqualTo("Header 3"));
        
        Assert.That(rows[1].Length, Is.EqualTo(3));
        Assert.That(rows[1][0], Is.EqualTo("Value 1 with, comma"));
        Assert.That(rows[1][1], Is.EqualTo("Value 2"));
        Assert.That(rows[1][2], Is.EqualTo("Value 3 "));
    }
    
    [Test]
    public void TestParseQuotedValuesWithQuotes()
    {
        var csv = 
            "Header 1,\"Header 2\",\"Header \"\"3\"\"\"\n" +
            "\"Value 1 with, \"\"comma\"\"\",Value 2,\"Value 3 \"";
        
        var parser = new Parser(DelimiterChar, EscapeChar);
        var rows = parser.GetRows(csv).ToList();
        
        Assert.That(rows.Count, Is.EqualTo(2));
        Assert.That(rows[0].Length, Is.EqualTo(3));
        Assert.That(rows[0][0], Is.EqualTo("Header 1"));
        Assert.That(rows[0][1], Is.EqualTo("Header 2"));
        Assert.That(rows[0][2], Is.EqualTo("Header \"3\""));
        
        Assert.That(rows[1].Length, Is.EqualTo(3));
        Assert.That(rows[1][0], Is.EqualTo("Value 1 with, \"comma\""));
        Assert.That(rows[1][1], Is.EqualTo("Value 2"));
        Assert.That(rows[1][2], Is.EqualTo("Value 3 "));
    }
    
    [Test]
    public void TestParseQuotedValuesWithNewlines()
    {
        var csv = 
            "Header 1,Header 2,Header 3\n" +
            "\"Value\n1\",Value 2,\"Value 3\nwith\nmultiple\n newlines \"";
        
        var parser = new Parser(DelimiterChar, EscapeChar);
        var rows = parser.GetRows(csv).ToList();
        
        Assert.That(rows.Count, Is.EqualTo(2));
        Assert.That(rows[0].Length, Is.EqualTo(3));
        Assert.That(rows[0][0], Is.EqualTo("Header 1"));
        Assert.That(rows[0][1], Is.EqualTo("Header 2"));
        Assert.That(rows[0][2], Is.EqualTo("Header 3"));
        
        Assert.That(rows[1].Length, Is.EqualTo(3));
        Assert.That(rows[1][0], Is.EqualTo("Value\n1"));
        Assert.That(rows[1][1], Is.EqualTo("Value 2"));
        Assert.That(rows[1][2], Is.EqualTo("Value 3\nwith\nmultiple\n newlines "));
    }
    
    [Test]
    public void TestParseQuotedValuesWithComplexCharacters()
    {
        var csv = 
            "Header 1,Header 2,\"Header\n3\nwith\t tab\"\n" +
            "\"Value 1 with\n(,.;\"\"'/\\€#&!@[]+-_—–´`^¨™*<>°§?=%)\",Value 2,\t";
        
        var parser = new Parser(DelimiterChar, EscapeChar);
        var rows = parser.GetRows(csv).ToList();
        
        Assert.That(rows.Count, Is.EqualTo(2));
        Assert.That(rows[0].Length, Is.EqualTo(3));
        Assert.That(rows[0][0], Is.EqualTo("Header 1"));
        Assert.That(rows[0][1], Is.EqualTo("Header 2"));
        Assert.That(rows[0][2], Is.EqualTo("Header\n3\nwith\t tab"));
        
        Assert.That(rows[1].Length, Is.EqualTo(3));
        Assert.That(rows[1][0], Is.EqualTo("Value 1 with\n(,.;\"'/\\€#&!@[]+-_—–´`^¨™*<>°§?=%)"));
        Assert.That(rows[1][1], Is.EqualTo("Value 2"));
        Assert.That(rows[1][2], Is.EqualTo("\t"));
    }
}