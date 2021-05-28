using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private baseCharacter script;
    private Text HPdisplay,MPdisplay;
    private int HP, MAXHP, MP, MAXMP;
    [SerializeField] private bool showMP;


    // Start is called before the first frame update
    void Start()
    {
        HPdisplay = this.transform.GetChild(0).gameObject.GetComponent<Text>();
        MPdisplay = this.transform.GetChild(1).gameObject.GetComponent<Text>();
        if(!showMP)
        {
            MPdisplay.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        MAXHP = script.getMaxHP();
        HP = script.getHP();
        HPdisplay.text = HP.ToString() + " / " + MAXHP.ToString(); 
        if(showMP)
        {
            MAXMP = script.getMaxMP();
            MP = script.getMP();
            MPdisplay.text = MP.ToString() + " / " + MAXMP.ToString();
        }
    }
}
