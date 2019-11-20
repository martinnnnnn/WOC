using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WOC_Core;

namespace WOC_Server
{
    public class ServerSession : Session
    {
        GameServer server;


        public ServerSession(GameServer s)
        {
            server = s;
        }

        public override void HandleAPICall(IPacketData data)
        {
            switch (data)
            {
                case PD_AccountMake accountMake:
                    HandleAPICall(accountMake);
                    break;
                case PD_AccountModify accountModify:
                    HandleAPICall(accountModify);
                    break;
                case PD_AccountDelete accountDelete:
                    HandleAPICall(accountDelete);
                    break;
                case PD_AccountConnect accountConnect:
                    HandleAPICall(accountConnect);
                    break;
                case PD_AccountDisconnect accountDisconnect:
                    HandleAPICall(accountDisconnect);
                    break;
                case PD_AccountAddFriend accountAddFriend:
                    HandleAPICall(accountAddFriend);
                    break;
                case PD_AccountRemoveFriend accountRemoveFriend:
                    HandleAPICall(accountRemoveFriend);
                    break;
                case PD_AccountNewDeck accountNewDeck:
                    HandleAPICall(accountNewDeck);
                    break;
                case PD_AccountAddCard accountAddCard:
                    HandleAPICall(accountAddCard);
                    break;
                case PD_AccountRenameDeck accountRenameDeck:
                    HandleAPICall(accountRenameDeck);
                    break;
                case PD_AccountDeleteDeck accountDeleteDeck:
                    HandleAPICall(accountDeleteDeck);
                    break;
                case PD_AccountSetCurrentDeck accountSetCurrentDeck:
                    HandleAPICall(accountSetCurrentDeck);
                    break;
                case PD_ServerChat serverChat:
                    HandleAPICall(serverChat);
                    break;
                case PD_ServerListPlayers serverListPlayers:
                    HandleAPICall(serverListPlayers);
                    break;
                case PD_InfoOnlineList infoOnlineList:
                    HandleAPICall(infoOnlineList);
                    break;
                case PD_BattleStart battleStart:
                    HandleAPICall(battleStart);
                    break;
                case PD_BattleStatePlayer battleStatePlayer:
                    HandleAPICall(battleStatePlayer);
                    break;
                case PD_BattleStateMainPlayer battleStateMainPlayer:
                    HandleAPICall(battleStateMainPlayer);
                    break;
                case PD_BattleStateMonster battleStateMonster:
                    HandleAPICall(battleStateMonster);
                    break;
                case PD_BattleState battleState:
                    HandleAPICall(battleState);
                    break;
                case PD_BattleMonsterTurnStart battleMonsterTurnStart:
                    HandleAPICall(battleMonsterTurnStart);
                    break;
                case PD_BattleMonsterTurnEnd battleMonsterTurnEnd:
                    HandleAPICall(battleMonsterTurnEnd);
                    break;
                case PD_BattlePlayerTurnStart battlePlayerTurnStart:
                    HandleAPICall(battlePlayerTurnStart);
                    break;
                case PD_BattlePlayerTurnEnd battlePlayerTurnEnd:
                    HandleAPICall(battlePlayerTurnEnd);
                    break;
                case PD_BattleCardPlayed battleCardPlayed:
                    HandleAPICall(battleCardPlayed);
                    break;
                case PD_BattleCardDrawn battleCardDrawn:
                    HandleAPICall(battleCardDrawn);
                    break;
                default:
                    Debug.Assert(false, "NOT IMPLEMENTED YET.");
                    break;
            }
        }

