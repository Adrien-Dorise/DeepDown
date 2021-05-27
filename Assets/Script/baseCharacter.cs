using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

//To attach to Player game object
public class baseCharacter : MonoBehaviour
{

    //Machine Learning
    protected bool isAgent;


    //Others
    private float startTime;


    //Components
    private Rigidbody2D rigidBody;
    private Animator anim;

    //Movements
    [SerializeField] private bool isInputEnable;
    [SerializeField] protected float speed;
    protected float baseSpeed;
    [SerializeField] protected Vector2 lookingDir;

    //Attack
    [SerializeField] protected LayerMask attackLayerMask;
    protected Collider2D[] enemyColl, allyColl;
    private RaycastHit2D hitForward;
    Vector2 rayDir;
    [SerializeField] protected bool isFire1Pressed, isFire2Pressed, isFire3Pressed, isBusy, isSpecial1Available, isSpecial2Available;
    [SerializeField] protected float atkRange, special1Range, special2Range;
    [SerializeField] protected int atkDelay, special1Delay, special2Delay;


    //Hit
    [SerializeField] bool isInvincible;
    [SerializeField] private float ejectionForce, invincibilityDelay, inviFrameSpeed;

    //Status
    [SerializeField] public bool isKO, isHealing;
    [SerializeField] protected int HP, MAXHP, ATK, DEF;
    protected int baseATK, baseDEF;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        //Machine Learning
        isAgent = false;

        //Components
        rigidBody = this.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        

        //Movement
        isInputEnable = true;
        baseSpeed = speed = 2f;

        //Attacks
        isBusy = false;
        isFire1Pressed = isFire2Pressed = false;
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
        HP = 100;
        MAXHP = HP;
        baseDEF = DEF = 2;
    }

    private void setBoolDefault()
    {
        isBusy = false;
        isInvincible = false;
    }


    protected virtual void move()
    {
        if(!isAgent)
        {

        
        if (Input.GetAxis("Horizontal") > 0)
        { this.transform.GetComponentInChildren<SpriteRenderer>().flipX = true; }
        if (Input.GetAxis("Horizontal") < 0)
        { this.transform.GetComponentInChildren<SpriteRenderer>().flipX = false; }

        this.transform.Translate(Vector2.up * speed * Time.fixedDeltaTime * Input.GetAxis("Vertical"));
        this.transform.Translate(Vector2.right * speed * Time.fixedDeltaTime * Input.GetAxis("Horizontal"));
        anim.SetFloat("Speed", Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical")));


        //We update the looking direciton vector when one of the player is pressing a direction
        if (Input.GetAxis("Vertical") + Input.GetAxis("Horizontal") != 0) 
        { lookingDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized; }
       
        Debug.DrawRay(transform.position, lookingDir * 2f, Color.white);

        }

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

            StartCoroutine(hitRoutine());
            //anim.SetTrigger("SetHurt");
            Vector2 direction = (new Vector2(this.transform.position.x, this.transform.position.y) - hitPosition).normalized;
            rigidBody.AddForce(direction * ejectionForce);


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
        anim.SetTrigger("SetKO");
    }

    public int getMaxHP()
    {
        return MAXHP;
    }

    public void getHeal(int HPrecovered)
    {
        isHealing = true;
        this.transform.GetChild(1).gameObject.GetComponent<Animator>().SetTrigger("isHealing");
        if (HP < MAXHP)
        {
            HP += HPrecovered;
            if (HP > MAXHP)
            {
                HP = MAXHP;
            }
        }
        isHealing = false;
    }



    public void resurection(int HPrestored)
    {
        
        anim.SetTrigger("SetRez");
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
        isSpecial1Available = isSpecial2Available = true;
        isInvincible = false;
        isInputEnable = true;
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

    protected virtual void Update()
    {
        

        if(DEF >=80)
        {
            DEF = 80;
        }

        if(!isAgent)
        {
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

    protected virtual void FixedUpdate()
    {

        if(isBusy)
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
            if (isFire1Pressed && !isBusy)
            {
                anim.SetTrigger("SetAttack");
                
            }
            
            
            if (isFire2Pressed && !isBusy && isSpecial1Available)
            {
                anim.SetTrigger("SetSpecial1");
                
            }

            if (isFire3Pressed && !isBusy && isSpecial2Available)
            {
                anim.SetTrigger("SetSpecial2");

            }

            isFire1Pressed = false;
            isFire2Pressed = false;
            isFire3Pressed = false;

        }
        
    }
}
