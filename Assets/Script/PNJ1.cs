using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PNJ1 : MonoBehaviour
{
    private Text text;
    private GameObject canvasObject;
    [SerializeField] private string[] sentences = new string[10];
    [SerializeField] private int stringNumber;
    private int currentDialogue;
    private Color myColor;

    // Start is called before the first frame update
    void Start()
    {
        canvasObject = this.transform.GetChild(0).gameObject;
        canvasObject.SetActive(false);
        text = canvasObject.transform.GetChild(0).GetComponent<Text>();

        myColor = this.GetComponent<SpriteRenderer>().color;
        //Dialogue set up
        currentDialogue = 0;
        /*stringNumber = 6;
        sentences[0] = "Hello Adventurer !";
        sentences[1] = "I must tell you:\nHere you might be able to control your movements.";
        sentences[2] = "But it might get a little bit difficult\nwhen you enter this dungeon...";
        sentences[3] = "Can't say I didn't warn you !";
        sentences[4] = "Are you even listening ?!\n Maybe you lack some hidden layers...!";
        sentences[5] = "Looking at you, you are closer to\nMachine Learning than Deep Learning...!";*/
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if(collision.transform.GetComponent<baseCharacter>().isMainCharacter)
            {
                dialogue();
            }
            
        }

        
    }

    private void OnMouseEnter()
    {
        if (PlayerPrefs.GetString("character") == "Mouse")
        {
            dialogue();
        }
    }


    private void OnMouseExit()
    {
        if ( PlayerPrefs.GetString("character") == "Mouse")
        {
            if (canvasObject.activeSelf)
            {

                canvasObject.SetActive(false);
            }
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            canvasObject.SetActive(false);
        }
    }

    private void dialogue()
    {
        sentences[currentDialogue] = sentences[currentDialogue].Replace("\\n", "\n");
        text.text = sentences[currentDialogue];
        
        canvasObject.SetActive(true);
        currentDialogue++;
        if (currentDialogue >= stringNumber)
        {
            currentDialogue = 0;
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