        #region SERVER
        // TODO : send validation for chat
        public void HandleAPICall(PD_ServerChat data)
        {
            try
            {
                switch (data.type)
                {
                    case PD_ServerChat.Type.LOCAL:
                        server.Broadcast(data, null);
                        break;
                    case PD_ServerChat.Type.FRIENDS:
                        //data.message = data.message.Remove(0, 4);
                        server.BroadcastTo(data, server.sessions.Where(s => account.friends.Contains(s.account.name) && s.account.connected));
                        break;
                    case PD_ServerChat.Type.GLOBAL:
                        server.Broadcast(data, null);
                        break;
                    default:
                        Console.WriteLine("Message type not supported");
                        break;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("[SERVER] Failed to broadcast message.");
            }
        }

        public void HandleAPICall(PD_InfoOnlineList data)
        {
            if (!AssureConnected(data.id)) return;

            Send(new PD_InfoOnlineList { names = server.sessions.Select(s => s.account.name).Where(n => n != account.name).ToList() });
        }

        public void HandleAPICall(PD_ServerListPlayers data)
        {
            Debug.Assert(false, "NOT IMPLEMENTED YET.");
        }


        #endregion

        #region ACCOUNT
        public void HandleAPICall(PD_AccountMake data)
        {
            Debug.Assert(account == null);
            string errorMessage = "";

            if (String.IsNullOrEmpty(data.email) || String.IsNullOrEmpty(data.password) || String.IsNullOrEmpty(data.name))
            {
                errorMessage = "Not enough info to create acount.";
            }
            else
            {
                account = new Account()
                {
                    email = data.email,
                    password = data.password,
                    name = data.name,
                    connected = true,
                    session = this
                };

                bool result = server.users.TryAdd(data.email, account);

                if (!result)
                {
                    account = null;
                    errorMessage = "This email is already used.";
                }
                else
                {
                    server.Broadcast(new PD_AccountConnected { name = account.name }, this);
                }
            }

            Send(new PD_Validation(data.id, errorMessage), true);
        }

        public void HandleAPICall(PD_AccountModify data)
        {
            if (!AssureConnected(data.id)) return;

            Debug.Assert(account.email == data.oldEmail, "Cannot try to modify a user that's not the one currently connected.");
            string errorMessage = "";

            bool result = server.users.TryRemove(data.oldEmail, out Account user);

            if (result)
            {
                if (!String.IsNullOrEmpty(data.newEmail))
                {
                    account.email = data.newEmail;
                    user.email = data.newEmail;
                }
                if (!String.IsNullOrEmpty(data.newPassword))
                {
                    account.password = data.newPassword;
                    user.password = data.newPassword;
                }
                if (!String.IsNullOrEmpty(data.newName))
                {
                    account.name = data.newName;
                    user.name = data.newName;
                }

                result = server.users.TryAdd(user.email, user);
                errorMessage = result ? "" : "New email already exists.";

                if (result && data.oldName != data.newName)
                {
                    server.Broadcast(new PD_AccountNameModify { oldName = data.oldName, newName = data.newName }, this);
                }
            }
            else
            {
                errorMessage = "Could not find the email.";
            }

            Send(new PD_Validation(data.id, errorMessage));
        }

        public void HandleAPICall(PD_AccountDelete data)
        {
            if (!AssureConnected(data.id)) return;

            string errorMessage = "";
            bool result = server.users.TryRemove(data.email, out Account deletingAccount);
            if (result)
            {
                if (deletingAccount.password == data.password)
                {
                    server.Broadcast(new PD_AccountDeleted { name = account.name }, this);
                    account = null;
                }
                else
                {
                    errorMessage = "Wrong password.";
                }
            }
            else
            {
                errorMessage = "Could not find matching email.";
            }

            Send(new PD_Validation(data.id, errorMessage));
        }

        public void HandleAPICall(PD_AccountConnect data)
        {
            string errorMessage = "";
            bool result = server.users.TryGetValue(data.email, out Account connectingAccount);

            if (result)
            {
                if (connectingAccount.password == data.password)
                {
                    connectingAccount.connected = true;
                    connectingAccount.session = this;
                    account = connectingAccount;
                    server.Broadcast(new PD_AccountConnected { name = account.name }, this);
                }
                else
                {
                    errorMessage = "Wrong password.";
                }
            }
            else
            {
                errorMessage = "Could not find matching email.";
            }

            Send(new PD_Validation(data.id, errorMessage));
        }

        public void HandleAPICall(PD_AccountDisconnect data)
        {
            if (!AssureConnected(data.id)) return;

            string errorMessage = "";
            bool result = server.users.TryGetValue(data.email, out Account disconnectingAccount);
            if (result)
            {
                server.Broadcast(new PD_AccountDisconnected { name = account.name }, this);
                disconnectingAccount.connected = false;
                disconnectingAccount.session = null;
                account = null;
            }
            else
            {
                errorMessage = "Could not find matching email.";
            }

            Send(new PD_Validation(data.id, errorMessage), true);
        }

        public void HandleAPICall(PD_AccountAddFriend data)
        {
            if (!AssureConnected(data.id)) return;

            string errorMessage = "";
            if (account.name != data.name)
            {
                Account newFriend = server.users.FirstOrDefault(acc => acc.Value.name == data.name).Value;

                if (newFriend == null)
                {
                    errorMessage = "Could not find your friend.";
                }
                else if (account.friends.FirstOrDefault(acc => acc == data.name) != default(string))
                {
                    errorMessage = "Already your friend !";
                }
                else
                {
                    account.friends.Add(newFriend.name);
                    newFriend.friends.Add(account.name);
                    newFriend.session?.Send(new PD_AccountAddFriend { name = account.name });
                }
            }
            else
            {
                errorMessage = "You cannot add yourself.";
            }

            Send(new PD_Validation(data.id, errorMessage));
        }

        public void HandleAPICall(PD_AccountRemoveFriend data)
        {
            if (!AssureConnected(data.id)) return;

            string errorMessage = "";
            if (account.name != data.name)
            {
                Account oldFriend = server.users.FirstOrDefault(acc => acc.Value.name == data.name).Value;

                if (oldFriend == null)
                {
                    errorMessage = "Could not find your friend.";
                }
                else if (account.friends.FirstOrDefault(acc => acc == data.name) == default(string))
                {
                    errorMessage = "Not your friend !";
                }
                else
                {
                    account.friends.Remove(oldFriend.name);
                    oldFriend.friends.Remove(account.name);
                    oldFriend.session?.Send(new PD_AccountRemoveFriend { name = account.name });
                }
            }
            else
            {
                errorMessage = "You cannot add yourself.";
            }

            Send(new PD_Validation(data.id, errorMessage));
        }

        public void HandleAPICall(PD_AccountNewDeck data)
        {
            if (!AssureConnected(data.id)) return;
            string errorMessage = "";

            if (account.decks.Find(d => d.name == data.name) == null)
            {
                Deck newDeck = new Deck() { name = data.name };
                account.decks.Add(newDeck);
                account.currentDeck = account.currentDeck ?? newDeck;
            }
            else
            {
                errorMessage = "A deck with the same name already exists.";
            }

            Send(new PD_Validation(data.id, errorMessage));
        }

        public void HandleAPICall(PD_AccountAddCard data)
        {
            if (!AssureConnected(data.id)) return;
            string errorMessage = "";

            //Deck deck = account.decks.Find(d => d.name == data.deckName);
            //if (deck != null)
            //{
            //    if (server.cards.Find(c => c.name == data.cardName) != null)
            //    {
            //        deck.cardNames.Add(data.cardName);
            //    }
            //    else
            //    {
            //        errorMessage = "Wrong card name.";
            //    }
            //}
            //else
            //{
            //    errorMessage = "Could not find the deck.";
            //}

            Send(new PD_Validation(data.id, errorMessage));
        }


        public void HandleAPICall(PD_AccountRenameDeck data)
        {
            if (!AssureConnected(data.id)) return;
            string errorMessage = "";

            Deck deck = account.decks.Find(d => d.name == data.oldName);
            if (deck != null)
            {
                deck.name = data.newName;
            }
            else
            {
                errorMessage = "No deck with this name was found.";
            }

            Send(new PD_Validation(data.id, errorMessage));
        }
        public void HandleAPICall(PD_AccountDeleteDeck data)
        {
            if (!AssureConnected(data.id)) return;
            string errorMessage = "";

            if (account.decks.RemoveAll(d => d.name == data.name) != 1)
            {
                errorMessage = "Wrong deck name.";
            }

            Send(new PD_Validation(data.id, errorMessage));
        }
        public void HandleAPICall(PD_AccountSetCurrentDeck data)
        {
            if (!AssureConnected(data.id)) return;
            string errorMessage = "";

            if (!account.SetCurrentDeck(data.name))
            {
                errorMessage = "No deck with this name was found.";
            };

            Send(new PD_Validation(data.id, errorMessage));
        }
        #endregion


        #region BATTLE
        public void HandleAPICall(PD_BattleStart data)
        {
            if (!AssureConnected(data.id)) return;

            if (server.battle == null || !server.battle.hasStarted)
            {
                // TODO : init battle here
                server.Broadcast(data, null);
                server.InitBattle();
            }
        }
        
        public void HandleAPICall(PD_BattleStatePlayer data)
        {
            Debug.Assert(false, "NOT IMPLEMENTED YET.");
        }
        public void HandleAPICall(PD_BattleStateMainPlayer data)
        {
            if (!AssureConnected(data.id)) return;
            Debug.Assert(false, "NOT IMPLEMENTED YET.");
        }
        public void HandleAPICall(PD_BattleStateMonster data)
        {
            if (!AssureConnected(data.id)) return;
            Debug.Assert(false, "NOT IMPLEMENTED YET.");
        }
        public void HandleAPICall(PD_BattleState data)
        {
            if (!AssureConnected(data.id)) return;

            Send(server.GetBattleState(account.name));
        }

        public void HandleAPICall(PD_BattleMonsterTurnStart data)
        {
            Debug.Assert(false, "NOT IMPLEMENTED YET.");
        }
        public void HandleAPICall(PD_BattleMonsterTurnEnd data)
        {
            Debug.Assert(false, "NOT IMPLEMENTED YET.");
        }

        public void HandleAPICall(PD_BattlePlayerTurnStart data)
        {
            Debug.Assert(false, "NOT IMPLEMENTED YET.");
        }
        public void HandleAPICall(PD_BattlePlayerTurnEnd data)
        {
            if (!AssureConnected(data.id)) return;

            server.battle.PlayerTurnEnd(data.playerName);

            server.Broadcast(data, this);
            SendValidation(data.id);
        }

        public void HandleAPICall(PD_BattleCardPlayed data)
        {
            if (!AssureConnected(data.id)) return;
            //string errorMessage = "";

            Card card = server.battle.PlayCard(data.ownerName, data.cardIndex, data.targetName, true);
            if (card != null)
            {
                card.effects.ForEach(e =>
                {
                    CardEffect effect = e as CardEffect;
                    PD_BattleCardEffect effectMessage = null;

                    switch (e)
                    {
                        case CardEffectDamage damage:
                            effectMessage = new PD_BattleCardEffectDamage { value = damage.value };
                            break;
                        case CardEffectHeal heal:
                            effectMessage = new PD_BattleCardEffectHeal { value = heal.value };
                            break;
                        case CardEffectDraw draw:
                            effectMessage = new PD_BattleCardEffectDraw { value = draw.value };
                            break;
                        //case CardEffectDiscard damage:
                            //    break;
                    }

                    data.effects.Add(effectMessage);
                });

                server.Broadcast(data, this);
            }
            else
            {
                //errorMessage = "Could not play this card";
            }

            //SendValidation(data.id, errorMessage);
        }

        public void HandleAPICall(PD_BattleCardDrawn data)
        {
            Debug.Assert(false, "NOT IMPLEMENTED YET.");
        }

        #endregion

        public bool AssureConnected(Guid id)
        {
            if (account == null || !account.connected)
            {
                Send(new PD_Validation(id, "You need to be connected to do this."));
                return false;
            }
            return true;
        }
    }
}
