using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class mageAgent : mainAgent
{
  





    //Observation (inputs)
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(distanceVector(target, character));// 2 sensors
        sensor.AddObservation(distanceVector(target, character).magnitude);// 1 sensor
        sensor.AddObservation(baseScript.getHP()); //1 sensor
        sensor.AddObservation(baseScript.getMP()); //1 sensor
        sensor.AddObservation(baseScript.isBusy); //1sensor
        sensor.AddObservation(baseScript.isSpecial1Available); //1sensor
        sensor.AddObservation(baseScript.isSpecial2Available);  //1 MAGE sensor

        sensor.AddObservation(distanceVector(ally1, character)); //2 sensors
        sensor.AddObservation(distanceVector(ally1, character).magnitude);// 1 sensor
        sensor.AddObservation(distanceVector(ally2, character)); //2 sensors
        sensor.AddObservation(distanceVector(ally2, character).magnitude);// 1 sensor
        sensor.AddObservation(ally1.GetComponent<baseCharacter>().getHP());
        sensor.AddObservation(ally2.GetComponent<baseCharacter>().getHP());


        //Monsters sensors
        if (enemy1 != null)
        {
            if (enemy1.tag != "Monster1") //5 sensors
            {
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
            }
            else
            {
                sensor.AddObservation(distanceVector(enemy1, character));
                sensor.AddObservation(distanceVector(enemy1, character).magnitude);
                sensor.AddObservation(enemy1.GetComponent<baseMonster>().getHP());
                switch (enemy1.tag)
                {
                    case "Fish":
                        sensor.AddObservation(1);
                        break;
                    case "Octopus":
                        sensor.AddObservation(2);
                        break;
                    case "Tree":
                        sensor.AddObservation(3);
                        break;
                    case "Snake":
                        sensor.AddObservation(4);
                        break;
                    case "Worm":
                        sensor.AddObservation(5);
                        break;


                }
            }
        }
        else
        {
            sensor.AddObservation(0);
            sensor.AddObservation(0);
            sensor.AddObservation(0);
            sensor.AddObservation(0);
            sensor.AddObservation(0);
        }


        if (enemy2 != null)
        {
            if (enemy2.tag != "Monster1") //5 sensors
            {
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
                sensor.AddObservation(0);
            }
            else
            {
                sensor.AddObservation(distanceVector(enemy2, character));
                sensor.AddObservation(distanceVector(enemy2, character).magnitude);
                sensor.AddObservation(enemy2.GetComponent<baseMonster>().getHP());
                switch (enemy2.tag)
                {
                    case "Fish":
                        sensor.AddObservation(1);
                        break;
                    case "Octopus":
                        sensor.AddObservation(2);
                        break;
                    case "Tree":
                        sensor.AddObservation(3);
                        break;
                    case "Snake":
                        sensor.AddObservation(4);
                        break;
                    case "Worm":
                        sensor.AddObservation(5);
                        break;


                }
            }
        }
        else
        {
            sensor.AddObservation(0);
            sensor.AddObservation(0);
            sensor.AddObservation(0);
            sensor.AddObservation(0);
            sensor.AddObservation(0);
        }





    }

    //Reward
    //Note: MaxStep can be usd ot avoid a run to last to long by putting a limit of time for each run (avoid static behaviour from agent)


    protected override void reward()
    {
        if (distanceVector(this.gameObject, target).magnitude <= rewardDistTarget)
        {
            Debug.Log("TargetReward");
            SetReward(1f);
            //EndEpisode();
        }
        else
        {
            SetReward(-1f);
        }

        if (distanceVector(this.gameObject, target).magnitude < lastDistance)
        {
           // Debug.Log("DistanceReward");
            //SetReward(0.2f);
        }
        lastDistance = distanceVector(this.gameObject, target).magnitude;

        if (baseScript.getHP() <= 0)
        {
            Debug.Log("DeadReward");
            AddReward(-1f);
            EndEpisode();

        }

        if (baseScript.getHP() < lastHP)
        {
            Debug.Log("HitReward");
            AddReward(-0.05f);
            lastHP = baseScript.getHP();
        }


        if (step >= maxStep)
        {
            //SetReward(-1f);
            Debug.Log("Max step reward");
            EndEpisode();
        }


        if (baseScript.isHittingWall)
        {
            AddReward(-0.01f);
            Debug.Log("Wall");
        }


        //Atk reward
        if (Atk == 1)
        {
            if ((baseScript.isBusy) || (baseScript.getMP() <= baseScript.MPCostAtk)) //If attack when busy
            {
                Debug.Log("busy reward");
                AddReward(-0.1f);
            }
            else
            {
                if (enemy1 == null) //make sure enemy1 was not destroy
                {

                }
                else
                {
                    if (distanceVector(enemy1, character).magnitude > baseScript.atkRange + addedRange || enemy1.tag == "Player") //If attack when monster not nearby
                    {
                        Debug.Log("Mage atk while too far");
                        AddReward(-0.2f);
                    }
                    else
                    {
                        Debug.Log("Mage atk reward");
                        AddReward(0.1f);
                    }
                }
            }
            
           
        }



        if (Spe1 == 1)
        {
            if (baseScript.isBusy || !baseScript.isSpecial1Available || (baseScript.getMP() <= baseScript.MPCostSpe1)) //If attack when busy
            {

                AddReward(-0.1f);
            }
            else
            {
                if (baseScript.getHP() < baseScript.MAXHP * 0.8f
                    ||
                    (distanceVector(ally1, character).magnitude <= baseScript.special1Range + addedRange && ally1.GetComponent<baseCharacter>().getHP() < ally1.GetComponent<baseCharacter>().MAXHP)
                    ||
                    (distanceVector(ally2, character).magnitude <= baseScript.special1Range + addedRange && ally2.GetComponent<baseCharacter>().getHP() < ally2.GetComponent<baseCharacter>().MAXHP))
                {
                    
                    if (distanceVector(this.gameObject, target).magnitude <= rewardDistTarget)
                    {
                        Debug.Log("Mage heal reward");
                        AddReward(0.5f);
                    }
                    else
                    {
                        Debug.Log("Mage heal while far from flag");
                        AddReward(0.05f);
                    }
                        
                    
                }
                else
                {
                    Debug.Log("Mage heal useless");
                    AddReward(-0.3f);
                }


            }

    
        }

        if (Spe2 == 1)
        {

            if (baseScript.isBusy || !baseScript.isSpecial2Available || baseScript.getMP() <= baseScript.MPCostSpe2)
            {
                Debug.Log("busy reward");
                AddReward(-0.1f);
            }
            else
            {
                if (distanceVector(ally1, character).magnitude >= baseScript.special2Range + addedRange && distanceVector(ally2, character).magnitude >= baseScript.special1Range + addedRange) //If attack when monster not nearby
                {
                    Debug.Log("Mage rez while too far");
                    AddReward(-0.3f);
                }
                else
                {
                    if (ally2.GetComponent<baseCharacter>().getHP() > 0 && ally2.GetComponent<baseCharacter>().getHP() > 0)
                    {
                        Debug.Log("Mage rez while useless");
                        AddReward(-0.3f);
                    }
                    else
                    {
                        Debug.Log("Mage rez reward");
                        AddReward(1f);
                    }
                }
            }
        }

            
        


        if(enemy1 ==null)
        { }
        else
        {
            if (enemy1.tag == "Tree" && distanceVector(enemy1, character).magnitude <= 5f)
            {
                AddReward(0.3f);
            }
            
        }

        if (enemy2 == null)
        { }
        else
        {
            if (enemy2.tag == "Tree" && distanceVector(enemy2, character).magnitude <= 5f)
            {
                AddReward(0.3f);
            }
        }



    }

    /*
    public override void addOutputs(ActionBuffers actions)
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
    */
}