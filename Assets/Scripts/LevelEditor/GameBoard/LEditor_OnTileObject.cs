using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LEditor_OnTileObject : LEdtior_GameBoardObject
{
    //[SerializeField]
    //int IdInFactory = 0;

    public LEditor_TileObject theTileSetOn;

    public ObjectType theType;

    public bool isHinderance;

    private int size;
    public int Size
    {
        get
        {
            if (size != 0)
                return size;
            else
                return (int)(trigger.size.x * trigger.size.y);
        }

        private set
        {
            size = value;
        }
    }

    public LEditor_SelectableObject selectableComponents;

    public LEditor_Button correspondingButton;

    public bool detected;


    public virtual void GameUpdate()
    {
        this.ColorControl();


        if (selectableComponents != null)
        {
            selectableComponents.GameUpdate();
        }

    }

    public override void Setup(Vector2 placedPosition, Transform parent)
    {
        //Debug.Log("I am going to be placed at " + placedPosition);
        base.Setup(placedPosition, parent);
        transform.position = placedPosition;
        transform.parent = parent;
        SetSortingLayer(this.GetComponent<SpriteRenderer>());
        theTileSetOn = parent.GetComponent<LEditor_TileObject>();
        gameObject.layer = LayerMask.NameToLayer("OnTileObject");

        if (this.GetComponent<Player>() == null)
        {
            correspondingButton = GameObject.Find(string.Format(ObjectName + "Button")).GetComponent<LEditor_OnTileObjectButton>();
            if (correspondingButton == null)
            {
                Debug.LogWarning(this.name + "can't find its corresponding button.");
            }
        }
        else
        {
            correspondingButton = GameObject.Find("PlayerButton").GetComponent<LEditor_Button>();
            if (correspondingButton == null)
            {
                Debug.LogWarning(this.name + "can't find its corresponding button.");
            }
        }

        if(selectableComponents != null)
        {
            selectableComponents.Setup(theTileSetOn, this);
        }

        if (isHinderance)
        {
            trigger.enabled = false;
        }
        

        idInFactory = LevelEditor.Instance.EditingGameboard.factory.GetOnTileFactoryId(this);
       
        LevelEditor.Instance.EditingGameboard.UpgradeTile(theTileSetOn,theTileSetOn.TileId);
    }

    public virtual void BePickUp(LEdtior_GameBoardObject newO, int id)
    {
        Debug.Log("pickingUp");
        if (LevelEditor.Instance.clickedBoardObjectButton != null)
        {
            LevelEditor.Instance.clickedBoardObjectButton.CancelButton();
        }
        if (GetComponent<Player>() != null)
        {
            LevelEditor.Instance.isPlayerPlaced = false;
        }
        LevelEditor.Instance.StartMovingObject(this);
        theTileSetOn.CleanTile(true);
        LEditor_TileContainer.OnTileClicked -= this.BePickUp;
        Hover.Instance.transform.rotation = this.transform.rotation;
    }

    public void TurnColor(Color color)
    {
        spriteRender.color = color;
    }

    public virtual OnTileData Save()
    {
        OnTileData data = new OnTileData(idInFactory, theTileSetOn.TileId, new List<int>(), isHinderance, this);
        return data;
    }

    public virtual void Load(OnTileData data)
    {
        this.isHinderance = data.isHinderance;
    }

}
