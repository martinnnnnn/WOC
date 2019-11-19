using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOC_Core;
using UnityEditor;
using UnityEngine;

namespace WOC_Client
{
    public class ClientSession : Session
    {
        NetworkInterface network = null;

        public List<string> onlineAccountNames = new List<string>();

        public ConcurrentDictionary<Guid, IPacketData> awaitingValidations = new ConcurrentDictionary<Guid, IPacketData>();

        public ClientSession(NetworkInterface network)
        {
            this.network = network;
        }

        public override void HandleAPICall(IPacketData data)
        {
            switch (data)
            {
                case PD_Validation validation:
                    HandleValidation(validation);
                    break;
                default:
                    network.ReceiveMessage(data);
                    break;
            }
        }

        public void HandleValidation(PD_Validation data)
        {
            bool result = awaitingValidations.TryRemove(data.validationId, out IPacketData validatedPacket);

            if (!result)
            {
                Debug.Log("Could not find packet awaiting validation.");
                return;
            }

            if (!data.isValid)
            {
                Debug.Log("[CLIENT] Validation denied for packet " + validatedPacket.GetType().Name + " -> " + data.errorMessage);
                return;
            }

            Debug.Log("[CLIENT] Applying validation for packet " + validatedPacket.GetType().Name + ", still " + awaitingValidations.Count + " packet awaiting validation.");
            HandleAPICall(validatedPacket);
        }
    }
}
