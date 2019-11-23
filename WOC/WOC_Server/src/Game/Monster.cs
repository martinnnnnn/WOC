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
        public double baseTime;
        public int damage;
        public int period;

        public Monster(string name, int life, int damage, int period, int location, double baseTime)
        {
            this.name = name;
            this.life = life;
            this.damage = damage;
            this.period = period;
            this.location = location;
            this.baseTime = baseTime;
        }
    }
}
