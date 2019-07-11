using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WOC_Core;


namespace WOC_Server
{
    public class BattleRoom
    {
        public string Name;
        public Battle battle;

        TCPServer server;
        SynchronizedCollection<ServerSession> sessions = new SynchronizedCollection<ServerSession>();

        public void Add(ServerSession session)
        {
            if (!locked)
            {
                sessions.Add(session);
            }
        }
        public void Clear() => sessions.Clear();
        public void Remove(ServerSession s) => sessions.Remove(s);

        public void ForEach(Action<ServerSession> p) => sessions.ToList().ForEach(p);

        public List<string> PlayerList { get => sessions.Select(s => s.Name).ToList(); }

        public BattleRoom(string name, TCPServer server, int randomSeed)
        {
            Name = name;
            this.server = server;

            LOG.Print("[ROOM] Battle initialization...");
            battle = new Battle(randomSeed);
            battle.OnBattleEnd += BattleOver;

            Initiative.Max = 50;
            Hand.Max = 3;

            // CARDS
            List<Card> cardsMap = new List<Card>()
            {
                // name | mana cost | exhaust | effects list
                new Card("smol_dmg", 1, false, new List<CardEffect>
                {
                    new CardEffectDamage(1)
                }),
                new Card("hek", 2, false, new List<CardEffect>
                {
                    new CardEffectHeal(2)
                }),
                new Card("big_dmg", 3, false, new List<CardEffect>
                {
                    new CardEffectDamage(10)
                })
            };
            LOG.Print("[ROOM] Adding cards");
            cardsMap.ForEach(c => battle.Add(c));

            //PNJS
            List<Actor> actors = new List<Actor>()
            {
                // battle | character | name | first init
                new PNJActor(battle, new Character(Character.Race.OGRE, Character.Category.BARBARIAN, 20), "monstre1", 5),
                new PNJActor(battle, new Character(Character.Race.OGRE, Character.Category.BARBARIAN, 20), "monstre2", 5),
                new PNJActor(battle, new Character(Character.Race.OGRE, Character.Category.CHAMAN, 15), "monstre3", 5)
            };
            LOG.Print("[ROOM] Adding PNJs");
            actors.ForEach(a => battle.Add(a));
        }

        bool locked = false;
        public bool Start()
        {
            locked = true;
            return battle.Start();
        }

        void BattleOver()
        {
            LOG.Print("Battle over !!!\n Moving sessions back to the main server.");
            ForEach(s => server.sessions.Add(s));
            sessions.Clear();
            server.battleRooms.Remove(this);
        }

        public async Task Broadcast(string msg, Session toIgnore = null)
        {
            List<Task> tasks = new List<Task>();

            foreach (Session session in sessions)
            {
                if (toIgnore == null || session != toIgnore)
                {
                    tasks.Add(session.SendAsync(msg));
                }
            }
            await Task.WhenAll(tasks);
        }

        public async Task Broadcast(IPacketData data, Session toIgnore = null)
        {
            await Broadcast(Serialization.ToJson(data), toIgnore);
        }
    }
}