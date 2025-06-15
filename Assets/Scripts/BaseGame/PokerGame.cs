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
    private bool preflopDone = false;
    private bool flopDone = false;
    private bool turnDone = false;
    private bool riverDone = false;
    private bool showdownDone = false;
    public enum BettingRound
    {
        PreFlop, Flop, Turn, River
    }
    public BettingRound bettingRound { get; private set; }
    void Start()
    {
        humanPlayer = GameObject.FindWithTag("Player");
        players.Add(humanPlayer.GetComponent<HumanPlayer>());

        for (int i = 0; i < algorithmPlayerCount; i++)
        {
            GameObject newAlgorithmPlayer = Instantiate(algorithmPlayer);
            GameObject newManager = Instantiate(algorithmPlayerManager);  // Create new variable for instance
            if (newManager != null && newManager.GetComponent<AlgorithmManager>() != null)
            {
                newManager.GetComponent<AlgorithmManager>().aiPlayerObject = newAlgorithmPlayer;
            }
            players.Add(newAlgorithmPlayer.GetComponent<AlgorithmPlayer>());
            managers.Add(newManager);
        }

        foreach (Player player in players)
        {
            player.Balance = InputFieldHandler.savedInteger;
            Debug.Log("yeah");
            Debug.Log(player.Balance);
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
        Debug.Log(players.Count());
        foreach (GameObject m in managers)
        {
            m.GetComponent<AlgorithmManager>().DisplayAIHand();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && !preflopDone)
        {
            PreFlop();
            preflopDone = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && preflopDone && !flopDone)
        {
            Flop();
            flopDone = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && flopDone && !turnDone)
        {
            Turn();
            turnDone = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && turnDone && !riverDone)
        {
            River();
            riverDone = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) && riverDone && !showdownDone)
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

            foreach (Player player in players)
            {
                if (!player.IsTurn || player.LastAction == Player.PlayerAction.AllIn) continue;  // Skip players who have folded or are all-in

                player.MakeBet(bettingRound, currentBet, pots);  // This will trigger the player's UI or AI decision 

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
    bool AnyoneIsAllIn()
    {
        int count = 0;
        foreach (var player in players) if (player.LastAction == Player.PlayerAction.AllIn) count++; 
        return count > 0;
    }
    void PreFlop() //betting round before any cards are revealed
    {
        bettingRound = BettingRound.PreFlop;
        ProcessBettingRound(BettingRound.PreFlop);
        round.NextCard();
        round.NextCard();
        round.NextCard();
    }
    void Flop() //betting round after first three cards are revealed
    {
        bettingRound = BettingRound.Flop;
        ProcessBettingRound(BettingRound.Flop);
        round.NextCard();
    }
    void Turn() //betting round before final card reveal
    {
        bettingRound = BettingRound.Turn;
        ProcessBettingRound(BettingRound.Turn);
        round.NextCard();
    }
    void River() //final betting round
    {
        bettingRound = BettingRound.River;
        ProcessBettingRound(BettingRound.River);
    }
    void ShowDown() // reveal all cards and declare winner and split pot
    {
        foreach (GameObject m in managers)
        {
            m.GetComponent<AlgorithmManager>().FlipCards();
        }
        foreach (var player in players)
        {
            player.Stack = 0;
            player.CurrentBet = 0;
        }
        pots = null;
    }
}
