using System;
using UnityEngine;
using System.Collections.Generic;

public class Player
{
    public List<Card> deck = new List<Card>();
    public int level1Score;
    public int ID;
    public void InitializeDeck()
    {
        deck.AddRange(CreateRandomCards());
    }

    public Card DrawCard(List<Card> deck)
    {
        if(deck.Count <= 0)
        {
            return null;
        }
        int index = UnityEngine.Random.Range(0, deck.Count);
        Card drawnCard = deck[index];
        deck.RemoveAt(index);
        this.deck.Add(drawnCard);
        return drawnCard;
        
    }

    public void CalculateLevel1Score()
    {
        foreach (Card card in deck)
        {
            level1Score += card.value;
        }

        Debug.Log("\n Player : " + level1Score);
    }

    public void InitializeLevel2Deck(List<Card> level2Deck)
    {
        
        int numCards = level1Score / 10; 
        for (int i = 0; i < numCards; i++)
        {
            DrawCardFromLevel2Deck(level2Deck);
        }
        deck.Add(new Card(11, "J")); 
    }

    public void DrawCardsBasedOnLevel1Score(List<Card> level2Deck)
    {
       
        int numCards = level1Score / 10; 
        for (int i = 0; i < numCards; i++)
        {
            DrawCardFromLevel2Deck(level2Deck);
        }
    }

    void DrawCardFromLevel2Deck(List<Card> level2Deck)
    {
        if(level2Deck.Count == 0)
        {
            return;
        }
        int index = UnityEngine.Random.Range(0, level2Deck.Count);
        Debug.Log("level2Deck : " + level2Deck.Count);
        Debug.Log("index : " + index);
        Card drawnCard = level2Deck[index];
        level2Deck.RemoveAt(index);
        this.deck.Add(drawnCard);
    }
    public Card PlayCard()
    {
        return deck[0];
    }

    public void UpdateDeck(List<Card> level2Deck)
    {
    }

    List<Card> CreateRandomCards()
    {
        List<Card> randomCards = new List<Card>();
        int numCards = UnityEngine.Random.Range(2, 11);
        string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
        for (int i = 0; i < numCards; i++)
        {
            string randomSuit = suits[UnityEngine.Random.Range(0, suits.Length)];
            int randomValue = UnityEngine.Random.Range(2, 11);
            randomCards.Add(new Card(randomValue, randomSuit));
            Debug.Log("Random Card : " + randomValue + " "+ randomSuit);
        }
        
        return randomCards;

    }
}
public class Card
{
    public int value;
    public string suit;

    public Card(int value, string suit)
    {
        this.value = value;
        this.suit = suit;
    }
}