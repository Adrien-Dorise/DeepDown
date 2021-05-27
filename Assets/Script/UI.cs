using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private baseCharacter script;
    private Text HPdisplay;
    private int HP, MAXHP;


    // Start is called before the first frame update
    void Start()
    {
        HPdisplay = GetComponentInChildren<Text>();
        
    }

    // Update is called once per frame
    void Update()
    {
        MAXHP = script.getMaxHP();
        HP = script.getHP();
        HPdisplay.text = HP.ToString() + " / " + MAXHP.ToString(); 
    }
}
