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
        }
    }

    void Update()
    {
        if (balanceText != null && player != null)
        {
            balanceText.text = "Balance: $" + player.Balance.ToString();
        }

        if (potBalance != null && pokerGame != null && pokerGame.pots != null)
        {
            potBalance.text = "Pot Balance: $" + pokerGame.pots.Amount.ToString();
        }
    }
}
