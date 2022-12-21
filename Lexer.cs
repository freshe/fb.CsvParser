/*
MIT License

Copyright (c) 2022 Fredrik Blank

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
    private readonly char _escapeChar;
    
    private const int BufferSize = 16384;
    private readonly StringBuilder _buffer = new();
    private readonly List<string> _tokenPool = new(2);
    
    private State _state = State.Default;
    
    public Lexer(char delimiterChar, char escapeChar)
    {
        _delimiterChar = delimiterChar;
        _escapeChar = escapeChar;
    }

    public IEnumerable<string> GetTokens(Stream data)
    {
        using var reader = new StreamReader(data, Encoding.UTF8);
        var buffer = new char[BufferSize];
        
        int read;
        var lastChar = '\0';
        
        while ( (read = reader.Read(buffer, 0, BufferSize)) > 0)
        {
            var chunk = new string(buffer, 0, read);
            foreach (var c in chunk)
            {
                var tokens = Tokenize(c);
                if (tokens != null)
                {
                    foreach (var token in tokens)
                    {
                        yield return token;
                    }
                }

                lastChar = c;
            }
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

    public IEnumerable<string> GetTokens(string text)
    {
        foreach (var c in text)
        {
            var tokens = Tokenize(c);
            if (tokens != null)
            {
                foreach (var token in tokens)
                {
                    yield return token;
                }
            }
        }

        if (_buffer.Length > 0)
        {
            yield return _buffer.ToString();
            _buffer.Clear();
        }

        if (text[^1] == _delimiterChar)
        {
            yield return string.Empty;
        }
    }
    
    private List<string>? Tokenize(char c)
    {
        switch (_state)
        {
            case State.Default:
                if (c == _escapeChar)
                {
                    _state = State.InQuotedField;
                }
                else if (c == _delimiterChar)
                {
                    _tokenPool.Clear();
                    _tokenPool.Add(string.Empty);
                    return _tokenPool;
                }
                else if (c == Const.NewlineCharacter)
                {
                    var value = _buffer.ToString();
                    _buffer.Clear();
                    _tokenPool.Clear();
                    _tokenPool.Add(value);
                    _tokenPool.Add(Const.NewlineString);
                    return _tokenPool;
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
                    _tokenPool.Clear();
                    _tokenPool.Add(value);
                    _state = State.Default;
                    return _tokenPool;
                }
                if (c == Const.NewlineCharacter)
                {
                    var value = _buffer.ToString();
                    _buffer.Clear();
                    _tokenPool.Clear();
                    _tokenPool.Add(value);
                    _tokenPool.Add(Const.NewlineString);
                    _state = State.Default;
                    return _tokenPool;
                }
                if (c != Const.CarriageReturnCharacter)
                {
                    _buffer.Append(c);
                }
                break;
            case State.InQuotedField:
                if (c == _escapeChar)
                {
                    _state = State.InQuotedFieldEscapedQuote;
                }
                else if (c != Const.CarriageReturnCharacter)
                {
                    _buffer.Append(c);
                }
                break;
            case State.InQuotedFieldEscapedQuote:
                if (c == _escapeChar)
                {
                    _buffer.Append(c);
                    _state = State.InQuotedField;
                }
                else if (c == _delimiterChar)
                {
                    var value = _buffer.ToString();
                    _buffer.Clear();
                    _tokenPool.Clear();
                    _tokenPool.Add(value);
                    _state = State.Default;
                    return _tokenPool;
                }
                else if (c == Const.NewlineCharacter)
                {
                    var value = _buffer.ToString();
                    _buffer.Clear();
                    _tokenPool.Clear();
                    _tokenPool.Add(value);
                    _tokenPool.Add(Const.NewlineString);
                    _state = State.Default;
                    return _tokenPool;
                }
                else
                {
                    throw new Exception("Malformed CSV");
                }
                break;
        }

        return null;
    }
}