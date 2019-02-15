
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace DarkCrystal.CommandLine
{
    public class TokenException : Exception
    {
        public Token Token { get; protected set; }
        public new string Message { get; protected set; }

        protected TokenException()
        {
        }

        public TokenException(string message, Token token)
        {
            this.Message = message;
            this.Token = token;
        }
    }
}