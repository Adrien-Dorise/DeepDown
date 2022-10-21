using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class camera : MonoBehaviour
{
    [SerializeField] private bool isCinemachine;
    private GameObject vcam, DPScharacter;
    private CinemachineVirtualCamera vCamScript;

    public float dragSpeed = 0.3f;
    private Vector3 dragOrigin;

    // Start is called before the first frame update
    void Start()
    {
        

        vcam = GameObject.Find("CM vcam1").gameObject;
        vCamScript = vcam.GetComponent<CinemachineVirtualCamera>();
        if (PlayerPrefs.GetString("character") == "Mouse")
        {
            isCinemachine = false;
            vcam.SetActive(false);

            if (SceneManager.GetActiveScene().name == "Lobby1" || SceneManager.GetActiveScene().name == "Lobby2" || SceneManager.GetActiveScene().name == "Lobby3")
            {
                DPScharacter = GameObject.Find("DPS").transform.GetChild(0).gameObject;
            }
            else
            {
                DPScharacter = GameObject.Find("DPSAgent").transform.GetChild(0).gameObject;
            }
            
        }
        else
        {
            isCinemachine = true;
            vcam.SetActive(true);
            switch (PlayerPrefs.GetString("character"))
            {
                case "DPS":
                    vcam.transform.GetComponent<CinemachineVirtualCamera>().Follow = GameObject.Find("DPS").transform.GetChild(0).transform;
                    break;
                case "Tank":
                    vcam.transform.GetComponent<CinemachineVirtualCamera>().Follow = GameObject.Find("Tank").transform.GetChild(0).transform;
                    break;
                case "Mage":
                    vcam.transform.GetComponent<CinemachineVirtualCamera>().Follow = GameObject.Find("Mage").transform.GetChild(0).transform;
                    break;
            }
            
        }
    }

    private void mouseMove()
    {
        
        if (Input.GetMouseButtonDown(2))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(2)) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(dragOrigin - Input.mousePosition);
        Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0);

        this.transform.Translate(move, Space.World);
    
     }

    private void resetPos()
    {
        this.transform.position = new Vector3(DPScharacter.transform.position.x, DPScharacter.transform.position.y, this.transform.position.z);


    }



    // Update is called once per frame
    void FixedUpdate()
    {
        if(!isCinemachine)
        {
            mouseMove();
            if(Input.GetKey("space"))
            {
                resetPos();
            }
            
        }
        
    }
}
