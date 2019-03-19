using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WOC
{
    public class Battle : MonoBehaviour
    {
        public Fighter[] players;
        public int playerStartIndex = 0;
        private int currentPlayer;
        CardList cardList;

        private void Start()
        {
            Network.Init();

            cardList = GetComponent<CardList>();
            //cardList.ReadFile(Application.dataPath + "/data/cards.xml");
            cardList.ReadJson(Application.dataPath + "/data/cards.json");

            currentPlayer = playerStartIndex;
            foreach (var p in players)
            {
                p.BattleInit();
            }
            players[currentPlayer].StartTurn();
        }

        private void Update()
        {
            players[currentPlayer].PlayTurn();
        }

        public void OnEndButton()
        {
            players[currentPlayer].EndTurn();
            currentPlayer = (currentPlayer + 1) % players.Length;
            players[currentPlayer].StartTurn();
        }

        public void OnAggroChange()
        {
            Player biggestAggro = GetBiggestAggro();
            foreach(var fighter in players)
            {
                fighter.OnAggroChange(biggestAggro.character);
            }
        }

        public Player GetBiggestAggro()
        {
            int biggestAggroIndex = -1;
            for (int i = 0; i < players.Length; ++i)
            {
                Player p = players[i] as Player;
                if (biggestAggroIndex == -1 || (p != null && p.aggro > (players[biggestAggroIndex] as Player).aggro))
                {
                    biggestAggroIndex = i;
                }
            }
            return players[biggestAggroIndex] as Player;
        }
    }
}