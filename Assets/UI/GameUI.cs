using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WOC_Core;
using WOC_Client;
using System.Threading.Tasks;
using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Net;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

namespace WOC_Client
{
    public class GameUI : MonoBehaviour
    {
        NetworkInterface network;

        public GameObject ConnectPanel;
        public TMP_InputField textfieldEmail;
        public TMP_InputField textfieldPassword;
        public TMP_InputField textfieldName;
        public Button signin;
        public Button signup;

        // main panels
        public GameObject MainPanel;

        public Button disconnect;

        public TMP_InputField chatTextField;
        public GameObject chatMessagesContainer;
        public GameObject chatMessageObject;
        List<GameObject> chatMessages = new List<GameObject>();

        public GameObject onlinePlayersPanel;
        public Button showOnlinePlayers;
        public GameObject onlinePlayersContainer;
        public GameObject onlinePlayerObject;
        List<GameObject> onlinePlayers = new List<GameObject>();

        public GameObject partyPlayersPanel;
        public Button showPartyPlayers;
        public GameObject partyPlayersContainer;
        public GameObject partyPlayerObject;
        List<GameObject> partyPlayers = new List<GameObject>();

        public GameObject invitePlayerModal;
        public Button addPlayerToParty;
        public Button refusePlayerToParty;

        public Button startBattle;
        public Button endTurn;
        
        private void Start()
        {
            network = FindObjectOfType<NetworkInterface>();
            network.Callback_AccountMake += HandleAPICall;
            network.Callback_AccountConnect += HandleAPICall;
            network.Callback_AccountDisconnect += HandleAPICall;
            network.Callback_AccountDisconnected += HandleAPICall;
            network.Callback_ServerChat += HandleAPICall;
            network.Callback_AccountConnected += HandleAPICall;
            network.Callback_PartyInvite += HandleAPICall;
            network.Callback_PartyMemberNew += HandleAPICall;
            network.Callback_PartyMemberLeave += HandleAPICall;
            network.Callback_InfoOnlineList += HandleAPICall;

            ConnectPanel.SetActive(true);
            MainPanel.SetActive(false);
            onlinePlayersPanel.SetActive(false);

            signin.onClick.AddListener(() =>
            {
                network.SendMessage(new PD_AccountConnect { email = textfieldEmail.text, password = textfieldPassword.text }, true);
            });

            signup.onClick.AddListener(() =>
            {
                network.SendMessage(new PD_AccountMake { email = textfieldEmail.text, password = textfieldPassword.text, name = textfieldName.text }, true);
            });

            disconnect.onClick.AddListener(() =>
            {
                network.SendMessage(new PD_AccountDisconnect
                {
                    email = network.session.account.email
                }, false);
            });

            chatTextField.onSubmit.AddListener((string msg) =>
            {
                network.SendMessage(new PD_ServerChat
                {
                    senderName = network.session.account.name,
                    message = msg,
                    type = PD_ServerChat.Type.GLOBAL
                }, false);
                chatTextField.text = "";
            });

            showOnlinePlayers.onClick.AddListener(() =>
            {
                if (!onlinePlayersPanel.activeSelf)
                {
                    showOnlinePlayers.name = "Hide player list";
                    onlinePlayersPanel.SetActive(true);
                }
                else
                {
                    showOnlinePlayers.name = "Show player list";
                    onlinePlayersPanel.SetActive(false);
                }
            });

            //refusePlayerToParty.onClick.AddListener(() =>
            //{
            //    addPlayerToParty.onClick.RemoveAllListeners();
            //    addPlayerToParty.onClick.AddListener(() =>
            //    {
            //        invitePlayerModal.SetActive(false);
            //    });
            //    invitePlayerModal.SetActive(false);
            //});

            startBattle.onClick.AddListener(() =>
            {
                network.SendMessage(new PD_BattleStart {}, validate : false);
            });

            endTurn.onClick.AddListener(() =>
            {
                network.SendMessage(new PD_BattlePlayerTurnEnd { name = network.session.account.name });
            });
        }

        private void Update()
        {
           
        }

        public void HandleAPICall(PD_AccountMake data)
        {
            network.session.account = new Account()
            {
                email = data.email,
                password = data.password,
                name = data.name,
                connected = true
            };
            ConnectPanel.SetActive(false);
            MainPanel.SetActive(true);
            network.SendMessage(new PD_InfoOnlineList());
        }

