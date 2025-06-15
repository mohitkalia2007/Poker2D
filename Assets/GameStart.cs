using UnityEngine;

public class GameStart : MonoBehaviour
{
    public GameObject gameObject;
    bool keyNotPress = true;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && keyNotPress)
        {
            Instantiate(gameObject);
        }
    }
}
