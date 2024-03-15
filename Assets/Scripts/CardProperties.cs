using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardProperties : MonoBehaviour
{
    public int value;
    public string suit;

    public void SetUp(Card card)
    {
        value = card.value;
        suit = card.suit;
    }
}
