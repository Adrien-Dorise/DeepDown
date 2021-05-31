using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

//To attach to Player game object
public class baseCharacter : MonoBehaviour
{

    //Machine Learning
    public bool isAgent;


    //Others
    private float startTime;


    //Components
    private Rigidbody2D rigidBody;
    private Animator anim;

    //Inputs
    [SerializeField] private bool isInputEnable;
    public float inputX, inputY;
    public bool isFire1Pressed, isFire2Pressed, isFire3Pressed;


    //Movements
    [SerializeField] protected float speed;
    protected float baseSpeed;
    [SerializeField] protected Vector2 lookingDir;

    //Attack
    [SerializeField] protected LayerMask attackLayerMask;
    protected Collider2D[] enemyColl, allyColl;
    private RaycastHit2D hitForward;
    Vector2 rayDir;
    [SerializeField] protected bool isBusy, isSpecial1Available, isSpecial2Available;
    [SerializeField] protected float atkRange, special1Range, special2Range;
    [SerializeField] protected int atkDelay, special1Delay, special2Delay;
    
    //MP
    [SerializeField] protected int MPCostAtk, MPCostSpe1, MPCostSpe2, MPRegen;
    [SerializeField] protected float MPRegenDelay;
    protected bool isMPRetored;


    //Hit
    [SerializeField] bool isInvincible;
    [SerializeField] private float ejectionForce, invincibilityDelay, inviFrameSpeed;

    //Status
    [SerializeField] public bool isKO, isHealing;
    [SerializeField] protected int HP, MAXHP, MP, MAXMP, ATK, DEF, MAXDEF;
    protected int baseATK, baseDEF;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        //Machine Learning
        isAgent = false;

        //Components
        rigidBody = this.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        //Inputs
        inputX = inputY = 0f;
        isFire1Pressed = isFire2Pressed = isFire3Pressed = false;

        //Movement
        isInputEnable = true;
        baseSpeed = speed = 2f;

        //Attacks
        isBusy = false;
        isSpecial1Available = isSpecial2Available = true;
        atkRange = 0.65f;
        atkDelay = 1; //Delay in s (unused because processed in animator)
        special1Range = 2f;
        special1Delay = 5; //Delay in s
        special2Range = 5f;
        special2Delay = 5;

        //Hit
        isInvincible = false;
        ejectionForce = 300;
        invincibilityDelay = 1f; //Delay in s
        inviFrameSpeed = 0.05f; //Delay in s

