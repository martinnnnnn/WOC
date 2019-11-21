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
        public bool hasStarted = false;


        public Battle(GameServer server, List<Player> players, List<Monster> monsters)
        {
            this.server = server;
            this.players.AddRange(players);
            this.monsters.AddRange(monsters);
            random = new Random();
            hasStarted = true;
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
            server.Broadcast(new PD_BattleMonsterTurnStart { startTime = DateTime.UtcNow });
            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                server.Broadcast(new PD_BattleMonsterTurnEnd { });
                await Task.Delay(TimeSpan.FromSeconds(2));
                PlayersTurnStart();
            });
        }

        List<Player> playingPlayers = new List<Player>();
        float turnTimeFactor = 1.0f;
        public void PlayersTurnStart()
        {
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
                    return card;
                }
            }

            return null;
        }
    }

}
