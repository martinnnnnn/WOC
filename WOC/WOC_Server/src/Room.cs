using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WOC_Core;


namespace WOC_Server
{
    public class Room
    {
        public string Name;
        public Battle battle;

        TCPServer server;
        SynchronizedCollection<ServerSession> sessions = new SynchronizedCollection<ServerSession>();

        public bool Add(ServerSession session)
        {
            if (!locked)
            {
                sessions.Add(session);
                return true;
            }
            return false;
        }

        public void Clear() => sessions.Clear();
        public void Remove(ServerSession s) => sessions.Remove(s);

        public void ForEach(Action<ServerSession> p) => sessions.ToList().ForEach(p);

        public List<string> PlayerList { get => sessions.Select(s => s.account.name).ToList(); }

        public Room(string name, TCPServer server, int randomSeed)
        {
            Name = name;
            this.server = server;

            Console.WriteLine("[ROOM] Battle construction...");
            battle = new Battle(randomSeed);
            battle.OnBattleEnd += BattleOver;

            Initiative.Max = 50;
            Hand.Max = 3;

            if (Card.Count() == 0)
            {
                server.cards.ForEach(c => Card.Add(c));
            }

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

            ForEach(s =>
            {
                s.account.actor = new PlayerActor(s.account.name, 5, 20);
                var actor = s.account.actor;
                if (battle.Add(actor))
                {
                    actor.AddCards(s.account.defaultDeck.cardNames);
                }
            });
        }

        bool locked = false;
        public bool Start()
        {
            locked = true;
            return battle.Start();
        }

        void BattleOver()
        {
            Console.WriteLine("Battle over !!!\n Moving sessions back to the main server.");
            ForEach(s => server.sessions.Add(s));
            ForEach(s => s.room = null);
            sessions.Clear();
            server.rooms.Remove(this);
        }

        public void Broadcast(IPacketData data, Session toIgnore = null)
        {
            //List<Task> tasks = new List<Task>();

            //foreach (Session session in sessions)
            //{
            //    if (toIgnore == null || session != toIgnore)
            //    {
            //        tasks.Add(session.SendAsync(data));
            //    }
            //}
            //await Task.WhenAll(tasks);

            foreach (Session session in sessions)
            {
                if (toIgnore == null || session != toIgnore)
                {
                    session.Send(data);
                }
            }
        }
    }
}