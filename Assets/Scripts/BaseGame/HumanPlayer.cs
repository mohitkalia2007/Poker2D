using System;
using System.Collections.Generic;
using System.Linq;
using BaseGame;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using TMPro;
using System.Threading.Tasks;

namespace BaseGame
{
    public class HumanPlayer : Player
    {
        [SerializeField] private Button foldButton;
        [SerializeField] private Button checkButton;
        [SerializeField] private Button callButton;
        [SerializeField] private Button raiseButton;
        [SerializeField] private Button allInButton;
        [SerializeField] private TMP_InputField raiseInput;

        private bool isWaitingForAction = false;
        private int minimumBet = 0;
        private Pot currentPot;
        private PokerGame.BettingRound currentBettingRound;
        public List<PokerCard> playerHand = new List<PokerCard>();
        private TaskCompletionSource<int> betCompletionSource;

        void Start()
        {
            playerHand.Add(cards[0]);
            playerHand.Add(cards[1]);

            // Set up button listeners but disable buttons initially
            foldButton.onClick.AddListener(Fold);
            checkButton.onClick.AddListener(Check);
            callButton.onClick.AddListener(Call);
            raiseButton.onClick.AddListener(Raise);
            allInButton.onClick.AddListener(AllIn);

            SetButtonsInteractable(false);
        }

        void Update()
        {
            UpdateButtonStates();
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
            checkButton.interactable = (minimumBet == 0 || minimumBet == CurrentBet);
            callButton.interactable = (minimumBet > 0 && minimumBet > CurrentBet && Balance >= (minimumBet - CurrentBet));
            raiseButton.interactable = (Balance > minimumBet);
            allInButton.interactable = (Balance > 0);
        }

        private void SetButtonsInteractable(bool interactable)
        {
            foldButton.interactable = interactable;
            checkButton.interactable = interactable;
            callButton.interactable = interactable;
            raiseButton.interactable = interactable;
            allInButton.interactable = interactable;
        }

        public override int MakeBet(PokerGame.BettingRound bettingRound, int minimum, Pot pots)
        {
            currentBettingRound = bettingRound;
            minimumBet = minimum;
            currentPot = pots;
            isWaitingForAction = true;
            betCompletionSource = new TaskCompletionSource<int>();

            // Enable valid buttons based on the situation
            UpdateButtonStates();

            // Wait for the player's action
            Task<int> betTask = betCompletionSource.Task;
            betTask.Wait();

            return betTask.Result;
        }
        protected override int GetHighestCard()
        {
            if (playerHand.Count == 0) return 0;
            return playerHand.Max(card => card.GetCardNumber());
        }
        private void Fold()
        {
            if (!isWaitingForAction) return;
            
            LastAction = PlayerAction.Fold;
            IsTurn = false;
            isWaitingForAction = false;
            SetButtonsInteractable(false);
            betCompletionSource?.SetResult(0);
        }

        private void Check()
        {
            if (!isWaitingForAction) return;
            
            LastAction = PlayerAction.Check;
            isWaitingForAction = false;
            SetButtonsInteractable(false);
            betCompletionSource?.SetResult(CurrentBet);
        }

        private void Call()
        {
            if (!isWaitingForAction) return;
            
            int callAmount = minimumBet - CurrentBet;
            if (Balance >= callAmount)
            {
                Balance -= callAmount;
                CurrentBet += callAmount;
                currentPot.Amount += callAmount;  // Changed from raiseAmount to callAmount
                LastAction = PlayerAction.Call;
            }
            isWaitingForAction = false;
            SetButtonsInteractable(false);
            betCompletionSource?.SetResult(CurrentBet);
        }

        private void Raise()
        {
            if (!isWaitingForAction || raiseInput == null) return;

            if (int.TryParse(raiseInput.text, out int raiseAmount))
            {
                if (raiseAmount > Balance || raiseAmount <= minimumBet)
                {
                    Debug.LogWarning("Invalid raise amount");
                    return;
                }

                Balance -= raiseAmount;
                CurrentBet = raiseAmount;
                currentPot.Amount += raiseAmount;
                LastAction = PlayerAction.Raise;
                isWaitingForAction = false;
                SetButtonsInteractable(false);
                betCompletionSource?.SetResult(CurrentBet);
            }
        }

        private void AllIn()
        {
            if (!isWaitingForAction) return;

            int allInAmount = Balance;
            CurrentBet += Balance;
            currentPot.Amount += allInAmount;  // Changed from raiseAmount to allInAmount
            Balance = 0;
            LastAction = PlayerAction.AllIn;
            IsTurn = false;
            isWaitingForAction = false;
            SetButtonsInteractable(false);
            betCompletionSource?.SetResult(CurrentBet);
        }
    }
}