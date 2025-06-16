using BaseGame;
using UnityEngine;

public class RaiseInputFieldHandler : MonoBehaviour
{
    public static int raiseInput;
    public HumanPlayer player;

    public void Start()
    {
        player = FindFirstObjectByType<HumanPlayer>();
    }
    
    public void GrabFromInputField(string input)
    {
        raiseInput = int.Parse(input);
        player.Raise(raiseInput);
        Debug.Log("hey kids");
        Debug.Log(raiseInput);
    }
}
