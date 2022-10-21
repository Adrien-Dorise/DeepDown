using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Doors : MonoBehaviour
{
    private int D1Number, D2Number;
    // Start is called before the first frame update
    void Start()
    {
        D1Number = 1;
        D2Number = 1;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            int dungeonChoice;
            string dungeonName = "Dungeon";

            switch(this.name)
            {
                case "Doors1":
                    dungeonChoice = Random.Range(1, D1Number+1);
                    dungeonName += "1." + dungeonChoice;
                    SceneManager.LoadScene(dungeonName);
                    break;

                case "Doors2":
                    dungeonChoice = Random.Range(1, D2Number+1);
                    dungeonName += "2." + dungeonChoice;
                    SceneManager.LoadScene(dungeonName);
                    break;

                case "Doors3":
                    PlayerPrefs.SetString("LevelAccessible", "2");
                    PlayerPrefs.Save();
                    SceneManager.LoadScene("Lobby2");
                    break;

                case "Doors4":
                    PlayerPrefs.SetString("LevelAccessible", "3");
                    PlayerPrefs.Save();
                    SceneManager.LoadScene("Lobby3");
                    break;
            }
        }
    
    }


}
