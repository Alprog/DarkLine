
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace DarkCrystal.CommandLine
{
    public interface IGlobalObjectResolver
    {
        SyntaxObject GetObject(Token token, bool fakeExecution);
        string AutoComplete(string startText);
    }
}