using UnityEngine;
using TMPro;

public class GameStart : MonoBehaviour
{
    public GameObject objects;
    public TMP_Text  promptText; // Add reference to UI Text component
    bool keyNotPress = true;

    void Start()
    {
        // Make sure to assign the Text component in Inspector
        if (promptText == null)
        {
            Debug.LogError("Please assign a UI Text component to promptText!");
        }
    }

    void Update()
    {
        if (Input.anyKeyDown && keyNotPress)
        {
            Instantiate(objects);
            keyNotPress = false;
            if (promptText != null)
            {
                promptText.text = ""; // Clear the text when key is pressed
            }
        }
        else
        {
            if (promptText != null && keyNotPress)
            {
                promptText.text = "Press any key to start"; // Display prompt text
            }
        }
    }
}