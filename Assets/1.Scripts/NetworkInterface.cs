using UnityEngine;
using System.Collections.Concurrent;
using System.Collections.Generic;
using WOC_Core;
using WOC_Client;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using System.Threading;

namespace WOC_Client
{
    public class NetworkInterface : MonoBehaviour
    {
        public ClientSession session;

        struct OutPacket
        {
            public IPacketData data;
            public bool force;
            public bool validate;
        }

        ConcurrentQueue<OutPacket> outgoingMessages = new ConcurrentQueue<OutPacket>();
        ConcurrentQueue<IPacketData> incomingMessages = new ConcurrentQueue<IPacketData>();

        public List<string> accountNames = new List<string>();

        private void Start()
        {
            UnitySystemConsoleRedirector.Redirect();
            Connect();
            Callback_InfoOnlineList += HandleAPICall;
            Callback_AccountConnected += HandleAPICall;
            Callback_AccountDisconnected += HandleAPICall;
        }


        void OnApplicationQuit()
        {
            cts.Cancel();
            session.SendClose();
            session.Close();
        }

        CancellationTokenSource cts;
        public void Connect()
        {
            cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
            Task.Run(() => 
            {
                session = new ClientSession(this);
                var udpClient = new UdpClient();
                udpClient.Client.ReceiveTimeout = 3000;
                var RequestData = Encoding.ASCII.GetBytes(Serialization.ToJson(new PD_Discovery { }));
                var ServerEp = new IPEndPoint(IPAddress.Any, 0);
                string serverIp = "";
                bool serverFound = false;
                udpClient.EnableBroadcast = true;

                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();

                while (!serverFound && sw.ElapsedMilliseconds < 1000 * 60 && !token.IsCancellationRequested)
                {
                    try
                    {
                        Console.WriteLine("[DISCOVERY] Looking for a server...");
                        udpClient.Send(RequestData, RequestData.Length, new IPEndPoint(IPAddress.Broadcast, 8888));

                        var ServerResponseData = udpClient.Receive(ref ServerEp);
                        var ServerResponse = Encoding.ASCII.GetString(ServerResponseData);
                        serverIp = ServerEp.Address.ToString();

                        PD_Discovery data = Serialization.FromJson<PD_Discovery>(ServerResponse);

                        Console.WriteLine("[DISCOVERY] Found a server : " + serverIp);
                        udpClient.Close();
                        serverFound = true;
                    }
                    catch (Exception)
                    {
                        System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
                        Console.WriteLine("[DISCOVERY] Coudln't find a server.");
                        Thread.Sleep(2000);
                    }

                }

                if (serverFound)
                {
                    session.Connect(serverIp, 54001);
                }
                else
                {
                    Console.WriteLine("No server found : giving up.");
                }
            });
        }

        private void Update()
        {
            while (incomingMessages.TryDequeue(out IPacketData packet))
            {
                HandleAPICall(packet);
            }

            while (outgoingMessages.TryDequeue(out OutPacket packet))
            {
                if (packet.validate)
                {
                    session.awaitingValidations.TryAdd(packet.data.id, packet.data);
                }
                session.Send(packet.data, packet.force);
            }
        }

        public void SendMessage(IPacketData packet, bool force = false, bool validate = true)
        {
            outgoingMessages.Enqueue(new OutPacket { data = packet, force = force, validate = validate });
        }

        public void ReceiveMessage(IPacketData packet)
        {
            incomingMessages.Enqueue(packet);
        }

        public void HandleAPICall(PD_InfoOnlineList data)
        {
            accountNames.Clear();
            accountNames.AddRange(data.names);
        }

        public void HandleAPICall(PD_AccountConnected data)
        {
            accountNames.Add(data.name);
        }

        public void HandleAPICall(PD_AccountDisconnected data)
        {
            accountNames.Remove(data.name);
        }

