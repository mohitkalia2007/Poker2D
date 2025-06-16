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
    public PokerGame pokerGame;
    GameObject pokerGameObj;


    void Start()
    {
        pokerGameObj = GameObject.Find("PokerGame");
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
        if (pokerGame != null && pokerGame.pots != null)
        {
            potBalance.text = $"Pot: ${pokerGame.pots.Amount}";
            Debug.Log($"Pot Updated: ${pokerGame.pots.Amount}");
        }
        else
        {
            potBalance.text = "Pot: $0";
        }

    }
}