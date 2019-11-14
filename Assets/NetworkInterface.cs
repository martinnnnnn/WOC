using UnityEngine;
using System.Collections.Concurrent;
using WOC_Core;
using WOC_Client;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using System.Threading;

public class NetworkInterface : MonoBehaviour
{
    public ClientSession session;

    struct OutPacket
    {
        public IPacketData data;
        public bool force;
    }

    ConcurrentQueue<OutPacket> outgoingMessages = new ConcurrentQueue<OutPacket>();
    ConcurrentQueue<IPacketData> incomingMessages = new ConcurrentQueue<IPacketData>();

    private void Start()
    {
        UnitySystemConsoleRedirector.Redirect();
        Connect();
    }

    public void Connect()
    {
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

            while (!serverFound && sw.ElapsedMilliseconds < 1000 * 60)
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
                Console.WriteLine("Could not find a server");
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
            session.awaitingValidations.TryAdd(packet.data.id, packet.data);
            session.Send(packet.data, packet.force);
        }
    }

    public void SendMessage(IPacketData packet, bool force = false)
    {
        outgoingMessages.Enqueue(new OutPacket { data = packet, force = force });
    }

    public void ReceiveMessage(IPacketData packet)
    {
        incomingMessages.Enqueue(packet);
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
            case PD_AccountAddFriend accountAddFriend:
                Callback_AccountAddFriend?.Invoke(accountAddFriend);
                break;
            case PD_AccountRemoveFriend accountRemoveFriend:
                Callback_AccountRemoveFriend?.Invoke(accountRemoveFriend);
                break;
            case PD_AccountAddCharacter accountAddCharacter:
                Callback_AccountAddCharacter?.Invoke(accountAddCharacter);
                break;
            case PD_AccountModifyCharacter accountModifyCharacter:
                Callback_AccountModifyCharacter?.Invoke(accountModifyCharacter);
                break;
            case PD_AccountDeleteCharacter accountDeleteCharacter:
                Callback_AccountDeleteCharacter?.Invoke(accountDeleteCharacter);
                break;
            case PD_AccountSetDefaultCharacter accountSetDefaultCharacter:
                Callback_AccountSetDefaultCharacter?.Invoke(accountSetDefaultCharacter);
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
            case PD_AccountSetDefaultDeck accountSetDefaultDeck:
                Callback_AccountSetDefaultDeck?.Invoke(accountSetDefaultDeck);
                break;
            case PD_ServerListPlayers serverListPlayers:
                Callback_ServerListPlayers?.Invoke(serverListPlayers);
                break;
            case PD_RoomAddPNJ roomAddPNJ:
                Callback_RoomAddPNJ?.Invoke(roomAddPNJ);
                break;
            case PD_RoomInitBattle roomBattleInit:
                Callback_RoomInitBattle?.Invoke(roomBattleInit);
                break;
            case PD_RoomStartBattle roomBattleStart:
                Callback_RoomStartBattle?.Invoke(roomBattleStart);
                break;
            case PD_BattlePlayCard battleCardPlayed:
                Callback_BattlePlayCard?.Invoke(battleCardPlayed);
                break;
            case PD_BattleEndTurn battleEndTurn:
                Callback_BattleEndTurn?.Invoke(battleEndTurn);
                break;
            case PD_BattleState battleState:
                Callback_BattleState?.Invoke(battleState);
                break;
            default:
                System.Diagnostics.Debug.Assert(false, "API call not implemented !!");
                break;
        }
    }


    public Action<PD_Validation> Callback_Validation;
    public Action<PD_AccountNameModify> Callback_AccountNameModify;
    public Action<PD_ServerChat> Callback_ServerChat;
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
    public Action<PD_AccountAddCharacter> Callback_AccountAddCharacter;
    public Action<PD_AccountModifyCharacter> Callback_AccountModifyCharacter;
    public Action<PD_AccountDeleteCharacter> Callback_AccountDeleteCharacter;
    public Action<PD_AccountSetDefaultCharacter> Callback_AccountSetDefaultCharacter;
    public Action<PD_AccountNewDeck> Callback_AccountNewDeck;
    public Action<PD_AccountAddCard> Callback_AccountAddCard;
    public Action<PD_AccountRenameDeck> Callback_AccountRenameDeck;
    public Action<PD_AccountDeleteDeck> Callback_AccountDeleteDeck;
    public Action<PD_AccountSetDefaultDeck> Callback_AccountSetDefaultDeck;
    public Action<PD_ServerListPlayers> Callback_ServerListPlayers;
    public Action<PD_RoomAddPNJ> Callback_RoomAddPNJ;
    public Action<PD_RoomInitBattle> Callback_RoomInitBattle;
    public Action<PD_RoomStartBattle> Callback_RoomStartBattle;
    public Action<PD_BattlePlayCard> Callback_BattlePlayCard;
    public Action<PD_BattleEndTurn> Callback_BattleEndTurn;
    public Action<PD_BattleState> Callback_BattleState;
}
