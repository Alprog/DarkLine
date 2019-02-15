
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace DarkCrystal.Sample
{
    public static class World
    {
        public static Character Player = new Character("Player", 100);
        private static Character[] Enemies;
        
        static World()
        {
            Enemies = new Character[]
            {
                new Character("Enemy1", 50),
                new Character("Enemy2", 66),
                new Character("Enemy3", 25)
            };
        }

        public static Character GetEnemy(int index)
        {
            return Enemies[index];
        }
    }
}
