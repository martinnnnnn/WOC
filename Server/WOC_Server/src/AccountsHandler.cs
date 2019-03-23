using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WOC_Network;

namespace WOC_Server
{
    class AccountsManager
    {
        List<Account> liveAccounts = new List<Account>();
        List<Account> allAccounts = new List<Account>();

        Account Create(string newName, bool autoConnect = true)
        {
            Account newAccount = null;

            if (allAccounts.Find(acc => acc.name == newName) == null)
            {
                newAccount = new Account() { name = newName };
                allAccounts.Add(newAccount);
                if (autoConnect)
                {
                    liveAccounts.Add(newAccount);
                }
            }
            
            return newAccount;
        }

        void Connect(string name)
        {
            Account account = allAccounts.Find(acc => acc.name == name);
            if (account != null)
            {
                liveAccounts.Add(account);
            }
        }

        void Disconnect(string name)
        {
            liveAccounts.RemoveAll(acc => acc.name == name);
        }


        void Load(string jaccounts)
        {

        }

        void Save(string path)
        {

        }
    }
}


//namespace WOC_Server
//{
//    class Account
//    {
//        public int liveId;
//        public string name;
//        public string password;
//        public bool online;
//    }

//    class AccountsHandler
//    {
//        Dictionary<string, Account> accounts = new Dictionary<string, Account>();
//        static int currentId = 0;

//        public void AddFromJson(string jhandler)
//        {
//            foreach(var pair in JsonConvert.DeserializeObject<Dictionary<string, Account>>(jhandler))
//            {
//                accounts[pair.Key] = pair.Value;
//            }
//        }

//        public string ToJson()
//        {
//            return JsonConvert.SerializeObject(accounts, Formatting.Indented);
//        }

//        public bool Connect(string providedName, string password)
//        {
//            bool result = false;

//            accounts.TryGetValue(providedName, out Account account);
//            if (password == account.password)
//            {
//                account.online = true;
//                result = true;
//            }
//            return result;
//        }

//        public bool Disconnect(string providedName, string password)
//        {
//            bool result = false;

//            accounts.TryGetValue(providedName, out Account account);
//            if (password == account.password)
//            {
//                account.online = false;
//                result = true;
//            }
//            return result;
//        }

//        public bool Create(string newName, string newPassword)
//        {
//            bool result = false;
//            if (!accounts.ContainsKey(newName))
//            {
//                accounts.Add(newName, new Account()
//                {
//                    liveId = currentId++,
//                    name = newName,
//                    password = newPassword,
//                    online = true
//                });
//                Connect(newName, newPassword);
//                result = true;
//            }

//            return result;
//        }

//        public string List()
//        {
//            StringBuilder sb = new StringBuilder();
//            StringWriter sw = new StringWriter(sb);

//            using (JsonWriter writer = new JsonTextWriter(sw))
//            {
//                writer.Formatting = Formatting.Indented;

//                writer.WriteStartObject();
//                writer.WritePropertyName("account_list_result");
//                writer.WriteStartArray();
//                foreach (var acc in accounts.Values)
//                {
//                    writer.WriteStartObject();
//                    writer.WritePropertyName("name");
//                    writer.WriteValue(acc.name);
//                    writer.WritePropertyName("online");
//                    writer.WriteValue(acc.online);
//                    writer.WriteEndObject();
//                }
//                writer.WriteEndArray();
//                writer.WriteEndObject();
//            }

//            return sb.ToString();
//        }
//    }
//}

