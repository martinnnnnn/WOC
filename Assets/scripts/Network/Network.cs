using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WOC_Network;
using UnityEngine;
using System.Collections;
using System.Net.Sockets;

namespace WOC
{

    public class ValidationCallback
    {
        public Guid toValidate;
        public delegate bool ValidationDelegate(Guid id, PD_Validate validation);
        public ValidationDelegate Validation;
    }


    public class Network : Singleton<Network>
    {
        public string ip = "127.0.0.1";
        public int port = 8000;

        Account account = new Account();

        ClientSideSession session;

        public Action<Account> OnAccountInfo;
        public Action<string, string> OnChatMessageReceived;
        public Action<List<string>> OnAccountsListUpdated;

        Action<PD_Validate> OnHandleValidate;

        protected void Start()
        {
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

        public void HandleIncoming(string jmessage)
        {
            try
            {
                IPacketData packet = PacketData.FromJson(jmessage);

                if (packet != null)
                {
                    switch (packet)
                    {
                        case PD_Validate data:
                            HandleValidate(data);
                            break;
                        case PD_Info<Account> data:
                            HandleAccountInfo(data);
                            break;
                        case PD_Info<AccountList> data:
                            HandleAccountList(data);
                            break;
                        case PD_Chat data:
                            HandleChatMessage(data);
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Unknow JSON message : " + jmessage);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error while parsing JSON message : " + jmessage);
            }
        }

        List<ValidationCallback> validationCallbaks = new List<ValidationCallback>();

        private void HandleValidate(PD_Validate data)
        {
            validationCallbaks.RemoveAll(callback => callback.Validation(callback.toValidate, data));
        }

        private void HandleAccountInfo(PD_Info<Account> data)
        {
            account = data.info;
            OnAccountInfo?.Invoke(account);
        }

        private void HandleAccountList(PD_Info<AccountList> data)
        {
            OnAccountsListUpdated?.Invoke(data.info.names);
        }

        private void HandleChatMessage(PD_Chat data)
        {
            OnChatMessageReceived?.Invoke(data.senderName, data.message);
        }




        public void TryConnect(string accountname, string password)
        {
            StartCoroutine(TryConnectRoutine(accountname, password));
        }

        public bool OnConnectionValidation(Guid toValidate, PD_Validate validation)
        {
            if (toValidate == validation.validationId)
            {
                if (validation.isValid)
                {
                    //OnHandleValidate -= OnValidation;
                    StartCoroutine(RequestInfoRoutine("account"));
                    StartCoroutine(RequestInfoRoutine("account_list"));
                }
                return true;
            }
            return false;
        }

        //public void OnAccountInfo(PD_Info<Account> data)
        //{
        //    account = data.info;
        //    OnAccountInfo?.Invoke(account);
        //}

        public void SendChatMessage(string message)
        {
            StartCoroutine(SendChatMessageRoutine(message));
        }

        IEnumerator SendChatMessageRoutine(string msg)
        {
            PD_Chat packet = new PD_Chat()
            {
                senderName = account.name,
                message = msg
            };
            string message = PacketData.ToJson(packet);
            var write = WriteAsync(message);
            while (!write.IsCompleted)
            {
                yield return null;
            }
        }

        IEnumerator RequestInfoRoutine(string type)
        {
            PD_InfoRequest packet = new PD_InfoRequest()
            {
                infoType = type
            };
            string message = PacketData.ToJson(packet);
            var write = WriteAsync(message);
            while (!write.IsCompleted)
            {
                yield return null;
            }
        }

        //Guid connectid;
        IEnumerator TryConnectRoutine(string accountname, string password)
        {
            PD_AccountConnect packet = new PD_AccountConnect()
            {
                name = accountname
            };
            //connectid = packet.id;
            string message = PacketData.ToJson(packet);

            //OnHandleValidate += OnValidation;
            validationCallbaks.Add(new ValidationCallback()
            {
                toValidate = packet.id,
                Validation = OnConnectionValidation
            });

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
            TcpClient tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(ip, port);

            session = new ClientSideSession(tcpClient, this);
            var sessionTask = session.StartAsync();

            //session.HandleAccountInfo += OnAccountInfo;
            //session.HandleChatMessage += ChatMessageReceived;
            //session.HandleAccountList += AccountListUpdated;

            try
            {
                await sessionTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}


//network.InfoRequest("account").Wait();
