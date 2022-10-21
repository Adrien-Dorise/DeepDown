using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trainingManager : MonoBehaviour
{
    [SerializeField] private GameObject flag, wine;
    [SerializeField] private int step, MaxStep = 850, zoneNumber;
    [SerializeField] private float[] rangeXMin, rangeXMax, rangeYMin, rangeYMax;
    [SerializeField] GameObject monsters;
    private GameObject parentMonsters;
    [SerializeField] GameObject prefab; //Put the monster prefab here
    private int currentZone;


    // Start is called before the first frame update
    void Start()
    {
        step = MaxStep -5;
        currentZone = 0;
        parentMonsters = monsters.transform.parent.gameObject;

    }

    private void moveFlag()
    {
        currentZone = Random.Range(0, zoneNumber); //int random is max exclusive
        flag.transform.localPosition = new Vector3(Random.Range(rangeXMin[currentZone], rangeXMax[currentZone]), Random.Range(rangeYMin[currentZone], rangeYMax[currentZone]), 0);
        wine.transform.localPosition = new Vector3(Random.Range(rangeXMin[currentZone], rangeXMax[currentZone]), Random.Range(rangeYMin[currentZone], rangeYMax[currentZone]), 0);
        wine.SetActive(true);
        Destroy(monsters);
        monsters = Instantiate(prefab, parentMonsters.transform);

    }

    private void Update()
    {
        if (step > MaxStep)
        {
            moveFlag();
            step = 0;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        step += 1;



    }
}
