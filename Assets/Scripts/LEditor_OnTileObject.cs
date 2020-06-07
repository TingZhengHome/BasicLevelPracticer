using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LEditor_OnTileObject : Edtior_GameBoardObject
{
    public enum types {connectable, portable, pickalbe, pushable, normal, player}

    public LEditor_TileObject theTileSetOn;

    public types thisType;

    private int size;

    public LEditor_Button correspondingButton;

    public bool isButton;
    public bool isExit;

    public List<LEditor_SelectableObject> selectableCompnents = new List<LEditor_SelectableObject>();

    public bool detected;

    public int Size
    {
        get
        {
            if (size != null)
                return size;
            else
                return (int)(trigger.size.x * trigger.size.y);
        }

        private set
        {
            size = value;
        }
    }

    
    public virtual void GameUpdate()
    {
        if (LevelEditor.Instance.currentEditingState == LevelEditor.editingState.mapBuilding)
        {
            if (theTileSetOn != null && theTileSetOn.detected == false)
            {
                spriteRender.color = LevelEditor.Instance.EditingGameboard.defaultColor;
                detected = false;
            }
        }

        for (int i = 0; i < selectableCompnents.Count; i++)
        {
            selectableCompnents[i].GameUpdate();
        }
        
    }

    public override void Setup(Vector2 placedPosition, Transform parent)
    {
        transform.position = placedPosition;
        transform.parent = parent;
        SetLayer(this.GetComponent<SpriteRenderer>());
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

        if (selectableCompnents.Count == 0)
        {
            CheckSelectable();
        }
        else
        {
            for (int i = 0; i < selectableCompnents.Count; i++)
            {
                selectableCompnents[i].Setup(theTileSetOn, this);
            }
        }

        trigger = GetComponent<BoxCollider2D>();
        trigger.enabled = false;
        spriteRender = this.GetComponent<SpriteRenderer>();
    }

    public virtual void PickUp(Edtior_GameBoardObject newO, int id)
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
        LEditor_TileObject.OnTileClicked -= this.PickUp;
        Hover.Instance.transform.rotation = this.transform.rotation;
    }

    void CheckSelectable()
    {
        switch (thisType)
        {
            case types.connectable:
                if (GetComponent<LEditor_ConnectableObject>() == null)
                {
                    LEditor_ConnectableObject connectable = gameObject.AddComponent<LEditor_ConnectableObject>();
                    connectable.Setup(theTileSetOn, this);
                    selectableCompnents.Add(connectable);
                }
                break;
            case types.portable:
                if (GetComponent<LEditor_PortableObject>() == null)
                {
                    LEditor_PortableObject portable = gameObject.AddComponent<LEditor_PortableObject>();
                    portable.Setup(theTileSetOn, this);
                    selectableCompnents.Add(portable);
                }
                break;
        }
    }


    public void TurnColor(Color color)
    {
        spriteRender.color = color;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "hover")
        {
            TurnColor(Color.white);
        }
    }
}
