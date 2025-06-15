using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using BaseGame;

public class AlgorithmManager : MonoBehaviour
{
    [SerializeField] private int maxHandSize;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private Transform spawnPoint;
    public GameObject aiPlayerObject; // Reference to your AI player GameObject

    private List<GameObject> handCards = new();

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

         float spacing = 1f / (handCards.Count + 1); // Prevent overlap
        //float firstCardPosition = 0.5f - (handCards.Count - 1) * cardSpacing / 2;

        Spline spline = splineContainer.Spline;
        for (int i = 0; i < handCards.Count; i++)
        {
            float p = spacing + i * (i+1);

            Vector3 splinePosition = spline.EvaluatePosition(p);
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up = spline.EvaluateUpVector(p);
            Quaternion rotation = Quaternion.LookRotation(forward, up);

            //Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);

            handCards[i].transform.DOMove(splinePosition, 0.25f);
            handCards[i].transform.DOLocalRotateQuaternion(rotation, 0.25f);
        }
    }
}