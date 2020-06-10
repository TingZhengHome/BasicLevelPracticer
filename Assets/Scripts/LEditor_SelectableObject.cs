using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LEditor_SelectableObject : LEdtior_GameBoardObject
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


    public virtual void Setup(LEditor_TileObject theTileSetOn, LEdtior_GameBoardObject attachedObject)
    {
        this.theTileSetOn = theTileSetOn;
        if (attachedObject.GetComponent<LEditor_OnTileObject>() != null)
        {
            attachedOnTileObject = attachedObject.GetComponent<LEditor_OnTileObject>();
        }
    }

    protected virtual void SenseHover()
    {

    }

    public void SelectObject(LEdtior_GameBoardObject newObject, int id) //to Selectable
    {
        if (theTileSetOn != null)
        {
            if (theTileSetOn.TileId == id)
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

            LEditor_TileContainer.OnTileClicked -= SelectObject;
            LEditor_TileContainer.OnTileClicked += UnSelectObject;
        }
    }

    public void UnSelectObject(LEdtior_GameBoardObject newObject, int id) 
    {
        if (theTileSetOn != null)
        {
            if (LevelEditor.Instance.currentEditingState == LevelEditor.editingState.mapBuilding &&
                LevelEditor.Instance.selectedObject == this)
            {
                Debug.Log("Unselecting tile" + theTileSetOn.TileId);
                LevelEditor.Instance.EscapeSelectingState();
                this.selected = false;
                TurnColor(LevelEditor.Instance.EditingGameboard.defaultColor);
                Debug.Log("Tile" + theTileSetOn.TileId + " is unselected.");
            }
            LEditor_TileContainer.OnTileClicked -= UnSelectObject;
        }
    }

    public void UnSelectThis() 
    {
        LevelEditor.Instance.EscapeSelectingState();
        this.selected = false;
        TurnColor(LevelEditor.Instance.EditingGameboard.defaultColor);
    }

    public void PickUpThis()
    {
        if (GetComponent<LEditor_OnTileObject>() != null && this.selected)
        {
            GetComponent<LEditor_OnTileObject>().PickUp(theTileSetOn, theTileSetOn.TileId);
        }
    }
}
