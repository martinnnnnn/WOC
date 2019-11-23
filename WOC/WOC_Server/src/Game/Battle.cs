using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using WOC_Core;

namespace WOC_Server
{
    public class Battle
    {
        public GameServer server;
        public List<Player> players = new List<Player>();
        public List<Monster> monsters = new List<Monster>();

        public float timeRemaining = 60;

        public Random random;
        public bool isOngoing = false;
        public int turnCount;

        public Battle(GameServer server, List<Player> players, List<Monster> monsters)
        {
            this.server = server;
            this.players.AddRange(players);
            this.monsters.AddRange(monsters);
            random = new Random();
            isOngoing = true;
            turnCount = 0;
            this.players.ForEach(p =>
            {
                p.Init(this);
            });
        }


        public void Update(float dt)
        {
            playingPlayers.ForEach(p => p.Update(dt));
        }

        public void MonstersTurnStart()
        {
            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                server.Broadcast(new PD_BattleMonsterTurnStart { startTime = DateTime.UtcNow });

                Random rand = new Random();
                foreach (var monster in monsters)
                {
                    if (turnCount % monster.period == 0)
                    {
                        var player = players[rand.Next(0, players.Count)];
                        server.Broadcast(new PD_BattleMonsterAttack
                        {
                            monster = monster.name,
                            target = player.name,
                            damage = monster.damage
                        });

                        player.life -= monster.damage;
                    }
                }

                HandleDamage();
                await Task.Delay(TimeSpan.FromSeconds(5));
                server.Broadcast(new PD_BattleMonsterTurnEnd { });
                await Task.Delay(TimeSpan.FromSeconds(5));
                PlayersTurnStart();
            });
        }

        List<Player> playingPlayers = new List<Player>();
        float turnTimeFactor = 1.0f;
        public void PlayersTurnStart()
        {
            turnCount++;
            foreach (var player in players)
            {
                player.InitTurn(monsters[0].baseTime * turnTimeFactor);
                playingPlayers.Add(player);
            }
            turnTimeFactor += 0.5f;
        }

        public void PlayerTurnEnd(Player player)
        {
            player.EndTurn();
            playingPlayers.Remove(player);
            if (playingPlayers.Count == 0)
            {
                MonstersTurnStart();
            }
        }

        public void PlayerTurnEnd(string playerName)
        {
            PlayerTurnEnd(playingPlayers.Find(p => p.name == playerName));
        }

        public Card PlayCard(string playerName, int cardIndex, string targetName, bool force = false)
        {
            var player = playingPlayers.Find(p => p.name == playerName);
            if (player != null)
            {
                Card card = player.hand.Get(cardIndex);
                Console.WriteLine("[BATTLE] played card {0} with cost {1}", card.name, card.timeCost);
                if (player.PlayCard(cardIndex, targetName, force))
                {
                    HandleDamage();
                    return card;
                }
            }

            return null;
        }

        List<Combatant> dieded = new List<Combatant>();
        internal void HandleDamage()
        {
            players.FindAll(p => p.life <= 0).ForEach(dead =>
            {
                server.Broadcast(new PD_BattlePlayerDead { playerName = dead.name });
                dieded.Add(dead);
            });
            var deadPlayers = players.RemoveAll(p => p.life <= 0);

            monsters.FindAll(m => m.life <= 0).ForEach(dead =>
            {
                server.Broadcast(new PD_BattleMonsterDead { monsterName = dead.name });
                dieded.Add(dead);
            });
            var deadMonsters = monsters.RemoveAll(m => m.life <= 0);

            if (monsters.Count == 0)
            {
                server.Broadcast(new PD_BattleEnd { victory = true });
                isOngoing = false;
            }
            else if (players.Count == 0)
            {
                server.Broadcast(new PD_BattleEnd { victory = false });
                isOngoing = false;
            }

            if (!isOngoing)
            {
                server.HandleBattleEnd();
            }
        }
    }

}
