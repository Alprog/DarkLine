
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace DarkCrystal.CommandLine
{
    public class UnexpectedTokenException : TokenException
    {
        public UnexpectedTokenException(string expectedString, Token gottenToken)
        {
            var format = "Unexpected token at {0}. Expect {1}, got {2}";
            this.Message = String.Format(format, gottenToken.Position, expectedString, gottenToken.Type);
            this.Token = gottenToken;
        }
    }
}