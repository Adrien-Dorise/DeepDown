using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;


public class tankAgent : mainAgent
{

    


    //Observation (inputs)
    public override void CollectObservations(VectorSensor sensor)
    {
        if(character == null)
        {
           
        }
        else
        {
            sensor.AddObservation(distanceVector(target, character));// 2 sensors
            sensor.AddObservation(distanceVector(target, character).magnitude);// 1 sensor
            sensor.AddObservation(baseScript.getHP()); //1 sensor
            sensor.AddObservation(baseScript.isBusy); //1sensor
            sensor.AddObservation(baseScript.isSpecial1Available); //1sensor
                                                                   //sensor.AddObservation(baseScript.isSpecial2Available);  //1 MAGE sensor

            sensor.AddObservation(distanceVector(ally1, character)); //2 sensors
            sensor.AddObservation(distanceVector(ally1, character).magnitude);// 1 sensor
            sensor.AddObservation(distanceVector(ally2, character)); //2 sensors
            sensor.AddObservation(distanceVector(ally2, character).magnitude);// 1 sensor


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
        

    }



    //Reward
    //Note: MaxStep can be used ot avoid a run to last to long by putting a limit of time for each run (avoid static behaviour from agent)
    protected override void reward()
    {
        if (distanceVector(this.gameObject, target).magnitude <= rewardDistTarget)
        {
            Debug.Log("TargetReward");
            SetReward(1f);
           // EndEpisode();
        }
        else
        {
            SetReward(-1f);
        }

        if (distanceVector(this.gameObject, target).magnitude < lastDistance)
        {
            //Debug.Log("DistanceReward");
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
            if(distanceVector(this.gameObject, target).magnitude <= rewardDistTarget)
            {
                if (baseScript.isSpecial1Available)
                {
                    Debug.Log("Tank HitReward");
                    AddReward(0.4f);
                    lastHP = baseScript.getHP();
                }
                else
                {
                    Debug.Log("tank hit Shield Reward");
                    AddReward(0.5f);
                    lastHP = baseScript.getHP();
                }
            }
            else
            {
                if (baseScript.isSpecial1Available)
                {
                    Debug.Log("Tank HitReward while far from flag");
                    AddReward(-0.1f);
                    lastHP = baseScript.getHP();
                }
                else
                {
                    Debug.Log("tank hit Shield Reward while far from flag");
                    AddReward(-0.1f);
                    lastHP = baseScript.getHP();
                }
            }
            

        }

        
        if (step >= maxStep)
        {
            //SetReward(-1f);
            Debug.Log("Max step reward");
            EndEpisode();
        }
        



        if (baseScript.isHittingWall)
        {
            AddReward(-0.05f);
            Debug.Log("Wall");
        }





        //Atk reward

        if(Atk == 1)
        {
            if (baseScript.isBusy) //If attack when busy
            {
                Debug.Log("busy reward");
                AddReward(-0.1f);
            }
            else
            {
                if (enemy1 == null)
                { }
                else
                {
                    if (distanceVector(enemy1, character).magnitude > baseScript.atkRange + addedRange || enemy1.tag == "Player") //If attack when monster not nearby
                    {
                        Debug.Log("Tank atk while too far");
                        AddReward(-0.2f);
                    }
                    else
                    {
                        Debug.Log("Tank atk reward");
                        AddReward(0.05f);
                    }
                }
            }
            
        }

        if (Spe1 == 1)
        {
            if (baseScript.isBusy) //If attack when busy
            {
                Debug.Log("busy reward");
                AddReward(-0.1f);
            }
            else
            {
                if (enemy1 == null)
                { }
                else
                {
                    if (distanceVector(enemy1, character).magnitude > 4.5f || enemy1.tag == "Player") //If attack when monster not nearby
                    {
                        Debug.Log("Tank shield while too far");
                        AddReward(-0.3f);
                    }
                    else
                    {
                        Debug.Log("Tank shield reward");
                        AddReward(-0.1f);
                    }
                }
            }

            
        }



        if(enemy1 == null)
        {

        }
        else
        {
            if (enemy1.tag == "Snake" && distanceVector(enemy1, character).magnitude <= 2f)
            {
                Debug.Log("Tank snake scared");
                AddReward(-0.55f);
            }
        }

        if (enemy2 == null)
        {

        }
        else
        {
            if (enemy2.tag == "Snake" && distanceVector(enemy2, character).magnitude <= 2f)
            {
                Debug.Log("Tank snake scared");
                AddReward(-0.55f);
            }
        }





    }


}

