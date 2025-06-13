using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BaseGame
{
    public class Round : MonoBehaviour
    {
        private readonly System.Random random = new System.Random();
        public HumanPlayer humanPlayer;
        private string[] suit = { "diamond", "spade", "heart", "club" };
        private List<Player> players;
        private PokerCard[] houseHand = new PokerCard[5];
        private int knownCardCount = 0;
        public List<PokerCard> knownCards;
        public PokerCard[] HouseHand { get { return houseHand; } set { houseHand = value; } }
        public void AddPlayer(Player player) { players.Add(player); }
        public void NextCard()
        {
            if (knownCardCount < houseHand.Length) knownCards.Add(houseHand[knownCardCount]);
            knownCards.Last().IsFaceUp = true;
        }
        public List<Player> DeclareWinner(List<Player> players)
        {
            List<int> bestHand = new List<int>();
            foreach (Player player in players)
            {
                bestHand.Add(GetBestHand(player));
            }
            int maxHand = bestHand.Max();
            List<Player> winners = players.Where((player, index) => bestHand[index] == maxHand).ToList();
            if (winners.Count() > 1)
            {
                return TieBreaker(winners, maxHand);
            }
            return winners;
        }
        private List<Player> TieBreaker(List<Player> list, int maxHand) //This method has a lot of issues but im too lazy to fix them at the moment
        {
            int max = 0;
            switch (maxHand)
            {
                case 0:
                    for (int i = 0; i < list.Count(); i++)
                    {
                        if (GetHighestCard(list.ElementAt(i)) < max) list.RemoveAt(i--);
                        max = GetHighestCard(list.ElementAt(i));
                    }
                    return list;
                case 1: case 2:
                    for (int i = 0; i < list.Count(); i++)
                    {
                        if (FindPair(list.ElementAt(i)) < max) list.RemoveAt(i--);
                        max = FindPair(list.ElementAt(i));
                    }
                    return list;
                case 3: case 6:
                    for (int i = 0; i < list.Count(); i++)
                    {
                        if (FindThreeOfKind(list.ElementAt(i)) < max) list.RemoveAt(i--);
                        max = FindThreeOfKind(list.ElementAt(i));
                    }
                    return list;
                case 4: case 8:
                    for (int i = 0; i < list.Count(); i++)
                    {
                        if (FindStraight(list.ElementAt(i)) < max) list.RemoveAt(i--);
                        max = FindStraight(list.ElementAt(i));
                    }
                    return list;
                case 5: // TECHNICALLY THIS IMPLEMENTATION FOR THIS CASE IS WRONG BUT IM LAZY AND THIS WILL WORK LIKE 80% OF THE TIME
                    for (int i = 0; i < list.Count(); i++)
                    {
                        if (GetHighestCard(list.ElementAt(i)) < max) list.RemoveAt(i--);
                        max = GetHighestCard(list.ElementAt(i));
                    }
                    return list;
                case 7:
                    for (int i = 0; i < list.Count(); i++)
                    {
                        if (FindFourOfKind(list.ElementAt(i)) < max) list.RemoveAt(i--);
                        max = FindFourOfKind(list.ElementAt(i));
                    }
                    return list;
                default: return list;
            }
        }
        public List<PokerCard> FullHand(Player player)
        {
            if (player == null) return null;
            List<PokerCard> fullHand = new List<PokerCard> { player.Hand[0], player.Hand[1] };
            if (knownCards != null)
            {
                fullHand.AddRange(knownCards);
            }
            fullHand.Sort((a, b) => a.CompareTo(b));
            return fullHand;
        }
        public int GetHighestCard(Player player)
        {
            PokerCard[] cards = player.Hand;
            return cards[0].GetCardNumber() > cards[1].GetCardNumber() ? cards[0].GetCardNumber() : cards[1].GetCardNumber();
        }
        public int FindPair(Player player)
        {
            if (player == null) return 0;
            List<PokerCard> playerHand = FullHand(player);
            if (playerHand.Count() < 2) return 0;
            int cardNum;

            for (int i = playerHand.Count() - 1; i > 1; i--)
            {
                cardNum = playerHand.ElementAt(i).GetCardNumber();
                if (cardNum == playerHand.ElementAt(i - 1).GetCardNumber()) return cardNum;
            }
            return 0;
        }
        public int FindTwoPair(Player player)
        {
            if (player == null) return 0;
            List<PokerCard> playerHand = FullHand(player);
            if (playerHand.Count() < 4) return 0;

            int firstPair = FindPair(player);
            if (firstPair == 0) return 0;

            int secPair = 0;
            for (int i = 0; i < playerHand.Count() - 2; i++)
            {
                int cardNum = playerHand.ElementAt(i).GetCardNumber();
                if (cardNum == playerHand.ElementAt(i + 1).GetCardNumber())
                {
                    firstPair = cardNum;
                    break;
                }
            }

            return (secPair != firstPair && secPair != 0) ? firstPair : 0;
        }
        public int FindThreeOfKind(Player player)
        {
            if (player == null) return 0;
            List<PokerCard> playerHand = FullHand(player);
            if (playerHand.Count() < 3) return 0;

            for (int i = playerHand.Count() - 1; i > 2; i--)
            {
                int cardNum = playerHand.ElementAt(i).GetCardNumber();
                if (cardNum == playerHand.ElementAt(i - 1).GetCardNumber() &&
                    cardNum == playerHand.ElementAt(i - 2).GetCardNumber()) return cardNum;
            }
            return 0;
        }
        public int FindStraight(Player player)
        {
            if (player == null) return 0;
            List<PokerCard> playerHand = FullHand(player);
            if (playerHand.Count() < 5) return 0;
            int len = playerHand.Count();

            int maxCard = 0;
            int count = 0;
            for (int i = playerHand.Count() - 2; i > 0; i--)
            {
                PokerCard card = playerHand.ElementAt(i);
                if (card.CompareTo(playerHand.ElementAt(i - 1)) == 1 && card.CompareTo(playerHand.ElementAt(i + 1)) == -1)
                {
                    count++;
                }
            }
            return maxCard + 4;
        }
        public string FindFlush(Player player)
        {
            if (player == null) return null;
            List<PokerCard> playerHand = FullHand(player);
            if (playerHand.Count() < 5) return null;
            int suitCount = 0;

            foreach (string s in suit)
            {
                for (int i = 0; i < playerHand.Count(); i++)
                {
                    if (playerHand.ElementAt(i).GetSuit().Equals(s)) suitCount++;
                }
                if (suitCount >= 5) return s;
            }
            return null;
        }
        public int FindFullHouse(Player player)
        {
            if (player == null) return 0;
            List<PokerCard> playerHand = FullHand(player);
            if (playerHand.Count() < 5) return 0;

            if (FindPair(player) != 0 && FindThreeOfKind(player) != 0)
            {
                int threeKind = FindThreeOfKind(player);
                int pair = 0;

                for (int i = 0; i < playerHand.Count() - 1; i++)
                {
                    int cardNum = playerHand.ElementAt(i).GetCardNumber();
                    if (playerHand.ElementAt(i).IsSameNum(playerHand.ElementAt(i + 1)))
                    {
                        pair = cardNum;
                        break;
                    }
                }
                return (pair != 0 && pair != threeKind) ? threeKind : 0;
            }
            return 0;
        }
        public int FindFourOfKind(Player player)
        {
            if (player == null) return 0;
            List<PokerCard> playerHand = FullHand(player);
            if (playerHand.Count() < 4) return 0;

            for (int i = 0; i < playerHand.Count() - 4; i++)
            {
                int cardNum = playerHand.ElementAt(i).GetCardNumber();
                if (playerHand.ElementAt(i).IsSameNum(playerHand.ElementAt(i + 1)) &&
                    playerHand.ElementAt(i).IsSameNum(playerHand.ElementAt(i + 2)) &&
                    playerHand.ElementAt(i).IsSameNum(playerHand.ElementAt(i + 3))) return cardNum;
            }
            return 0;
        }
        public int FindStraightFlush(Player player)
        {
            if (player == null) return 0;
            List<PokerCard> playerHand = FullHand(player);
            if (playerHand.Count() < 5) return 0;

            string flush = FindFlush(player);
            if (!(flush == null) || !(FindStraight(player) > 0)) return 0;

            for (int i = 1; i < playerHand.Count(); i++)
            {
                if (!playerHand.ElementAt(i).GetSuit().Equals(flush))
                {
                    playerHand.RemoveAt(i);
                    i--;
                }
            }
            int count = 0;
            for (int i = playerHand.Count() - 2; i > 0; i--)
            {
                PokerCard card = playerHand.ElementAt(i);
                if (card.CompareTo(playerHand.ElementAt(i - 1)) == 1 && card.CompareTo(playerHand.ElementAt(i + 1)) == -1) count++;
            }
            if (count >= 3)
            {
                for (int i = playerHand.Count() - 2; i > 0; i--)
                {
                    PokerCard card = playerHand.ElementAt(i);
                    if (card.CompareTo(playerHand.ElementAt(i - 1)) == 1 && card.CompareTo(playerHand.ElementAt(i + 1)) == -1)
                    {
                        return playerHand.ElementAt(i + 1).GetCardNumber();
                    }
                }
            }
            return 0;
        }
        public bool HasRoyalFlush(Player player)
        {
            if (player == null) return false;
            List<PokerCard> playerHand = FullHand(player);
            if (playerHand.Count() < 5) return false;

            string flush = FindFlush(player);
            if (!(flush == null) || !(FindStraight(player) > 0)) return false;

            int num = 10;
            foreach (PokerCard card in playerHand)
            {
                if (card.GetSuit().Equals(flush) && card.GetCardNumber() == num)
                {
                    num++;
                }
            }

            return num == 15;
        }
        public int GetBestHand(Player player)
        {
            if (HasRoyalFlush(player)) return 9;
            if (FindStraightFlush(player) != 0) return 8;
            if (FindFourOfKind(player) != 0) return 7;
            if (FindFullHouse(player) != 0) return 6;
            if (FindFlush(player) != null) return 5;
            if (FindStraight(player) != 0) return 4;
            if (FindThreeOfKind(player) != 0) return 3;
            if (FindTwoPair(player) != 0) return 2;
            if (FindPair(player) != 0) return 1;
            return 0;
        }
    }
}