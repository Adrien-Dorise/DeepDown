using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private GameObject pauseObject, gameOverObject, UI;
    public bool isPaused;
    private bool isMouseControl;
    private GameObject moveTarget;
    [SerializeField] private GameObject DPS, DPSAgent, Tank, TankAgent, Mage, MageAgent;
    [SerializeField] private GameObject activeDPS, activeTank, activeMage;

    // Start is called before the first frame update
    void Start()
    {
        isPaused = false;
        pauseObject = GameObject.Find("Pause Menu").gameObject;
        pauseObject.SetActive(false);
        gameOverObject = GameObject.Find("GameOver").gameObject;
        gameOverObject.SetActive(false);
        UI = GameObject.Find("UI").gameObject;

        if (PlayerPrefs.GetString("character") == "Mouse")
        {
            isMouseControl = true;
        }
        else
        {
            isMouseControl = false;
        }



        if (SceneManager.GetActiveScene().name == "Lobby1" || SceneManager.GetActiveScene().name == "Lobby2" || SceneManager.GetActiveScene().name == "Lobby3")
        {
            DPSAgent.SetActive(false);
            TankAgent.SetActive(false);
            MageAgent.SetActive(false);
            activeDPS = DPS;
            activeTank = Tank;
            activeMage = Mage;
        }
        else
        {
            if (isMouseControl)
            {
                DPS.SetActive(false);
                Tank.SetActive(false);
                Mage.SetActive(false);
                activeDPS = DPSAgent;
                activeTank = TankAgent;
                activeMage = MageAgent;
            }
            else
            {
                switch (PlayerPrefs.GetString("character"))
                {
                    case "DPS":
                        DPSAgent.SetActive(false);
                        Tank.SetActive(false);
                        Mage.SetActive(false);
                        activeDPS = DPS;
                        activeTank = TankAgent;
                        activeMage = MageAgent;
                        break;
                    case "Tank":
                        DPS.SetActive(false);
                        TankAgent.SetActive(false);
                        Mage.SetActive(false);
                        activeDPS = DPSAgent;
                        activeTank = Tank;
                        activeMage = MageAgent;
                        break;
                    case "Mage":
                        DPS.SetActive(false);
                        Tank.SetActive(false);
                        MageAgent.SetActive(false);
                        activeDPS = DPSAgent;
                        activeTank = TankAgent;
                        activeMage = Mage;
                        break;
                }
            }

       
            
            
        }
        moveTarget = GameObject.Find("Move Target").gameObject;
        if(isMouseControl)
        {
            moveTarget.GetComponent<SpriteRenderer>().enabled = true;
        }
        else
        {
            moveTarget.GetComponent<SpriteRenderer>().enabled = false;
        }

        UI.transform.GetChild(0).GetChild(0).GetComponent<UI>().script = activeDPS.GetComponentInChildren<baseCharacter>();
        UI.transform.GetChild(0).GetChild(1).GetComponent<UI>().script = activeTank.GetComponentInChildren<baseCharacter>();
        UI.transform.GetChild(0).GetChild(2).GetComponent<UI>().script = activeMage.GetComponentInChildren<baseCharacter>();

        }

    public void triggerIsPaused()
    {
        isPaused = !isPaused;
    }

    private void mouseClick()
    {
        Vector2 mousePos;
        if (Input.GetMouseButtonDown(0))
        {
            mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y,0);
            moveTarget.transform.position = Camera.main.ScreenToWorldPoint(mousePos);
            moveTarget.transform.position = new Vector3(moveTarget.transform.position.x, moveTarget.transform.position.y, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("escape"))
        {
            triggerIsPaused();
        }
        if(isPaused)
        {
            
            pauseObject.SetActive(true);
        }
        if (!isPaused)
        {
            Time.timeScale = 1;
            pauseObject.SetActive(false);
        }

        if(isMouseControl)
        {
            mouseClick();
        }


        if(activeDPS.GetComponentInChildren<baseCharacter>().getHP() <=0 && activeTank.GetComponentInChildren<baseCharacter>().getHP() <= 0 && activeMage.GetComponentInChildren<baseCharacter>().getHP() <= 0)
        {
            Time.timeScale = 0;
            isPaused = false;
            gameOverObject.SetActive(true);
        }

    }
}
