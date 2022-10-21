using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class mainAgent : Agent
{

    //Main scripts
    [SerializeField] protected baseCharacter baseScript;
    protected GameObject character;
    protected int MAXHP;

    //ML stuff
    [SerializeField] bool isTraining;
    [SerializeField] protected GameObject target;
    [SerializeField] protected GameObject sinObject;
    [SerializeField] protected GameObject ally1;
    [SerializeField] protected GameObject ally2;
    [SerializeField] protected GameObject enemy1;
    [SerializeField] protected GameObject enemy2;
    protected int lastHP;
    protected float rewardDistTarget;
    protected float lastDistance;
    protected int maxStep, step;

    [SerializeField] protected int moveX, moveY, Atk, Spe1, Spe2;

    //Visualize output
    [SerializeField] protected float outX;
    [SerializeField] protected float outY;
    [SerializeField] protected int Fire1;
    [SerializeField] protected int Fire2;

    //Vision
    [SerializeField] protected float visionRange;
    [SerializeField] protected LayerMask hitboxLayerMask;
    [SerializeField] protected Collider2D[] potentialTarget;


    //other
    protected float addedRange;



    protected virtual void Start()
    {
        isTraining = false; //Change that depending on training or play phase
        character = this.transform.parent.gameObject;
        baseScript = this.transform.parent.gameObject.GetComponentInChildren<baseCharacter>();
        lastDistance = 0f;
        lastHP = baseScript.getMaxHP();
        MAXHP = baseScript.getMaxHP();

        addedRange = 0.5f;
        //ML stuff
        /*
        target = GameObject.Find("Learning Target").gameObject;
        sinObject = GameObject.Find("Move Target").gameObject;
        ally1 = GameObject.Find("Tank").gameObject;
        ally2 = GameObject.Find("Mage").gameObject;
        */


        enemy1 = character.gameObject; //By taking this as enemy, we do the distance between this and this -> vector2(0,0)
        enemy2 = character.gameObject; //By taking this as enemy, we do the distance between this and this -> vector2(0,0)
        rewardDistTarget = 4f;
        maxStep = 1000;
        step = 0;

        //Vision
        visionRange = 10f;
    }

    protected Vector2 distanceVector(GameObject object1, GameObject object2)
    {
        
        float x, y;

        x = object1.transform.position.x - object2.transform.position.x;
        y = object1.transform.position.y - object2.transform.position.y;
        return new Vector2(x, y);
    }


    private void findClosestEnemy()
    {
        Vector2 targetDist;

        if (enemy1 == null)
        {
            enemy1 = character.gameObject;
        }
        if (distanceVector(character, enemy1).magnitude > visionRange)
        {
            enemy1 = character.gameObject;
        }

        if (enemy2 == null)
        {
            enemy2 = character.gameObject;
        }
        if (distanceVector(character, enemy2).magnitude > visionRange)
        {
            enemy2 = character.gameObject;
        }

        potentialTarget = Physics2D.OverlapCircleAll(character.transform.position, visionRange, hitboxLayerMask);


        //Debug.Log("FindEnemy");
        float minDist1 = 10000f;
        float minDist2 = minDist1;
        for (int i = 0; i < potentialTarget.Length; i++) //Find closest monster
        {
            //Debug.Log(potentialTarget[i].transform.tag);
            switch (potentialTarget[i].transform.tag)
            {
                case "Snake":
                case "Tree":
                case "Octopus":
                case "Fish":
                case "Worm":
                    if (!potentialTarget[i].transform.GetComponent<baseMonster>().isDead)
                    {
                        targetDist = distanceVector(character.gameObject, potentialTarget[i].gameObject);
                        if (Mathf.Abs(targetDist.magnitude) <= minDist1)
                        {

                            enemy2 = enemy1;
                            minDist2 = minDist1;



                            enemy1 = potentialTarget[i].transform.parent.GetChild(0).gameObject;
                            minDist1 = Mathf.Abs(targetDist.magnitude);

                        }
                    }
                    break;
            }
            
        }
        if (GameObject.ReferenceEquals(enemy1, enemy2))
        {
            enemy2 = character.gameObject;
        }

    }









    //This function is called when a new training run is starting
    public override void OnEpisodeBegin()
    {
        if (isTraining)
        {

            //character.transform.localPosition = new Vector3(0, 0, 0); //We reinitialize the agent position (we use local position in case of multiple training, we want to reinitialize in the local environment.
            MAXHP = baseScript.getMaxHP();
            lastHP = MAXHP;
            baseScript.resurection(MAXHP);
            //baseScript.anim.SetBool("SetRez", false);
            step = 0;
            //character.transform.localPosition = new Vector3(Random.Range(-9f, 0f), Random.Range(4, 12f), 0);
            //To random start

            int randomStart;
            randomStart = Random.Range(0, 3);
            switch (randomStart)
            {
                case 0:
                    character.transform.localPosition = new Vector3(Random.Range(-9f, -0f), Random.Range(-5f, 0f), 0);
                    break;
                case 1:
                    character.transform.localPosition = new Vector3(Random.Range(-9f, 0f), Random.Range(4f, 12f), 0);
                    break;
                case 2:
                    character.transform.localPosition = new Vector3(Random.Range(-17f, -13f), Random.Range(-0.5f,8.5f), 0);
                    break;

            }

        }
    }






    //Decision + Action (outputs)
    public override void OnActionReceived(ActionBuffers actions)
    {

        switch (actions.DiscreteActions[0]) //Movement X
        {
            case 0:
                outX = baseScript.inputX = 0;
                break;
            case 1:
                outX = baseScript.inputX = 1;
                break;
            case 2:
                outX = baseScript.inputX = -1;
                break;
        }

        switch (actions.DiscreteActions[1]) //movement Y
        {
            case 0:
                outY = baseScript.inputY = 0;
                break;
            case 1:
                outY = baseScript.inputY = 1;
                break;
            case 2:
                outY = baseScript.inputY = -1;
                break;
        }

        switch (actions.DiscreteActions[2]) //Fire1
        {
            case 0:
                baseScript.isFire1Pressed = false;
                break;
            case 1:
                baseScript.isFire1Pressed = true;
                break;
        }

        switch (actions.DiscreteActions[3]) //Fire2
        {
            case 0:
                baseScript.isFire2Pressed = false;
                break;
            case 1:
                baseScript.isFire2Pressed = true;
                break;
        }

        moveX = actions.DiscreteActions[0];
        moveY = actions.DiscreteActions[1];
        Atk = actions.DiscreteActions[2];
        Spe1 = actions.DiscreteActions[3];
        Spe2 = actions.DiscreteActions[0];


        if (character.transform.parent.name == "MageAgent")
        {
            switch (actions.DiscreteActions[4]) //Fire3
            {
                case 0:
                    baseScript.isFire3Pressed = false;
                    break;
                case 1:
                    baseScript.isFire3Pressed = true;
                    break;
            }
            Spe2 = actions.DiscreteActions[4];
        }

            addOutputs(actions);
        
}

    public virtual void addOutputs(ActionBuffers actions) { }


    //Reward
    //Note: MaxStep can be usd ot avoid a run to last to long by putting a limit of time for each run (avoid static behaviour from agent)

    //TO OVERRIDE
    protected virtual void reward()
    {
        



    }






    //Controlling everything ourselves to be sure it is working (You can choose Heuristic or Default in Behavior type in unit)
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        if (Input.GetAxisRaw("Horizontal") == 0)
        {
            discreteActions[0] = 0;
        }
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            discreteActions[0] = 1;
        }
        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            discreteActions[0] = 2;
        }

        if (Input.GetAxisRaw("Vertical") == 0)
        {
            discreteActions[1] = 0;
        }
        if (Input.GetAxisRaw("Vertical") > 0)
        {
            discreteActions[1] = 1;
        }
        if (Input.GetAxisRaw("Vertical") < 0)
        {
            discreteActions[1] = 2;
        }


        discreteActions[2] = 0;
        discreteActions[3] = 0;
        if (Input.GetButton("Fire1"))
        {
            discreteActions[2] = 1;
        }
        if (Input.GetButton("Fire2"))
        {
            discreteActions[3] = 1;
        }

        if(character.transform.parent.name == "MageAgent")
        {
            discreteActions[4] = 0;
            if (Input.GetButton("Fire3"))
            {
                discreteActions[4] = 1;
            }
        }


    }



    private void Update()
    {
        baseScript.isAgent = true;
        baseScript.hasToFollow = false;
        findClosestEnemy();
        


    }

    private void FixedUpdate()
    {

        reward();
        if (isTraining)
        {
            step += 1;
        }

    }

}