        public void HandleAPICall(IPacketData data)
        {
            switch (data)
            {
                case PD_AccountNameModify accountNameModify:
                    Callback_AccountNameModify?.Invoke(accountNameModify);
                    break;
                case PD_ServerChat serverChat:
                    Callback_ServerChat?.Invoke(serverChat);
                    break;
                case PD_AccountConnected accountConnected:
                    Callback_AccountConnected?.Invoke(accountConnected);
                    break;
                case PD_AccountDisconnected accountdisconnected:
                    Callback_AccountDisconnected?.Invoke(accountdisconnected);
                    break;
                case PD_AccountDeleted accountDeleted:
                    Callback_AccountDeleted?.Invoke(accountDeleted);
                    break;
                case PD_AccountMake accountMake:
                    Callback_AccountMake?.Invoke(accountMake);
                    break;
                case PD_AccountModify accountModify:
                    Callback_AccountModify?.Invoke(accountModify);
                    break;
                case PD_AccountDelete accountDelete:
                    Callback_AccountDelete?.Invoke(accountDelete);
                    break;
                case PD_AccountConnect accountConnect:
                    Callback_AccountConnect?.Invoke(accountConnect);
                    break;
                case PD_AccountDisconnect accountDisconnect:
                    Callback_AccountDisconnect?.Invoke(accountDisconnect);
                    break;
                case PD_InfoOnlineList infoOnlineList:
                    Callback_InfoOnlineList?.Invoke(infoOnlineList);
                    break;
                case PD_AccountAddFriend accountAddFriend:
                    Callback_AccountAddFriend?.Invoke(accountAddFriend);
                    break;
                case PD_AccountRemoveFriend accountRemoveFriend:
                    Callback_AccountRemoveFriend?.Invoke(accountRemoveFriend);
                    break;
                case PD_AccountNewDeck accountNewDeck:
                    Callback_AccountNewDeck?.Invoke(accountNewDeck);
                    break;
                case PD_AccountAddCard accountAddCard:
                    Callback_AccountAddCard?.Invoke(accountAddCard);
                    break;
                case PD_AccountRenameDeck accountRenameDeck:
                    Callback_AccountRenameDeck?.Invoke(accountRenameDeck);
                    break;
                case PD_AccountDeleteDeck accountDeleteDeck:
                    Callback_AccountDeleteDeck?.Invoke(accountDeleteDeck);
                    break;
                case PD_AccountSetCurrentDeck accountSetCurrentDeck:
                    Callback_AccountSetCurrentDeck?.Invoke(accountSetCurrentDeck);
                    break;
                case PD_ServerListPlayers serverListPlayers:
                    Callback_ServerListPlayers?.Invoke(serverListPlayers);
                    break;
                case PD_BattleStart battleStart:
                    Callback_BattleStart?.Invoke(battleStart);
                    break;
                case PD_BattleEnd battleEnd:
                    Callback_BattleEnd?.Invoke(battleEnd);
                    break;
                case PD_BattleStatePlayer battleStatePlayer:
                    Callback_BattleStatePlayer?.Invoke(battleStatePlayer);
                    break;
                case PD_BattleStateMainPlayer battleStateMainPlayer:
                    Callback_BattleStateMainPlayer?.Invoke(battleStateMainPlayer);
                    break;
                case PD_BattleStateMonster battleStateMonster:
                    Callback_BattleStateMonster?.Invoke(battleStateMonster);
                    break;
                case PD_BattleState battleState:
                    Callback_BattleState?.Invoke(battleState);
                    break;
                case PD_BattleMonsterTurnStart battleMonsterTurnStart:
                    Callback_BattleMonsterTurnStart?.Invoke(battleMonsterTurnStart);
                    break;
                case PD_BattleMonsterAttack battleMonsterAttack:
                    Callback_BattleMonsterAttack?.Invoke(battleMonsterAttack);
                    break;
                case PD_BattleMonsterTurnEnd battleMonsterTurnEnd:
                    Callback_BattleMonsterTurnEnd?.Invoke(battleMonsterTurnEnd);
                    break;
                case PD_BattlePlayerTurnStart battlePlayerTurnStart:
                    Callback_BattlePlayerTurnStart?.Invoke(battlePlayerTurnStart);
                    break;
                case PD_BattlePlayerTurnEnd battlePlayerTurnEnd:
                    Callback_BattlePlayerTurnEnd?.Invoke(battlePlayerTurnEnd);
                    break;
                case PD_BattleCardPlayed battleCardPlayed:
                    Callback_BattleCardPlayed?.Invoke(battleCardPlayed);
                    break;
                case PD_BattleCardDrawn battleCardDrawn:
                    Callback_BattleCardDrawn?.Invoke(battleCardDrawn);
                    break;
                case PD_BattleDiscardToDraw battleDiscardToDraw:
                    Callback_BattleDiscardToDraw?.Invoke(battleDiscardToDraw);
                    break;
                default:
                    System.Diagnostics.Debug.Assert(false, "API call not implemented !!");
                    break;
            }
        }

