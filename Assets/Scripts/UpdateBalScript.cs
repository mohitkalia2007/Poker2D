using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using BaseGame;
using TMPro;
public class UpdateBalScript : MonoBehaviour
{
    public Player player;
    public TextMeshProUGUI balanceText;
    public TextMeshProUGUI potBalance;  // Changed from Text to TextMeshProUGUI
    private PokerGame pokerGame;

    void Start()
    {
        GameObject pokerGameObj = GameObject.Find("PokerGame");
        if (pokerGameObj != null)
        {
            pokerGame = pokerGameObj.GetComponent<PokerGame>();
            potBalance.text = "Pot: $" + pokerGame.pots.ToString();

        }
        else
        {
            potBalance.text = "Pot: $0";
        }
    }

    void Update()
    {
        if (balanceText != null && player != null)
        {
            balanceText.text = "Balance: $" + player.Balance.ToString();
        }
        GameObject pokerGameObj = GameObject.Find("PokerGame");
        if (pokerGameObj != null)
        {
            pokerGame = pokerGameObj.GetComponent<PokerGame>();
            potBalance.text = "Pot: $" + pokerGame.pots.ToString();
            
        }else
        {
            potBalance.text = "Pot: $0";
        }

    }
}