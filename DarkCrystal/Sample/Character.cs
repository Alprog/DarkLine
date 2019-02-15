
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace DarkCrystal.Sample
{
    public class Character
    {
        public string Name;
        public int HP;
        
        public Character(string name, int hp)
        {
            this.Name = name;
            this.HP = hp;
        }

        public string GetPartInfo(PartType partType)
        {
            return String.Format("{0} of {1} character", partType.ToString(), Name);
        }
    }
}
