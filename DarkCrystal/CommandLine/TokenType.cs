
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace DarkCrystal.CommandLine
{
    public enum TokenType
    {
        Invalid,
        Value,
        Identifier,
        Dot,
        OpenBracket,
        ClosedBracket,
        Comma,
        Operator,
        AssignmentOperator,
        EndOfLine,
        Autocomplete
    }
}