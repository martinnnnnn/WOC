using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WOC_Core;

namespace Playground
{

    public class ClientSession : Session
    {
        public Actor actor;
        public Battle battle;

        public override void HandleIncomingMessage(IPacketData data)
        {
            LOG.Print("[CLIENT] received a packet. {0}", data);
            base.HandleIncomingMessage(data);

            switch (data)
            {
                case PD_Chat chat:
                    LOG.Print(chat.senderName + " : " + chat.message);
                    break;

                case PD_PlayerAdd player:
                    PlayerActor newActor = new PlayerActor(
                        battle,
                        new Character(player.charaRace, player.charaCategory, player.charaLife, player.charaName),
                        new Hand(player.handStartCount, player.handMaxCount),
                        player.name,
                        player.cardsName,
                        player.aggroIncrement,
                        player.manaMax);
                    battle.Add(newActor);
                    break;

                case PD_CardPlayed cardPlayed:
                    PlayerActor owner = battle.Actors.Find(a => cardPlayed.ownerName == a.Name) as PlayerActor;
                    Card card = owner.hand.Get(cardPlayed.cardIndex);
                    Character target = battle.Actors.Find(a => a.character.Name == cardPlayed.targetName).character;

                    LOG.Print("[CLIENT] Found Actor ? {0}", (cardPlayed.ownerName == actor.Name) ? "true" : "false");
                    LOG.Print("[CLIENT] Found Card ? {0}", (card.name == cardPlayed.cardName) ? "true" : "false");
                    LOG.Print("[CLIENT] Found Character ? {0}", (target != null) ? "true" : "false");

                    owner.PlayCard(card, target);
                    break;
            }
        }

        public void Init()
        {
            LOG.Print("[CLIENT] Battle initialization...");
            battle = new Battle();

            Initiative.Max = 50;

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
                    new CardEffectDamage(4)
                })
            };
            LOG.Print("[CLIENT] Adding card");
            cardsMap.ForEach(c => battle.Add(c));

            //PNJS
            List<Actor> actors = new List<Actor>()
            {
                // battle | character | name | first init
                new PNJActor(battle, new Character(Character.Race.OGRE, Character.Category.BARBARIAN, 50), "monstre1", 5),
                new PNJActor(battle, new Character(Character.Race.OGRE, Character.Category.BARBARIAN, 50), "monstre2", 5),
                new PNJActor(battle, new Character(Character.Race.OGRE, Character.Category.CHAMAN, 50), "monstre3", 5)
            };
            actors.ForEach(a => battle.Add(a));
        }

        public void AddActor_1()
        {
            // battle | character(race, category, life, name) | hand | name | deck | aggroIncrement | max mana
            battle.Add(new PlayerActor(battle, new Character(Character.Race.ELFE, Character.Category.DRUID, 12, "grrr"), new Hand(2, 3), "player1", new List<string> { "smol_dmg", "smol_dmg", "smol_dmg", "smol_dmg" }, 2, 30));
            
            SendAsync(new PD_PlayerAdd
            {
                name = "player1",
                charaRace = Character.Race.ELFE,
                charaCategory = Character.Category.DRUID,
                charaLife = 12,
                charaName = "grrr",
                handStartCount = 2,
                handMaxCount = 3,
                cardsName = new List<string> { "smol_dmg", "smol_dmg", "smol_dmg", "smol_dmg" },
                aggroIncrement =2,
                manaMax = 30
            }).Wait();
        }

        public void AddActor_2()
        {
            battle.Add(new PlayerActor(battle, new Character(Character.Race.HUMAN, Character.Category.PALADIN, 12, "gromelo"), new Hand(2, 3), "player2", new List<string> { "hek", "hek", "big_dmg", "big_dmg", "hek" }, 3, 30));

            SendAsync(new PD_PlayerAdd
            {
                name = "player2",
                charaRace = Character.Race.HUMAN,
                charaCategory = Character.Category.PALADIN,
                charaLife = 12,
                charaName = "gromelo",
                handStartCount = 2,
                handMaxCount = 3,
                cardsName = new List<string> { "hek", "hek", "big_dmg", "big_dmg", "hek" },
                aggroIncrement = 3,
                manaMax = 30
            }).Wait();
        }

        public void AddActor_3()
        {
            battle.Add(new PlayerActor(battle, new Character(Character.Race.ELFE, Character.Category.SORCERER, 12, "branigan"), new Hand(2, 3), "player3", new List<string> { "smol_dmg", "smol_dmg", "big_dmg", "big_dmg", "big_dmg", "big_dmg" }, 4, 30));

            SendAsync(new PD_PlayerAdd
            {
                name = "player3",
                charaRace = Character.Race.ELFE,
                charaCategory = Character.Category.SORCERER,
                charaLife = 12,
                charaName = "branigan",
                handStartCount = 2,
                handMaxCount = 3,
                cardsName = new List<string> { "smol_dmg", "smol_dmg", "big_dmg", "big_dmg", "big_dmg", "big_dmg" },
                aggroIncrement = 4,
                manaMax = 30
            }).Wait();
        }
    }

    class NetworkClientPlayground
    {
        static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            ClientSession session = new ClientSession();
            string name = "default";

            bool exit = false;
            while (!exit)
            {
                string input = Console.ReadLine();
                
                switch (input)
                {
                    case "close":
                        session.SendClose().Wait();
                        session.Close();
                        break;
                    case "connect":
                        session.Connect("127.0.0.1", 54001);
                        break;
                    case "init":
                        session.Init();
                        break;
                    case "add_player_1":
                        session.AddActor_1();
                        break;
                    case "add_player_2":
                        session.AddActor_2();
                        break;
                    case "add_player_3":
                        session.AddActor_3();
                        break;
                    case "exit":
                        exit = true;
                        break;
                    default:
                        if (input.StartsWith("name"))
                        {
                            name = input.Split('=')[1];
                        }
                        else
                        {
                            session.SendAsync(new PD_Chat
                            {
                                senderName = name,
                                message = input
                            }).Wait();
                        }
                        break;
                }
            }
        }
    }
}
