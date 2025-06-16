using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using BaseGame;
using TMPro;

public class AlgorithmManager : MonoBehaviour
{
    [SerializeField] private int maxHandSize;
    [SerializeField] private GameObject cardPrefab;
    public SplineContainer splineContainer;
    [SerializeField] private Transform spawnPoint;
    public GameObject aiPlayerObject; // Reference to your AI player GameObject
    [SerializeField] float spacingMultiplier = .750f;
    private List<GameObject> handCards = new();
    int dispBal = 0;
    [SerializeField] private GameObject textPrefab; // Assign TMP prefab in inspector
    private TMP_Text balanceText;
    void Start()
    {
        // Initialize the TextMeshPro
        GameObject textObj = Instantiate(textPrefab, Vector3.zero, Quaternion.identity);
        balanceText = textObj.GetComponent<TMP_Text>();
        
        if (balanceText == null)
        {
            Debug.LogError("TextMeshPro component not found on prefab!");
            return;
        }

        // Initial setup of the text
        balanceText.alignment = TextAlignmentOptions.Center;
        balanceText.fontSize = 12;
        UpdateBalanceText();
    }
    void Update()
    {
        if (aiPlayerObject == null || balanceText == null) return;

        AlgorithmPlayer p = aiPlayerObject.GetComponent<AlgorithmPlayer>();
        dispBal = p.Balance;

        // Get the position at the start of the spline (t=0)
        Vector3 localPosition = splineContainer.Spline.EvaluatePosition(0);
        Vector3 worldPosition = splineContainer.transform.TransformPoint(localPosition);
        
        // Update text position and content
        balanceText.transform.position = worldPosition;
        UpdateBalanceText();
    }

    private void UpdateBalanceText()
    {
        if (balanceText != null)
        {
            balanceText.text = $"Balance: ${dispBal}";
        }
    }
    // Call this from your game logic
    public void DisplayAIHand()
    {
        spawnPoint = GameObject.Find("CardSpawnPoint").transform;
        AlgorithmPlayer aiPlayer = aiPlayerObject.GetComponent<AlgorithmPlayer>();

        for (int i = 0; i < aiPlayer.Hand.Length; i++)
        {
            if (aiPlayer.Hand[i] == null) continue;
            string suit = aiPlayer.Hand[i].GetSuit();
            int number = aiPlayer.Hand[i].GetCardNumber();
            DrawCard(suit, number);
        }
    }
    private void DrawCard(string suit, int number)
    {
        if (handCards.Count >= maxHandSize)
        {
            return;
        }

        GameObject g = Instantiate(cardPrefab, spawnPoint.position, spawnPoint.rotation);
        PokerCard pokerCardScript = g.GetComponent<PokerCard>();
        pokerCardScript.SetSuit(suit);
        pokerCardScript.SetCardNumber(number);

        handCards.Add(g);
        UpdateCardPositions();
    }

    private void UpdateCardPositions()
    {
        if (handCards.Count == 0) return;
        float totalWidth = spacingMultiplier;
        float startOffset = (1f - totalWidth) / 2f;
        float spacing = totalWidth / (handCards.Count + 1);
        
        Spline spline = splineContainer.Spline;

        for (int i = 0; i < handCards.Count; i++)
        {
            float t = startOffset + (spacing * (i + 1));

            // Get position in world space by transforming local position
            Vector3 localPosition = spline.EvaluatePosition(t);
            Vector3 worldPosition = splineContainer.transform.TransformPoint(localPosition);
            Vector2 position2D = new Vector2(worldPosition.x, worldPosition.y);

            // Transform tangent to world space as well
            Vector3 localTangent = spline.EvaluateTangent(t);
            Vector3 worldTangent = splineContainer.transform.TransformDirection(localTangent);
            Vector2 tangent2D = new Vector2(worldTangent.x, worldTangent.y).normalized;

            float angle = Mathf.Atan2(tangent2D.y, tangent2D.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

            Vector3 finalPosition = new Vector3(position2D.x, position2D.y, 0f);

            handCards[i].transform.DOMove(finalPosition, 0.5f)
                .SetEase(Ease.OutQuint);
            handCards[i].transform.DORotateQuaternion(rotation, 0.5f)
                .SetEase(Ease.OutQuint);
        }
    }
}