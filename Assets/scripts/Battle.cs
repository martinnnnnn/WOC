using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle : MonoBehaviour
{
    public Player[] players;
    public int playerStartIndex = 0;
    private int currentPlayer;

    private void Start()
    {
        //players = FindObjectsOfType<Player>();
        currentPlayer = playerStartIndex;
        players[currentPlayer].StartTurn();
    }

    bool startgame = false;
    private void Update()
    {
        {
            players[currentPlayer].PlayTurn();
        }
    }

    public void OnStartButton()
    {
        startgame = true;
    }

    public void OnEndButton()
    {
        players[currentPlayer].EndTurn();
        currentPlayer = (currentPlayer + 1) % players.Length;
        players[currentPlayer].StartTurn();
    }
}
