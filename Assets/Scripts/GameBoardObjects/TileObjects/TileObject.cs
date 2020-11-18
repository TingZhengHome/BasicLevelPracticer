using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileObject : GameBoardObject {

    int tileId;

    public int TileId
    {
        get
        {
            return tileId;
        }

        private set
        {
            tileId = value;
        }
    }

    [SerializeField]
    Text idText;

    public ObjectType theType;



    public OnTileObject objectOnThis;

    

    // Use this for initialization
    void Start () {
        spriterenderer = GetComponent<SpriteRenderer>();
        collide = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    public void GameUpdate()
    {
        if (property != null)
        {
            property.GameUpdate();
        }
        if (objectOnThis != null)
        {
            objectOnThis.GameUpdate();
        }

        //TriggerControl();
    }

    public void Initialize(LEditor_TileObject tile)
    {
        TileId = tile.TileId;
        idText.text = TileId.ToString();

        spriterenderer = GetComponent<SpriteRenderer>();
        collide = GetComponent<BoxCollider2D>();

        isHindrance = tile.isHinderance;

        if (tile.objectOn != null)
        {
            objectOnThis = tile.objectOn.GetComponent<OnTileObject>();
            objectOnThis.GetComponent<OnTileObject>().enabled = true;
            objectOnThis.Initialize(tile.objectOn);
                if (tile.objectOn.tag == "player")
            {
                Player playerObj = tile.objectOn.GetComponent<Player>();
                playerObj.GetComponent<Player>().enabled = true;
                playerObj.Initilize();
                objectOnThis = null;
            }

            if (objectOnThis != null && objectOnThis.Property != null)
            {
                objectOnThis.Property.Initialize(objectOnThis);
            }
        }

        TriggerControl();
    }

    public void TriggerControl()
    {
        if (!isHindrance)
        {
            if (objectOnThis != null)
            {
                collide.enabled = false;
            }
            else
            {
                collide.enabled = true;
            }
        }
        else
        {
            collide.enabled = true;
            collide.isTrigger = false;
        }
    }

    public void SetObjectOn(OnTileObject onTile)
    {
        if (objectOnThis != null)
        {
            Destroy(objectOnThis.gameObject);
        }
        objectOnThis = onTile;
        collide.enabled = false;
    }

    public override void Interact(Player interacter)
    {
        if (!isHindrance && objectOnThis == null)
        {
            OnTileObject onTilePlayer = interacter.GetComponent<OnTileObject>();
            float distanceToThis = (transform.position - interacter.transform.position).magnitude;
            float distanceToExit = (interacter.transform.position - onTilePlayer.theTileSetOn.transform.position).magnitude;
            if (distanceToThis < distanceToExit)
            {                
                onTilePlayer.theTileSetOn.objectOnThis = null;
                onTilePlayer.theTileSetOn = this;
                objectOnThis = onTilePlayer;
                Debug.Log("Player is now on " + gameObject.name + tileId);
            }
        }

        if (Property != null)
        {
            Property.Interact(interacter);
        }
    }

    public override void Interact(GameBoardObject interacter)
    {
        base.Interact(interacter);

        if (Property != null)
        {
            Property.Interact(interacter);
        }
    }
}
