using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class pause : MonoBehaviour
{
    private GameObject gameManagerObject;
    // Start is called before the first frame update
    void Start()
    {
        gameManagerObject = GameObject.Find("Game Manager").gameObject;  
    }

    public void resumeButton()
    {
        gameManagerObject.GetComponent<GameManager>().triggerIsPaused();
    }

    public void quitButton()
    {
        Application.Quit();
    }

    public void lobbyButton()
    {
        SceneManager.LoadScene("Lobby" + PlayerPrefs.GetString("LevelAccessible"));
    }

}
