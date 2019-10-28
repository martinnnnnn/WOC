using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UIElements.Runtime;
using UnityEngine.UIElements;
using WOC_Core;
using WOC_Client;
using System.Threading.Tasks;
using System;
using System.Diagnostics;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Net;

public class SignInUI : MonoBehaviour
{
    ClientSession session;

    private void Start()
    {
        Task.Run(() =>
        {
            session = new ClientSession();
            var udpClient = new UdpClient();
            udpClient.Client.ReceiveTimeout = 10000;
            var RequestData = Encoding.ASCII.GetBytes(Serialization.ToJson(new PD_Discovery { }));
            var ServerEp = new IPEndPoint(IPAddress.Any, 0);
            string serverIp = "";
            bool serverFound = false;
            udpClient.EnableBroadcast = true;
            while (!serverFound)
            {
                try
                {
                    UnityEngine.Debug.Log("[DISCOVERY] Looking for a server...");
                    udpClient.Send(RequestData, RequestData.Length, new IPEndPoint(IPAddress.Broadcast, 8888));

                    var ServerResponseData = udpClient.Receive(ref ServerEp);
                    var ServerResponse = Encoding.ASCII.GetString(ServerResponseData);
                    serverIp = ServerEp.Address.ToString();

                    PD_Discovery data = Serialization.FromJson<PD_Discovery>(ServerResponse);

                    UnityEngine.Debug.Log("[DISCOVERY] Found a server : " + serverIp);
                    udpClient.Close();
                    serverFound = true;
                }
                catch (Exception)
                {
                    StackTrace stackTrace = new StackTrace();
                    UnityEngine.Debug.Log("[DISCOVERY] Coudln't find a server.");
                    Thread.Sleep(2000);
                }

                session.Connect(serverIp, 54001);
            }
        });
    }

    static bool done = false;
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
                if (session.account != null) return;

                Task.Run(() => SendWithValidation(new PD_AccountMake { email = email2.text, password = password2.text, name = name2.text }, true));
            });
        }

        IEnumerator SendWithValidation(IPacketData data, bool force = false)
        {
            UnityEngine.Debug.Log("omg");
            System.Diagnostics.Debug.Assert(session.awaitingValidations.TryAdd(data.id, data));
            var task = session.SendAsync(data, force);
            while (!task.IsCompleted)
            {
                UnityEngine.Debug.Log("hello");
                yield return new WaitForFixedUpdate();
            }
        }
    }
}
