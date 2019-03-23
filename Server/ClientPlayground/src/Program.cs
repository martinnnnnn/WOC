using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WOC_Network;

namespace WOC
{

    class Game
    {
        NetworkClient network;
        Account account;

        public void Start()
        {
            network = new NetworkClient();
        }
        
        public void Run()
        {
            var nettask = network.StartListener();

            //network.AccountConnect("martin").Wait();
            network.InfoRequest("account").Wait();

            //string userInput = string.Empty;
            //while (userInput != "quit")
            //{
            //    if (userInput.StartsWith("acc create"))
            //    {
            //        var hello = AccountCreate(userInput.Split(' ')[2]);
            //        if (hello.IsCompleted)
            //        {

            //        }
            //    }

            //    Console.Write(">");
            //    userInput = Console.ReadLine();
            //}

            nettask.Wait();
        }
        
        async Task AccountCreate(string name)
        {
            await network.AccountCreate(name);

        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.Start();
            game.Run();

            //network = new NetworkClient();
            //var nettask = network.StartListener();

            //// application logic
            //AccountCreate().Wait();

            //nettask.Wait();


            //Client client = new Client();
            //var clientTask = client.StartListener();

            //string userInput = string.Empty;
            //while (userInput != "quit")
            //{
            //    userInput = Console.ReadLine();
            //    client.WriteAsync(userInput).Wait();
            //}
            //clientTask.Wait();
        }

        //static async Task AccountCreate()
        //{
        //    Guid packetId = await network.AccountCreate("martin");

        //}
    }
}
