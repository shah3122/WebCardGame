using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject cardPopupPrefab;
    public float initialPopupDurationLevel1 = 10f; 
    public float subsequentPopupDurationLevel1 = 5f; 
    public float popupDurationLevel2 = 10f; 
    public int numPlayers = 3;

    private List<Player> players = new List<Player>();
    private List<Card> level1Deck = new List<Card>();
    private List<Card> level2Deck = new List<Card>();

    void Start()
    {
        
        for (int i = 0; i < numPlayers; i++)
        {
            players.Add(new Player());
            players[i].InitializeDeck();
        }
      
        InitializeLevel1Deck();
        InitializeLevel2Deck();    
        StartCoroutine(Level1GameLoop());
    }

    IEnumerator Level1GameLoop()
    {
        while (level1Deck.Count > 0)
        {
            foreach (Player player in players)
            {
                player.DrawCard(level1Deck);
            }
            ShowCardPopup(initialPopupDurationLevel1);
            yield return new WaitForSeconds(initialPopupDurationLevel1);
            initialPopupDurationLevel1 = subsequentPopupDurationLevel1;
        }
        CalculateLevel1Scores();

        TransitionToLevel2();
        StartCoroutine(Level2GameLoop());
    }

    IEnumerator Level2GameLoop()
    {
        while (level2Deck.Count > 0)
        {
            foreach (Player player in players)
            {
                player.DrawCardsBasedOnLevel1Score(level2Deck);
            }

            ShowCardPopup(popupDurationLevel2);
            yield return new WaitForSeconds(popupDurationLevel2);

            List<Card> playedCards = new List<Card>();
            foreach (Player player in players)
            {
                Card playedCard = player.PlayCard();
                playedCards.Add(playedCard);
            }

            DetermineWinners(playedCards);

            foreach (Player player in players)
            {
                player.UpdateDeck(level2Deck);
            }
        }

        DetermineGameWinner();
    }

    void InitializeLevel1Deck()
    {
        for (int i = 0; i < numPlayers; i++)
        {
            level1Deck.AddRange(CreateRandomCards());
        }
    }

    void InitializeLevel2Deck()
    {
        for (int i = 0; i < 4; i++)
        {
            level2Deck.Add(new Card(11, "J")); // Jester
            level2Deck.Add(new Card(14, "A")); // Ace
            level2Deck.Add(new Card(13, "K")); // King
            level2Deck.Add(new Card(12, "Q")); // Queen
        }
    }

    List<Card> CreateRandomCards()
    {
        List<Card> randomCards = new List<Card>();
        int numCards = UnityEngine.Random.Range(2, 11); 
        string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
        for (int i = 0; i < numCards; i++)
        {
            string randomSuit = suits[UnityEngine.Random.Range(0, suits.Length)];
            int randomValue = UnityEngine.Random.Range(2, 15); 
            randomCards.Add(new Card(randomValue, randomSuit));
        }
        return randomCards;
    }

    void ShowCardPopup(float duration)
    {
        Debug.Log("Showing Card PopUp");
    }

    void DetermineWinners(List<Card> playedCards)
    {
        Debug.Log("DetermineWinners" + playedCards);
    }

    void DetermineGameWinner()
    {
        Debug.Log("DetermineGameWinner");
    }

    void CalculateLevel1Scores()
    {       
        foreach (Player player in players)
        {
            player.CalculateLevel1Score();
        }
    }

    void TransitionToLevel2()
    {        
        foreach (Player player in players)
        {
            player.InitializeLevel2Deck(level2Deck);
        }
    }
}


