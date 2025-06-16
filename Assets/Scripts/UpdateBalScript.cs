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

    void Start()
    {
        // Find the PokerGame clone in the scene

    try
    {
          pokerGame = GameObject.Find("PokerGame(Clone)").GetComponent<PokerGame>();
    }
    catch (System.Exception e)
    {
        Debug.LogError("Could not find game instance: " + e.Message);
    }

    
   
        
    }

    void Update()
    
    {
        if (balanceText != null && player != null)
        {
            balanceText.text = "Balance: $" + player.Balance.ToString();
        }

        UpdatePotDisplay();

    }
    private void UpdatePotDisplay()
    {
         try
             {
          pokerGame = GameObject.Find("PokerGame(Clone)").GetComponent<PokerGame>();
             }
            catch (System.Exception e)
            {
            Debug.LogError("Could not find game instance: " + e.Message);
        }
        if (pokerGame != null && pokerGame.pots != null && potBalance != null)
        {
            potBalance.text = $"Pot: $" + pokerGame.pots.Amount.ToString();
            Debug.Log($"Current Pot: ${pokerGame.pots.Amount}"); // Debug line
        }
    }
}