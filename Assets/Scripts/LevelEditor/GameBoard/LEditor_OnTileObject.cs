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
        base.Setup(placedPosition, parent);
        //transform.position = placedPosition;
        //transform.parent = parent;
        //SetLayer(this.GetComponent<SpriteRenderer>());
        theTileSetOn = parent.GetComponent<LEditor_TileObject>();
        if (this.GetComponent<Player>() == null)
        {
            correspondingButton = GameObject.Find(string.Format(ObjectName + "Button")).GetComponent<LEditor_Button>();
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

        if (selectableComponents == null)
        {
            CheckSelectable();
        }
        else
        {
            selectableComponents.Setup(theTileSetOn, this, interactable);
        }

        //trigger = GetComponent<BoxCollider2D>();
        trigger.enabled = false;
        //spriteRender = this.GetComponent<SpriteRenderer>();
        idInFactory = LevelEditor.Instance.EditingGameboard.factory.GetOnTileFactoryId(this);
        if (interactable != null)
        {
            interactable.IdInFactory = LevelEditor.Instance.EditingGameboard.factory.GetInteractableFactoryId(interactable);
        }
        LevelEditor.Instance.EditingGameboard.UpgradeTile(theTileSetOn,theTileSetOn.TileId);
    }

    public virtual void PickUp(LEdtior_GameBoardObject newO, int id)
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
        LEditor_TileContainer.OnTileClicked -= this.PickUp;
        Hover.Instance.transform.rotation = this.transform.rotation;
    }

    void CheckSelectable()
    {
        switch (theType)
        {
            case ObjectType.connectable:
                if (GetComponent<LEditor_ConnectableObject>() == null)
                {
                    LEditor_ConnectableObject connectable = gameObject.AddComponent<LEditor_ConnectableObject>();
                    connectable.Setup(theTileSetOn, this, interactable);
                    selectableComponents = connectable;
                    //selectableCompnents.Add(connectable);
                }
                break;
            case ObjectType.portable:
                if (GetComponent<LEditor_PortableObject>() == null)
                {
                    LEditor_PortableObject portable = gameObject.AddComponent<LEditor_PortableObject>();
                    portable.Setup(theTileSetOn, this, interactable);
                    selectableComponents = portable;
                    //selectableCompnents.Add(portable);
                }
                break;
        }
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
