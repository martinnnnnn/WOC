using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public List<string> friends = new List<string>();


        public bool connected = false;
        public PlayerActor actor = null;
        public Character defaultCharacter = null;
        public List<Character> characters = new List<Character>();

        public Session session = null;
        // game history stats

        public void SetDefaultCharacter(string name)
        {
            defaultCharacter = characters.Find(c => c.Name == name);
            Debug.Assert(defaultCharacter != null);
        }

    }
}
