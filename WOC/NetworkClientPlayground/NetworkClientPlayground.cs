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
    class NetworkClientPlayground
    {
        static ClientSession session = new ClientSession();

        static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

        
            session.Init();
            session.Connect("127.0.0.1", 54001);

            string name = "default";

            bool exit = false;
            while (!exit)
            {
                string[] input = Console.ReadLine().Split('=');
                switch (input[0])
                {
                    case "help":
                        Help();
                        break;
                    case "name":
                        name = input[1];
                        break;
                    case "connect":
                        session.Connect("127.0.0.1", 54001);
                        break;
                    case "close":
                        session.SendClose().Wait();
                        session.Close();
                        break;
                    case "battle_start":
                        if (session.battle.Init())
                        {
                            session.SendAsync(new PD_BattleStart()).Wait();
                        }
                        break;
                    case "playcard":
                        PlayCard();
                        break;
                    case "endturn":
                        if (session.actor.EndTurn())
                        {
                            session.battle.NextActor().StartTurn();
                            session.SendAsync(new PD_TurnEnd()).Wait();
                        }
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
                        session.SendAsync(new PD_Chat
                        {
                            senderName = name,
                            message = string.Concat(input)
                        }).Wait();
                        break;
                }
            }
        }

        static void Help()
        {
            LOG.Print("> help" + "\n" +
                    "> name" + "\n" +
                    "> connect" + "\n" +
                    "> close" + "\n" +
                    "> battle_start" + "\n" +
                    "> playcard" + "\n" +
                    "> endturn" + "\n" +
                    "> add_player_1" + "\n" +
                    "> add_player_2" + "\n" +
                    "> add_player_3" + "\n" +
                    "> exit" + "\n");
        }

        static void PlayCard()
        {
            PlayerActor actor = session.actor;
            Card card;
            if (actor != session.battle.GetCurrentActor())
            {
                return;
            }
            int cardIndex = -1;
            do
            {
                Console.Write("> Hand : {1}\n> What's your move (-1 to end turn)? ", actor.Name, string.Join(",", actor.hand.AsArray().Select(c => c.name).ToArray()));
                cardIndex = int.Parse(Console.ReadLine());
                card = actor.hand.Get(cardIndex);
                if (card == null && cardIndex != -1)
                {
                    LOG.Print("You need to give me a valid index !");
                }
            } while (card == null && cardIndex != -1);

            bool cardPlayed = false;
            int targetIndex = -1;
            while (!cardPlayed)
            {
                Console.Write("> Potential targets : {0}\nWho is your target ?", string.Join(",", session.battle.Actors.Select(a => a.character.Name).ToArray()));
                targetIndex = int.Parse(Console.ReadLine());
                if ((targetIndex < 0 || targetIndex >= session.battle.Actors.Count) && targetIndex != -1)
                {
                    LOG.Print("You need to give me a valid index !");
                }
                else if (targetIndex != -1)
                {
                    Character target = session.battle.Actors[targetIndex].character;
                    cardPlayed = actor.PlayCard(card, session.battle.Actors[targetIndex].character);
                    if (cardPlayed)
                    {
                        session.SendAsync(new PD_CardPlayed
                        {
                            ownerName = actor.Name,
                            targetName = target.Name,
                            cardIndex = cardIndex,
                            cardName = card.name
                        }).Wait();
                    }
                }
            }
        }
    }
}
