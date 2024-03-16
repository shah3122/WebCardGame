using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameManager : MonoBehaviour
{
    public GameObject cardPopupPrefab;
    public GameObject botPrefab;
    public GameObject playerPrefab;
    public GameObject dealer;
    public GameObject cardPrefab;
    public GameObject levelComplete;
    public GameObject scorePrefab;
    public GameObject cardHolder1;
    public GameObject cardHolder2;
    public GameObject cardHolder3;

    public Transform[] spawnPoints;
    public Transform canvas;
    public Transform scoresParent;

    public float initialPopupDurationLevel1 = 10f; 
    public float subsequentPopupDurationLevel1 = 5f; 
    public float popupDurationLevel2 = 10f;
    public float cardMovementDuration;
    public float cardMoveSpeed;
    public int numPlayers = 3;


    private List<Player> players = new List<Player>();
    private List<Card> level1Deck = new List<Card>();
    private List<Card> level2Deck = new List<Card>();
    private List<PlayerProperties> playerPropertues = new List<PlayerProperties>();

    public bool isSinglePlayer;
    

    void Start()
    {        
        for (int i = 0; i < numPlayers; i++)
        {
            players.Add(new Player());
            players[i].InitializeDeck();
            players[i].ID = i;
            if (isSinglePlayer)
            {
                if (i == 0)
                {
                    GameObject player = Instantiate(playerPrefab, spawnPoints[i].position,Quaternion.identity, canvas.transform);
                    player.GetComponent<PlayerProperties>().SetUp(players[i]);
                    player.transform.SetSiblingIndex(1);
                    player.name = "Player";
                }
                else
                {
                    GameObject bot = Instantiate(botPrefab, spawnPoints[i].position, Quaternion.identity, canvas.transform);
                    bot.GetComponent<PlayerProperties>().SetUp(players[i]);
                    bot.GetComponentInChildren<Text>().text = "Bot:" + i;
                    bot.transform.SetSiblingIndex(1);
                    bot.name = "Bot : " + i;
                }
            }            
        }      
        InitializeLevel1Deck();
        InitializeLevel2Deck();    
        StartCoroutine(Level1GameLoop());
    }

    IEnumerator Level1GameLoop()
    {
        while (level1Deck.Count > 0)
        {
            int i = 0;

            foreach (Player player in players)
            {
                Card card = player.DrawCard(level1Deck);
                string suitFolderName = card.suit; 
                string cardSpritePath = $"Cards/Level1/{suitFolderName}/{card.value}"; 
                Debug.Log("Cards : " + cardSpritePath);
                Sprite cardImage = (Resources.Load<Sprite>(cardSpritePath));
                GameObject cardObject = null;
                if (i == 0)
                {
                    cardObject = Instantiate(cardPrefab, dealer.transform.position, Quaternion.identity, cardHolder1.transform);
                    StartCoroutine(MoveCardToPlayer(cardObject, cardHolder1.transform.position));
                }   
                if (i == 1)
                {
                    cardObject = Instantiate(cardPrefab, dealer.transform.position, Quaternion.identity, cardHolder2.transform);
                    StartCoroutine(MoveCardToPlayer(cardObject, cardHolder2.transform.position));
                }
                if (i == 2)
                {
                    cardObject = Instantiate(cardPrefab, dealer.transform.position, Quaternion.identity, cardHolder3.transform);
                    StartCoroutine(MoveCardToPlayer(cardObject, cardHolder3.transform.position));
                }
                cardObject.name = card.suit + ":" + card.value;
                cardObject.GetComponent<CardProperties>().SetUp(card);
                cardObject.GetComponent<Image>().sprite = cardImage;                
                StartCoroutine(MoveCardToPlayer(cardObject, spawnPoints[i].transform.position));
                yield return new WaitForSeconds(0.25f);
                i++;
            }
            yield return new WaitForSeconds(1f);
            //initialPopupDurationLevel1 = subsequentPopupDurationLevel1;
        }
        yield return new WaitForSeconds(1f);
        CalculateLevel1Scores();
      //  GameObject cardObject = Instantiate(Resources.Load<GameObject>(cardSpritePath));

        
        //TransitionToLevel2();
        //StartCoroutine(Level2GameLoop());
    }
    IEnumerator MoveCardToPlayer(GameObject cardObject, Vector3 playerPosition)
    {
        // Get the initial and target positions
        Vector3 initialPosition = dealer.transform.position;
        Vector3 targetPosition = playerPosition;

        // Set the card's initial position
        cardObject.transform.position = initialPosition;

        // Calculate the distance between initial and target positions
        float journeyLength = Vector3.Distance(initialPosition, targetPosition);

        // Move the card to the player's position over time
        float startTime = Time.time;
        while (Time.time - startTime < cardMovementDuration)
        {
            // Calculate the distance covered and the journey fraction
            float distCovered = (Time.time - startTime) * cardMoveSpeed;
            float fracJourney = distCovered / journeyLength;

            // Move the card using Lerp
            cardObject.transform.position = Vector3.Lerp(initialPosition, new Vector3(targetPosition.x, targetPosition.y-23f, targetPosition.z), fracJourney);

            // Wait for the next frame
            yield return null;
        }

        // Ensure the card reaches the player's position
        cardObject.transform.position = new Vector3(targetPosition.x, targetPosition.y - 23f, targetPosition.z);

        // Add the card to the player's hand
        //drawnCard.GetPlayer().AddCard(cardObject.GetComponent<Card>());
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
            level2Deck.Add(new Card(15, "J")); // Jester
            level2Deck.Add(new Card(14, "A")); // Ace
            level2Deck.Add(new Card(13, "K")); // King
            level2Deck.Add(new Card(12, "Q")); // Queen
            level2Deck.Add(new Card(11, "J")); // Jack
        }
    }

    List<Card> CreateRandomCards()
    {
        List<Card> randomCards = new List<Card>();
        int numCards = 10; 
        string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
        for (int i = 0; i < numCards; i++)
        {

            string randomSuit = suits[UnityEngine.Random.Range(0, suits.Length)];
            int randomValue = UnityEngine.Random.Range(2, 11);
            while (CheckCardExistence(randomValue, randomSuit, randomCards) || CheckCardExistence(randomValue, randomSuit, level1Deck))
            {
                randomSuit = suits[UnityEngine.Random.Range(0, suits.Length)];
                randomValue = UnityEngine.Random.Range(2, 11);
            }
            
            randomCards.Add(new Card(randomValue, randomSuit));
        }
        return randomCards;
    }

    public bool CheckCardExistence(int valueNew,string suitNew,List<Card> cardsList)
    {
        bool cardFound = false;
        foreach(Card card in cardsList)
        {
            if(card.suit == suitNew && card.value == valueNew)
            {
                cardFound = true;
            }
        }
        return cardFound;
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
        levelComplete.SetActive(true);

        foreach (Player player in players)
        {
            player.CalculateLevel1Score();
            PlayerProperties playerNew = FindPlayerbyID(player);
            playerNew.SetUp(player);
            GameObject score = Instantiate(scorePrefab, scoresParent);
            score.GetComponent<ScorePrefab>().SetUpScorePrefab(playerNew.name, playerNew.level1Score);
        }
        PlayerProperties winner = Findwinner();
        levelComplete.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text = "Winner is " + winner.name + " with score of " + winner.level1Score;
    }

    public PlayerProperties FindPlayerbyID(Player player)
    {
        PlayerProperties[] playerP = FindObjectsOfType<PlayerProperties>();
        foreach(PlayerProperties players in playerP)
        {
            if(players.ID == player.ID)
            {
                return players;
            }
        }
        return null;
    }

    public PlayerProperties Findwinner()
    {

        PlayerProperties[] playerP = FindObjectsOfType<PlayerProperties>();
        PlayerProperties winner;
        int largest = playerP[0].level1Score; // Assume 'a' is the largest initially
        winner = playerP[0];
        // Compare 'b' with current largest
        if (playerP[1].level1Score > largest)
        {
            largest = playerP[1].level1Score; // Update largest if 'b' is greater
            winner = playerP[1];
        }

        // Compare 'c' with current largest
        if (playerP[2].level1Score > largest)
        {
            largest = playerP[2].level1Score; // Update largest if 'c' is greater
            winner = playerP[2];
        }

        return winner;
    }
    void TransitionToLevel2()
    {        
        foreach (Player player in players)
        {
            player.InitializeLevel2Deck(level2Deck);
        }
    }
}


