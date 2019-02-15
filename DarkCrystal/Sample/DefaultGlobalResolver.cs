
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using DarkCrystal.CommandLine;

namespace DarkCrystal.Sample
{
    public class DefaultGlobalResolver : GlobalObjectResolver
    {
        public DefaultGlobalResolver()
        {
            AddClass("World", typeof(World));
            AddClass("PartType", typeof(PartType));
            AddValueGetter("Player", typeof(Character), () => World.Player);
        }
    }
}