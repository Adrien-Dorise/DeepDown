using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class gameOver : MonoBehaviour
{
    // Start is called before the first frame update


    public void resumeButton()
    {
        if (PlayerPrefs.HasKey("LevelAccessible"))
        {
            SceneManager.LoadScene("Lobby" + PlayerPrefs.GetString("LevelAccessible"));
        }
        else
        {
            SceneManager.LoadScene("Lobby1");
            PlayerPrefs.SetString("LevelAccessible", "1");
            PlayerPrefs.Save();
        }
    }

    public void quitButton()
    {
        Application.Quit();
    }

}
