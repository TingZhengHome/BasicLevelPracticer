using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LEditor_OnTileObjectButton : LEditor_Button
{
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void PickUp(LEditor_OnTileObject pickUp)
    {
        if (pickUp != null)
        {
            pickUp.transform.position = transform.position;
            pickUp.transform.parent = transform.parent;
            sprite = pickUp.spriteRender.sprite;
        }
    }

    public void Emptize()
    {
        sprite = null;
    }
}
