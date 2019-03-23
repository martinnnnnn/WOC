using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using WOC_Network;

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
            Network.Instance.ConnectCompleted += OnConnectCompleted;
            Network.Instance.ChatMessageReceived += OnChatMessage;
            Network.Instance.AccountListUpdated += OnAccountList; 
            Network.Instance.TryConnect(accountName.text, accountPassword.text);
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


        public void OnChatMessage(PD_Chat data)
        {
            chatMessagesList.text += "\n" + data.senderName + " : " + data.message;
        }

        public void OnAccountList(PD_Info<AccountList> data)
        {
            onlinePlayersList.text = "";
            data.info.names.ForEach(name => onlinePlayersList.text += name + "\n");
        }
    }
}

