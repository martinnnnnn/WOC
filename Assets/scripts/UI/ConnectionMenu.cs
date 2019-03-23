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


        public void OnConnectButton()
        {
            Network.Instance.ConnectCompleted += OnConnectCompleted;
            Network.Instance.TryConnect(accountName.text, accountPassword.text);
        }

        public void OnConnectCompleted(Account account)
        {
            Debug.Log("on connect completed");
            Network.Instance.ConnectCompleted -= OnConnectCompleted;
            menuConnect.SetActive(false);
            menuHome.SetActive(true);
            charactersList.text = "";
            account.characters.ForEach(c => charactersList.text += c.name + "\t\t" + c.type + "\n");
            decksList.text = "";
            account.decks.ForEach(d => decksList.text += d.name + "\n");
            cardsList.text = "";
            account.decks[0].cards.ForEach(d => cardsList.text += d.name + "\n");
        }
    }
}

