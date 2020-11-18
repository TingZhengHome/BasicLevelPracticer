using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fragile : InteractableProperty{

    bool isSteppedOn;

    [SerializeField]
    Sprite fragileSprite, hollowSprite;

    [SerializeField]
    OnTileObject fillingObject;

	// Use this for initialization
	void Start ()
    {
        collide = GetComponent<BoxCollider2D>();
        spriterenderer = GetComponent<SpriteRenderer>();
    }

    public override void Initialize(GameBoardObject gameBoardObject)
    {
        base.Initialize(gameBoardObject);
    }

    public override void GameUpdate()
    {
        SenseStep();
    }

    public void SenseStep()
    {
        if (GetComponent<TileObject>().objectOnThis != null)
        {
            if (GetComponent<TileObject>().objectOnThis.tag == "player")
            {
                isSteppedOn = true;
            }


            if (GetComponent<TileObject>().objectOnThis.name == fillingObject.name)
            {
                Destroy(GetComponent<TileObject>().objectOnThis.gameObject, 1f);
                Invoke("BeFilled", 1.2f);
            }
        }

        if (isSteppedOn == true)
        {
            if (GetComponent<TileObject>().objectOnThis == null)
            {
                Collapse();
            }
        }

    }

    public void Collapse()
    {
        Hollow collapsed = gameObject.AddComponent<Hollow>();
        spriterenderer.sprite = hollowSprite;
        collapsed.Initialize(GetComponent<TileObject>());
        TurnHindrance();
        Destroy(this.GetComponent<Fragile>());
    }

    public void BeFilled()
    {
        Hollow collapsed = gameObject.AddComponent<Hollow>();
        collapsed.Initialize(GetComponent<TileObject>());
        collapsed.BeFilled();
        

        Destroy(this.GetComponent<Fragile>());
    }

}
