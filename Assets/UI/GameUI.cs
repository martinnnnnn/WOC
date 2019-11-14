using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UIElements.Runtime;
using UnityEngine.UIElements;
using WOC_Core;
using WOC_Client;
using System.Threading.Tasks;
using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Net;
using UnityEditor;

public class GameUI : MonoBehaviour
{
    NetworkInterface network;

    VisualElement signing;
    VisualElement chat;
    Button disconnect;

    private void Start()
    {
        network = FindObjectOfType<NetworkInterface>();
        network.Callback_AccountMake += HandleAPICall;
        network.Callback_AccountConnect += HandleAPICall;
        network.Callback_AccountDisconnect += HandleAPICall;
        network.Callback_ServerChat += HandleAPICall;
    }

    private void Init()
    {
        PanelRenderer panelRenderer = GetComponent<PanelRenderer>();

        // Signin panel
        signing = panelRenderer.visualTree.Q("signing");

        TextField email1 = signing.Q<TextField>("email-field1");
        TextField password1 = signing.Q<TextField>("password-field1");
        signing.Q<Button>("signin").clicked += (() =>
        {
            network.SendMessage(new PD_AccountConnect { email = email1.text, password = password1.text }, true);
        });

        TextField email2 = signing.Q<TextField>("email-field2");
        TextField password2 = signing.Q<TextField>("password-field2");
        TextField name2 = signing.Q<TextField>("name-field2");
        signing.Q<Button>("signup").clicked += (() =>
        {
            network.SendMessage(new PD_AccountMake { email = email2.text, password = password2.text, name = name2.text }, true);
        });
        signing.visible = true;

        // chat
        chat = panelRenderer.visualTree.Q("chat");
        TextField chat_textfield = chat.Q<VisualElement>("bottom").Q<TextField>("chat-textfield");
        chat.Q<VisualElement>("bottom").Q<Button>("chat-send").clicked += (() =>
        {
            network.SendMessage(new PD_ServerChat
            {
                senderName = network.session.account.name,
                message = chat_textfield.text,
                type = PD_ServerChat.Type.GLOBAL
            }, false);
        });
        chat.visible = false;

        disconnect = panelRenderer.visualTree.Q<Button>("disconnect");
        disconnect.visible = false;
        disconnect.clicked += (() =>
        {
            network.SendMessage(new PD_AccountDisconnect
            {
                email = network.session.account.email
            }, false);
        });
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
        signing.visible = false;
        chat.visible = true;
        disconnect.visible = true;
    }

    public void HandleAPICall(PD_AccountConnect data)
    {
        network.session.account.connected = true;
        signing.visible = false;
        chat.visible = true;
        disconnect.visible = true;
    }

    public void HandleAPICall(PD_ServerChat data)
    {
        chat.Q<Label>("chat-messages").text += "\n" + data.senderName + " : " + data.message;
        chat.Q<VisualElement>("bottom").Q<TextField>("chat-textfield").value = "";
    }

    public void HandleAPICall(PD_AccountDisconnect data)
    {
        network.session.account.connected = false;
        signing.visible = true;
        chat.visible = false;
        disconnect.visible = false;
    }

    static bool initdone = false;
    void Update()
    {
        if (!initdone)
        {
            Init();
            initdone = true;
        }
    }
}
