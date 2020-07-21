using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTileObject : GameBoardObject
{
    public TileObject theTileSetOn;

    public condition theType;

    public bool isPlayer;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    public void GameUpdate()
    {
        if (theType == condition.movable || theType == condition.pickable)
        {
            SortingLayerSystem.Instance.UpdateLayer(this.GetComponent<SpriteRenderer>());
        }

        if (property != null)
        {
            Property.GameUpdate();
        }
    }

    public void Initialize(LEditor_OnTileObject onTileOb)
    {

        theTileSetOn = onTileOb.theTileSetOn.GetComponent<TileObject>();
        GetComponent<LEditor_OnTileObject>().enabled = false;

        spriterenderer = GetComponent<SpriteRenderer>();
        trigger = GetComponent<BoxCollider2D>();

        trigger.enabled = true;

        isHinderance = onTileOb.isHinderance;
        if (isHinderance)
        {
            trigger.isTrigger = false;
        }

        if (onTileOb.interactable != null)
        {
            CheckTypeAndSetupProperty(onTileOb);
        }

    }

    public void CheckTypeAndSetupProperty(LEditor_OnTileObject onTileOb)
    {
        switch (onTileOb.theType)
        {
            case global::condition.movable:
                theType = condition.movable;
                Movable movable = gameObject.AddComponent<Movable>();
                Property = movable;
                break;

            case global::condition.pickable:
                theType = condition.pickable;
                Pickable pickable = gameObject.AddComponent<Pickable>();
                Property = pickable;
                break;

            case condition.connectable:
                theType = condition.connectable;
                Connectable connectable = gameObject.AddComponent<Connectable>();
                Property = connectable;
                break;

            case condition.portable:
                Portable portable = gameObject.AddComponent<Portable>();
                Property = portable;
                break;
        }
    }


    public override void Interact(Player interacter)
    {
        if (Property != null)
        {
            Property.Interact(interacter);
        }
    }
}

