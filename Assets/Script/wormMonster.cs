using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wormMonster : baseMonster
{
    [SerializeField] private GameObject position1, position2;
    [SerializeField] private bool goPosition1;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        goPosition1 = false;
    }

    private Vector2 vector2Transform(GameObject obj)
    {
        return new Vector2(obj.transform.position.x, obj.transform.position.y);
    }

    private void move()
    {
        if (!goPosition1)
        {
            this.GetComponent<SpriteRenderer>().flipX = true;
            this.transform.Translate((position2.gameObject.transform.position - this.gameObject.transform.position).normalized * Time.fixedDeltaTime * speed);
        }
        else
        {
            this.GetComponent<SpriteRenderer>().flipX = false;
            this.transform.Translate((position1.gameObject.transform.position - this.gameObject.transform.position).normalized * Time.fixedDeltaTime * speed);
        }

    }
    // Update is called once per frame
    protected override void Update()
    {
        move();
        if ((vector2Transform(position2.gameObject) - vector2Transform(this.gameObject)).magnitude <= 1f && !goPosition1)
        {
            goPosition1 = true;
        }
        if ((vector2Transform(position1.gameObject) - vector2Transform(this.gameObject)).magnitude <= 1f && goPosition1)
        {
            goPosition1 = false;
        }

        if (DEF >= 80)
        {
            DEF = 80;
        }


        if (HP <= 0)
        {
            death();
        }
    }
}