        public Action<PD_Validation> Callback_Validation;
        public Action<PD_AccountNameModify> Callback_AccountNameModify;
        public Action<PD_ServerChat> Callback_ServerChat;
        public Action<PD_PartyInvite> Callback_PartyInvite;
        public Action<PD_PartyMemberNew> Callback_PartyMemberNew;
        public Action<PD_PartyMemberLeave> Callback_PartyMemberLeave;
        public Action<PD_InfoOnlineList> Callback_InfoOnlineList;
        public Action<PD_AccountConnected> Callback_AccountConnected;
        public Action<PD_AccountDisconnected> Callback_AccountDisconnected;
        public Action<PD_AccountDeleted> Callback_AccountDeleted;
        public Action<PD_AccountMake> Callback_AccountMake;
        public Action<PD_AccountModify> Callback_AccountModify;
        public Action<PD_AccountDelete> Callback_AccountDelete;
        public Action<PD_AccountConnect> Callback_AccountConnect;
        public Action<PD_AccountDisconnect> Callback_AccountDisconnect;
        public Action<PD_AccountAddFriend> Callback_AccountAddFriend;
        public Action<PD_AccountRemoveFriend> Callback_AccountRemoveFriend;
        public Action<PD_AccountNewDeck> Callback_AccountNewDeck;
        public Action<PD_AccountAddCard> Callback_AccountAddCard;
        public Action<PD_AccountRenameDeck> Callback_AccountRenameDeck;
        public Action<PD_AccountDeleteDeck> Callback_AccountDeleteDeck;
        public Action<PD_AccountSetCurrentDeck> Callback_AccountSetCurrentDeck;
        public Action<PD_ServerListPlayers> Callback_ServerListPlayers;
        public Action<PD_BattleStart> Callback_BattleStart;
        public Action<PD_BattleEnd> Callback_BattleEnd;
        public Action<PD_BattleStatePlayer> Callback_BattleStatePlayer;
        public Action<PD_BattleStateMainPlayer> Callback_BattleStateMainPlayer;
        public Action<PD_BattleStateMonster> Callback_BattleStateMonster;
        public Action<PD_BattleState> Callback_BattleState;
        public Action<PD_BattleMonsterTurnStart> Callback_BattleMonsterTurnStart;
        public Action<PD_BattleMonsterAttack> Callback_BattleMonsterAttack;
        public Action<PD_BattleMonsterTurnEnd> Callback_BattleMonsterTurnEnd;
        public Action<PD_BattlePlayerTurnStart> Callback_BattlePlayerTurnStart;
        public Action<PD_BattlePlayerTurnEnd> Callback_BattlePlayerTurnEnd;
        public Action<PD_BattleCardPlayed> Callback_BattleCardPlayed;
        public Action<PD_BattleCardDrawn> Callback_BattleCardDrawn;
        public Action<PD_BattleDiscardToDraw> Callback_BattleDiscardToDraw;

        
    }
}

