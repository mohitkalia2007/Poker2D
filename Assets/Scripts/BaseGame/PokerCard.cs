using System.Collections.Generic;
using BaseGame;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.UI;

namespace BaseGame
{
    public class PokerCard : MonoBehaviour
    {
        [SerializeField] int num;
        [SerializeField] string suit;
        private bool isFaceUp = false;
        List<string> suits = new List<string>{ "diamond", "spade", "heart", "club" };
        public bool IsFaceUp { get { return isFaceUp; } set { isFaceUp = value; } }
        public PokerCard(int number, string suit)
        {
            num = number;
            this.suit = suit;
        }
        public override string ToString()
        {
            return $"{num}, {suit}";
        }
        public int GetCardNumber()
        {
            return num;
        }
        public string GetSuit()
        {
            return suit;
        }
        public bool Equals(PokerCard obj)
        {
            return obj.GetCardNumber() == this.GetCardNumber() && obj.GetSuit() == this.GetSuit();
        }


        public void SetCardNumber(int number)
        {
            if (number < 2 || number > 14) throw new System.Exception("Invalid card number");
            num = number;
        }   
        public void SetSuit(string suit)
        {
            if (!suits.Contains(suit)) throw new System.Exception("Invalid suit");
            this.suit = suit;
        }
        public bool IsSameSuit(PokerCard obj)
        {
            return obj.GetSuit() == this.GetSuit();
        }
        public bool IsSameNum(PokerCard obj)
        {
            return obj.GetCardNumber() == this.GetCardNumber();
        }
        public int CompareTo(PokerCard obj)
        {
            if (obj == null) throw new System.Exception("Card is null");
            if (this.Equals(obj)) return 0;
            if (this.IsSameSuit(obj)) return this.GetCardNumber() - obj.GetCardNumber();
            int thisSuitNum = suits.IndexOf(this.GetSuit());
            if (thisSuitNum != -1)
            {
                int objSuitNum = suits.IndexOf(obj.GetSuit());
                if (objSuitNum != -1)
                {

                    return thisSuitNum - objSuitNum + 10*(this.GetCardNumber() - obj.GetCardNumber());
                }
            }
            throw new System.Exception("Suit of card is not an ace, spade, heart, or club.");
        }
    }    
}

