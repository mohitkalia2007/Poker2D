using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseGame;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

namespace BaseGame
{
    public class HumanPlayer : Player
    {
        [SerializeField] private Button foldButton;
        [SerializeField] private Button checkButton;
        [SerializeField] private Button callButton;
        [SerializeField] private Button allInButton;
        [SerializeField] private TMP_InputField raiseInput; // Keep only the input field for raise
        
        private TaskCompletionSource<int> betTaskSource;
        private bool isWaitingForAction = false;
        private int minimumBet = 0;
        private Pot currentPot;
        private PokerGame.BettingRound currentBettingRound;
        public List<PokerCard> playerHand = new List<PokerCard>();

        void Start()
        {
            playerHand.Add(cards[0]);
            playerHand.Add(cards[1]);

            // Set up button listeners (remove raise button listener)
            foldButton.onClick.AddListener(Fold);
            checkButton.onClick.AddListener(Check);
            callButton.onClick.AddListener(Call);
            allInButton.onClick.AddListener(AllIn);

            // Add input field enter/submit handler
            raiseInput.onSubmit.AddListener(HandleRaiseInput);

            SetButtonsInteractable(false);
        }

        public override async Task<int> MakeBet(PokerGame.BettingRound bettingRound, int minimum, Pot pots)
        {
            currentBettingRound = bettingRound;
            minimumBet = minimum;
            currentPot = pots;
            isWaitingForAction = true;
            betTaskSource = new TaskCompletionSource<int>();

            // Enable valid buttons based on the situation
            UpdateButtonStates();

            Debug.Log($"Waiting for human player action. Current bet: {minimum}");
            
            // Wait for the player to make a decision
            int result = await betTaskSource.Task;
            Debug.Log($"Player made decision: {LastAction}, bet amount: {result}");
            return result;
        }

        private void UpdateButtonStates()
        {
            if (!isWaitingForAction)
            {
                SetButtonsInteractable(false);
                return;
            }

            // Enable/disable buttons based on valid actions
            foldButton.interactable = true;
            checkButton.interactable = minimumBet == 0 || minimumBet == CurrentBet;
            callButton.interactable = minimumBet > 0 && minimumBet > CurrentBet && Balance >= (minimumBet - CurrentBet);
            allInButton.interactable = Balance > 0;
            
            // Enable/disable raise input field instead of button
            raiseInput.interactable = Balance > minimumBet;
        }

        private void SetButtonsInteractable(bool interactable)
        {
            foldButton.interactable = interactable;
            checkButton.interactable = interactable;
            callButton.interactable = interactable;
            allInButton.interactable = interactable;
            raiseInput.interactable = interactable;
        }

        private void Fold()
        {
            if (!isWaitingForAction) return;
            
            LastAction = PlayerAction.Fold;
            IsTurn = false;
            isWaitingForAction = false;
            SetButtonsInteractable(false);
            betTaskSource?.SetResult(0);
            Debug.Log("Player folded");
        }

        private void Check()
        {
            if (!isWaitingForAction) return;
            
            LastAction = PlayerAction.Check;
            isWaitingForAction = false;
            SetButtonsInteractable(false);
            betTaskSource?.SetResult(CurrentBet);
            Debug.Log("Player checked");
        }

        private void Call()
        {
            if (!isWaitingForAction) return;
            
            int callAmount = minimumBet - CurrentBet;
            if (Balance >= callAmount)
            {
                Balance -= callAmount;
                CurrentBet += callAmount;
                currentPot.Amount += callAmount;
                LastAction = PlayerAction.Call;
            }
            isWaitingForAction = false;
            SetButtonsInteractable(false);
            betTaskSource?.SetResult(CurrentBet);
            Debug.Log($"Player called {callAmount}");
        }

        private void HandleRaiseInput(string value)
        {
            if (!isWaitingForAction || string.IsNullOrEmpty(value)) return;

            if (int.TryParse(value, out int raiseAmount))
            {
                if (raiseAmount > Balance)
                {
                    Debug.LogWarning("Cannot raise more than balance");
                    raiseInput.text = "";
                    return;
                }
                
                if (raiseAmount <= minimumBet)
                {
                    Debug.LogWarning($"Must raise more than minimum bet ({minimumBet})");
                    raiseInput.text = "";
                    return;
                }

                Balance -= raiseAmount;
                CurrentBet = raiseAmount;
                currentPot.Amount += raiseAmount;
                LastAction = PlayerAction.Raise;
                isWaitingForAction = false;
                SetButtonsInteractable(false);
                betTaskSource?.SetResult(CurrentBet);
                Debug.Log($"Player raised to {raiseAmount}");
            }
            else
            {
                Debug.LogWarning("Invalid raise amount");
                raiseInput.text = "";
            }
        }

        private void AllIn()
        {
            if (!isWaitingForAction) return;

            int allInAmount = Balance;
            CurrentBet += Balance;
            currentPot.Amount += allInAmount;
            Balance = 0;
            LastAction = PlayerAction.AllIn;
            IsTurn = false;
            isWaitingForAction = false;
            SetButtonsInteractable(false);
            betTaskSource?.SetResult(CurrentBet);
            Debug.Log($"Player went all in with {allInAmount}");
        }

        protected override int GetHighestCard()
        {
            if (playerHand.Count == 0) return 0;
            return playerHand.Max(card => card.GetCardNumber());
        }
    }
}