
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace DarkCrystal.CommandLine
{
    public class AutoCompleteException : Exception
    {
        public string AutoCompletedText;

        public AutoCompleteException(string autoCompletedText)
        {
            this.AutoCompletedText = autoCompletedText;
        }
    }
}