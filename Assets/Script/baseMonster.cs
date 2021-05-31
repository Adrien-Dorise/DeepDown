using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class baseMonster : MonoBehaviour
{
    //Others
    private float startTime;
    

    //Components
    private Rigidbody2D rigidBody;
    private Animator anim;
    private GameObject startPoint;

    //Movements
    private Vector2 dir;
    [SerializeField] private float speed, chillSpeed;


    //Attack
    [SerializeField] private LayerMask attackLayerMask; //Put only hitbox for this layer
    Collider2D[] enemyColl;
    private RaycastHit2D hitForward;
    Vector2 rayDir;
    [SerializeField] private bool isFirePressed, isBusy;
    [SerializeField] private float atkRange;
    [SerializeField] private int atkDelay;

    //Hit
    [SerializeField] bool isInvincible;
    [SerializeField] private float ejectionForce, invincibilityDelay, inviFrameSpeed;

    //Status
    public bool isDead;
    [SerializeField] private int HP, ATK, DEF;

    //AI
    [SerializeField] private LayerMask aggroLayerMask; //Put only player for this layer
    private Collider2D[] potentialTarget;
    [SerializeField] private GameObject target;
    [SerializeField] private bool isAggro;
    [SerializeField] private float beginAtkRange, aggroRange, minDistFromTarget, startingPointRange;
    private float distFromTarget, distFromStartingPoint, distTargetStartPoint;

    // Start is called before the first frame update
    void Start()
    {
        //Components
        rigidBody = this.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        startPoint = this.transform.parent.GetChild(1).gameObject;

        //Movements
        speed = 0.5f;
        chillSpeed = 0.3f;

        //Attack
        isBusy = false;
        isFirePressed = false;
        atkRange = 1f;
        atkDelay = 1000; //Delay in ms

        //Hit
        isInvincible = false;
        ejectionForce = 500f;
        invincibilityDelay = 1f; //Delay in s
        inviFrameSpeed = 0.05f; //Delay in s

        //Status
        isDead = false;
        HP = 100;
        ATK = 15;
        DEF = 2;

        //AI
        isAggro = false;
        target = GameObject.Find("Base Target").gameObject; //A base target must be created to initialize this gameObject
        beginAtkRange = 1f;
        aggroRange = 5f;
        minDistFromTarget = 0.5f;
        startingPointRange = 5f;
    }

    private void setBoolDefault()
    {
        isBusy = false;
        isInvincible = false;
    }


    private void move(Vector2 destination)
    {
        dir = (destination - new Vector2(this.transform.position.x, this.transform.position.y)).normalized;
        this.transform.Translate(dir * speed * Time.fixedDeltaTime);
    }



    private void triggerAttack()
    {
        if (!isBusy)
        {
            isBusy = true;
            anim.SetTrigger("SetAttack");
        }
    }

    private void attack()
    {
        //We use a circle to check collision around player
        enemyColl = Physics2D.OverlapCircleAll(this.transform.position, atkRange, attackLayerMask);
        if (enemyColl.Length > 0)
        {
            
            for (int i = 0; i < enemyColl.Length; i++)
            {
                Debug.Log(this.transform.parent.name + " hit " + enemyColl[i].transform.parent.transform.parent.name);
                switch (enemyColl[i].tag)
                {
                    case "Player":
                        if(!enemyColl[i].GetComponentInParent<baseCharacter>().isKO) //We make sure that the target is alive;
                        {
                            enemyColl[i].GetComponentInParent<baseCharacter>().getHit(ATK, transform.position);
                        }
                        break;
                }
            }  
        }
    }

    private void endAtk()
    {
        isBusy = false;
    }




    public void getHit(int damage,Vector2 hitPosition)
    {
        if (!isInvincible)
        {
            isInvincible = true;

            HP -= (int)(damage * (1f - (float)DEF / 100f));
            StartCoroutine(hitRoutine());
            Vector2 direction = (new Vector2(this.transform.position.x, this.transform.position.y) - hitPosition).normalized;
            rigidBody.AddForce(direction * ejectionForce);


        }
    }

    private IEnumerator hitRoutine()
    {
        startTime = Time.time;
        while(Time.time - startTime <= invincibilityDelay)
        {
            this.GetComponent<SpriteRenderer>().enabled = !this.GetComponent<SpriteRenderer>().enabled;
            yield return new WaitForSeconds(inviFrameSpeed);
        }

        this.GetComponent<SpriteRenderer>().enabled = true;
        isInvincible = false;
    }




    //We chill when no target are in proximity
    private void chill()
    {
        distFromStartingPoint = (startPoint.transform.position - this.transform.position).magnitude;
        if (distFromStartingPoint > startingPointRange / 2)
        {
            dir = (startPoint.transform.position - this.transform.position).normalized;
            this.transform.Translate(dir * chillSpeed * Time.fixedDeltaTime);
        }


        if (distFromStartingPoint <= startingPointRange / 2)
        {
            dir = (startPoint.transform.position - this.transform.position + new Vector3(Random.Range(-1,1), Random.Range(-1, 1),0)).normalized;
            this.transform.Translate(dir * chillSpeed * Time.fixedDeltaTime);
        }
        Debug.DrawRay(this.transform.position, dir * 2);
    }


    private void aggro()
    {
        //Put Aggro
        //We put the aggro when a character is within aggro range of monster
        //The closest character is the target

        potentialTarget = Physics2D.OverlapCircleAll(this.transform.position, aggroRange, aggroLayerMask);


        for(int i = 0; i < potentialTarget.Length; i++)
        {
            //Make sure that targer is not dead
            if (!isAggro)
            {
                switch (potentialTarget[i].transform.tag)
                {
                    
                    case "Player":
                        if (!potentialTarget[i].transform.GetComponent<baseCharacter>().isKO)
                        {
                            //Debug.Log("target found: " + potentialTarget[i].transform.parent.tag);
                            target = potentialTarget[i].gameObject;
                            isAggro = true;
                        }
                        break;
                }
            }
        }
            



        distFromTarget = (target.transform.position - this.transform.position).magnitude;
        distTargetStartPoint = (startPoint.transform.position - target.transform.position).magnitude;



        //Removing Aggro

        //We remove aggro when player is leaving monster perimeter
        if (isAggro && distTargetStartPoint > startingPointRange)
        {
            isAggro = false;
        }

        //We remove aggro if character is dead
        switch(target.tag)
        {
            case "Player":
                if(target.GetComponentInParent<baseCharacter>().isKO)
                {
                    isAggro = false;
                }
                break;
        }




        //target = GameObject.Find("Player").gameObject.transform.GetChild(0).gameObject;
    }

    private void death()
    {
        isDead = true;
        Destroy(this.transform.parent.gameObject);
    }

    public int getHP()
    {
        return HP;
    }


    private void AI()
    {
        aggro();
        distFromTarget = (target.transform.position - this.transform.position).magnitude;
        distTargetStartPoint = (startPoint.transform.position - target.transform.position).magnitude;

        //We see if our target is in our aggro range. If yes, we move toward it. But if we are too close from target, we stop moving to let some space
        if (isAggro)
        {
            if(distFromTarget >= minDistFromTarget)
            {
                move(new Vector2(target.transform.position.x, target.transform.position.y));
            }
            
        }
        //If we are far from a the target, we can chill as fuck
        else
        {
            chill();
        }



        //If the target is within atk range, we start the attack animation
        if (distFromTarget <= beginAtkRange)
        {
            triggerAttack();
        }

    }



    // Update is called once per frame
    void Update()
    {

        if (DEF >= 80)
        {
            DEF = 80;
        }

        AI();

        if(HP <= 0 )
        {
            death();
        }
        
    }
}
