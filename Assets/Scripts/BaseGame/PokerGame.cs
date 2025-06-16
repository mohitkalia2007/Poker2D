using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using BaseGame;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.Splines;

public class PokerGame : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int bigBlind;
    public int humanPlayerCount;
    public int algorithmPlayerCount;
    public GameObject algorithmPlayer;
    [SerializeField] GameObject algorithmPlayerManager;
    public GameObject humanPlayer;
    public GameObject pokerCard;
    public Round round;
    public List<Player> players = new List<Player>();
    public List<GameObject> managers = new List<GameObject>();
    public Pot pots = new Pot();
    PokerCard[,] deck = new PokerCard[4, 13];
    string[] suits = { "diamonds", "spades", "hearts", "clubs" };
    private readonly System.Random random = new System.Random();
    public int currentBet = 0;
    public bool preflopDone = false;
    public bool flopDone = false;
    public bool turnDone = false;
    public bool riverDone = false;
    public bool showdownDone = false;
    [SerializeField] private List<SplineContainer> availableSplines = new List<SplineContainer>(); // Drag your splines here
    public CommunityManager communityManager;
    public enum BettingRound
    {
        PreFlop, Flop, Turn, River
    }
    public BettingRound bettingRound { get; private set; }
    void Start()
    {
        communityManager = GameObject.Find("CommunityManager").GetComponent<CommunityManager>();
        availableSplines.Clear();
        humanPlayer = GameObject.FindWithTag("Player");
        players.Add(humanPlayer.GetComponent<HumanPlayer>());

        for (int i = 1; i <= 5; i++)
        {
            string splineName = $"AlgorithmSpline {i}";
            GameObject splineObj = GameObject.Find(splineName);

            if (splineObj != null)
            {
                SplineContainer spline = splineObj.GetComponent<SplineContainer>();
                if (spline != null)
                {
                    availableSplines.Add(spline);
                }
                else
                {
                    Debug.LogError($"SplineContainer component missing on {splineName}");
                }
            }
            else
            {
                Debug.LogError($"Could not find GameObject named {splineName}");
            }
        }
        // Verify we have enough splines before creating AI players
        if (availableSplines.Count < algorithmPlayerCount)
        {
            Debug.LogError($"Not enough splines found! Need {algorithmPlayerCount} but only found {availableSplines.Count}");
            return;
        }
        // Create AI players
        for (int i = 0; i < algorithmPlayerCount; i++)
        {
            GameObject newAlgorithmPlayer = Instantiate(algorithmPlayer);
            GameObject newManager = Instantiate(algorithmPlayerManager);
            AlgorithmManager manager = newManager.GetComponent<AlgorithmManager>();

            if (manager != null)
            {
                manager.splineContainer = availableSplines[i];
                manager.aiPlayerObject = newAlgorithmPlayer;

                if (manager.splineContainer == null)
                {
                    Debug.LogError($"Failed to assign spline {i} to manager");
                }
            }

            players.Add(newAlgorithmPlayer.GetComponent<AlgorithmPlayer>());
            managers.Add(newManager);
        }

        foreach (Player player in players)
        {
            player.Balance = InputFieldHandler.savedInteger;
        }
        for (int i = 0; i < deck.GetLength(0); i++)
        {
            for (int j = 0; j < deck.GetLength(1); j++)
            {
                GameObject newCard = Instantiate(pokerCard);
                PokerCard cardComponent = newCard.GetComponent<PokerCard>();
                cardComponent.Init(j + 2, suits[i]);
                deck[i, j] = cardComponent;
            }
        }
        foreach (Player player in players)
        {
            int randomSuit;
            int randomRank;
            PokerCard[] hand = new PokerCard[2];
            do
            {
                randomSuit = random.Next(0, 4);
                randomRank = random.Next(0, 13);
                if (deck[randomSuit, randomRank] != null)
                {
                    hand[0] = deck[randomSuit, randomRank];
                    deck[randomSuit, randomRank] = null;
                    break;
                }
            } while (true);
            do
            {
                randomSuit = random.Next(0, 4);
                randomRank = random.Next(0, 13);
                if (deck[randomSuit, randomRank] != null)
                {
                    hand[1] = deck[randomSuit, randomRank];
                    deck[randomSuit, randomRank] = null;
                    break;
                }
            } while (true);
            player.Hand = hand;
        }
        PokerCard[] houseHand = new PokerCard[5];
        for (int i = 0; i < 5; i++)
        {
            do
            {
                int randomSuit = random.Next(0, 4);
                int randomRank = random.Next(0, 13);
                if (deck[randomSuit, randomRank] != null)
                {
                    houseHand[i] = deck[randomSuit, randomRank];
                    deck[randomSuit, randomRank] = null;
                    break;
                }
            } while (true);
        }
        round.HouseHand = houseHand;
        foreach (GameObject m in managers)
        {
            m.GetComponent<AlgorithmManager>().DisplayAIHand();
        }
        foreach (PokerCard c in round.HouseHand)
        {
            communityManager.GetComponent<CommunityManager>().DrawCard(c.GetSuit(), c.GetCardNumber());
        }
    }
    private bool keyNotPress = true;
    void Update()
    {
        // Check if the current betting round is complete and move to the next one
        if (Input.GetKeyDown(KeyCode.Space) && keyNotPress)
        {
            Debug.Log("PreFlop");
            PreFlop();
            keyNotPress = false;
            preflopDone = true;
        }
        else if (preflopDone && !flopDone && !turnDone && !riverDone && !showdownDone)
        {
            Debug.Log("Flop");
            Flop();
            flopDone = true;
        }
        else if (preflopDone && flopDone && !turnDone && !riverDone && !showdownDone)
        {
            Debug.Log("Turn");
            Turn();
            turnDone = true;
        }
        else if (preflopDone && flopDone && turnDone && !riverDone && !showdownDone)
        {
            Debug.Log("River");
            River();
            riverDone = true;
        }
        else if (preflopDone && flopDone && turnDone && riverDone && !showdownDone)
        {
            ShowDown();
            showdownDone = true;
        }
    }
    private void ProcessBettingRound(BettingRound bettingRound)
    {
        Console.WriteLine($"--- {bettingRound} Betting Round ---");
        currentBet = 0;
        if (bettingRound == BettingRound.PreFlop) currentBet = bigBlind;

        bool bettingComplete = false;

        while (!bettingComplete)
        {
            bettingComplete = true;
            Debug.Log(players.Count());

            foreach (Player player in players)
            {
                Debug.Log("player");
                if (!player.IsTurn || player.LastAction == Player.PlayerAction.AllIn) continue;  // Skip players who have folded or are all-in
                Debug.Log("player");

                player.MakeBet(bettingRound, currentBet, pots);  // This will trigger the player's UI or AI decision 
                Debug.Log(player.CurrentBet);
                Debug.Log(player.LastAction);
                // Handle the player's decision (this will be received through events/callbacks)
                if (player.CurrentBet > currentBet)
                {
                    currentBet = player.CurrentBet;
                    bettingComplete = false;  // Need another round if someone raises
                }
                if (player.LastAction == Player.PlayerAction.AllIn)
                {
                    player.IsTurn = false;
                }
            }
        }
    }
    public void PreFlop() //betting round before any cards are revealed
    {
        bettingRound = BettingRound.PreFlop;
        ProcessBettingRound(BettingRound.PreFlop);
    }
    public void Flop() //betting round after first three cards are revealed
    {
        round.NextCard();
        round.NextCard();
        round.NextCard();
        bettingRound = BettingRound.Flop;
        ProcessBettingRound(BettingRound.Flop);
    }
    public void Turn() //betting round before final card reveal
    {
        round.NextCard();
        communityManager.handCards[3].GetComponent<PokerCard>().IsFaceUp = true;
        bettingRound = BettingRound.Turn;
        ProcessBettingRound(BettingRound.Turn);   

    }
    public void River() //final betting round
    {
        round.NextCard();
        communityManager.handCards[4].GetComponent<PokerCard>().IsFaceUp = true;
        bettingRound = BettingRound.River;
        ProcessBettingRound(BettingRound.River);
    
    }
    public void ShowDown() // reveal all cards and declare winner and split pot
    {
        foreach (Player m in players)
        {
            if (m is AlgorithmPlayer player)
            {
                player.Hand[0].IsFaceUp = true;
                player.Hand[1].IsFaceUp = true;   
            }
        }
        foreach (var player in players)
        {
            player.Stack = 0;
            player.CurrentBet = 0;
        }
        pots = null;
    }
}
