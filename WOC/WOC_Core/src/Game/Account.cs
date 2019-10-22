using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOC_Core
{
    public class Account
    {
        public string email;
        public string password;
        public string name;

        public bool connected = false;
        public PlayerActor actor = null;
        public List<Character> characters = new List<Character>();

        // game history stats
    }
}
