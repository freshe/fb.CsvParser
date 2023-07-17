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

using System.Text;

namespace fb.CsvParser;

internal class Lexer
{
    private enum State
    {
        Default,
        InField,
        InQuotedField,
        InQuotedFieldEscapedQuote
    }

    private readonly char _delimiterChar;
    private readonly char _quoteChar;
    
    private const int BufferSize = 65536;
    private readonly StringBuilder _buffer = new();
    private readonly string[] _tokenPool = new string[2];
    private State _state = State.Default;
    
    public Lexer(char delimiterChar, char quoteChar)
    {
        _delimiterChar = delimiterChar;
        _quoteChar = quoteChar;
    }

    public IEnumerable<string> GetTokens(string text)
    {
        foreach (var c in text)
        {
            var count = Tokenize(c);
            
            if (count > 0)
            {
                for (var i = 0; i < count; i++)
                {
                    yield return _tokenPool[i];
                }
            }
        }

        if (_buffer.Length > 0)
        {
            yield return _buffer.ToString();
            _buffer.Clear();
        }

        if (text.Length > 0 && text[^1] == _delimiterChar)
        {
            yield return string.Empty;
        }
    }

    public async IAsyncEnumerable<string> GetTokensAsync(TextReader reader)
    {
        var buffer = new char[BufferSize];
        var lastChar = '\0';
        int read;
        
        while ((read = await reader.ReadAsync(buffer, 0, BufferSize)) > 0)
        {
            var chunk = new string(buffer, 0, read);
            
            foreach (var c in chunk)
            {
                var count = Tokenize(c);
                if (count > 0)
                {
                    for (var i = 0; i < count; i++)
                    {
                        yield return _tokenPool[i];
                    }
                }
            }

            lastChar = chunk[^1];
        }

        if (_buffer.Length > 0)
        {
            yield return _buffer.ToString();
            _buffer.Clear();
        }
        
        if (lastChar == _delimiterChar)
        {
            yield return string.Empty;
        }
    }

    public IEnumerable<string> GetTokens(TextReader reader)
    {
        var buffer = new char[BufferSize];
        var lastChar = '\0';
        int read;
        
        while ((read = reader.Read(buffer, 0, BufferSize)) > 0)
        {
            var chunk = new string(buffer, 0, read);
            
            foreach (var c in chunk)
            {
                var count = Tokenize(c);
                
                if (count > 0)
                {
                    for (var i = 0; i < count; i++)
                    {
                        yield return _tokenPool[i];
                    }
                }
            }

            lastChar = chunk[^1];
        }

        if (_buffer.Length > 0)
        {
            yield return _buffer.ToString();
            _buffer.Clear();
        }
        
        if (lastChar == _delimiterChar)
        {
            yield return string.Empty;
        }
    }

    private int Tokenize(char c)
    {
        switch (_state)
        {
            case State.Default:
                if (c == _quoteChar)
                {
                    _state = State.InQuotedField;
                }
                else if (c == _delimiterChar)
                {
                    _tokenPool[0] = string.Empty;
                    return 1;
                }
                else if (c == Const.NewlineCharacter)
                {
                    var value = _buffer.ToString();
                    _buffer.Clear();
                    _tokenPool[0] = value;
                    _tokenPool[1] = Const.NewlineString;
                    return 2;
                }
                else if (c != Const.CarriageReturnCharacter)
                {
                    _state = State.InField;
                    _buffer.Append(c);
                }
                break;
            case State.InField:
                if (c == _delimiterChar)
                {
                    var value = _buffer.ToString();
                    _buffer.Clear();
                    _tokenPool[0] = value;
                    _state = State.Default;
                    return 1;
                }
                if (c == Const.NewlineCharacter)
                {
                    var value = _buffer.ToString();
                    _buffer.Clear();
                    _tokenPool[0] = value;
                    _tokenPool[1] = Const.NewlineString;
                    _state = State.Default;
                    return 2;
                }
                if (c != Const.CarriageReturnCharacter)
                {
                    _buffer.Append(c);
                }
                break;
            case State.InQuotedField:
                if (c == _quoteChar)
                {
                    _state = State.InQuotedFieldEscapedQuote;
                }
                else if (c != Const.CarriageReturnCharacter)
                {
                    _buffer.Append(c);
                }
                break;
            case State.InQuotedFieldEscapedQuote:
                if (c == _quoteChar)
                {
                    _buffer.Append(c);
                    _state = State.InQuotedField;
                }
                else if (c == _delimiterChar)
                {
                    var value = _buffer.ToString();
                    _buffer.Clear();
                    _tokenPool[0] = value;
                    _state = State.Default;
                    return 1;
                }
                else if (c == Const.NewlineCharacter)
                {
                    var value = _buffer.ToString();
                    _buffer.Clear();
                    _tokenPool[0] = value;
                    _tokenPool[1] = Const.NewlineString;
                    _state = State.Default;
                    return 2;
                }
                else if (c != Const.CarriageReturnCharacter)
                {
                    throw new Exception("Malformed CSV");
                }
                break;
        }

        return 0;
    }
}