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
        static List<Card> cards;
        static void Main(string[] args)
        {
            InitCards();
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            bool exit = false;

            var udpClient = new UdpClient();
            udpClient.Client.ReceiveTimeout = 10000;
            var RequestData = Encoding.ASCII.GetBytes(Serialization.ToJson(new PD_Discovery { }));
            var ServerEp = new IPEndPoint(IPAddress.Any, 0);
            string serverIp = "";
            bool serverFound = false;
            udpClient.EnableBroadcast = true;
            while (!serverFound)
            {
                try
                {
                    Console.WriteLine("[DISCOVERY] Looking for a server...");
                    udpClient.Send(RequestData, RequestData.Length, new IPEndPoint(IPAddress.Broadcast, 8888));

                    var ServerResponseData = udpClient.Receive(ref ServerEp);
                    var ServerResponse = Encoding.ASCII.GetString(ServerResponseData);
                    serverIp = ServerEp.Address.ToString();

                    PD_Discovery data = Serialization.FromJson<PD_Discovery>(ServerResponse);
                
                    Console.WriteLine("[DISCOVERY] Found a server : {0}", serverIp);
                    udpClient.Close();
                    serverFound = true;
                }
                catch (Exception)
                {
                    StackTrace stackTrace = new StackTrace();
                    Console.WriteLine("[DISCOVERY] Coudln't find a server.");
                    Thread.Sleep(2000);
                }
            }

            session.Connect(serverIp, 54001);

            Dictionary<string[], Action<string[]>> commands = new Dictionary<string[], Action<string[]>>(new StringArrayComparer())
            {
                // HELP & EXIT
                { new string[1] { "help" }, (arg) => Help() },
                { new string[1] { "exit" }, (arg) => exit = true },

                { new string[1] { "/" }, (arg) => Chat(arg, PD_ServerChat.Type.LOCAL) },
                { new string[1] { "/f" }, (arg) => Chat(arg, PD_ServerChat.Type.FRIENDS) },
                { new string[1] { "/all" }, (arg) => Chat(arg, PD_ServerChat.Type.GLOBAL) },
                { new string[1] { "info" }, (arg) => Info(arg) },

                //PD_UserMake
                { new string[2] { "account", "make" }, (arg) => AccountMake(arg) },
                { new string[2] { "account", "delete" }, (arg) => AccountDelete(arg) },
                { new string[2] { "account", "modify" }, (arg) => AccountModify(arg) },
                { new string[2] { "account", "connect" }, (arg) => AccountConnect(arg) },
                { new string[2] { "account", "disconnect" }, (arg) => AccountDisconnect(arg) },
                { new string[2] { "account", "list" }, (arg) => AccountList(arg) },
                //TODO need to retrieve account info on login

                { new string[2] { "friend", "add" }, (arg) => AddFriend(arg) },
                { new string[2] { "friend", "remove" }, (arg) => RemoveFriend(arg) },
                { new string[2] { "friend", "list" }, (arg) => String.Join(", ", session.account.friends) },

                { new string[2] { "character", "add" }, (arg) => CharacterAdd(arg) },
                { new string[2] { "character", "modify" }, (arg) => CharacterModify(arg) },
                { new string[2] { "character", "delete" }, (arg) => CharacterDelete(arg) },
                { new string[2] { "character", "default" }, (arg) => CharacterSetDefault(arg) },

                { new string[2] { "deck", "new" }, (arg) => DeckNew(arg) },
                { new string[2] { "deck", "add" }, (arg) => DeckAdd(arg) },
                { new string[2] { "deck", "rename" }, (arg) => DeckRename(arg) },
                { new string[2] { "deck", "delete" }, (arg) => DeckDelete(arg) },
                { new string[2] { "deck", "default" }, (arg) => DeckSetDefault(arg) },

                //// NAME
                //{ new string[1] { "name" }, (arg) => Console.WriteLine("[CLIENT] Your name is {0}.", session.Name) },
                //{ new string[2] { "name", "change" }, (arg) =>
                //    {
                //        if (arg.Length == 0)
                //        {
                //            Console.WriteLine("[PLAYGROUND] You need to specify a name.");
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
                //{ new string[2] { "room", "delete" }, (arg) => Console.WriteLine("[CLIENT] Room deletion not supported yet.") },

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
                //        Console.WriteLine("[PLAYGROUND] You are not in a room ; starting a battle is not possible.");
                //        return;
                //    }

                //    if (session.room.battle.Start())
                //    {
                //        if (session.room.battle.GetCurrentActor() == session.room.actor)
                //        {
                //            Console.WriteLine("[PLAYGROUND] It's my turn !");
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
                //        Console.WriteLine("[PLAYGROUND] No character with this name.");
                //        return;
                //    }
                //    if (session.room == null)
                //    {
                //        Console.WriteLine("[PLAYGROUND] You're not in a room yet.");
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
                //            Console.WriteLine("[PLAYGROUND] It's my turn !");
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
                        Console.WriteLine("[PLAYGROUND] Invalid command");
                    }
                }
            }
        }
        static void SendWithValidation(IPacketData data, bool force = false)
        {
            Debug.Assert(session.awaitingValidations.TryAdd(data.id, data));
            session.Send(data, force);
        }

        static void Info(string[] args)
        {
            switch (args[0])
            {
                case "account":
                    Console.WriteLine("Email : {0} | Name : {1}\nStatus : {2}\nFriends count : {3} | Characters count : {4} | Decks count : {5}",
                        session.account.email, session.account.name,
                        session.account.connected ? "Online" : "Offline",
                        session.account.friends.Count, session.account.characters.Count, session.account.decks.Count);
                    break;
                case "friends":
                    Console.WriteLine("Friends list : {0}", string.Join(", ", session.account.friends));
                    break;
                case "characters":
                    string[] charaInfo = new string[session.account.characters.Count];
                    int index = 0;
                    session.account.characters.ForEach(c =>
                    {
                        charaInfo[index] = string.Format("{0} | {1} | {2} | {3}", c.Name, c.race.ToString(), c.category.ToString(), c.Life);
                        index++;
                    });
                    Console.WriteLine("Default Character : {0}", session.account.defaultCharacter?.Name);
                    Console.WriteLine("Characters list :\n{0}", string.Join("\n", charaInfo));
                    break;
                case "decks":
                    string[] deckInfo = new string[session.account.decks.Count];
                    int indx = 0;
                    session.account.decks.ForEach(d =>
                    {
                        deckInfo[indx] = string.Format("{0} -> {1}", d.name, string.Format(", ", d.cardNames));
                        indx++;
                    });
                    Console.WriteLine("Default Deck : {0}", session.account.defaultDeck?.name);
                    Console.WriteLine("Decks list :\n{0}", string.Join("\n", deckInfo));
                    break;
            }
        }

        
        //name
        //race
        //category
        //life


        static void AccountMake(string[] args)
        {
            if (session.account != null) return;

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

            SendWithValidation(new PD_AccountMake { email = accEmail, password = accPassord, name = accName }, true);
        }


        static void AccountDelete(string[] args)
        {
            if (!AssureConnected()) return;

            SendWithValidation(new PD_AccountDelete { email = session.account.email, password = session.account.password });
        }

        static void AccountModify(string[] args)
        {
            if (!AssureConnected()) return;

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

            SendWithValidation(new PD_AccountModify
            {
                oldEmail = session.account.email,
                oldPassword = session.account.password,
                oldName = session.account.name,
                newEmail = accEmail,
                newPassword = accPassord,
                newName = accName
            });
        }

        static void AccountConnect(string[] args)
        {
            if (session.account != null)
            {
                SendWithValidation(new PD_AccountConnect { email = session.account.email, password = session.account.password }, true);
            }
            else
            {
                Console.WriteLine("[PLAYGROUND] You need to create a account first.");
            }
        }

        static void AccountDisconnect(string[] args)
        {
            if (!AssureConnected()) return;

            SendWithValidation(new PD_AccountDisconnect { email = session.account.email });
        }

        static void AccountList(string[] args)
        {
            if (!AssureConnected()) return;

            Console.WriteLine("Online players : {0}", string.Join(", ", session.onlineAccountNames));
        }

        static void Chat(string[] args, PD_ServerChat.Type t)
        {
            if (!AssureConnected()) return;

            session.Send(new PD_ServerChat
            {
                senderName = session.account.name,
                message = string.Join(" ", args),
                type = t
            });
        }

        static void AddFriend(string[] args)
        {
            if (!AssureConnected()) return;

            if (session.account.friends.Find(f => f == args[0]) == default(string))
            {
                SendWithValidation(new PD_AccountAddFriend { name = args[0] });
            }
            else
            {
                Console.WriteLine("You cannot add a friend twice.");
            }
        }

        static void RemoveFriend(string[] args)
        {
            if (!AssureConnected()) return;

            if (session.account.friends.Find(f => f == args[0]) != default(string))
            {
                SendWithValidation(new PD_AccountRemoveFriend { name = args[0] });
            }
            else
            {
                Console.WriteLine("You cannot delete a friend you don't have.");
            }
        }

        static void CharacterAdd(string[] args)
        {
            if (!AssureConnected()) return;

            string inputName = "default name";
            int inputLife = 10;
            Character.Race inputRace = Character.Race.ELFE;
            Character.Category inputCategory = Character.Category.BARBARIAN;

            foreach (string arg in args)
            {
                string[] parameter = arg.Split('=');
                switch (parameter[0])
                {
                    case "name":
                        inputName = parameter[1];
                        break;
                    case "life":
                        int.TryParse(parameter[1], out inputLife);
                        break;
                    case "race":
                        Enum.TryParse(parameter[1], true, out inputRace);
                        break;
                    case "category":
                        Enum.TryParse(parameter[1], true, out inputCategory);
                        break;
                }
            }

            SendWithValidation(new PD_AccountAddCharacter
            {
                name = inputName,
                race = inputRace,
                category = inputCategory,
                life = inputLife
            });
        }

        static void CharacterModify(string[] args)
        {
            Console.WriteLine("Not supported");
        }

        static void CharacterDelete(string[] args)
        {
            if (!AssureConnected()) return;

            var toRemove = session.account.characters.FirstOrDefault(c => c.Name == args[0]);
            if (toRemove != null)
            {
                SendWithValidation(new PD_AccountDeleteCharacter { name = toRemove.Name });
            }
        }
        
        static void CharacterSetDefault(string[] args)
        {
            if (!AssureConnected()) return;

            var toDefault = session.account.characters.FirstOrDefault(c => c.Name == args[0]);
            if (toDefault != null)
            {
                SendWithValidation(new PD_AccountSetDefaultCharacter { name = toDefault.Name });
            }
        }

        static void DeckNew(string[] args)
        {
            if (!AssureConnected()) return;

            string inputName = args.Length > 0 ? args[0] : "default_deck";

            SendWithValidation(new PD_AccountNewDeck
            {
                name = inputName
            });
        }

        static void DeckAdd(string[] args)
        {
            if (!AssureConnected()) return;
            Debug.Assert(session.account.defaultDeck != null);

            Card card = null;
            string deckName = session.account.defaultDeck.name;

            foreach (string arg in args)
            {
                string[] parameter = arg.Split('=');
                switch (parameter[0])
                {
                    case "card":
                        card = cards.Find(c => c.name == parameter[1]);
                        break;
                    case "deck":
                        deckName = parameter[1];
                        break;
                }
            }

            if (card != null)
            {
                SendWithValidation(new PD_AccountAddCard
                {
                    deckName = deckName,
                    cardName = card.name
                });
            }
        }

        static void DeckRename(string[] args)
        {
            if (!AssureConnected()) return;
            Debug.Assert(session.account.defaultDeck != null);

            string oName = "";
            string nName = "";

            foreach (string arg in args)
            {
                string[] parameter = arg.Split('=');
                switch (parameter[0])
                {
                    case "old":
                        oName = parameter[1];
                        break;
                    case "new":
                        nName = parameter[1];
                        break;
                }
            }

            if (!string.IsNullOrEmpty(oName) && !string.IsNullOrEmpty(oName))
            {
                SendWithValidation(new PD_AccountRenameDeck
                {
                    oldName = oName,
                    newName = nName
                });
            }
        }

        static void DeckDelete(string[] args)
        {
            if (!AssureConnected()) return;

            SendWithValidation(new PD_AccountDeleteDeck
            {
                name = args[0]
            });
        }

        static void DeckSetDefault(string[] args)
        {
            if (!AssureConnected()) return;

            SendWithValidation(new PD_AccountSetDefaultDeck
            {
                name = args[0]
            });
        }

        static public bool AssureConnected()
        {
            if (session.account == null || !session.account.connected)
            {
                Console.WriteLine("You need to be connected to do this.");
                return false;
            }
            return true;
        }

        static void Help()
        {
            Console.WriteLine("> help                                : All the help you need.");
            Console.WriteLine("> exit                                : Exists the game.");
                                                             
            Console.WriteLine("> name                                : Shows your name.");
            Console.WriteLine("> name change <new_name>              : Changes your name to <new_name>.");
                                                             
            Console.WriteLine("> server connect                      : Connects to the server.");
            Console.WriteLine("> server disconnect                   : Disconnects.");

            Console.WriteLine("> player list [optional]<room_name>   : The names of players in room <room_name>, or in the lobby if no room name is specified.");
            Console.WriteLine("> player add 1|2|3                    : Assigns player 1, 2 or 3 to you.");

            Console.WriteLine("> battle start                        : Starts the battle.");
            Console.WriteLine("> battle add pnj                      : Creates a PNJ. parameters : name, race, category, initiative");

            Console.WriteLine("> turn play                           : Play a card.");
            Console.WriteLine("> turn end                            : End your turn.");

            Console.WriteLine("> room make <name>                    : Creates a room with the name <name>.");
            Console.WriteLine("> room join <name>                    : Joins the room called <name>.");
            Console.WriteLine("> room leave                          : Leaves the room.");
            Console.WriteLine("> room delete                         : {NOT IMPLEMENTED} Deletes the room.");
        }

        static void InitCards()
        {
            cards = new List<Card>()
                {
                    // name | mana cost | exhaust | effects list
                    new Card("smol_dmg", 1, false, new List<CardEffect>
                    {
                        new CardEffectDamage(5)
                    }),
                    new Card("hek", 2, false, new List<CardEffect>
                    {
                        new CardEffectHeal(2)
                    }),
                    new Card("big_dmg", 3, false, new List<CardEffect>
                    {
                        new CardEffectDamage(10)
                    }),
                    new Card("smol_dmg2", 1, false, new List<CardEffect>
                    {
                        new CardEffectDamage(5)
                    }),
                    new Card("hek2", 2, false, new List<CardEffect>
                    {
                        new CardEffectHeal(2)
                    }),
                    new Card("big_dmg2", 3, false, new List<CardEffect>
                    {
                        new CardEffectDamage(10)
                    }),
                        new Card("smol_dmg3", 1, false, new List<CardEffect>
                    {
                        new CardEffectDamage(5)
                    }),
                    new Card("hek3", 2, false, new List<CardEffect>
                    {
                        new CardEffectHeal(2)
                    }),
                    new Card("big_dmg3", 3, false, new List<CardEffect>
                    {
                        new CardEffectDamage(10)
                    }),
                    new Card("smol_dmg4", 1, false, new List<CardEffect>
                    {
                        new CardEffectDamage(5)
                    }),
                    new Card("hek4", 2, false, new List<CardEffect>
                    {
                        new CardEffectHeal(2)
                    }),
                    new Card("big_dmg4", 3, false, new List<CardEffect>
                    {
                        new CardEffectDamage(10)
                    }),
                    new Card("smol_dmg5", 1, false, new List<CardEffect>
                    {
                        new CardEffectDamage(5)
                    }),
                    new Card("hek5", 2, false, new List<CardEffect>
                    {
                        new CardEffectHeal(2)
                    }),
                    new Card("big_dmg5", 3, false, new List<CardEffect>
                    {
                        new CardEffectDamage(10)
                    }),
                    new Card("smol_dmg6", 1, false, new List<CardEffect>
                    {
                        new CardEffectDamage(5)
                    }),
                    new Card("hek6", 2, false, new List<CardEffect>
                    {
                        new CardEffectHeal(2)
                    }),
                    new Card("big_dmg6", 3, false, new List<CardEffect>
                    {
                        new CardEffectDamage(10)
                    }),
                    new Card("smol_dmg7", 1, false, new List<CardEffect>
                    {
                        new CardEffectDamage(5)
                    }),
                    new Card("hek7", 2, false, new List<CardEffect>
                    {
                        new CardEffectHeal(2)
                    }),
                    new Card("big_dmg7", 3, false, new List<CardEffect>
                    {
                        new CardEffectDamage(10)
                    }),
                };
            Card.Init(cards); 
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
        //        Console.WriteLine("[PLAYGROUND] You need to be in a room to do that");
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
        //        Console.WriteLine("[PLAYGROUND] A PNJ with this name already exists.");
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
        //        Console.WriteLine("[PLAYGROUND] You need to be in a room to do that.");
        //        return;
        //    }

        //    if (session.room.battle.HasStarted)
        //    {
        //        Console.WriteLine("[PLAYGROUND] The battle has not started yet.");
        //        return;
        //    }

        //    Battle battle = session.room.battle;
        //    PlayerActor actor = session.room.actor;
        //    Card card;
        //    if (actor != battle.GetCurrentActor())
        //    {
        //        Console.WriteLine("[BATTLE] It's not your turn !");
        //        return;
        //    }
        //    if (actor.hand.Count == 0)
        //    {
        //        Console.WriteLine("[BATTLE] No more cards left !");
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
        //            Console.WriteLine("You need to give me a valid index !");
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
        //            Console.WriteLine("You need to give me a valid index !");
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


