using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WOC_Core
{
    class User
    {
        public string email;
        public string password;
        public string name;

        public PlayerActor actor;
        public List<Character> characters = new List<Character>();

        // game history stats
    }
}
