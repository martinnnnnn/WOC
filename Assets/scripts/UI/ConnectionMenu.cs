using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace WOC
{
    public class ConnectionMenu : MonoBehaviour
    {
        public TMPro.TMP_InputField accountName;
        public TMPro.TMP_InputField accountPassword;

        public void OnConnectButton()
        {
            Network.Instance.TryConnect(accountName.text, accountPassword.text);
        }
    }
}

