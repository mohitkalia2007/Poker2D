using System;
using System.Collections.Generic;
using System.Linq;
using BaseGame;
using JetBrains.Annotations;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.Threading.Tasks;


namespace BaseGame
{
    public class AlgorithmPlayer : Player 
    {
        public GameObject gameObject;
        PokerGame game;
        void Start()
        {
            game = GameObject.FindGameObjectWithTag("GameController").GetComponent<PokerGame>();
        }
        public override async Task<int> MakeBet(PokerGame.BettingRound bettingRound, int minimum, Pot pots)
        {
            // Handle preflop differently
            if (bettingRound == PokerGame.BettingRound.PreFlop)
            {
                // Simple preflop strategy
                double random = UnityEngine.Random.value;
                if (random < 0.2) // 20% chance to fold weak hands
                {
                    Fold();
                    return 0;
                }
                else
                {
                    Call(minimum);
                    return minimum;
                }
            }

            double ehs = EffectiveHS(bettingRound);
            int potSize = pots.Amount;
            int minBet = minimum;
            int betAmount = 0;
            
            // More conservative thresholds and betting amounts
            if (ehs > 0.8) // Very strong hand (reduced from 0.95)
            {
                betAmount = Math.Min(Balance, Math.Max(minBet, potSize / 2));
                if (betAmount > Balance / 2) // Prevent too aggressive betting
                {
                    betAmount = minBet;
                    Call(betAmount);
                }
                else
                {
                    Raise(betAmount);
                }
            }
            else if (ehs > 0.6) // Strong hand (reduced from 0.75)
            {
                if (minBet < Balance / 4) // Only call if bet is reasonable
                {
                    betAmount = minBet;
                    Call(betAmount);
                }
                else
                {
                    Fold();
                    betAmount = 0;
                }
            }
            else if (ehs > 0.4) // Medium strength hand (reduced from 0.5)
            {
                if (minBet == 0)
                {
                    Check();
                    betAmount = 0;
                }
                else if (potSize > (minBet * 3) && minBet < Balance / 5)
                {
                    betAmount = minBet;
                    Call(betAmount);
                }
                else
                {
                    Fold();
                    betAmount = 0;
                }
            }
            else // Weak hand
            {
                if (minBet == 0)
                {
                    Check();
                    betAmount = 0;
                }
                else
                {
                    Fold();
                    betAmount = 0;
                }
            }

            await Task.Delay(500);
            Debug.Log($"AI player bet: {betAmount}, EHS: {ehs}");
            return betAmount;
        }
        double EffectiveHS(PokerGame.BettingRound bettingRound) { //Returns a percentile value from 0-1 compared to all other posible hands
            if (bettingRound == PokerGame.BettingRound.PreFlop) return CurrHS();
            if (bettingRound == PokerGame.BettingRound.Flop) return CurrHS() * (1 - Potential()[0]) + (1 - CurrHS()) * Potential()[1];
            if (bettingRound == PokerGame.BettingRound.Turn) return CurrHS() * (1 - PotentialTurn()[0]) + (1 - CurrHS()) * PotentialTurn()[1];
            if (bettingRound == PokerGame.BettingRound.River) return CurrHS();
            else throw new Exception();
        }
        double[] Potential() 
        {
            // Hand potential array, each index represents ahead, tied, and behind
            int[,] HP = new int[3, 3]; 
            int[] HPTotal = new int[3];
            int ourrank = GetHighestCard();
            int index = 0;
            int ahead = 0;
            int behind = 1;
            int tied = 2;

            PokerCard[] deck = MakeDeck(); //Inntialize an array of the deck that has removed the cards that are in the deck 
            
            for (int i = 0; i < deck.Length; i++) 
            {
                for (int j = 0; j < deck.Length; j++) // for each two cards in an oppenent's hand
                {
                    if (i == j || deck[i] == null || deck[j] == null) continue; // if the cards do not exist or are the same card just skip this case

                    List<PokerCard> oppcards = new List<PokerCard> { deck[i], deck[j] };
                    int opprank = Math.Max(GetHighestCard(game.round.knownCards), GetHighestCard(oppcards)); // get the highest rank between the oppenent's hand and the board

                    // add one tally to the index of HPTotal at the index of which that hand is
                    if (ourrank > opprank) index = ahead;
                    else if (ourrank == opprank) index = tied;
                    else index = behind;
                    HPTotal[index] += 1;

                    // All possible board cards to come in the turn and river
                    for (int a = 0; a < deck.Length; a++)
                    {
                        for (int b = 0; b < deck.Length; b++)
                        {
                            if (a == b || deck[a] == null || deck[b] == null || a == i || a == j || b == i || b == j) continue; // removing impossible cases

                            // make a possible 5-card board:
                            PokerCard[] board = new PokerCard[5];
                            int ind = 0;
                            foreach (PokerCard c in game.round.knownCards) board[ind++] = c;
                            board[3] = deck[a];
                            board[4] = deck[b];

                            // get the best rank between our cards and the board cards and also the opponent cards and our cards
                            int ourbest = Math.Max(GetHighestCard(Hand.ToList()), GetHighestCard(new List<PokerCard>(board)));
                            int oppbest = Math.Max(GetHighestCard(board.ToList()), GetHighestCard(new List<PokerCard>(oppcards)));

                            // similar to last time but now with the 2d array; 
                            // ex: currently our cards are better, but could be worse in the future then add one to HP[index, behind] where index is ahead
                            if (ourbest > oppbest) HP[index, ahead] += 1;
                            else if (ourbest == oppbest) HP[index, tied] += 1;
                            else HP[index, behind] += 1;
                        }
                    }
                }
            }

            // Ppot: were behind but moved ahead
            double Ppot = (HP[behind, ahead] + HP[behind, tied] / 2 + HP[tied, ahead] / 2) / (HPTotal[behind] + HPTotal[tied]);
            // Npot: were ahead but fell behind
            double Npot = (HP[ahead, behind] + HP[tied, behind] / 2 + HP[ahead, tied] / 2) / (HPTotal[ahead] + HPTotal[tied]);
            double[] pot = { Ppot, Npot };
            return pot;
        }
        double[] PotentialTurn() 
        {
            // Hand potential array, each index represents ahead, tied, and behind
            int[,] HP = new int[3, 3]; 
            int[] HPTotal = new int[3];
            int ourrank = GetHighestCard();
            int index = 0;
            int ahead = 0;
            int behind = 1;
            int tied = 2;

            PokerCard[] deck = MakeDeck(); //Inntialize an array of the deck that has removed the cards that are in the deck 
            
            for (int i = 0; i < deck.Length; i++) 
            {
                for (int j = 0; j < deck.Length; j++) // for each two cards in an oppenent's hand
                {
                    if (i == j || deck[i] == null || deck[j] == null) continue; // if the cards do not exist or are the same card just skip this case

                    List<PokerCard> oppcards = new List<PokerCard> { deck[i], deck[j] };
                    int opprank = Math.Max(GetHighestCard(game.round.knownCards), GetHighestCard(oppcards)); // get the highest rank between the oppenent's hand and the board

                    // add one tally to the index of HPTotal at the index of which that hand is
                    if (ourrank > opprank) index = ahead;
                    else if (ourrank == opprank) index = tied;
                    else index = behind;
                    HPTotal[index] += 1;

                    // All possible board cards to come in the river
                    for (int a = 0; a < deck.Length; a++)
                    {
                        if (deck[a] == null  || a == i || a == j ) continue; // removing impossible cases

                        // make a possible 5-card board:
                        PokerCard[] board = new PokerCard[5];
                        int ind = 0;
                        foreach (PokerCard c in game.round.knownCards) board[ind++] = c;
                        board[4] = deck[a];

                        // get the best rank between our cards and the board cards and also the opponent cards and our cards
                        int ourbest = Math.Max(GetHighestCard(Hand.ToList()), GetHighestCard(new List<PokerCard>(board)));
                        int oppbest = Math.Max(GetHighestCard(board.ToList()), GetHighestCard(new List<PokerCard>(oppcards)));

                        // similar to last time but now with the 2d array; 
                        // ex: currently our cards are better, but could be worse in the future then add one to HP[index, behind] where index is ahead
                        if (ourbest > oppbest) HP[index, ahead] += 1;
                        else if (ourbest == oppbest) HP[index, tied] += 1;
                        else HP[index, behind] += 1;
                    }
                }
            }

            // Ppot: were behind but moved ahead
            double Ppot = (HP[behind, ahead] + HP[behind, tied] / 2 + HP[tied, ahead] / 2) / (HPTotal[behind] + HPTotal[tied]);
            // Npot: were ahead but fell behind
            double Npot = (HP[ahead, behind] + HP[tied, behind] / 2 + HP[ahead, tied] / 2) / (HPTotal[ahead] + HPTotal[tied]);
            double[] pot = { Ppot, Npot };
            return pot;
        }
        double CurrHS()
        {
            PokerCard[] deck = MakeDeck();
            int ahead = 0;
            int behind = 0;
            int tied = 0;
            int ourrank = GetHighestCard();

            for (int i = 0; i < deck.Length; i++)
            {
                for (int j = 0; j < deck.Length; j++)
                {
                    if (i == j || deck[i] == null || deck[j] == null) continue;
                    List<PokerCard> oppcards = new List<PokerCard> { deck[i], deck[j] };

                    int opprank = Math.Max(GetHighestCard(game.round.knownCards), GetHighestCard(oppcards));
                    if (ourrank > opprank) ahead++;
                    else if (ourrank == opprank) tied++;
                    else behind++;
                }
            }
            return (ahead + tied / 2) / (ahead + tied + behind);
        }
        int GetHighestCard(List<PokerCard> cards)
        {
            int max = 0;
            foreach (PokerCard c in cards) if (c.GetCardNumber() > max) max = c.GetCardNumber();
            return max;
        }
        PokerCard[] MakeDeck()
        {
            PokerCard[] deck = new PokerCard[52];
            string[] suits = { "diamonds", "spades", "hearts", "clubs" };

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 13; j++)
                {
                    deck[i * j] = Instantiate(gameObject).GetComponent<PokerCard>();
                    deck[i * j].Init(j+2, suits[i]);
                }
                    
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    if (deck[i * j] == cards[0] || deck[i * j] == cards[1]) deck[i * j] = null;
                    foreach (PokerCard c in game.round.knownCards)
                        if (deck[i * j] == c) deck[i * j] = null;
                }
            }
            return deck;
        }
        public int TotalPot(Pot[] pots)
        {
            int totalPotSize = 0;
            foreach (Pot pot in pots) if (pot.EligiblePlayers.Contains(this)) totalPotSize += pot.Amount;
            return totalPotSize;
        }
        protected override int GetHighestCard() { return game.round.GetHighestCard(this); }
    }
}
