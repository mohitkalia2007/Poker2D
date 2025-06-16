using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using BaseGame;

public class HandManager : MonoBehaviour
{
    [SerializeField] private int maxHandSize;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject humanPlayer;
    private List<GameObject> handCards = new();
    public static event Action OnGameStart;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnGameStart?.Invoke();
            for (int i = 0; i < 2; i++)
            {
                HumanPlayer player = humanPlayer.GetComponent<HumanPlayer>();
                String suit = player.Hand[i].GetSuit();
                int num = player.Hand[i].GetCardNumber();
                DrawCard(suit, num);

            }
        }
    }
    private void DrawCard(String suit, int number)
    {
        if (handCards.Count >= maxHandSize)
        {
            return;
        }
        GameObject g = Instantiate(cardPrefab, spawnPoint.position, spawnPoint.rotation);
        PokerCard pokerCardScript = g.GetComponent<PokerCard>();
        pokerCardScript.SetSuit(suit);
        pokerCardScript.SetCardNumber(number);
        pokerCardScript.IsFaceUp = true;
        handCards.Add(g);
        UpdateCardPositions();
    }
    
    private void UpdateCardPositions()
    {
        if (handCards.Count == 0) return;

        float spacingMultiplier = 1.25f;
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