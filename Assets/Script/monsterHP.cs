using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class monsterHP : MonoBehaviour
{
    private GameObject parent;
    private Text txt;
    // Start is called before the first frame update
    void Start()
    {
        parent = this.transform.parent.gameObject;
        txt = this.transform.GetChild(0).GetComponent<Text>(); ;
    }

    // Update is called once per frame
    void Update()
    {
        txt.text = parent.GetComponent<baseMonster>().getHP().ToString();
    }
}
