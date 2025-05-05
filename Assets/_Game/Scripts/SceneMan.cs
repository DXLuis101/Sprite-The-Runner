using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneMan : MonoBehaviour
{
    public Text bestDistanceText;
    public Text maxCoinsText;

    // Start is called before the first frame update
    void Start()
    {
        bestDistanceText.text = "Best Distance:" + PlayerPrefs.GetInt("highscoreD") + "M";
        maxCoinsText.text = "Best Coins:" + PlayerPrefs.GetInt("highscoreC");
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
