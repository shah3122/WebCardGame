using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProperties : MonoBehaviour
{
    public List<Card> deck = new List<Card>();
    public int level1Score;
    public int ID;
    public bool isBot;
    public void SetUp(Player player)
    {
        deck = player.deck;
        level1Score = player.level1Score;
        ID = player.ID;
    }
}
