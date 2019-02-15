
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace DarkCrystal.CommandLine
{
    public struct Token
    {
        public TokenType Type;
        public int Position;
        public int Length;
        public object Data;

        public Token(TokenType type, int position, int length, object data = null)
        {
            this.Type = type;
            this.Position = position;
            this.Length = length;
            this.Data = data;
        }

        public bool IsEndExpressionToken()
        {
            switch (Type)
            {
                case TokenType.Comma:
                case TokenType.ClosedBracket:
                case TokenType.EndOfLine:
                    return true;

                default:
                    return false;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Type, Data);
        }
    }
}