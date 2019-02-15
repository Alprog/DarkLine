
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEditor;
using UnityEngine;
using DarkCrystal.CommandLine;

namespace DarkCrystal.Sample
{
    public class CommandLineWindow : EditorWindow
    {
        private const string DescriptionText =
            "Type command. Examples:\n" +
            " Player.HP\n" +
            " Player.HP = 14 + 3 * 2\n" +
            " World.GetEnemy(2).GetPartInfo(PartType.Head)\n\n";
        
        private CommandLine.CommandLine CommandLine;
        private string CommandLineText;
        private string OutputText;

        [MenuItem("Tools/CommandLineWindow")]
        static void Init()
        {
            (EditorWindow.GetWindow(typeof(CommandLineWindow)) as CommandLineWindow).Show();
        }

        private void OnEnable()
        {
            var globalResolver = new DefaultGlobalResolver();
            CommandLine = new CommandLine.CommandLine(globalResolver);
        }

        private void OnGUI()
        {
            GUILayout.Label(DescriptionText);

            CommandLine.Draw(ref CommandLineText, ref OutputText);
            if (OutputText == null)
            {
                OutputText = String.Format("<color=green>{0}</color>\nWell formed", CommandLineText);
            }

            var style = new GUIStyle(UnityEngine.GUI.skin.label);
            style.richText = true;
            GUILayout.Label(OutputText, style);
            if (GUILayout.Button("Run"))
            {
                Run();
            }
        }

        private void Run()
        {
            try
            {
                var result = CommandLine.Execute(CommandLineText);
                OutputText = result?.ToString() ?? "<null>";
            }
            catch (TokenException exception)
            {
                OutputText = exception.Message;
            }
            catch (Exception exception)
            {
                OutputText = exception.Message;
            }
        }
    }
}