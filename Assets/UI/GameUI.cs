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

        public GameObject MainPanel;
        public TMP_InputField textFieldChat;
        public Button disconnect;

        private void Start()
        {
            network = FindObjectOfType<NetworkInterface>();
            network.Callback_AccountMake += HandleAPICall;
            network.Callback_AccountConnect += HandleAPICall;
            network.Callback_AccountDisconnect += HandleAPICall;
            network.Callback_ServerChat += HandleAPICall;

            ConnectPanel.SetActive(true);
            MainPanel.SetActive(false);

            signin.onClick.AddListener(() =>
            {
                network.SendMessage(new PD_AccountConnect { email = textfieldEmail.text, password = textfieldPassword.text }, true);
            });

            signup.onClick.AddListener(() =>
            {
                network.SendMessage(new PD_AccountMake { email = textfieldEmail.text, password = textfieldPassword.text, name = textfieldName.text }, true);
            });

            //disconnect.onClick.AddListener(() =>
            //{
            //    network.SendMessage(new PD_AccountDisconnect
            //    {
            //        email = network.session.account.email
            //    }, false);
            //});

            textFieldChat.onSubmit.AddListener((string msg) =>
            {
                network.SendMessage(new PD_ServerChat
                {
                    senderName = network.session.account.name,
                    message = msg,
                    type = PD_ServerChat.Type.GLOBAL
                }, false);
                textFieldChat.text = "";
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
        }

        public void HandleAPICall(PD_AccountConnect data)
        {
            network.session.account.connected = true;
            ConnectPanel.SetActive(false);
            MainPanel.SetActive(true);
        }


        public GameObject chatPanel;
        public GameObject textObject;
        public List<GameObject> chatMessages = new List<GameObject>();

        public void HandleAPICall(PD_ServerChat data)
        {
            if (chatMessages.Count > 100)
            {
                Destroy(chatMessages[0]);
                chatMessages.RemoveAt(0);
            }

            GameObject newText = Instantiate(textObject, chatPanel.transform);
            newText.GetComponent<TMP_Text>().text = data.senderName + " : " + data.message;
            chatMessages.Add(newText);
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