        public void HandleAPICall(PD_AccountConnect data)
        {
            network.session.account.connected = true;
            ConnectPanel.SetActive(false);
            MainPanel.SetActive(true);
        }

        public void HandleAPICall(PD_ServerChat data)
        {
            if (chatMessages.Count > 100)
            {
                Destroy(chatMessages[0]);
                chatMessages.RemoveAt(0);
            }

            GameObject newText = Instantiate(chatMessageObject, chatMessagesContainer.transform);
            newText.GetComponent<TMP_Text>().text = data.senderName + " : " + data.message;
            chatMessages.Add(newText);
        }

        public void HandleAPICall(PD_InfoOnlineList data)
        {
            foreach (string name in data.names)
            {
                GameObject newAccount = Instantiate(onlinePlayerObject, onlinePlayersContainer.transform);
                Button button = newAccount.GetComponent<Button>();
                button.GetComponentInChildren<TMP_Text>().text = name;
                button.onClick.AddListener(() =>
                {
                    invitePlayerModal.SetActive(true);
                    addPlayerToParty.onClick.AddListener(() =>
                    {
                        network.SendMessage(new PD_PartyInvite { inviter = network.session.account.name, invity = name });
                        addPlayerToParty.onClick.RemoveAllListeners();
                        addPlayerToParty.onClick.AddListener(() =>
                        {
                            invitePlayerModal.SetActive(false);
                        });
                        invitePlayerModal.SetActive(false);
                    });
                });
                onlinePlayers.Add(newAccount);
            }
        }

        public void HandleAPICall(PD_AccountConnected data)
        {
            GameObject newAccount = Instantiate(onlinePlayerObject, onlinePlayersContainer.transform);
            Button button = newAccount.GetComponent<Button>();
            button.GetComponentInChildren<TMP_Text>().text = data.name;
            button.onClick.AddListener(() =>
            {
                invitePlayerModal.SetActive(true);
                addPlayerToParty.onClick.AddListener(() =>
                {
                    network.SendMessage(new PD_PartyInvite { inviter = network.session.account.name, invity = data.name });
                    addPlayerToParty.onClick.RemoveAllListeners();
                    addPlayerToParty.onClick.AddListener(() =>
                    {
                        invitePlayerModal.SetActive(false);
                    });
                    invitePlayerModal.SetActive(false);
                });
            });
            onlinePlayers.Add(newAccount);
        }

        public void HandleAPICall(PD_AccountDisconnected data)
        {
            var toRemove = onlinePlayers.Find(p => p.GetComponentInChildren<TMP_Text>().text == data.name);
            Destroy(toRemove);
            onlinePlayers.Remove(toRemove);
        }
        


        public Button acceptPartyInvite;
        public Button refusePartyInvite;
        public GameObject invitationModal;
        public TMP_Text invitationText;
        public void HandleAPICall(PD_PartyInvite data)
        {
            acceptPartyInvite.onClick.RemoveAllListeners();
            refusePartyInvite.onClick.RemoveAllListeners();

            invitationModal.SetActive(true);
            invitationText.text = string.Format("You have been invited by {0} to join his party.", data.inviter);

            acceptPartyInvite.onClick.AddListener(() =>
            {
                invitationModal.SetActive(false);
                network.SendMessage(new PD_PartyMemberNew { memberName = network.session.account.name });
                network.session.account.partyAccounts.AddRange(data.partyAccounts);
            });
            refusePartyInvite.onClick.AddListener(() =>
            {
                invitationModal.SetActive(false);
            });
        }

        public void HandleAPICall(PD_PartyMemberNew data)
        {
            network.session.account.partyAccounts.Add(data.memberName);

            GameObject newPartyPlayer = Instantiate(partyPlayerObject, partyPlayersContainer.transform);
            newPartyPlayer.GetComponent<TMP_Text>().text = data.memberName;
            partyPlayers.Add(newPartyPlayer);
        }

        public void HandleAPICall(PD_PartyMemberLeave data)
        {
            network.session.account.partyAccounts.Remove(data.memberName);
            var player = partyPlayers.Find(p => p.GetComponent<TMP_Text>().text == data.memberName);
            Destroy(player);
            chatMessages.Remove(player);
        }

        public void HandleAPICall(PD_AccountDisconnect data)
        {
            network.session.account.connected = false;
            ConnectPanel.SetActive(true);
            MainPanel.SetActive(false);
        }
    }

    [Serializable]
    public class ChatMessage
    {
        //public string content;
        public TMP_Text textObject;
    }
}


