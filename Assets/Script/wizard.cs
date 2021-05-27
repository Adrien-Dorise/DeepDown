using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wizard : baseCharacter
{
    //attack is a big range circle OF FLAMES OF DEATH
    //Spé1 is healing
    //Spé2 is resurection

    //Wizard class
    [SerializeField] private float spellMovingSpeed;
    [SerializeField] private bool stillAttacking;
    private int HPhealed;
    [SerializeField] private float healingRatio, healingDelay;

    [SerializeField] private float HPrez;

    protected override void Start()
    {
        base.Start();
        //Movements
        baseSpeed = speed = speed;

        //Attack
        atkRange = 1.1f;
        special1Range = special1Range;
        atkDelay = atkDelay;
        special1Delay = special1Delay;


        //Status
        MAXHP = HP = 100;
        baseATK = ATK = 10;
        baseDEF = DEF = 1;

        //Wizard class
        baseSpeed = speed;
        spellMovingSpeed = 0.35f;
        stillAttacking = false;
        healingRatio = 0.001f; // calculation is HPmax * healing ratio
        healingDelay = 0.5f; //Time between healing, in second
        HPrez = 0.5f; //Heal half of HP when rez
    }


    protected override void attack()
    {
        if (!isBusy)
        {
            isBusy = true;
            speed = spellMovingSpeed;
            stillAttacking = true;
            StartCoroutine(stillAtk());

        }
    }

    private IEnumerator stillAtk()
    {
        while (stillAttacking)
        {
            base.debugCircleRay(atkRange, Color.green);
            //We use a circle to check collision around player
            enemyColl = Physics2D.OverlapCircleAll(this.transform.position, atkRange, attackLayerMask);
            if (enemyColl.Length > 0)
            {
                for (int i = 0; i < enemyColl.Length; i++)
                {
                    Debug.Log(this.transform.parent.name + " hit " + enemyColl[i].transform.parent.transform.parent.name);
                    switch (enemyColl[i].tag)
                    {
                        case "Monster1":
                            enemyColl[i].GetComponentInParent<baseMonster>().getHit(ATK, transform.position);
                            break;
                    }
                }
            }
            yield return null;
        }

    }

    protected override void endAtk()
    {
        speed = baseSpeed;
        stillAttacking = false;
        base.endAtk();
    }


    protected override void special1()
    {
        if (!isBusy && isSpecial1Available)
        {
            isBusy = true;
            stillAttacking = true;
            isSpecial1Available = false;
            speed = spellMovingSpeed;
            StartCoroutine(stillSpecial1());

        }
    }


    private IEnumerator stillSpecial1()
    {

        while (stillAttacking)
        {
            base.debugCircleRay(atkRange, Color.green);
            //We use a circle to check collision around player
            allyColl = Physics2D.OverlapCircleAll(this.transform.position, atkRange, attackLayerMask);
            if (allyColl.Length > 0)
            {
                for (int i = 0; i < allyColl.Length; i++)
                {
                    Debug.Log(this.transform.parent.name + " healed " + allyColl[i].transform.parent.transform.parent.name);
                    switch (allyColl[i].tag)
                    {
                        case "Player":
                            HPhealed = (int)(allyColl[i].GetComponentInParent<baseCharacter>().getMaxHP() / healingRatio);
                            allyColl[i].GetComponentInParent<baseCharacter>().getHeal(HPhealed);
                            break;
                    }
                }
            }
            yield return new WaitForSeconds(healingDelay);
        }

    }


    protected override void endSpecial1()
    {
        speed = baseSpeed;
        stillAttacking = false;
        base.endSpecial1();
    }




    protected override void special2()
    {
        if (!isBusy && isSpecial1Available)
        {    
            isBusy = true;
            speed = spellMovingSpeed;
            stillAttacking = true;
            isSpecial2Available = false;
            StartCoroutine(stillSpecial2());

        }
    }


    private IEnumerator stillSpecial2()
    {

        while (stillAttacking)
        {
            base.debugCircleRay(atkRange, Color.green);
            //We use a circle to check collision around player
            allyColl = Physics2D.OverlapCircleAll(this.transform.position, atkRange, attackLayerMask);
            if (allyColl.Length > 0)
            {
                for (int i = 0; i < allyColl.Length; i++)
                {
                    Debug.Log(this.transform.parent.name + " healed " + allyColl[i].transform.parent.transform.parent.name);
                    switch (allyColl[i].tag)
                    {
                        case "Player":
                            if(allyColl[i].GetComponentInParent<baseCharacter>().isKO)
                            {
                                HPhealed = (int)(allyColl[i].GetComponentInParent<baseCharacter>().getMaxHP() * HPrez);
                                allyColl[i].GetComponentInParent<baseCharacter>().resurection(HPhealed);
                            }
                            break;
                    }
                }
            }
            yield return new WaitForSeconds(healingDelay);
        }

    }


    protected override void endSpecial2()
    {
        speed = baseSpeed;
        stillAttacking = false;
        base.endSpecial2();
    }


}

