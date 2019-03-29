using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WOC_Network;

namespace WOC_Server
{
    class BattleInstance
    {
        public Server server;
        public List<ServerSideSession> sessions = new List<ServerSideSession>();
        public Monster monster;
        public BattleInfo info;
        private bool hasStarted = false;

        public void Start()
        {
            hasStarted = true;
        }

        public void HandleIncoming(Session sender, PD_BattleAction packet)
        {
            switch (packet)
            {
                case PD_BattleRegister data:
                    HandleRegistration(sender, data);
                    break;
                case PD_BattleDisconnect data:
                    HandleDisconnect(sender, data);
                    break;
                case PD_BattleStart data:
                    Start();
                    break;
            }
        }

        private void HandleDisconnect(Session sender, PD_BattleDisconnect data)
        {
            if (sessions.Exists(s => s.account.name == data.accountName))
            {
                sessions.RemoveAll(s => s.account.name == data.accountName);
                sender.SendValidation(data.id).Wait();
            }
            else
            {
                sender.SendValidation(data.id, "account_not_connected").Wait();
            }
            
        }

        private void HandleRegistration(Session sender, PD_BattleRegister data)
        {
            if (hasStarted)
            {
                sender.SendValidation(data.id, "battle_has_started").Wait();
                return;
            }

            if (!server.FindSession(data.accountName, out Session sess))
            {
                sender.SendValidation(data.id, "account_not_connected").Wait();
                return;
            }

            if (sessions.Contains(sess))
            {
                sender.SendValidation(data.id, "account_already_registered").Wait();
                return;
            }

            sessions.Add(sess as ServerSideSession);
            sender.SendValidation(data.id).Wait();
        }
    }
}
