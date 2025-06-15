using UnityEngine;
using TMPro;

public class InputFieldHandler : MonoBehaviour
{
    //[SerializeField] private TMP_InputField integerInputField;
    
    // Static variable to hold the value across scenes
    [SerializeField] private int savedInteger;
    
    public void GrabFromInputField(string input)
    {
        savedInteger = int.Parse(input);
    }
}