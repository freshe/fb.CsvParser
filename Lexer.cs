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
    
    private readonly StringBuilder _buffer = new();
    private readonly char _delimiterChar;
    private readonly char _escapeChar;
    
    private State _state = State.Default;
    
    public Lexer(char delimiterChar, char escapeChar)
    {
        _delimiterChar = delimiterChar;
        _escapeChar = escapeChar;
    }

    public IEnumerable<string> GetTokens(string text)
    {
        var tokens = Tokenize(text);

        foreach (var token in tokens)
        {
            yield return token;
        }
        
        if (text[^1] == _delimiterChar)
        {
            yield return string.Empty;
        }
    }

    private IEnumerable<string> Tokenize(string text)
    {
        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            
            switch (_state)
            {
                case State.Default:
                    if (c == _escapeChar)
                    {
                        _state = State.InQuotedField;
                    }
                    else if (c == _delimiterChar)
                    {
                        yield return string.Empty;
                    }
                    else if (c == Const.NewlineCharacter)
                    {
                        yield return _buffer.ToString();
                        _buffer.Clear();
                        yield return Const.NewlineCharacter.ToString();
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
                        yield return _buffer.ToString();
                        _buffer.Clear();
                        _state = State.Default;
                    }
                    else if (c == Const.NewlineCharacter)
                    {
                        yield return _buffer.ToString();
                        _buffer.Clear();
                        yield return Const.NewlineCharacter.ToString();
                        _state = State.Default;
                    }
                    else if (c != Const.CarriageReturnCharacter)
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
                        yield return _buffer.ToString();
                        _buffer.Clear();
                        _state = State.Default;
                    }
                    else if (c == Const.NewlineCharacter)
                    {
                        yield return _buffer.ToString();
                        _buffer.Clear();
                        yield return Const.NewlineCharacter.ToString();
                        _state = State.Default;
                    }
                    else
                    {
                        throw new Exception("Malformed CSV");
                    }
                    break;
            }
        }

        if (_buffer.Length > 0)
        {
            yield return _buffer.ToString();
            _buffer.Clear();
        }
    }
}