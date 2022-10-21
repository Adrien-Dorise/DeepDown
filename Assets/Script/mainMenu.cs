using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{
    public Button DPS, Tank, Healer, Mouse;
    // Start is called before the first frame update



    public void loadLobby()
    {
        if(PlayerPrefs.HasKey("LevelAccessible"))
        {
            Debug.Log("Lobby" + PlayerPrefs.GetString("LevelAccessible"));
            SceneManager.LoadScene("Lobby" + PlayerPrefs.GetString("LevelAccessible"));
        }
        else
        {
            SceneManager.LoadScene("Lobby1");
            PlayerPrefs.SetString("LevelAccessible", "1");
            PlayerPrefs.Save();
        }
    }


    public void DPSbutton()
    {
        PlayerPrefs.SetString("character", "DPS");
        PlayerPrefs.Save();
        loadLobby();
    }
    public void TankButton()
    {
        PlayerPrefs.SetString("character", "Tank");
        PlayerPrefs.Save();
        loadLobby();
    }

    public void MageButton()
    {
        PlayerPrefs.SetString("character", "Mage");
        PlayerPrefs.Save();
        loadLobby();
    }

    public void MouseButton()
    {
        PlayerPrefs.SetString("character", "Mouse");
        PlayerPrefs.Save();
        loadLobby();
    }
    public void ResetButton()
    {
        PlayerPrefs.SetString("LevelAccessible", "1");
        PlayerPrefs.Save();
    }




}
