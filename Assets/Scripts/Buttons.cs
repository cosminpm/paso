using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    public void Exit()
    {
        Application.Quit();
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("Playing Scene");

        PlayerPrefs.SetInt("Score", 0);
        PlayerPrefs.SetString("Time", "0");
        PlayerPrefs.SetInt("Level", 0);
    }
}