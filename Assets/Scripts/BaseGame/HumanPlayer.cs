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
        public List<PokerCard> playerHand = new List<PokerCard>();
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

        public override int MakeBet(PokerGame.BettingRound bettingRound)
        {
            throw new NotImplementedException();
        }

        protected override int GetHighestCard()
        {
            throw new NotImplementedException();
        }
    }
}