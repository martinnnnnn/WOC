using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WOC
{
    public class Battle : MonoBehaviour
    {
        public Player[] players;
        public int playerStartIndex = 0;
        private int currentPlayer;
        CardList cardList;

        private void Start()
        {
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

        public Player GetBiggestAggro()
        {
            Player biggestAggroPlayer = players[0];
            for (int i = 1; i < players.Length; ++i)
            {
                if (players[i].aggro > biggestAggroPlayer.aggro)
                {
                    biggestAggroPlayer = players[i];
                }
            }
            return biggestAggroPlayer;
        }
    }
}