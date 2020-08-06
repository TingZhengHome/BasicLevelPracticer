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
        trigger = GetComponent<BoxCollider2D>();
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

        TriggerControl();
    }

    public void Initialize(LEditor_TileObject tile)
    {
        TileId = tile.TileId;
        idText.text = TileId.ToString();

        spriterenderer = GetComponent<SpriteRenderer>();
        trigger = GetComponent<BoxCollider2D>();

        isHinderance = tile.isHinderance;
        if (isHinderance)
        {
            trigger.isTrigger = false;
        }

        if (tile.InteractableO != null)
        {
            CheckAndSetUpInteractable(tile.InteractableO);
        }

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
                objectOnThis.Property.Initialize(objectOnThis, objectOnThis.GetComponent<LEditor_OnTileObject>().interactable);
            }
        }

        TriggerControl();
    }

    public void CheckAndSetUpInteractable(InteractableObject interactableO)
    {
        switch (interactableO.theType)
        {
            case global::ObjectType.movable:
                Movable movable = gameObject.AddComponent<Movable>();
                Property = movable;
                break;

            case ObjectType.pickable:
                Pickable pickable = gameObject.AddComponent<Pickable>();
                Property = pickable;
                break;

            case ObjectType.connectable:
                Connectable connectable = gameObject.AddComponent<Connectable>();
                Property = connectable;
                break;

            case ObjectType.portable:
                Portable portable = gameObject.AddComponent<Portable>();
                Property = portable;
                break;
        }
    }

    public void TriggerControl()
    {
        if (!isHinderance)
        {
            if (objectOnThis != null)
            {
                trigger.enabled = false;
            }
            else
            {
                trigger.enabled = true;
            }
        }
        else
        {
            trigger.enabled = true;
        }
    }

    public void SetObjectOn(OnTileObject onTile)
    {
        if (objectOnThis != null)
        {
            Destroy(objectOnThis.gameObject);
        }
        objectOnThis = onTile;
        trigger.enabled = false;
    }

    public override void Interact(Player interacter)
    {
        if (!isHinderance && objectOnThis == null)
        {
            OnTileObject onTilePlayer = interacter.GetComponent<OnTileObject>();
            float distanceToThis = (transform.position - interacter.transform.position).magnitude;
            float distanceToTheExiting = (interacter.transform.position - onTilePlayer.theTileSetOn.transform.position).magnitude;
            if (distanceToThis < distanceToTheExiting)
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
}
