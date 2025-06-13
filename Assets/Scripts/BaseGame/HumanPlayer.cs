using System;
using System.Collections.Generic;
using System.Linq;
using BaseGame;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;


namespace BaseGame
{
    public class HumanPlayer : Player
    {
        [SerializeField] private Button foldButton;
        [SerializeField] private Button checkButton;
        [SerializeField] private Button callButton;
        [SerializeField] private Button raiseButton;
        [SerializeField] private Button allInButton;
        List<PokerCard> playerHand = new List<PokerCard>();
        void Start()
        {
            playerHand.Add(cards[0]);
            playerHand.Add(cards[1]);

            foldButton.onClick.AddListener(Fold);
            checkButton.onClick.AddListener(Check);
        }
        void ChooseHand()
        {

        }

        public override void MakeBet(PokerGame.BettingRound bettingRound)
        {
            throw new NotImplementedException();
        }

        protected override int GetHighestCard()
        {
            throw new NotImplementedException();
        }

        protected override int FindPair()
        {
            throw new NotImplementedException();
        }

        protected override int FindTwoPair()
        {
            throw new NotImplementedException();
        }

        protected override int FindThreeOfKind()
        {
            throw new NotImplementedException();
        }

        protected override int FindStraight()
        {
            throw new NotImplementedException();
        }

        protected override string FindFlush()
        {
            throw new NotImplementedException();
        }

        protected override int FindFullHouse()
        {
            throw new NotImplementedException();
        }

        protected override int FindFourOfKind()
        {
            throw new NotImplementedException();
        }

        protected override int FindStraightFlush()
        {
            throw new NotImplementedException();
        }

        protected override bool HasRoyalFlush()
        {
            throw new NotImplementedException();
        }
    }
}