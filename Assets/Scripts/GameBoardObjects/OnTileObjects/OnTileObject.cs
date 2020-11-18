using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTileObject : GameBoardObject
{
    public TileObject theTileSetOn;

    public ObjectType theType;

    public bool isPlayer;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    public void GameUpdate()
    {
        if (theType == ObjectType.movable || theType == ObjectType.pickable)
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
        collide = GetComponent<BoxCollider2D>();

        collide.enabled = true;

        isHindrance = onTileOb.isHinderance;
        if (isHindrance)
        {
            collide.isTrigger = false;
        }
    }

    public override void Interact(Player interacter)
    {
        if (Property != null)
        {
            Property.Interact(interacter);
        }
    }

    public override void Interact(GameBoardObject interacter)
    {
        if (Property != null)
        {
            Property.Interact(interacter);
        }
    }

}

