using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
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

            var Client = new UdpClient();
            var RequestData = Encoding.ASCII.GetBytes(Serialization.ToJson(new PD_Discovery { }));
            var ServerEp = new IPEndPoint(IPAddress.Any, 0);
            string serverIp = "";

            Client.EnableBroadcast = true;
            try
            {
                Client.Send(RequestData, RequestData.Length, new IPEndPoint(IPAddress.Broadcast, 8888));

                var ServerResponseData = Client.Receive(ref ServerEp);
                var ServerResponse = Encoding.ASCII.GetString(ServerResponseData);
                serverIp = ServerEp.Address.ToString();

                PD_Discovery data = Serialization.FromJson<PD_Discovery>(ServerResponse);
                
                LOG.Print("[DISCOVERY] Found a server : {0}", serverIp);
                Client.Close();
            }
            catch (Exception)
            {
                StackTrace stackTrace = new StackTrace();
                LOG.Print("[DISCOVERY] Coudln't find a server.");
            }

            session.Connect(serverIp, 54001);

            Dictionary<string[], Action<string[]>> commands = new Dictionary<string[], Action<string[]>>(new StringArrayComparer())
            {
                // HELP & EXIT
                { new string[1] { "help" }, (arg) => Help() },
                { new string[1] { "exit" }, (arg) => exit = true },

                { new string[1] { "/" }, (arg) => Chat(arg) },

                //PD_UserMake
                { new string[2] { "account", "make" }, (arg) => AccountMake(arg) },
                { new string[2] { "account", "modify" }, (arg) => AccountModify(arg) },
                { new string[2] { "account", "connect" }, (arg) => AccountConnect(arg) },


                //PD_AccountModify


                //// NAME
                //{ new string[1] { "name" }, (arg) => LOG.Print("[CLIENT] Your name is {0}.", session.Name) },
                //{ new string[2] { "name", "change" }, (arg) =>
                //    {
                //        if (arg.Length == 0)
                //        {
                //            LOG.Print("[PLAYGROUND] You need to specify a name.");
                //            return;
                //        }
                //        session.SendAsync(new PD_NameModify { oldName = session.Name, newName = arg[0] }).Wait();
                //        session.Name = arg[0];
                //    }
                //},

                //// SERVER
                //{ new string[2] { "server", "connect" }, (arg) => session.Connect(serverIp, 54001) },
                //{ new string[2] { "server", "disconnect" }, (arg) =>
                //{
                //    session.SendClose().Wait();
                //    session.Close();
                //}},

                //// ROOM
                //{ new string[2] { "room", "make" }, (arg) =>
                //{
                //    RoomJoin(arg, true);
                //}},
                //{ new string[2] { "room", "join" }, (arg) =>
                //{
                //    RoomJoin(arg, false);

                //}},
                //{ new string[2] { "room", "leave" }, (arg) => session.SendAsync(new PD_RoomLeave { name = session.Name }).Wait() },
                //{ new string[2] { "room", "list" }, (arg) => session.SendAsync(new PD_RoomList { }).Wait() },
                //{ new string[2] { "room", "delete" }, (arg) => LOG.Print("[CLIENT] Room deletion not supported yet.") },

                //// PLAYER
                //{ new string[2] { "player", "list" }, (arg) => session.SendAsync(new PD_PlayerList { roomName = (arg.Length > 0) ? arg[0] : "" }).Wait() },
                //{ new string[2] { "player", "add" }, (arg) =>
                //{
                //    AddPlayer(arg);
                    
                //}},

                //// BATTLE
                //{ new string[2] { "battle", "start" }, (arg) =>
                //{
                //    if (session.room == null)
                //    {
                //        LOG.Print("[PLAYGROUND] You are not in a room ; starting a battle is not possible.");
                //        return;
                //    }

                //    if (session.room.battle.Start())
                //    {
                //        if (session.room.battle.GetCurrentActor() == session.room.actor)
                //        {
                //            LOG.Print("[PLAYGROUND] It's my turn !");
                //        }
                //        session.SendAsync(new PD_BattleStart()).Wait();
                //    }
                //}
                //},
                //{ new string[3] { "battle", "add", "pnj" }, (arg) =>
                //{
                //    AddPNJ(arg);
                //}},

                //{ new string[3] { "battle", "add", "player" }, (arg) =>
                //{
                //    PlayerActor actor = session.actors.Find(a => a.Chara.Name == arg[0]);
                //    if (arg.Length == 0 || actor == null)
                //    {
                //        LOG.Print("[PLAYGROUND] No character with this name.");
                //        return;
                //    }
                //    if (session.room == null)
                //    {
                //        LOG.Print("[PLAYGROUND] You're not in a room yet.");
                //        return;
                //    }

                //    string oldCharacterName = "";
                //    if (session.room.actor !=  null)
                //    {
                //        session.room.battle.Actors.Remove(actor);
                //    }

                //    session.room.actor = actor;
                //    session.room.battle.Add(actor);
                //    session.SendAsync(new PD_BattlePlayerAdd
                //    {
                //        playerName = actor.Name,
                //        characterName = actor.Chara.Name
                //    }).Wait();
                //}},

                //// TURN
                //{ new string[2] { "turn", "play" }, (arg) => PlayCard() },
                //{ new string[2] { "turn", "end" }, (arg) =>
                //{
                //    if (session.room.actor.EndTurn())
                //    {
                //        session.room.battle.NextActor().StartTurn();
                //        if (session.room.battle.GetCurrentActor() == session.room.actor)
                //        {
                //            LOG.Print("[PLAYGROUND] It's my turn !");
                //        }
                //        session.SendAsync(new PD_TurnEnd()).Wait();
                //    }
                //}},
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
                //if (input.Count > 0)
                {
                    arg.Reverse();
                    bool result = commands.TryGetValue(input.ToArray(), out Action<string[]> action);
                    if (result)
                    {
                        action(arg.ToArray());
                    }
                    else
                    {
                        LOG.Print("[PLAYGROUND] Invalid command");
                    }
                }
            }
        }

        static void AccountMake(string[] args)
        {
            string accEmail = "";
            string accPassord = "";
            string accName = "";

            foreach (string arg in args)
            {
                string[] parameter = arg.Split('=');
                switch (parameter[0])
                {
                    case "email":
                        accEmail = parameter[1];
                        break;
                    case "password":
                        accPassord = parameter[1];
                        break;
                    case "name":
                        accName = parameter[1];
                        break;
                }
            }

            session.account = new Account()
            {
                email = accEmail,
                password = accPassord,
                name = accName
            };
            session.SendAsync(new PD_AccountMake { email = accEmail, password = accPassord, name = accName }).Wait();
        }

        static void AccountModify(string[] args)
        {
            string accEmail = "";
            string accPassord = "";
            string accName = "";

            foreach (string arg in args)
            {
                string[] parameter = arg.Split('=');
                switch (parameter[0])
                {
                    case "email":
                        accEmail = parameter[1];
                        break;
                    case "password":
                        accPassord = parameter[1];
                        break;
                    case "name":
                        accName = parameter[1];
                        break;
                }
            }

            session.SendAsync(new PD_AccountModify
            {
                oldEmail = session.account.email,
                oldPassword = session.account.password,
                oldName = session.account.name,
                newEmail = accEmail,
                newPassword = accPassord,
                newName = accName
            }).Wait();

            if (!String.IsNullOrEmpty(accEmail))
            {
                session.account.email = accEmail;
            }
            if (!String.IsNullOrEmpty(accPassord))
            {
                session.account.password = accPassord;
            }
            if (!String.IsNullOrEmpty(accName))
            {
                session.account.name = accName;
            }
        }

        static void AccountConnect(string[] args)
        {
            if (session.account != null)
            {
                session.SendAsync(new PD_AccountConnect { email = session.account.email, password = session.account.password }).Wait();
            }
            else
            {
                LOG.Print("[PLAYGROUND] You need to create a account first.");
            }
        }

        static void Chat(string[] args)
        {
            if (session.account != null && session.account.connected)
            {
                session.SendAsync(new PD_ServerChat
                {
                    senderName = session.account.name,
                    message = string.Join(" ", args)
                }).Wait();
            }
            else
            {
                LOG.Print("You need to be connected to chat.");
            }
        }




        //static void RoomJoin(string[] args, bool create)
        //{
        //    if (args.Length == 0)
        //    {
        //        LOG.Print("[PLAYGROUND] You need to specify a name for the room.");
        //        return;
        //    }

        //    session.SendAsync(new PD_RoomJoin
        //    {
        //        playerName = session.Name,
        //        roomName = args[0],
        //        create = create
        //    }).Wait();
        //}

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
            LOG.Print("> battle add pnj                      : Creates a PNJ. parameters : name, race, category, initiative");

            LOG.Print("> turn play                           : Play a card.");
            LOG.Print("> turn end                            : End your turn.");

            LOG.Print("> room make <name>                    : Creates a room with the name <name>.");
            LOG.Print("> room join <name>                    : Joins the room called <name>.");
            LOG.Print("> room leave                          : Leaves the room.");
            LOG.Print("> room delete                         : {NOT IMPLEMENTED} Deletes the room.");
        }


        //static void AddPlayer(string[] args)
        //{
        //    //character
        //    string charaName = "default name";
        //    int life = 10;
        //    Character.Race race = Character.Race.ELFE;
        //    Character.Category category = Character.Category.BARBARIAN;

        //    int handStartingCount = 2;
        //    int aggroIncrement = 2;
        //    int manaMax = 2;

        //    foreach (string arg in args)
        //    {
        //        string[] parameter = arg.Split('=');
        //        switch (parameter[0])
        //        {
        //            case "name":
        //                charaName = parameter[1];
        //                break;
        //            case "life":
        //                int.TryParse(parameter[1], out life);
        //                break;
        //            case "race":
        //                Enum.TryParse(parameter[1], true, out race);
        //                break;
        //            case "category":
        //                Enum.TryParse(parameter[1], true, out category);
        //                break;
        //            case "hand":
        //                int.TryParse(parameter[1], out handStartingCount);
        //                break;
        //            case "aggro":
        //                int.TryParse(parameter[1], out aggroIncrement);
        //                break;
        //            case "mana":
        //                int.TryParse(parameter[1], out manaMax);
        //                break;
        //        }
        //    }

        //    PlayerActor actor = new PlayerActor(new Character(race, category, life), handStartingCount, session.Name, aggroIncrement, manaMax);
        //    session.actors.Add(actor);

        //    session.SendAsync(new PD_SessionPlayerAdd
        //    {
        //        name = actor.Name,
        //        charaRace = actor.character.race,
        //        charaCategory = actor.character.category,
        //        charaLife = (int)actor.character.Life,
        //        charaName = actor.character.Name,
        //        handStartCount = actor.hand.StartingCount,
        //        cardsName = actor.deck.Select(c => c.name).ToList(),
        //        aggroIncrement = (int)actor.aggro.IncrementRatio,
        //        manaMax = actor.mana.Max
        //    }).Wait();
        //}



        //static void AddPNJ(string[] args)
        //{
        //    if (session.room == null)
        //    {
        //        LOG.Print("[PLAYGROUND] You need to be in a room to do that");
        //        return;
        //    }

        //    string name = "default pnj name";
        //    int life = 10;
        //    Character.Race race = Character.Race.ELFE;
        //    Character.Category category = Character.Category.BARBARIAN;
        //    int initiative = 10;
        //    foreach (string arg in args)
        //    {
        //        string[] parameter = arg.Split('=');
        //        switch (parameter[0])
        //        {
        //            case "name":
        //                name = parameter[1];
        //                break;
        //            case "race":
        //                Enum.TryParse(parameter[1], true, out race);
        //                break;
        //            case "category":
        //                Enum.TryParse(parameter[1], true, out category);
        //                break;
        //            case "life":
        //                int.TryParse(parameter[1], out life);
        //                break;
        //            case "initiative":
        //                int.TryParse(parameter[1], out initiative);
        //                break;
        //        }
        //    }
        //    var pnj = new PNJActor(new Character(race, category, life), name, initiative);

        //    if (!session.room.battle.Add(pnj))
        //    {
        //        LOG.Print("[PLAYGROUND] A PNJ with this name already exists.");
        //        return;
        //    }

        //    session.SendAsync(new PD_PNJAdd
        //    {
        //        name = name,
        //        life = life,
        //        race = race,
        //        category = category,
        //        initiative = initiative
        //    }).Wait();
        //}

        //static void PlayCard()
        //{
        //    if (session.room == null)
        //    {
        //        LOG.Print("[PLAYGROUND] You need to be in a room to do that.");
        //        return;
        //    }

        //    if (session.room.battle.HasStarted)
        //    {
        //        LOG.Print("[PLAYGROUND] The battle has not started yet.");
        //        return;
        //    }

        //    Battle battle = session.room.battle;
        //    PlayerActor actor = session.room.actor;
        //    Card card;
        //    if (actor != battle.GetCurrentActor())
        //    {
        //        LOG.Print("[BATTLE] It's not your turn !");
        //        return;
        //    }
        //    if (actor.hand.Count == 0)
        //    {
        //        LOG.Print("[BATTLE] No more cards left !");
        //        return;
        //    }

        //    int cardIndex = -1;
        //    do
        //    {
        //        Console.Write("> Hand : {0}\n> What's your move (-1 to cancel)? ", string.Join(", ", actor.hand.AsArray().Select(c => c.name).ToArray()));
        //        bool result = int.TryParse(Console.ReadLine(), out cardIndex);
        //        card = actor.hand.Get(cardIndex);
        //        if (card == null && cardIndex != -1)
        //        {
        //            LOG.Print("You need to give me a valid index !");
        //        }
        //    } while (card == null && cardIndex != -1);

        //    bool cardPlayed = false;
        //    int targetIndex = -1;
        //    while (!cardPlayed)
        //    {
        //        Console.Write("> Potential targets : {0}\nWho is your target (-1 to cancel)?", string.Join(", ", battle.Actors.Select(a => a.character.Name).ToArray()));
        //        bool result = int.TryParse(Console.ReadLine(), out targetIndex);
        //        if (targetIndex == -1)
        //        {
        //            break;
        //        }
        //        if ((targetIndex < 0 || targetIndex >= battle.Actors.Count) && targetIndex != -1)
        //        {
        //            LOG.Print("You need to give me a valid index !");
        //        }
        //        else if (targetIndex != -1)
        //        {
        //            Character target = battle.Actors[targetIndex].character;
        //            cardPlayed = actor.PlayCard(card, battle.Actors[targetIndex].character);
        //            if (cardPlayed)
        //            {
        //                session.SendAsync(new PD_CardPlayed
        //                {
        //                    ownerName = actor.Name,
        //                    targetName = target.Name,
        //                    cardIndex = cardIndex,
        //                    cardName = card.name
        //                }).Wait();
        //            }
        //        }
        //    }
        //}
    }
}


