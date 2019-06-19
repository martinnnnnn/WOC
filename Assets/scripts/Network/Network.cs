using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WOC_Network;
using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using Newtonsoft.Json;

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



        CoroutineQueue coroutineQueue;


        protected void Start()
        {
            coroutineQueue = new CoroutineQueue(this);
            Run();
        }

        Task listenerTask;
        public void Run()
        {
            listenerTask = StartListenerAsync();
        }

        public void Disconnect()
        {
            Debug.Log("Closing stream");

            if (session != null)
            {
                PD_Shutdown packet = new PD_Shutdown();
                string message = PacketData.ToJson(packet);
                WriteAsync(message);
            }

            session?.netstream?.Close();
            session?.client?.Close();
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
                    Debug.Log("Unknow JSON message : " + jmessage);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Error while parsing JSON message : " + jmessage
                    + "\n" + e.Message
                    + "\n" + e.StackTrace);
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
            try
            {
                OnAccountsListUpdated?.Invoke(data.info.names);
            }
            catch(Exception e)
            {
                Debug.Log("Failed on OnAccountsListUpdated");
            }
        }

        private void HandleChatMessage(PD_Chat data)
        {
            OnChatMessageReceived?.Invoke(data.senderName, data.message);
        }

        public void TryCreate(string accountname, string password)
        {
            StartCoroutine(TryCreateRoutine(accountname, password));
        }


        public void TryConnect(string accountname, string password)
        {
            //listenerTask = StartListenerAsync();
            StartCoroutine(TryConnectRoutine(accountname, password));
        }

        public bool OnConnectionValidation(Guid toValidate, PD_Validate validation)
        {
            if (toValidate == validation.validationId)
            {
                if (validation.isValid)
                {
                    StartCoroutine(RequestInfoRoutine("account"));
                    StartCoroutine(RequestInfoRoutine("account_list"));
                }
                else
                {
                    Debug.Log("Connection error : " + validation.errorMessage);
                }
                return true;
            }
            return false;
        }


        public bool OnCreationValidation(Guid toValidate, PD_Validate validation)
        {
            if (toValidate == validation.validationId)
            {
                if (validation.isValid)
                {
                    StartCoroutine(RequestInfoRoutine("account"));
                    StartCoroutine(RequestInfoRoutine("account_list"));
                }
                else
                {
                    Debug.Log("Creation error : " + validation.errorMessage);
                }
                return true;
            }
            return false;
        }

        public void SendChatMessage(string message)
        {
            StartCoroutine(SendChatMessageRoutine(message));
        }

        IEnumerator SendChatMessageRoutine(string msg)
        {
            PD_Chat packet = new PD_Chat
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
            PD_InfoRequest packet = new PD_InfoRequest
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

        IEnumerator TryConnectRoutine(string accountname, string pw)
        {
            PD_AccountConnect packet = new PD_AccountConnect
            {
                name = accountname,
                password = pw
            };
            string message = PacketData.ToJson(packet);

            validationCallbaks.Add(new ValidationCallback
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

        IEnumerator TryCreateRoutine(string accountname, string pw)
        {
            PD_Create<Account> packet = new PD_Create<Account>
            {
                toCreate = new Account
                {
                    name = accountname,
                    password = pw
                }
            };
            string message = PacketData.ToJson(packet);

            validationCallbaks.Add(new ValidationCallback
            {
                toValidate = packet.id,
                Validation = OnCreationValidation
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
                Debug.Log("Failed to AsyncWrite : " + e.Message);
            }
        }

        private void OnApplicationQuit()
        {
            Disconnect();
        }

        private async Task StartListenerAsync()
        {
            TcpClient tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(ip, port);

            session = new ClientSideSession(tcpClient, this);

            var sessionTask = session.StartAsync();

            try
            {
                await sessionTask;
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
            }
            finally
            {
                Disconnect();
            }
        }
    }
}
