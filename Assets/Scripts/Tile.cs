using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : GameBoardObject {

    int tileId;

    SpriteRenderer spriterenderer;
    BoxCollider2D collider2d;

    public bool isEmpty;

    bool notWalkable;

    [SerializeField]
    public bool button;
    [SerializeField]
    public bool pressed;
    [SerializeField]
    GameBoardObject connectedObject;
    OnTileObject keyObject;

    public OnTileObject objectOnThis;

	// Use this for initialization
	void Start () {
        spriterenderer = GetComponent<SpriteRenderer>();
        collider2d = GetComponent<BoxCollider2D>();
    }
	
	// Update is called once per frame
	void Update () {
        PressedCheck();
    }

    public void Initialize(LEditor_TileObject tile)
    {
        tileId = tile.TileId;
        notWalkable = tile.isWalkable;

        if (tile.objectOn != null)
        {
           // tile.ObjectOnThis.GetComponent<OnTileObject>().enabled = true;
            objectOnThis = tile.objectOn.GetComponent<OnTileObject>();
            objectOnThis.enabled = true;
            objectOnThis.Initialize(tile.objectOn);
            tile.objectOn.GetComponent<LEditor_OnTileObject>().enabled = false;
            tile.objectOn.GetComponent<BoxCollider2D>().enabled = true;
            if (tile.objectOn.tag == "player")
            {
                Player playerObj = tile.objectOn.GetComponent<Player>();
                LevelManager.Instance.currentGameBoard.player = playerObj;
                playerObj.GetComponent<Player>().enabled = true;
                playerObj.Initilize();
                objectOnThis = null;
            }
        }
    }

    public void SetObjectOn(OnTileObject onTile)
    {
        if (objectOnThis != null)
        {
            Destroy(objectOnThis.gameObject);
        }
        objectOnThis = onTile;
    }

    public virtual GameBoardObject OnCollideCheck()
    {
        return null;
    }


    public virtual void PressedCheck()
    {
        if (button)
        {
            if (objectOnThis != null)
            {
                if (!pressed)
                {
                    if (objectOnThis.name == keyObject.name)
                    {
                        if (connectedObject.GetComponent<OnTileObject>() != null)
                        {
                            connectedObject.GetComponent<OnTileObject>().TurnOnAndOff();
                        }
                        pressed = true;
                    }
                }
            }
            else
            {
                if (pressed)
                {
                    pressed = false;
                }
            }
        }
    }
}


//    public virtual GameBoardObject OnOverlapCheck2()
//    {
//        var hit = Physics2D.OverlapBox(transform.position, collider2d.size * 0.2f, 0);
//        if (button)
//        {
//            if (hit != null && hit.tag == "onTileObject")
//            {
//                if (!pressed)
//                {
//                    if (hit.name == "Rock")
//                    {
//                        if (connectedObject.GetComponent<OnTileObject>() != null)
//                        {
//                            connectedObject.GetComponent<OnTileObject>().TurnOnAndOff();
//                        }
//                        pressed = true;
//                    }
//                }
//            }
//            else
//            {
//                if (pressed)
//                {
//                    pressed = false;
//                }
//            }
//        }

//        return null;
//    }
//}
