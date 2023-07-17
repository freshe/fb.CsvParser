/*
MIT License

Copyright (c) Fredrik B

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

namespace fb.CsvParser;

public sealed class Parser
{
    private readonly List<string> _buffer = new();
    private readonly char _delimiterChar;
    private readonly char _quoteChar;
    
    public Parser(char delimiterChar, char quoteChar)
    {
        _delimiterChar = delimiterChar;
        _quoteChar = quoteChar;
    }

    public IEnumerable<string[]> GetRows(string text)
    {
        var lexer = new Lexer(delimiterChar: _delimiterChar, quoteChar: _quoteChar);
        var tokens = lexer.GetTokens(text);
        
        foreach (var token in tokens)
        {
            if (token == Const.NewlineString)
            {
                yield return _buffer.ToArray();
                _buffer.Clear();
            }
            else
            {
                _buffer.Add(token);
            }
        }

        if (_buffer.Count > 0)
        {
            yield return _buffer.ToArray();
        }
    }

    public async IAsyncEnumerable<string[]> GetRowsAsync(TextReader reader)
    {
        var lexer = new Lexer(delimiterChar: _delimiterChar, quoteChar: _quoteChar);
        var tokens = lexer.GetTokensAsync(reader);

        await foreach (var token in tokens)
        {
            if (token == Const.NewlineString)
            {
                yield return _buffer.ToArray();
                _buffer.Clear();
            }
            else
            {
                _buffer.Add(token);
            }
        }
        
        if (_buffer.Count > 0)
        {
            yield return _buffer.ToArray();
        }
    }

    public IEnumerable<string[]> GetRows(TextReader reader)
    {
        var lexer = new Lexer(delimiterChar: _delimiterChar, quoteChar: _quoteChar);
        var tokens = lexer.GetTokens(reader);
        
        foreach (var token in tokens)
        {
            if (token == Const.NewlineString)
            {
                yield return _buffer.ToArray();
                _buffer.Clear();
            }
            else
            {
                _buffer.Add(token);
            }
        }

        if (_buffer.Count > 0)
        {
            yield return _buffer.ToArray();
        }
    }
}