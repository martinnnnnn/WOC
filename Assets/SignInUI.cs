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

public class SignInUI : MonoBehaviour
{
    NetworkInterface network;

    static bool done = false;


    private void Start()
    {
        network = FindObjectOfType<NetworkInterface>();
        network.Callback_AccountMake += HandleAPICall;
    }

    void Update()
    {
        if (!done)
        {
            VisualElement signing = GetComponent<PanelRenderer>().visualTree.Q("signing");

            TextField email1 = signing.Q<TextField>("email-field1");
            TextField password1 = signing.Q<TextField>("password-field1");
            signing.Q<Button>("signin").clicked += (() =>
            {

            });

            TextField email2 = signing.Q<TextField>("email-field2");
            TextField password2 = signing.Q<TextField>("name-field2");
            TextField name2 = signing.Q<TextField>("password-field2");
            signing.Q<Button>("signup").clicked += (() =>
            {
                network.SendMessage(new PD_AccountMake { email = email2.text, password = password2.text, name = name2.text });
            });
            done = true;
        }
    }

    public void HandleAPICall(PD_AccountMake data)
    {
        //account = new Account()
        //{
        //    email = data.email,
        //    password = data.password,
        //    name = data.name,
        //    connected = true
        //};
        Debug.Log("Your account has been created : " + data.name);
    }
}
