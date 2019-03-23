using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using WOC_Network;
using System.Collections.Generic;

namespace WOC
{
    public class ConnectionMenu : MonoBehaviour
    {
        public TMPro.TMP_InputField accountName;
        public TMPro.TMP_InputField accountPassword;

        public GameObject menuConnect;
        public GameObject menuHome;

        public TMPro.TextMeshProUGUI charactersList;
        public TMPro.TextMeshProUGUI decksList;
        public TMPro.TextMeshProUGUI cardsList;

        public TMPro.TextMeshProUGUI chatMessagesList;
        public TMPro.TextMeshProUGUI onlinePlayersList;

        public TMPro.TMP_InputField chatInput;

        private void Start()
        {
            menuConnect.SetActive(true);
            menuHome.SetActive(false);
            chatMessagesList.text = "";
            onlinePlayersList.text = "";
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Return) && chatInput.isFocused)
            {
                Network.Instance.SendChatMessage(chatInput.text);
                chatInput.text = "";
            }
        }


        public void OnConnectButton()
        {
            Network.Instance.TryConnect(accountName.text, accountPassword.text);
            Network.Instance.OnAccountInfo += OnConnectCompleted;
            Network.Instance.OnChatMessageReceived += OnChatMessage;
            Network.Instance.OnAccountsListUpdated += OnAccountList;
        }

        public void OnConnectCompleted(Account account)
        {
            menuConnect.SetActive(false);
            menuHome.SetActive(true);
            charactersList.text = "";
            account.characters.ForEach(c => charactersList.text += c.name + "\t\t" + c.type + "\n");
            decksList.text = "";
            account.decks.ForEach(d => decksList.text += d.name + "\n");
            cardsList.text = "";
            account.decks[0].cards.ForEach(d => cardsList.text += d.name + "\n");
        }


        public void OnChatMessage(string sender, string message)
        {
            chatMessagesList.text += "\n" + sender + " : " + message;
        }

        public void OnAccountList(List<string> accountsName)
        {
            onlinePlayersList.text = "";
            accountsName.ForEach(name => onlinePlayersList.text += name + "\n");
        }
    }
}

