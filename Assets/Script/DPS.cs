using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DPS : baseCharacter
{
    //DPS class
    private float Spe1AtkBoost;

    protected override void Start()
    {
        base.Start();
        //Movements
        baseSpeed = speed = speed;

        //Attack
        atkRange = atkRange;
        special1Range = special1Range;
        atkDelay = atkDelay;
        special1Delay = special1Delay;

        //Status
        MAXHP = HP = 150;
        baseATK = ATK = 25;
        baseDEF = DEF = 2;

        //DPS class
        Spe1AtkBoost = 2.1f;



    }


    protected override void special1()
    {
        if (!isBusy && isSpecial1Available)
        {
            isBusy = true;
            isSpecial1Available = false;
            //We use a circle to check collision around player
            enemyColl = Physics2D.OverlapCircleAll(this.transform.position, special1Range, attackLayerMask);
            if (enemyColl.Length > 0)
            {
                for (int i = 0; i < enemyColl.Length; i++)
                {
                    Debug.Log(this.transform.parent.name + " hit " + enemyColl[i].transform.parent.transform.parent.name);
                    switch (enemyColl[i].tag)
                    {
                        case "Monster1":
                            enemyColl[i].GetComponentInParent<baseMonster>().getHit((int)(ATK * Spe1AtkBoost), transform.position);
                            break;
                    }
                }
            }
        }
    }



}
