using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMan : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ToGame()
    {
        SceneManager.LoadScene("Level");
        Debug.Log("Start Game");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Exit Game");
    }
}
