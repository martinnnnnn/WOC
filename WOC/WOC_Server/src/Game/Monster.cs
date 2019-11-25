using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using WOC_Core;

namespace WOC_Server
{

    public class Monster : Combatant
    {
        public enum Target
        {
            NONE,
            RANDOM,
            ALL,
            LOCATION
        }

        public struct RoundValues
        {
            public int mana;
            public int damage;
            public Target target;
        }

        public RoundValues[] roundValues;
        public Dictionary<Player, int> currentMana = new Dictionary<Player, int>();
        public int currentDamage;

        public Monster(string name, int life, RoundValues[] roundValues, int location)
        {
            this.name = name;
            this.life = life;
            this.roundValues = roundValues;
            this.location = location;
        }
    }
}
