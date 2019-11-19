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
        public List<BattlePlayer> players = new List<BattlePlayer>();
        public List<Monster> monsters = new List<Monster>();

        public float timeRemaining = 60;
        public Action MonsterTurnStarted;

        public Random random;
        public bool hasStarted = false;


        public Battle(GameServer server, List<BattlePlayer> players, List<Monster> monsters)
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
            MonsterTurnStarted?.Invoke();
            PlayersTurnStart();
        }

        List<BattlePlayer> playingPlayers = new List<BattlePlayer>();
        public void PlayersTurnStart()
        {
            foreach (var player in players)
            {
                player.InitTurn(monsters[0].baseTime);
                playingPlayers.Add(player);
            }
        }

        public void PlayerTurnEnd(BattlePlayer player)
        {
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

        public bool PlayCard(string playerName, int cardIndex, string targetName, bool force = false)
        {
            var player = playingPlayers.Find(p => p.name == playerName);
            if (player != null)
            {
                return player.PlayCard(cardIndex, targetName, force);
            }

            return false;
        }
    }

}
