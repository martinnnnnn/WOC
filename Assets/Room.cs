using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOC_Core;

namespace WOC_Client
{
    public class Room
    {
        public string name;
        public Battle battle;


        public Room(string name, int randomSeed)
        {
            this.name = name;

            Console.WriteLine("[ROOM] Battle construction...");
            battle = new Battle(randomSeed);

            Initiative.Max = 50;
            Hand.Max = 3;

            //PNJS
            List<Actor> actors = new List<Actor>()
            {
                // battle | character | name | first init
                new PNJActor(new Character(Character.Race.OGRE, Character.Category.BARBARIAN, 20), "monstre1", 5),
                new PNJActor(new Character(Character.Race.OGRE, Character.Category.BARBARIAN, 20), "monstre2", 5),
                new PNJActor(new Character(Character.Race.OGRE, Character.Category.CHAMAN, 15), "monstre3", 5)
            };
            Console.WriteLine("[ROOM] Adding PNJs");
            actors.ForEach(a => battle.Add(a));
        }

        public void InitBattle()
        {
            Console.WriteLine("[ROOM] Battle initialization...");
        }
    }
}
