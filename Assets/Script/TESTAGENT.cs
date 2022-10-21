using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;


//To train the agent: in the virtual environment created for the project, use the command: "mlagents-learn --run-id=[RUN_NAME]
//Then, when the training is done, find the results/[FileName].onnx , put it in Unity and place it in the Model parameter of the agent
public class TESTAGENT : Agent
{
    [SerializeField] private GameObject target;
    [SerializeField] private float speed = 3f;




    private void Start()
    {
        //target = GameObject.Find("Goal").gameObject;
    }

    //This function is called when a new training run is starting
    public override void OnEpisodeBegin()
    {
        this.transform.localPosition = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-0.5f, 1f)); //We reinitialize the agent position (we use local position in case of multiple training, we want to reinitialize in the local environment.
        //target.transform.localPosition = new Vector3(Random.Range(-0.8f, 1f), 26.7f, Random.Range(-1f, 1f));
    }



    //Observation (inputs)
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.localPosition); //As position is vector3, here we are passing 3 informations into the sensor.
        sensor.AddObservation(target.transform.localPosition); //As position is vector3, here we are passing 3 informations into the sensor.
    }

    //Decision + Action (outputs)
    public override void OnActionReceived(ActionBuffers actions)
    {
        //Debug.Log(actions.DiscreteActions[0]);
        float moveX = actions.ContinuousActions[0]; //Output 1 refer to X movement
        float moveZ = actions.ContinuousActions[1]; //Output 2 refer to Z movement

        transform.localPosition += new Vector3(moveX, moveZ, 0) * Time.fixedDeltaTime * speed;

    }

    //Reward
    //Note: MaxStep can be usd ot avoid a run to last to long by putting a limit of time for each run (avoid static behaviour from agent)
    private void OnTriggerStay2D(Collider2D other)
    {
        //SetReward() is putting the reward value to a certain amount 
        //AddReward() is increasing the current reward
        
        if (other.tag == "Target")
        {
            Debug.Log("Enter");
            SetReward(1f);
            EndEpisode(); //End the current run and start a new training
        }

        if (other.tag == "Wall")
        {
            SetReward(-1f);
            EndEpisode(); //End the current run and start a new training
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Wall");

        SetReward(-1f);
        EndEpisode(); //End the current run and start a new training
        
    }

    //Controlling everything ourselves to be sure it is working (You can choose Heuristic or Default in Behavior type in unit)
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");

    }



}
