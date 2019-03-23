using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WOC
{
    public class Battle : MonoBehaviour
    {
        public BasePlayer[] players;
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

        bool isInit = false;
        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.H) && !isInit)
            //{
            //    isInit = true;
            //    cardList = GetComponent<CardList>();
            //    //cardList.ReadFile(Application.dataPath + "/data/cards.xml");
            //    cardList.ReadJson(Application.dataPath + "/data/cards.json");

            //    currentPlayer = playerStartIndex;
            //    foreach (var p in players)
            //    {
            //        p.BattleInit();
            //    }
            //    players[currentPlayer].StartTurn();
            //}
            //if (isInit)
            //{
            players[currentPlayer].PlayTurn();
            //}
        }

        public void OnEndButton()
        {
            players[currentPlayer].EndTurn();
            currentPlayer = (currentPlayer + 1) % players.Length;
            players[currentPlayer].StartTurn();
        }

        public void OnAggroChange()
        {
            BasePlayer biggestAggro = GetBiggestAggro();
            foreach(var fighter in players)
            {
                fighter.OnAggroChange(biggestAggro.character);
            }
        }

        public BasePlayer GetBiggestAggro()
        {
            int biggestAggro = -1;
            BasePlayer biggest = null;
            for (int i = 0; i < players.Length; ++i)
            {
                Aggro aggro = players[i].GetComponent<Aggro>();
                if (aggro != null)
                {
                    if (aggro.Value > biggestAggro)
                    {
                        biggest = players[i];
                    }
                }
            }
            return biggest;
        }
    }
}