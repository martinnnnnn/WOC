using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WOC_Server
{
    class Account
    {
        public int liveId;
        public string name;
        public string password;
        public bool online;
    }

    class AccountHandler
    {
        Dictionary<string, Account> accounts = new Dictionary<string, Account>();
        static int currentId = 0;

        public bool Connect(string providedName, string password)
        {
            bool result = false;

            accounts.TryGetValue(providedName, out Account account);
            if (password == account.password)
            {
                account.online = true;
                result = true;
            }
            return result;
        }

        public bool Disconnect(string providedName, string password)
        {
            bool result = false;

            accounts.TryGetValue(providedName, out Account account);
            if (password == account.password)
            {
                account.online = false;
                result = true;
            }
            return result;
        }

        public bool Create(string newName, string newPassword)
        {
            bool result = false;
            if (!accounts.ContainsKey(newName))
            {
                accounts.Add(newName, new Account()
                {
                    liveId = currentId++,
                    name = newName,
                    password = newPassword,
                    online = true
                });
                Connect(newName, newPassword);
                result = true;
            }

            return result;
        }

        public string GetAccountsList()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;

                writer.WriteStartObject();
                writer.WritePropertyName("account_list_result");
                writer.WriteStartArray();
                foreach (var acc in accounts.Values)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("name");
                    writer.WriteValue(acc.name);
                    writer.WritePropertyName("online");
                    writer.WriteValue(acc.online);
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
                writer.WriteEndObject();
            }

            return sb.ToString();
        }
    }
}
