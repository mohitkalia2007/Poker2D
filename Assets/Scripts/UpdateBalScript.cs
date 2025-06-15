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
    void Update()
    {
        if (balanceText != null && player != null)
        {
            balanceText.text = "Balance: $" + player.Balance.ToString();
        }
        
        potBalance.text = "Pot Balance: $" + GameObject.Find("PokerGame").GetComponent<PokerGame>.pots.Amount.ToString();
    }
}
