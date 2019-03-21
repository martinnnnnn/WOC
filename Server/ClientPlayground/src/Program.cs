using System;
using System.Collections.Generic;


namespace WOC
{
    class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client();
            var clientTask = client.StartListener();

            string userInput = string.Empty;
            while (userInput != "quit")
            {
                userInput = Console.ReadLine();
                client.WriteAsync(userInput).Wait();
            }
            clientTask.Wait();
        }
    }
}
