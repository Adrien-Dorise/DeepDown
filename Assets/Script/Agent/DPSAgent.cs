using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class DPSAgent : Agent {

    //Main scripts
    [SerializeField] private baseCharacter baseScript;
    private GameObject character;
    private DPS classScript;

    //ML stuff
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject sinObject;
    [SerializeField] private GameObject ally1;
    [SerializeField] private GameObject ally2;
    [SerializeField] private GameObject enemy1;
    [SerializeField] private GameObject enemy2;
    private int lastHP;
    private float rewardDistTarget;
    private float lastDistance;

    //Visualize output
    [SerializeField] private float outX;
    [SerializeField] private float outY;
    [SerializeField] private int Fire1;
    [SerializeField] private int Fire2;

    //Vision
    [SerializeField] private float visionRange;
    [SerializeField] private LayerMask hitboxLayerMask;
    [SerializeField] private Collider2D[] potentialTarget;


    private void Start()
    {
        character = this.transform.parent.gameObject;
        baseScript = this.transform.parent.gameObject.GetComponentInChildren<baseCharacter>();
        classScript = this.transform.parent.gameObject.gameObject.GetComponentInChildren<DPS>();
        lastDistance = 0f;
        lastHP = baseScript.getHP();

        //ML stuff
        enemy1 = this.gameObject; //By taking this as enemy, we do the distance between this and this -> vector2(0,0)
        enemy2 = this.gameObject; //By taking this as enemy, we do the distance between this and this -> vector2(0,0)
        rewardDistTarget = 1.5f;

        //Vision
        visionRange = 15f;
    }

    private Vector2 distanceVector(GameObject object1, GameObject object2)
    {
        float x, y;

        x = object1.transform.position.x - object2.transform.position.x;
        y = object1.transform.position.y - object2.transform.position.y;
        return new Vector2(x, y);
    }


    private void findClosestEnemy()
    {
        Vector2 targetDist;

        
        potentialTarget = Physics2D.OverlapCircleAll(this.transform.position, visionRange, hitboxLayerMask);


        Debug.Log("FindEnemy");
        float minDist1 = 10000f;
        float minDist2 = minDist1;
        for (int i = 0; i < potentialTarget.Length; i++)
        {
            Debug.Log(potentialTarget[i].transform.tag);
            switch (potentialTarget[i].transform.tag)
            {
                case "Monster1":
                    if (!potentialTarget[i].transform.parent.GetComponent<baseMonster>().isDead)
                    {
                        targetDist = distanceVector(this.gameObject, potentialTarget[i].gameObject);
                        if(Mathf.Abs(targetDist.magnitude) <= minDist1)
                        {

                            enemy2 = enemy1;
                            minDist2 = minDist1;



                            enemy1 = potentialTarget[i].transform.parent.gameObject;
                            minDist1 = Mathf.Abs(targetDist.magnitude);
                        }
                    }
                    break;
            }
        }
    }

        

       
    

    


    //This function is called when a new training run is starting
    public override void OnEpisodeBegin()
    {
        
        character.transform.localPosition = new Vector3(0,0,0); //We reinitialize the agent position (we use local position in case of multiple training, we want to reinitialize in the local environment.
       // target.transform.localPosition = new Vector3(Random.Range(-0.8f, 1f), 26.7f, Random.Range(-1f, 1f));
        
    }

    

    //Observation (inputs)
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(distanceVector(this.gameObject,target)); //As position is vector2, here we are passing 2 informations into the sensor.
        /*sensor.AddObservation(distanceVector(this.gameObject, sinObject)); //As position is vector2, here we are passing 2 informations into the sensor.
        sensor.AddObservation(distanceVector(this.gameObject, ally1)); //As position is vector2, here we are passing 2 informations into the sensor.
        sensor.AddObservation(ally1.GetComponent<baseCharacter>().getHP());
        sensor.AddObservation(distanceVector(this.gameObject, ally2)); //As position is vector2, here we are passing 2 informations into the sensor.
        sensor.AddObservation(ally2.GetComponent<baseCharacter>().getHP());

        
        sensor.AddObservation(distanceVector(this.gameObject, enemy1)); //As position is vector2, here we are passing 2 informations into the sensor.
        if (enemy1.tag == "Monster1")
        {
            sensor.AddObservation(enemy1.GetComponent<baseMonster>().getHP());
        }
        else
        {
            sensor.AddObservation(0);
        }

        sensor.AddObservation(distanceVector(this.gameObject, enemy2)); //As position is vector2, here we are passing 2 informations into the sensor.
        if (enemy2.tag == "Monster1")
        {
            sensor.AddObservation(enemy2.GetComponent<baseMonster>().getHP());
        }
        else
        {
            sensor.AddObservation(0);
        }
        */// 16 observations
    }
    
    

    //Decision + Action (outputs)
    public override void OnActionReceived(ActionBuffers actions)
    {
        //Debug.Log(actions.DiscreteActions[0]);
        outX = baseScript.inputX = actions.ContinuousActions[0]; //Output 1 refer to X movement
        outY= baseScript.inputY = actions.ContinuousActions[1]; //Output 2 refer to Y movement

        Fire1 = actions.DiscreteActions[0];
        Fire2 = actions.DiscreteActions[1];
        if (actions.DiscreteActions[0] == 1)
        { baseScript.isFire1Pressed = true; }
        if(actions.DiscreteActions[1] == 1)
        { baseScript.isFire2Pressed = true; }
        //if(actions.DiscreteActions[2] == 1)
        //{ baseScript.isFire3Pressed = true; }



    }

    

    //Reward
    //Note: MaxStep can be usd ot avoid a run to last to long by putting a limit of time for each run (avoid static behaviour from agent)


    private void reward()
    {
        if (distanceVector(this.gameObject, target).magnitude <= rewardDistTarget)
        {
            Debug.Log("TargetReward");
            SetReward(1f);
            EndEpisode();
        }

        if(distanceVector(this.gameObject, target).magnitude < lastDistance)
        {
            Debug.Log("DistanceReward");
            SetReward(0.05f);
        }
        lastDistance = distanceVector(this.gameObject, target).magnitude;

        if (baseScript.getHP() <= 0)
        {
            Debug.Log("DeadReward");
            SetReward(-1f);
            EndEpisode();

        }

        if(baseScript.getHP() < lastHP)
        {
            Debug.Log("HitReward");
            AddReward(-0.05f);
            lastHP = baseScript.getHP();
        }

    }





    
    //Controlling everything ourselves to be sure it is working (You can choose Heuristic or Default in Behavior type in unit)
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");

        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = 0;
        discreteActions[1] = 0;
        if (Input.GetButtonDown("Fire1"))
        {
            discreteActions[0] = 1;
        }
        if(Input.GetButtonDown("Fire2"))
        {
            discreteActions[1] = 1;
        }
       

    }

    

    private void Update()
    {
        baseScript.isAgent = true;
        findClosestEnemy();
        reward();
    }

}

