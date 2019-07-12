using System;
using System.Collections;
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
    sealed class StringArrayComparer : EqualityComparer<string[]>
    {
        public override bool Equals(string[] x, string[] y)
          => StructuralComparisons.StructuralEqualityComparer.Equals(x, y);

        public override int GetHashCode(string[] x)
          => StructuralComparisons.StructuralEqualityComparer.GetHashCode(x);
    }


    class Playground
    {

        static ClientSession session = new ClientSession();

        static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            bool exit = false;

            session.Connect("127.0.0.1", 54001);

            Dictionary<string[], Action<string[]>> commands = new Dictionary<string[], Action<string[]>>(new StringArrayComparer())
            {
                // HELP & EXIT
                { new string[1] { "help" }, (arg) => Help() },
                { new string[1] { "exit" }, (arg) => exit = true },

                // NAME
                { new string[1] { "name" }, (arg) => LOG.Print("[CLIENT] Your name is {0}.", session.Name) },
                { new string[2] { "name", "change" }, (arg) =>
                    {
                        if (arg.Length == 0)
                        {
                            LOG.Print("[PLAYGROUND] You need to specify a name.");
                            return;
                        }
                        session.SendAsync(new PD_NameModify { oldName = session.Name, newName = arg[0] }).Wait();
                        session.Name = arg[0];
                    }
                },

                // SERVER
                { new string[2] { "server", "connect" }, (arg) => session.Connect("127.0.0.1", 54001) },
                { new string[2] { "server", "disconnect" }, (arg) =>
                    {
                        session.SendClose().Wait();
                        session.Close();
                    }
                },

                // ROOM
                { new string[2] { "room", "make" }, (arg) =>
                    {
                        if (arg.Length == 0)
                        {
                            LOG.Print("[PLAYGROUND] You need to specify a name for the room.");
                            return;
                        }
                        session.SendAsync(new PD_RoomCreate { name = arg[0] }).Wait();
                    }
                },
                { new string[2] { "room", "join" }, (arg) =>
                    {
                        if (arg.Length == 0)
                        {
                            LOG.Print("[PLAYGROUND] You need to specify a name for the room.");
                            return;
                        }
                        session.SendAsync(new PD_RoomJoin { playerName = session.Name, roomName = arg[0] }).Wait();
                    }
                },
                { new string[2] { "room", "leave" }, (arg) => session.SendAsync(new PD_RoomLeave { name = session.Name }).Wait() },
                { new string[2] { "room", "list" }, (arg) => session.SendAsync(new PD_RoomList { }).Wait() },
                { new string[2] { "room", "delete" }, (arg) => LOG.Print("[CLIENT] Room deletion not supported yet.") },

                // PLAYER
                { new string[2] { "player", "list" }, (arg) => session.SendAsync(new PD_PlayerList { roomName = (arg.Length > 0) ? arg[0] : "" }).Wait() },
                { new string[2] { "player", "add" }, (arg) =>
                    {
                        if (arg.Length == 0)
                        {
                            LOG.Print("[PLAYGROUND] Which player do you want to add : 1, 2 or 3 ?");
                            return;
                        }
                        switch (arg[0])
                        {
                            case "1":
                                session.AddActor_1();
                                break;
                            case "2":
                                session.AddActor_2();
                                break;
                            case "3":
                                session.AddActor_3();
                                break;
                        }
                    }
                },

                // BATTLE
                { new string[2] { "battle", "start" }, (arg) =>
                    {
                        if (session.battle.Start())
                        {
                            if (session.battle.GetCurrentActor() == session.actor)
                            {
                                LOG.Print("[PLAYGROUND] It's my turn !");
                            }
                            session.SendAsync(new PD_BattleStart()).Wait();
                        }
                    }
                },
                { new string[3] { "battle", "add", "pnj" }, (arg) =>
                    {
                        AddPNJ();
                    }
                },

                // TURN
                { new string[2] { "turn", "play" }, (arg) => PlayCard() },
                { new string[2] { "turn", "end" }, (arg) =>
                    {
                        if (session.actor.EndTurn())
                        {
                            session.battle.NextActor().StartTurn();
                            if (session.battle.GetCurrentActor() == session.actor)
                            {
                                LOG.Print("[PLAYGROUND] It's my turn !");
                            }
                            session.SendAsync(new PD_TurnEnd()).Wait();
                        }
                    }
                },
            };

            while (!exit)
            {
                List<string> input = Console.ReadLine().Split(' ').ToList();
                List<string> arg = new List<string>();

                while (!commands.ContainsKey(input.ToArray()) && input.Count > 0)
                {
                    arg.Add(input.Last());
                    input.RemoveAt(input.Count - 1);
                }
                if (input.Count == 0)
                {
                    arg.Reverse();
                    session.SendAsync(new PD_Chat
                    {
                        senderName = session.Name,
                        message = string.Join(" ", arg)
                    }).Wait();
                }
                else
                {
                    arg.Reverse();
                    commands[input.ToArray()](arg.ToArray());
                }
            }
        }

        static void Help()
        {
            LOG.Print("> help                                : All the help you need.");
            LOG.Print("> exit                                : Exists the game.");
                                                             
            LOG.Print("> name                                : Shows your name.");
            LOG.Print("> name change <new_name>              : Changes your name to <new_name>.");
                                                             
            LOG.Print("> server connect                      : Connects to the server.");
            LOG.Print("> server disconnect                   : Disconnects.");

            LOG.Print("> player list [optional]<room_name>   : The names of players in room <room_name>, or in the lobby if no room name is specified.");
            LOG.Print("> player add 1|2|3                    : Assigns player 1, 2 or 3 to you.");

            LOG.Print("> battle start                        : Starts the battle.");

            LOG.Print("> turn play                           : Play a card.");
            LOG.Print("> turn end                            : End your turn.");

            LOG.Print("> room make <name>                    : Creates a room with the name <name>.");
            LOG.Print("> room join <name>                    : Joins the room called <name>.");
            LOG.Print("> room leave                          : Leaves the room.");
            LOG.Print("> room delete                         : {NOT IMPLEMENTED} Deletes the room.");
        }

        static void AddPNJ()
        {
            //new PNJActor(battle, new Character(Character.Race.OGRE, Character.Category.BARBARIAN, 20), "monstre1", 5),
        }


        static void PlayCard()
        {
            PlayerActor actor = session.actor;
            Card card;
            if (actor != session.battle.GetCurrentActor())
            {
                LOG.Print("[BATTLE] It's not your turn !");
                return;
            }
            if (actor.hand.Count == 0)
            {
                LOG.Print("[BATTLE] No more cards left !");
                return;
            }

            int cardIndex = -1;
            do
            {
                Console.Write("> Hand : {0}\n> What's your move (-1 to cancel)? ", string.Join(", ", actor.hand.AsArray().Select(c => c.name).ToArray()));
                bool result = int.TryParse(Console.ReadLine(), out cardIndex);
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
                Console.Write("> Potential targets : {0}\nWho is your target (-1 to cancel)?", string.Join(", ", session.battle.Actors.Select(a => a.character.Name).ToArray()));
                bool result = int.TryParse(Console.ReadLine(), out targetIndex);
                if (targetIndex == -1)
                {
                    break;
                }
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


