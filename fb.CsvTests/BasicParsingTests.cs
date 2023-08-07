using fb.CsvParser;

namespace fb.CsvTests;

public class BasicParsingTests
{
    private const char DelimiterChar = ',';
    private const char EscapeChar = '"';
    
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestParseEmptyStringShouldReturnNoElements()
    {
        var parser = new Parser(DelimiterChar, EscapeChar);
        var rows = parser.GetRows(string.Empty).ToList();
        
        Assert.That(rows.Count, Is.EqualTo(0));
    }
    
    [Test]
    public void TestParseWhiteSpaceShouldReturnOneRowAndOneValue()
    {
        var parser = new Parser(DelimiterChar, EscapeChar);
        var rows = parser.GetRows(" ").ToList();
        
        Assert.That(rows.Count, Is.EqualTo(1));
        Assert.That(rows[0].Length, Is.EqualTo(1));
        Assert.That(rows[0][0], Is.EqualTo(" "));
    }
    
    [Test]
    public void TestParseSingleRow()
    {
        var csv = "a,b,c";
        var parser = new Parser(DelimiterChar, EscapeChar);
        var rows = parser.GetRows(csv).ToList();
        
        Assert.That(rows.Count, Is.EqualTo(1));
        Assert.That(rows[0].Length, Is.EqualTo(3));
        Assert.That(rows[0][0], Is.EqualTo("a"));
        Assert.That(rows[0][1], Is.EqualTo("b"));
        Assert.That(rows[0][2], Is.EqualTo("c"));

        var parser2 = new Parser();
        var rows2 = parser2.GetRows(csv).ToList();
        
        Assert.That(rows2.Count, Is.EqualTo(1));
        Assert.That(rows2[0].Length, Is.EqualTo(3));
        Assert.That(rows2[0][0], Is.EqualTo("a"));
        Assert.That(rows2[0][1], Is.EqualTo("b"));
        Assert.That(rows2[0][2], Is.EqualTo("c"));
    }
    
    [Test]
    public void TestParseSingleRowWithEmptyHeader()
    {
        var csv = "a,b,c,";
        var parser = new Parser(DelimiterChar, EscapeChar);
        var rows = parser.GetRows(csv).ToList();
        
        Assert.That(rows.Count, Is.EqualTo(1));
        Assert.That(rows[0].Length, Is.EqualTo(4));
        Assert.That(rows[0][0], Is.EqualTo("a"));
        Assert.That(rows[0][1], Is.EqualTo("b"));
        Assert.That(rows[0][2], Is.EqualTo("c"));
        Assert.That(rows[0][3], Is.EqualTo(""));
    }
    
    [Test]
    public void TestParseSingleRowWithEmptyHeader2()
    {
        var csv = "a,,c,";
        var parser = new Parser(DelimiterChar, EscapeChar);
        var rows = parser.GetRows(csv).ToList();
        
        Assert.That(rows.Count, Is.EqualTo(1));
        Assert.That(rows[0].Length, Is.EqualTo(4));
        Assert.That(rows[0][0], Is.EqualTo("a"));
        Assert.That(rows[0][1], Is.EqualTo(""));
        Assert.That(rows[0][2], Is.EqualTo("c"));
        Assert.That(rows[0][3], Is.EqualTo(""));
    }
    
    [Test]
    public void TestParseSingleRowWithEmptyHeader3()
    {
        var csv = ",b,c,";
        var parser = new Parser(DelimiterChar, EscapeChar);
        var rows = parser.GetRows(csv).ToList();
        
        Assert.That(rows.Count, Is.EqualTo(1));
        Assert.That(rows[0].Length, Is.EqualTo(4));
        Assert.That(rows[0][0], Is.EqualTo(""));
        Assert.That(rows[0][1], Is.EqualTo("b"));
        Assert.That(rows[0][2], Is.EqualTo("c"));
        Assert.That(rows[0][3], Is.EqualTo(""));
    }
    
    [Test]
    public void TestParseMultipleRows()
    {
        var csv = "a,b,c\n1,2,3";
        var parser = new Parser(DelimiterChar, EscapeChar);
        var rows = parser.GetRows(csv).ToList();
        
        Assert.That(rows.Count, Is.EqualTo(2));
        Assert.That(rows[0].Length, Is.EqualTo(3));
        Assert.That(rows[0][0], Is.EqualTo("a"));
        Assert.That(rows[0][1], Is.EqualTo("b"));
        Assert.That(rows[0][2], Is.EqualTo("c"));
        
        Assert.That(rows[1][0], Is.EqualTo("1"));
        Assert.That(rows[1][1], Is.EqualTo("2"));
        Assert.That(rows[1][2], Is.EqualTo("3"));
    }
    
    [Test]
    public void TestParseMultipleRowsLastWhereValueIsEmpty()
    {
        var csv = "a,b,c\n1,2,";
        var parser = new Parser(DelimiterChar, EscapeChar);
        var rows = parser.GetRows(csv).ToList();
        
        Assert.That(rows.Count, Is.EqualTo(2));
        Assert.That(rows[0].Length, Is.EqualTo(3));
        Assert.That(rows[0][0], Is.EqualTo("a"));
        Assert.That(rows[0][1], Is.EqualTo("b"));
        Assert.That(rows[0][2], Is.EqualTo("c"));
        
        Assert.That(rows[1][0], Is.EqualTo("1"));
        Assert.That(rows[1][1], Is.EqualTo("2"));
        Assert.That(rows[1][2], Is.EqualTo(""));
    }
    
    [Test]
    public void TestParseMultipleRowsWhereSomeValuesAreEmpty()
    {
        var csv = "a,b,\n1,,3";
        var parser = new Parser(DelimiterChar, EscapeChar);
        var rows = parser.GetRows(csv).ToList();
        
        Assert.That(rows.Count, Is.EqualTo(2));
        Assert.That(rows[0].Length, Is.EqualTo(3));
        Assert.That(rows[0][0], Is.EqualTo("a"));
        Assert.That(rows[0][1], Is.EqualTo("b"));
        Assert.That(rows[0][2], Is.EqualTo(""));
        
        Assert.That(rows[1][0], Is.EqualTo("1"));
        Assert.That(rows[1][1], Is.EqualTo(""));
        Assert.That(rows[1][2], Is.EqualTo("3"));
    }
}