        //Status
        isKO = isHealing = false;
        baseATK = ATK = 5;
        MAXHP = HP = 100;
        MAXMP = MP = 0;
        baseDEF = DEF = 2;
        MAXDEF = 80; //Maximum percentage for damage reducing
        //MP
        MPCostAtk = 0;
        MPCostSpe1 = 0;
        MPCostSpe2 = 0;
        MPRegen = 0; //Regen per MPregenDelay
        MPRegenDelay = 0; //delay between MP recovering in second;
        isMPRetored = false;
}

    private void setBoolDefault()
    {
        isBusy = false;
        isInvincible = false;
    }


    protected virtual void move()
    {


        if (inputX > 0)
        { this.transform.GetComponentInChildren<SpriteRenderer>().flipX = true; }
        if (inputX < 0)
        { this.transform.GetComponentInChildren<SpriteRenderer>().flipX = false; }

        this.transform.Translate(Vector2.up * speed * Time.fixedDeltaTime * inputY);
        this.transform.Translate(Vector2.right * speed * Time.fixedDeltaTime * inputX);
        anim.SetFloat("Speed", Mathf.Abs(inputX) + Mathf.Abs(inputY));


        //We update the looking direction vector when one of the player is pressing a direction
        if (inputX + inputY != 0) 
        { lookingDir = new Vector2(inputX, inputY).normalized; }
       
        Debug.DrawRay(transform.position, lookingDir * 2f, Color.white);

        

    }


    protected virtual void attack()
    {
        if (!isBusy)
        {
            isBusy = true;
            //We use a circle to check collision around player
            enemyColl = Physics2D.OverlapCircleAll(this.transform.position, atkRange, attackLayerMask);
            if (enemyColl.Length > 0)
            {
                for(int i =0; i< enemyColl.Length; i++)
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
        }
    }

    protected virtual void endAtk()
    {
        isBusy = false;
    }





    protected virtual void special1()
    {
        isBusy = true;
        isSpecial1Available = false;
        //To override in herited script
    }


    protected virtual void endSpecial1()
    {
        isBusy = false;
        StartCoroutine(delaySpecial1());
    }

    protected IEnumerator delaySpecial1()
    {
        yield return new WaitForSeconds(special1Delay);
        isSpecial1Available = true;
        
    }




    protected virtual void special2()
    {
        isBusy = true;
        isSpecial2Available = false;
        //To override in herited script
    }


    protected virtual void endSpecial2()
    {
        isBusy = false;
        StartCoroutine(delaySpecial2());
    }

    protected IEnumerator delaySpecial2()
    {
        yield return new WaitForSeconds(special2Delay);
        isSpecial2Available = true;

    }





    public void getHit(int damage, Vector2 hitPosition)
    {
        if (!isInvincible)
        {
            isInvincible = true;
            HP -= (int)(damage * (1f - (float)DEF / 100f));

            if(HP >=0) //If character not dead from hit, we proceed with invulnerability frame and push
            {
                StartCoroutine(hitRoutine());
                //anim.SetTrigger("SetHurt");
                Vector2 direction = (new Vector2(this.transform.position.x, this.transform.position.y) - hitPosition).normalized;
                rigidBody.AddForce(direction * ejectionForce);
            }

        }
    }

    private IEnumerator hitRoutine()
    {
        startTime = Time.time;
        while (Time.time - startTime <= invincibilityDelay)
        {
            this.GetComponent<SpriteRenderer>().enabled = !this.GetComponent<SpriteRenderer>().enabled;
            yield return new WaitForSeconds(inviFrameSpeed);
        }

        this.GetComponent<SpriteRenderer>().enabled = true;
        isInvincible = false;
    }


    private void KO()
    {
        isKO = true;
        isInputEnable = false;
        this.GetComponent<BoxCollider2D>().enabled = false;
        anim.SetTrigger("SetKO");
    }


    private void regenMP()
    {

        if (MP < MAXMP && !isMPRetored)
        {
            isMPRetored = true;
            MP += MPRegen;
            StartCoroutine(MPDelay());

        }

    }

    private IEnumerator MPDelay()
    {
        yield return new WaitForSeconds(MPRegenDelay);
        isMPRetored = false;
    }


    public void getHeal(int HPrecovered)
    {
        isHealing = true;
        this.transform.GetChild(1).gameObject.GetComponent<Animator>().SetTrigger("isHealing");
        if (HP < MAXHP)
        {
            HP += HPrecovered;

        }
        isHealing = false;
    }



    public void resurection(int HPrestored)
    {

        anim.SetBool("SetRez", true);
        HP = HPrestored;


    }


    private void endResurection()
    {
        isKO = false;
        isHealing = false;
        isBusy = false;
        speed = baseSpeed;
        DEF = baseDEF;
        ATK = baseATK;
        this.GetComponent<BoxCollider2D>().enabled = true;
        isFire1Pressed = isFire2Pressed = isFire3Pressed = false;
        isSpecial1Available = isSpecial2Available = true;
        isInvincible = false;
        anim.SetBool("SetRez", false);
        isInputEnable = true;
    }


    //Uitlitaries
    public int getMaxHP()
    {
        return MAXHP;
    }

    public int getHP()
    {
        return HP;
    }

    public int getMaxMP()
    {
        return MAXMP;
    }

    public int getMP()
    {
        return MP;
    }

    private void checkStats()
    {
        if (HP > MAXHP)
        {
            HP = MAXHP;
        }

        if (HP < 0)
        {
            HP = 0;
        }
        
        if(MP > MAXMP)
        {
            MP = MAXMP;
        }
        if(MP < 0)
        {
            MP = 0;
        }

        if (DEF >= MAXDEF)
        {
            DEF = MAXDEF;
        }
        if (DEF < 0)
        {
            DEF = 0;
        }

    }

    protected void debugCircleRay(float range, Color color)
    {
        //Debug attack rays
        Debug.DrawRay(transform.position, lookingDir * range, color);
        Debug.DrawRay(transform.position, (lookingDir + Vector2.right).normalized * range, color);
        Debug.DrawRay(transform.position, (lookingDir + Vector2.left).normalized * range, color);
        Debug.DrawRay(transform.position, (lookingDir + Vector2.up).normalized * range, color);
        Debug.DrawRay(transform.position, (lookingDir + Vector2.down).normalized * range, color);
        Debug.DrawRay(transform.position, (-lookingDir + Vector2.right).normalized * range, color);
        Debug.DrawRay(transform.position, (-lookingDir + Vector2.left).normalized * range, color);
        Debug.DrawRay(transform.position, (-lookingDir + Vector2.up).normalized * range, color);
        Debug.DrawRay(transform.position, (-lookingDir + Vector2.down).normalized * range, color);
    }

    private void getInput()
    {

        if (!isAgent)
        {
            inputX = Input.GetAxis("Horizontal");
            inputY = Input.GetAxis("Vertical");
            if (Input.GetButtonDown("Fire1"))
            {
                isFire1Pressed = true;
            }
            if (Input.GetButtonDown("Fire2"))
            {
                isFire2Pressed = true;
            }
            if (Input.GetButtonDown("Fire3"))
            {
                isFire3Pressed = true;
            }
        }

    }


    protected virtual void Update()
    {
        //We check max/min stats
        

        checkStats();
        getInput();

   }

    protected virtual void FixedUpdate()
    {


        regenMP();

        if (isBusy)
        {
            debugCircleRay(atkRange, Color.red);
        }

        if(HP <=0 && !isKO)
        {
            KO();
        }

        if (isInputEnable)
        {
            move();

            //Attack
            if (isFire1Pressed && !isBusy && MP >= MPCostAtk)
            {
                anim.SetTrigger("SetAttack");
                
            }
            
            
            if (isFire2Pressed && !isBusy && isSpecial1Available && MP >= MPCostSpe1)
            {
                anim.SetTrigger("SetSpecial1");
                
            }

            if (isFire3Pressed && !isBusy && isSpecial2Available && MP >= MPCostSpe2)
            {
                anim.SetTrigger("SetSpecial2");

            }
            

            isFire1Pressed = false;
            isFire2Pressed = false;
            isFire3Pressed = false;

        }
        
    }
}
