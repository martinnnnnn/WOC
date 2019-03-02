using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle : MonoBehaviour
{
    public Player[] players;
    public int playerStartIndex = 0;
    private int currentPlayer;
    CardList cardList;

    private void Start()
    {
        cardList = GetComponent<CardList>();
        cardList.ReadFile(Application.dataPath + "/data/cards.xml");

        //players = FindObjectsOfType<Player>();
        currentPlayer = playerStartIndex;
        foreach (var p in players)
        {
            players[currentPlayer].BattleInit();
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
}
