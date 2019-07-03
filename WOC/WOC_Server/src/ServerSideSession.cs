//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading.Tasks;
//using WOC_Core;

//namespace WOC_Server
//{
//    public class ServerSideSession : Session
//    {
//        public Server server;
//        public Account account;

//        public ServerSideSession(TcpClient tcpClient, Server tcpServer) : base(tcpClient)
//        {
//            server = tcpServer;
//        }



//        protected override void HandleIncoming(string jmessage)
//        {
//            Console.WriteLine("I'm handling {0}", jmessage);
//            try
//            {
//                IPacketData packet = PacketData.FromJson(jmessage);

//                if (packet != null)
//                {
//                    switch (packet)
//                    {
//                        case PD_InfoRequest data:
//                            HandleInfoRequest(data);
//                            break;
//                        case PD_AccountConnect data:
//                            HandleAccountConnect(data);
//                            break;
//                        case PD_AccountDisconnect data:
//                            HandleAccountDisconnect(data);
//                            break;
//                        case PD_Create<Account> data:
//                            AccountCreate(data);
//                            break;
//                        case PD_Shutdown data:
//                            netstream.Close();
//                            client.Close();
//                            break;
//                        default:
//                            server.HandleIncoming(this, packet);
//                            break;
//                    }
//                }
//                else
//                {
//                    Console.WriteLine("Unknow JSON message : " + jmessage);
//                }
//            }
//            catch (Exception)
//            {
//                Console.WriteLine("Error while parsing JSON message : " + jmessage);
//            }
//        }

//        void HandleInfoRequest(PD_InfoRequest data)
//        {
//            switch(data.infoType)
//            {
//                case "account":
//                {
//                    Console.WriteLine("sending account info");
//                    PD_Info<Account> packet = new PD_Info<Account>()
//                    {
//                        info = this.account
//                    };
//                    string message = PacketData.ToJson(packet);

//                    SendAsync(message).Wait();
//                    break;
//                }
//                case "account_list":
//                {
//                    List<string> accNames = new List<string>();
//                    server.sessions.ForEach(session => accNames.Add(session.account.name));
//                    PD_Info<AccountList> packet = new PD_Info<AccountList>()
//                    {
//                        info = new AccountList()
//                        {
//                            names = accNames
//                        }
//                    };
//                    string message = PacketData.ToJson(packet);

//                    SendAsync(message).Wait();
//                    break;
//                }
                    
//            }
//        }


//        void AccountCreate(PD_Create<Account> data)
//        {
//            string errMessage = string.Empty;

//            string filePath = string.Format("{0}\\{1}\\{2}.json", "", "accounts", data.toCreate.name);
//            if (!File.Exists(filePath) && account == null)
//            {
//                try
//                {
//                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
//                    using (FileStream stream = File.Create(filePath))
//                    {
//                        account = data.toCreate;
//                        var bytesMessage = Encoding.UTF8.GetBytes(Serialization.ToJson(account));
//                        stream.WriteAsync(bytesMessage, 0, bytesMessage.Length).Wait();
//                    }
//                }
//                catch(Exception e)
//                {
//                    Console.WriteLine("Coudln't write new account : " + e.Message);
//                    errMessage = "database_connection_problem";
//                }
//            }
//            else
//            {
//                errMessage = "account_already_exists";
//            }

//            SendValidation(data.id, errMessage).Wait();
//        }

//        void HandleAccountConnect(PD_AccountConnect data)
//        {
//            string errMessage = string.Empty;

//            string filePath = string.Format("{0}/{1}/{2}.json", "", "accounts", data.name);
//            if (File.Exists(filePath))
//            {
//                string jaccount = File.ReadAllText(filePath);
//                account = Serialization.FromJson<Account>(jaccount);

//                if (account.password != data.password)
//                {
//                    errMessage = "wrong_password";
//                }
//            }
//            else
//            {
//                errMessage = "account_not_found";
//            }

//            SendValidation(data.id, errMessage).Wait();
//        }

//        void HandleAccountDisconnect(PD_AccountDisconnect data)
//        {
//            string errorMessage = string.Empty;

//            if (account != null)
//            {
//                string filePath = string.Format("{0}/{1}/{2}.json", "", "accounts", data.name);

//                string jaccount = Serialization.ToJson(account);
//                File.WriteAllText(filePath, jaccount);

//                account = null;
//            }
//            else
//            {
//                errorMessage = "no_account_connected";
//            }

//            SendValidation(data.id, errorMessage).Wait();
//        }
//    }
//}
