using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mage : baseCharacter
{
    //attack is a big range circle OF FLAMES OF DEATH
    //Spé1 is healing
    //Spé2 is resurection

    //mage class

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
        if (PlayerPrefs.GetString("character") == "Mage" && this.transform.parent.name != "MageAgent")
        {
            hasToFollow = false;
            isMainCharacter = true;
            moveTarget.transform.SetParent(this.transform);
            moveTarget.transform.localPosition = Vector3.zero;
        }
        else
        {
            isMainCharacter = false;
            hasToFollow = true;
        }

        //Attack
        atkRange = 2f;
        special1Range = 4.25f;
        special2Range = 4.5f;
        atkDelay = atkDelay;
        special1Delay = 1;
        special2Delay = special2Delay;


        //Status
        MAXHP = HP = 100;
        MAXMP = MP = 1000;
        baseATK = ATK = 20;
        baseDEF = DEF = 1;

        //Wizard class
        spellMovingSpeed = 2f;
        stillAttacking = false;
        healingRatio = 0.06f; // calculation is HPmax * healing ratio
        healingDelay = 0.5f; //Time between healing, in second
        HPrez = 0.5f; //Heal half of HP when rez
        MPCostAtk = 125;
        MPCostSpe1 = 100;
        MPCostSpe2 = 750;
        MPRegen = 15; //Regen per MPregenDelay
        MPRegenDelay = 0.75f; //delay between MP recovering in second;
        isMPRetored = false;

    }


    protected override void attack()
    {
        if (!isBusy && MP >= MPCostAtk)
        {
            base.attack();
            MP -= MPCostAtk;
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
                    //Debug.Log(this.transform.parent.name + " hit " + enemyColl[i].transform.parent.transform.parent.name);
                    switch (enemyColl[i].tag)
                    {
                        case "Monster1":
                            enemyColl[i].GetComponentInParent<baseMonster>().getHit(ATK, transform.position);
                            break;
                        case "Worm":
                            enemyColl[i].GetComponentInParent<baseMonster>().getHit(ATK, transform.position);
                            break;
                        case "Snake":
                            enemyColl[i].GetComponentInParent<baseMonster>().getHit(ATK, transform.position);
                            break;
                        case "Tree":
                            enemyColl[i].GetComponentInParent<baseMonster>().getHit(ATK, transform.position);
                            break;
                        case "Octopus":
                            enemyColl[i].GetComponentInParent<baseMonster>().getHit(ATK, transform.position);
                            break;
                        case "Fish":
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
        if (!isBusy && isSpecial1Available && MP >= MPCostSpe1)
        {
            base.special1();
            MP -= MPCostSpe1;
            stillAttacking = true;
            speed = spellMovingSpeed;
            StartCoroutine(stillSpecial1());

        }
    }


    private IEnumerator stillSpecial1()
    {

        while (stillAttacking)
        {
            base.debugCircleRay(special1Range, Color.green);
            //We use a circle to check collision around player
            allyColl = Physics2D.OverlapCircleAll(this.transform.position, special1Range, attackLayerMask);
            if (allyColl.Length > 0)
            {
                for (int i = 0; i < allyColl.Length; i++)
                {
                    //Debug.Log(this.transform.parent.name + " healed " + allyColl[i].transform.parent.transform.parent.name);
                    switch (allyColl[i].tag)
                    {
                        case "Player":
                            if (!allyColl[i].GetComponentInParent<baseCharacter>().isKO)
                            {
                                HPhealed = (int)(allyColl[i].GetComponentInParent<baseCharacter>().getMaxHP() * healingRatio);
                                allyColl[i].GetComponentInParent<baseCharacter>().getHeal(HPhealed);
                            }  
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
        if (!isBusy && isSpecial2Available && MP >= MPCostSpe2)
        {
            base.special2();
            MP -= MPCostSpe2;
            speed = spellMovingSpeed;
            stillAttacking = true;
            StartCoroutine(stillSpecial2());

        }
    }


    private IEnumerator stillSpecial2()
    {

        while (stillAttacking)
        {
            base.debugCircleRay(special2Range, Color.green);
            //We use a circle to check collision around player
            allyColl = Physics2D.OverlapCircleAll(this.transform.position, special2Range, attackLayerMask);
            if (allyColl.Length > 0)
            {
                for (int i = 0; i < allyColl.Length; i++)
                {
                    //Debug.Log(this.transform.parent.name + " healed " + allyColl[i].transform.parent.transform.parent.name);
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

