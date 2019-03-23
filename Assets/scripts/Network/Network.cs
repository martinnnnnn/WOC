using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WOC_Network;
using UnityEngine;
using System.Collections;
using System.Net.Sockets;

namespace WOC
{

    class Network : Singleton<Network>
    {
        public string ip;
        public int port;

        Account account = new Account();

        //NetworkClient network;
        ClientSideSession session;
        TcpClient tcpClient;

        public Action<Account> ConnectCompleted;
        
        protected void Start()
        {
            //network = new NetworkClient();
            tcpClient = new TcpClient();
            Run();
        }

        Task listenerTask;
        public void Run()
        {
            listenerTask = StartListenerAsync();

        }

        public void Shutdown()
        {

        }

        public void TryConnect(string accountname, string password)
        {
            StartCoroutine(TryConnectRoutine(accountname, password));
        }

        public void OnValidation(PD_Validate validation)
        {
            if (connectid == validation.validationId)
            {
                if (validation.isValid)
                {
                    session.HandleValidate -= OnValidation;
                    StartCoroutine(RequestInfoRoutine("account"));
                }
            }
        }

        public void OnAccountInfo(PD_Info<Account> data)
        {
            account = data.info;
            session.HandleAccountInfo -= OnAccountInfo;
            ConnectCompleted?.Invoke(account);
        }

        IEnumerator RequestInfoRoutine(string type)
        {
            PD_InfoRequest packet = new PD_InfoRequest()
            {
                infoType = type
            };
            string message = PacketData.ToJson(packet);
            session.HandleAccountInfo += OnAccountInfo;
            var write = WriteAsync(message);
            while (!write.IsCompleted)
            {
                yield return null;
            }
        }

        Guid connectid;
        IEnumerator TryConnectRoutine(string accountname, string password)
        {
            PD_AccountConnect packet = new PD_AccountConnect()
            {
                name = accountname
            };
            connectid = packet.id;
            string message = PacketData.ToJson(packet);
            session.HandleValidate += OnValidation;
            var write = WriteAsync(message);
            while (!write.IsCompleted)
            {
                yield return null;
            }
        }

        private async Task WriteAsync(string message)
        {
            try
            {
                await session.SendAsync(message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to AsyncWrite : {0}", e.Message);
            }
        }

        private async Task StartListenerAsync()
        {
            await tcpClient.ConnectAsync(ip, port);
            session = new ClientSideSession(tcpClient);
            var sessionTask = session.StartAsync();

            try
            {
                await sessionTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //    public async Task StartListener()
        //    {
        //        string host = "127.0.0.1";
        //        int port = 8000;

        //        TcpClient tcpClient = new TcpClient();

        //        await tcpClient.ConnectAsync(host, port);
        //        await StartHandleConnectionAsync(tcpClient);
        //    }

        //    private async Task StartHandleConnectionAsync(TcpClient tcpClient)
        //    {
        //        session = new ClientSideSession(tcpClient);
        //        var sessionTask = session.StartAsync();

        //        try
        //        {
        //            await sessionTask;
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.ToString());
        //        }
        //    }
    }
}


//network.InfoRequest("account").Wait();
