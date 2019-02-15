
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace DarkCrystal.Sample
{
    public static class CommandLineExtension
    {
        public static bool Draw(this CommandLine.CommandLine commandLine, ref string line, ref string outputText, Type expectedResultType = null)
        {
            int inputControllerID = GUIUtility.GetControlID(FocusType.Passive) + 1;
            if (DrawTextField(ref line))
            {
                var keyCode = Event.current.keyCode;
                if (keyCode != KeyCode.Backspace && keyCode != KeyCode.Delete)
                {
                    commandLine.Autocomplete(ref line, inputControllerID);
                }
                commandLine.Validate(line, out outputText, expectedResultType);
                return true;
            }
            return false;
        }

        public static void Autocomplete(this CommandLine.CommandLine commandLine, ref string line, int inputControllerID)
        {
            var editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), inputControllerID);

            var minIndex = Math.Min(editor.selectIndex, editor.cursorIndex);
            var maxIndex = Math.Max(editor.selectIndex, editor.cursorIndex);

            var startingText = line.Substring(0, editor.selectIndex);
            var endingText = line.Substring(maxIndex);
            var completedText = commandLine.AutoComplete(startingText) ?? String.Empty;

            line = startingText + completedText + endingText;
            editor.text = line;
            editor.cursorIndex = startingText.Length;
            editor.selectIndex = startingText.Length + completedText.Length;
        }

        public static bool DrawTextField(ref string value, params GUILayoutOption[] options)
        {
            var newValue = GUILayout.TextField(value, options);
            if (value != newValue)
            {
                value = newValue;
                return true;
            }
            return false;
        }
    }
}