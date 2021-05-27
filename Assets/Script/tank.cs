using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tank : baseCharacter
{
    //Tank class
    [SerializeField] private int DEFMutliplier;
    [SerializeField] private float DEFSpeed;

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
        MAXHP = HP = 500;
        baseATK = ATK = 3;
        baseDEF = DEF = 4;


        //Tank class
        DEFMutliplier = 5;
        DEFSpeed = 0.5f;


    }

    protected override void special1()
    {
        if (!isBusy && isSpecial1Available)
        {
            isBusy = true;
            isSpecial1Available = false;
            DEF *= DEFMutliplier;
            speed = DEFSpeed;

            //We use a circle to check collision around player
            
        }
    }


    protected override void endSpecial1()
    {
        DEF = baseDEF;
        speed = baseSpeed;
        base.endSpecial1();
    }


}
