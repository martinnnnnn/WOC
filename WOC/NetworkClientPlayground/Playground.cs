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



    class Playground
    {

        static ClientSession session = new ClientSession();

        static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            session.Connect("127.0.0.1", 54001);

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
                        session.SendAsync(new PD_NameModify { oldName = session.Name, newName = input[1] }).Wait();
                        session.Name = input[1];
                        break;
                    case "connect":
                        session.Connect("127.0.0.1", 54001);
                        break;
                    case "close":
                        session.SendClose().Wait();
                        session.Close();
                        break;
                    case "room_create":
                        session.SendAsync(new PD_RoomCreate { name = input[1] }).Wait();
                        break;
                    case "room_join":
                        session.SendAsync(new PD_RoomJoin { playerName = session.Name, roomName = input[1] }).Wait();
                        break;
                    case "room_leave":
                        session.SendAsync(new PD_RoomLeave { name = session.Name }).Wait();
                        break;
                    case "room_list":
                        session.SendAsync(new PD_RoomList { }).Wait();
                        break;
                    case "player_list":
                        session.SendAsync(new PD_PlayerList { roomName = (input.Length > 1) ? input[1] : "" }).Wait();
                        break;
                    case "battle_start":
                        if (session.battle.Start())
                        {
                            if (session.battle.GetCurrentActor() == session.actor)
                            {
                                LOG.Print("[PLAYGROUND] It's my turn !");
                            }
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
                            if (session.battle.GetCurrentActor() == session.actor)
                            {
                                LOG.Print("[PLAYGROUND] It's my turn !");
                            }
                            session.SendAsync(new PD_TurnEnd()).Wait();
                        }
                        break;
                    case "add_1":
                        session.AddActor_1();
                        break;
                    case "add_2":
                        session.AddActor_2();
                        break;
                    case "add_3":
                        session.AddActor_3();
                        break;
                    case "exit":
                        exit = true;
                        break;
                    default:
                        session.SendAsync(new PD_Chat
                        {
                            senderName = session.Name,
                            message = string.Concat(input)
                        }).Wait();
                        break;
                }
            }
        }

        static void Help()
        {
            LOG.Print("> help" + "\n" +
                    "> name=" + "\n" +
                    "> connect" + "\n" +
                    "> close" + "\n" +
                    "> battle_start" + "\n" +
                    "> room_create=" + "\n" +
                    "> room_join=" + "\n" +
                    "> room_leave" + "\n" +
                    "> room_list" + "\n" +
                    "> player_list=" + "\n" +
                    "> playcard" + "\n" +
                    "> endturn" + "\n" +
                    "> add_1" + "\n" +
                    "> add_2" + "\n" +
                    "> add_3" + "\n" +
                    "> exit" + "\n");
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
                Console.Write("> Hand : {1}\n> What's your move (-1 to cancel)? ", string.Join(", ", actor.hand.AsArray().Select(c => c.name).ToArray()));
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

//using CommandLine;
//using CommandLine.Text;
//using System.Collections.Generic;
//using System;
//using System.IO;
//using System.Linq;
//using System.Text;

//namespace ReadText.Demo
//{
//    interface IOptions
//    {
//        [Option('n', "lines",
//            Default = 5U,
//            SetName = "bylines",
//            HelpText = "Lines to be printed from the beginning or end of the file.")]
//        uint? Lines { get; set; }

//        [Option('c', "bytes",
//            SetName = "bybytes",
//            HelpText = "Bytes to be printed from the beginning or end of the file.")]
//        uint? Bytes { get; set; }

//        [Option('q', "quiet",
//            HelpText = "Suppresses summary messages.")]
//        bool Quiet { get; set; }

//        [Value(0, MetaName = "input file",
//            HelpText = "Input file to be processed.",
//            Required = true)]
//        string FileName { get; set; }
//    }

//    [Verb("head", HelpText = "Displays first lines of a file.")]
//    class HeadOptions : IOptions
//    {
//        public uint? Lines { get; set; }

//        public uint? Bytes { get; set; }

//        public bool Quiet { get; set; }

//        public string FileName { get; set; }

//        [Usage(ApplicationAlias = "ReadText.Demo.exe")]
//        public static IEnumerable<Example> Examples
//        {
//            get
//            {
//                yield return new Example("normal scenario", new HeadOptions { FileName = "file.bin" });
//                yield return new Example("specify bytes", new HeadOptions { FileName = "file.bin", Bytes = 100 });
//                yield return new Example("suppress summary", UnParserSettings.WithGroupSwitchesOnly(), new HeadOptions { FileName = "file.bin", Quiet = true });
//                yield return new Example("read more lines", new[] { UnParserSettings.WithGroupSwitchesOnly(), UnParserSettings.WithUseEqualTokenOnly() }, new HeadOptions { FileName = "file.bin", Lines = 10 });
//            }
//        }
//    }

//    [Verb("tail", HelpText = "Displays last lines of a file.")]
//    class TailOptions : IOptions
//    {
//        public uint? Lines { get; set; }

//        public uint? Bytes { get; set; }

//        public bool Quiet { get; set; }

//        public string FileName { get; set; }
//    }

//    class Program
//    {
//        public static int Main(string[] args)
//        {
//            Func<IOptions, string> reader = opts =>
//            {
//                var fromTop = opts.GetType() == typeof(HeadOptions);
//                return opts.Lines.HasValue
//                    ? ReadLines(opts.FileName, fromTop, (int)opts.Lines)
//                    : ReadBytes(opts.FileName, fromTop, (int)opts.Bytes);
//            };
//            Func<IOptions, string> header = opts =>
//            {
//                if (opts.Quiet)
//                {
//                    return string.Empty;
//                }
//                var fromTop = opts.GetType() == typeof(HeadOptions);
//                var builder = new StringBuilder("Reading ");
//                builder = opts.Lines.HasValue
//                    ? builder.Append(opts.Lines).Append(" lines")
//                    : builder.Append(opts.Bytes).Append(" bytes");
//                builder = fromTop ? builder.Append(" from top:") : builder.Append(" from bottom:");
//                return builder.ToString();
//            };
//            Action<string> printIfNotEmpty = text =>
//            {
//                if (text.Length == 0) { return; }
//                Console.WriteLine(text);
//            };

//            var result = Parser.Default.ParseArguments<HeadOptions, TailOptions>(args);
//            var texts = result
//                .MapResult(
//                    (HeadOptions opts) => Tuple.Create(header(opts), reader(opts)),
//                    (TailOptions opts) => Tuple.Create(header(opts), reader(opts)),
//                    _ => MakeError());

//            printIfNotEmpty(texts.Item1);
//            printIfNotEmpty(texts.Item2);

//            return texts.Equals(MakeError()) ? 1 : 0;
//        }

//        private static string ReadLines(string fileName, bool fromTop, int count)
//        {
//            var lines = File.ReadAllLines(fileName);
//            if (fromTop)
//            {
//                return string.Join(Environment.NewLine, lines.Take(count));
//            }
//            return string.Join(Environment.NewLine, lines.Reverse().Take(count));
//        }

//        private static string ReadBytes(string fileName, bool fromTop, int count)
//        {
//            var bytes = File.ReadAllBytes(fileName);
//            if (fromTop)
//            {
//                return Encoding.UTF8.GetString(bytes, 0, count);
//            }
//            return Encoding.UTF8.GetString(bytes, bytes.Length - count, count);
//        }

//        private static Tuple<string, string> MakeError()
//        {
//            return Tuple.Create("\0", "\0");
//        }
//    }
//}
