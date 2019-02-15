
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace DarkCrystal.CommandLine
{
    public class FormattedException : Exception
    {
        public FormattedException(string message, string line, Token token)
            : base(GetMessage(message, line, token))
        {
        }

        private static string GetMessage(string message, string line, Token token)
        {
            return GetHighlightedTokenString(line, token) + "\n" + message;
        }

        private static string GetHighlightedTokenString(string line, Token token)
        {
            var startString = line.Substring(0, token.Position);
            var tokenString = line.Substring(token.Position, token.Length);
            var endString = line.Substring(token.Position + token.Length);
            if (tokenString == String.Empty)
            {
                tokenString = "…";
            }
            return String.Format("{0}<b><color=red>{1}</color></b>{2}", startString, tokenString, endString);
        }
    }
}