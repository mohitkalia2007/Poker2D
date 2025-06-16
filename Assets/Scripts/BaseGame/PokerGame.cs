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
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

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
    private bool gameHasStarted = false;
    void OnEnable()
    {
        HandManager.OnGameStart += OnGameStartHandler;
    }
    void OnDisable()
    {
        HandManager.OnGameStart -= OnGameStartHandler;
    }
    private async Task ProcessBettingRound(BettingRound bettingRound)
    {
        Debug.Log($"--- {bettingRound} Betting Round ---");
        currentBet = 0;
        if (bettingRound == BettingRound.PreFlop) currentBet = bigBlind;

        bool bettingComplete = false;

        while (!bettingComplete)
        {
            bettingComplete = true;
            Debug.Log($"Number of players: {players.Count}");
            
            foreach (Player player in players.ToList())
            {
                if (!player.IsTurn || player.LastAction == Player.PlayerAction.AllIn || player.LastAction == Player.PlayerAction.Fold)
                {
                    Debug.Log($"Skipping player - IsTurn: {player.IsTurn}, LastAction: {player.LastAction}");
                    continue;
                }
               
                try
                {
                    Debug.Log($"Player making bet - Current bet: {currentBet}");
                    int playerBet = await player.MakeBet(bettingRound, currentBet, pots);

                    // Wait a frame to allow Unity to process
                    await Task.Yield();
                    
                    Debug.Log($"Player bet: {playerBet}, LastAction: {player.LastAction}");

                    if (playerBet > currentBet)
                    {
                        currentBet = playerBet;
                        bettingComplete = false;
                    }
                    
                    if (player.LastAction == Player.PlayerAction.AllIn || player.LastAction == Player.PlayerAction.Fold)
                    {
                        player.IsTurn = false;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error processing player bet: {e.Message}");
                }
            }

            await Task.Delay(100);
        }
    }

    public async Task PreFlop()
    {
        Debug.Log("Preflop");
        bettingRound = BettingRound.PreFlop;
        foreach (Player p in players)
        {
            p.IsTurn = true;
            p.LastAction = Player.PlayerAction.None;
        }
        await ProcessBettingRound(BettingRound.PreFlop);
    }

    public async Task Flop()
    {
        round.NextCard();
        round.NextCard();
        round.NextCard();
        bettingRound = BettingRound.Flop;
        foreach (Player p in players)
        {
            p.IsTurn = true;
            p.LastAction = Player.PlayerAction.None;
        }
        await ProcessBettingRound(BettingRound.Flop);
    }

    public async Task Turn()
    {
        round.NextCard();
        communityManager.handCards[3].GetComponent<PokerCard>().IsFaceUp = true;
        bettingRound = BettingRound.Turn;
        foreach (Player p in players)
        {
            p.IsTurn = true;
            p.LastAction = Player.PlayerAction.None;
        }
        await ProcessBettingRound(BettingRound.Turn);
    }

    public async Task River()
    {
        round.NextCard();
        communityManager.handCards[4].GetComponent<PokerCard>().IsFaceUp = true;
        bettingRound = BettingRound.River;
        foreach (Player p in players)
        {
            p.IsTurn = true;
            p.LastAction = Player.PlayerAction.None;
        }
        await ProcessBettingRound(BettingRound.River);
    }

    private async void OnGameStartHandler()
    {
        if (!gameHasStarted)
        {
            try
            {
                gameHasStarted = true;
                Debug.Log("Game Started!");
                
                foreach (Player p in players)
                {
                    p.IsTurn = true;
                    p.LastAction = Player.PlayerAction.None;
                }

                await Task.Delay(1000);
                // Properly await each betting round
                await PreFlop();
                Debug.Log("PreFlop complete");
                
                await Task.Delay(1000);
                await Flop();
                Debug.Log("Flop complete");
                
                await Task.Delay(1000);
                await Turn();
                Debug.Log("Turn complete");
                
                await Task.Delay(1000);
                await River();
                Debug.Log("River complete");
                
                await Task.Delay(1000);
                ShowDown();
                SceneManager.LoadScene("WinnerScene");
                Debug.Log("ShowDown complete");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error in game sequence: {e.Message}\n{e.StackTrace}");
            }
        }
    }
    public void ShowDown() // reveal all cards and declare winner and split pot
    {
        Debug.Log("showdown");
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
