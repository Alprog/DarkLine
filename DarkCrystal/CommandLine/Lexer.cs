
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;

namespace DarkCrystal.CommandLine
{
    public class Lexer
    {
        string operators = "*/+!|&>=<";

        public IEnumerator<Token> Tokenize(string line)
        {
            var token = new Token(TokenType.Invalid, 0, line.Length);
            int i = 0;
            while (i < line.Length)
            {
                var c = line[i];
                if (Char.IsWhiteSpace(c))
                {
                    // WhiteSpace
                    i++;
                    continue;
                }
                else if (Char.IsLetter(c) || c.Equals('_'))
                {
                    // Identifier or Keyword
                    int si = i; i++;
                    while (i < line.Length)
                    {
                        c = line[i];
                        if (Char.IsLetterOrDigit(c) || c.Equals('_'))
                        {
                            i++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    var tokenText = line.Substring(si, i - si);
                    if (tokenText.ToLower() == "true")
                    {
                        token = ValueToken(si, i - si, true);
                    }
                    else if (tokenText.ToLower() == "false")
                    {
                        token = ValueToken(si, i - si, false);
                    }
                    else if (tokenText.ToLower() == "null")
                    {
                        token = ValueToken(si, i - si, null);
                    }
                    else
                    {
                        if (i < line.Length && line[i].Equals('…'))
                        {
                            token = new Token(TokenType.Autocomplete, si, i - si, tokenText);
                            i++;
                        }
                        else
                        {
                            token = new Token(TokenType.Identifier, si, i - si, tokenText);
                        }
                    }
                }
                else if (c.Equals('\"'))
                {
                    // String
                    int si = i; i++;
                    bool ended = false;
                    while (i < line.Length)
                    {
                        c = line[i];
                        if (!c.Equals('\"'))
                        {
                            i++;
                        }
                        else
                        {
                            ended = true;
                            i++;
                            break;
                        }
                    }
                    if (ended)
                    {
                        var text = line.Substring(si + 1, i - si - 2);
                        token = ValueToken(si, i - si, text);
                    }
                    else
                    {
                        token = new Token(TokenType.Invalid, si, i - si);
                    }
                }
                else if (Char.IsDigit(c) || c.Equals('-'))
                {
                    // Minus or digit
                    if (c.Equals('-'))
                    {
                        if (token.Type == TokenType.Value ||
                            token.Type == TokenType.Identifier ||
                            token.Type == TokenType.ClosedBracket)
                        {
                            // force minus operator
                            i++;
                            token = new Token(TokenType.Operator, i, 1, Operator.Get("-"));
                            yield return token;
                            continue;
                        }
                    }

                    int si = i; i++;
                    while (i < line.Length)
                    {
                        c = line[i];
                        if (Char.IsDigit(c) || c.Equals('.'))
                        {
                            i++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    var tokenText = line.Substring(si, i - si);
                    if (tokenText == "-")
                    {
                        token = new Token(TokenType.Operator, si, i - si, Operator.Get(tokenText));
                        i++;
                    }
                    else
                    {
                        float number;
                        if (Single.TryParse(tokenText, NumberStyles.Any, CultureInfo.InvariantCulture, out number))
                        {
                            token = ValueToken(si, i - si, number);
                        }
                        else
                        {
                            token = new Token(TokenType.Invalid, si, i - si);
                        }
                    }
                }
                else if (operators.Contains(c.ToString()))
                {
                    // Operator
                    int si = i; i++;
                    while (i < line.Length)
                    {
                        c = line[i];
                        if (operators.Contains(c.ToString()))
                        {
                            i++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    var tokenText = line.Substring(si, i - si);
                    if (tokenText == "=")
                    {
                        token = new Token(TokenType.AssignmentOperator, si, i - si);
                    }
                    else
                    {
                        token = new Token(TokenType.Operator, si, i - si, Operator.Get(tokenText));
                    }
                }
                else if (c.Equals('.'))
                {
                    token = new Token(TokenType.Dot, i, 1);
                    i++;
                }
                else if (c.Equals('('))
                {
                    token = new Token(TokenType.OpenBracket, i, 1);
                    i++;
                }
                else if (c.Equals(')'))
                {
                    token = new Token(TokenType.ClosedBracket, i, 1);
                    i++;
                }
                else if (c.Equals(','))
                {
                    token = new Token(TokenType.Comma, i, 1);
                    i++;
                }
                else if (c.Equals('…'))
                {
                    token = new Token(TokenType.Autocomplete, i, 1, String.Empty);
                    i++;
                }
                else
                {
                    token = new Token(TokenType.Invalid, i, 1);
                    i++;
                }

                yield return token;
            }

            yield return new Token(TokenType.EndOfLine, i, 0);
        }

        private Token ValueToken(int position, int length, object data)
        {
            var token = new Token(TokenType.Value, position, length);
            var type = data?.GetType() ?? typeof(void);
            token.Data = new Value(type, data, token);
            return token;
        }
    }
}