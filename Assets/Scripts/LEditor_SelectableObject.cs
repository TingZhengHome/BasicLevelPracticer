﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LEditor_SelectableObject : Edtior_GameBoardObject
{

    public LEditor_TileObject theTileSetOn;
    public LEditor_OnTileObject attachedOnTileObject;
    bool isOnTile;

    public bool selected;

    private void Awake()
    {

    }

    public virtual void GameUpdate()
    {

    }


    public virtual void Setup(LEditor_TileObject theTileSetOn, Edtior_GameBoardObject attachedObject)
    {
        this.theTileSetOn = theTileSetOn;
        if (attachedObject.GetComponent<LEditor_OnTileObject>() != null)
        {
            attachedOnTileObject = attachedObject.GetComponent<LEditor_OnTileObject>();
        }
    }

    public virtual void ColorControl(Collider2D hit, Edtior_GameBoardObject handlingObject)
    {
        if (LevelEditor.Instance.selectedObject == this)
        {
            TurnColor(LevelEditor.Instance.EditingGameboard.selectedColor);
        }
    }

    protected virtual void SenseHover()
    {
    }

    public void SelectObject(Edtior_GameBoardObject newObject, int id) //to Selectable
    {
        if (theTileSetOn != null)
        {
            if (theTileSetOn.tileId == id)
            {
                if (!selected)
                {
                    LevelEditor.Instance.CancelButtonClick();
                    this.selected = true;
                    TurnColor(LevelEditor.Instance.EditingGameboard.selectedColor);
                    LevelEditor.Instance.selectedObject = this;
                }
                else
                {
                    UnSelectThis();
                }
            }

            LEditor_TileObject.OnTileClicked -= SelectObject;
            LEditor_TileObject.OnTileClicked += UnSelectObject;
        }
    }

    public void UnSelectObject(Edtior_GameBoardObject newObject, int id) //to Selectable
    {
        if (theTileSetOn != null)
        {
            if (LevelEditor.Instance.currentEditingState == LevelEditor.editingState.mapBuilding &&
                LevelEditor.Instance.selectedObject == this)
            {
                Debug.Log("Unselecting tile" + theTileSetOn.tileId);
                LevelEditor.Instance.EscapeSelectingState();
                this.selected = false;
                TurnColor(LevelEditor.Instance.EditingGameboard.defaultColor);
                Debug.Log("Tile" + theTileSetOn.tileId + " is unselected.");
            }
            LEditor_TileObject.OnTileClicked -= UnSelectObject;
        }
    }

    public void UnSelectThis() //to Selectable
    {
        LevelEditor.Instance.EscapeSelectingState();
        this.selected = false;
        TurnColor(LevelEditor.Instance.EditingGameboard.defaultColor);
    }

    public void PickUpThis()
    {
        if (GetComponent<LEditor_OnTileObject>() != null && this.selected)
        {
            GetComponent<LEditor_OnTileObject>().PickUp(theTileSetOn, theTileSetOn.tileId);
        }
    }
}
