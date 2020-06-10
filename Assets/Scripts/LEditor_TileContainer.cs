using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LEditor_TileContainer : LEdtior_GameBoardObject
{
    [SerializeField]
    private int slotId;

    public int SlotId
    {
        get
        {
            return slotId;
        }
    }
    [SerializeField]
    LEditor_TileObject containingTile;
    public static event Action<LEdtior_GameBoardObject, int> OnTileClicked;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame

    public void GameUpdate()
    {
        SenseHover();

        containingTile.GameUpdate();
    }

    public void Setup(Vector2 placedPositon, Transform parent, int id)
    {
        base.Setup(placedPositon, parent);
        slotId = id;
    }

    void SenseHover()
    {
        Collider2D hit = Physics2D.OverlapPoint(this.transform.position, LevelEditor.Instance.hoverLayer);
        LEdtior_GameBoardObject grabbingObject = null;
        int objectSize = 0;
        if (LevelEditor.Instance.clickedBoardObjectButton != null)
        {
            grabbingObject = LevelEditor.Instance.clickedBoardObjectButton.representObject;
        }
        else if (LevelEditor.Instance.movingObject != null)
        {
            grabbingObject = LevelEditor.Instance.movingObject;
        }

        containingTile.ColorControl(hit, grabbingObject);

        if (LevelEditor.Instance.currentEditingState == LevelEditor.editingState.mapBuilding)
        {
            if (containingTile != null)
            {
                if (!EventSystem.current.IsPointerOverGameObject() && hit != null)
                {
                    containingTile.detected = true;
                    if (grabbingObject != null)
                    {
                        objectSize = (int)(grabbingObject.trigger.size.x *
                                           grabbingObject.trigger.size.y);

                        if (Input.GetMouseButtonDown(0))
                        {
                            OnTileClicked += this.PlaceGameBoardObject;
                            TileClicked();
                        }
                    }
                    else
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            containingTile.EmptyHoverClickedEvents(grabbingObject, SlotId);
                            TileClicked();
                        }
                    }
                }
                else
                {
                    containingTile.detected = false;
                }
            }
        }
    }

    public void TileClicked()
    {
        if (LevelEditor.Instance.currentEditingState == LevelEditor.editingState.mapBuilding)
        {
            Debug.Log("Clicked tile" + SlotId);
            LEdtior_GameBoardObject handlingObject = null;
            if (LevelEditor.Instance.clickedBoardObjectButton != null)
            {
                if (!LevelEditor.Instance.isMovingPlacedObject)
                {
                    handlingObject = Instantiate(LevelEditor.Instance.clickedBoardObjectButton.representObject);
                    handlingObject.name = LevelEditor.Instance.clickedBoardObjectButton.representObject.name;
                }
                else
                {
                    handlingObject = LevelEditor.Instance.movingObject;
                    handlingObject.name = LevelEditor.Instance.movingObject.name;
                }
            }
            else if (LevelEditor.Instance.isMovingPlacedObject)
            {
                handlingObject = LevelEditor.Instance.movingObject;
                handlingObject.name = LevelEditor.Instance.movingObject.name;
            }

            if (OnTileClicked != null)
            {
                OnTileClicked(handlingObject, SlotId);
            }
        }

        else if (LevelEditor.Instance.currentEditingState == LevelEditor.editingState.settingConnection ||
                 LevelEditor.Instance.currentEditingState == LevelEditor.editingState.settingPortals)
        {
            LEditor_SelectableObject selected = null;
            if (LevelEditor.Instance.selectedObject != null)
            {
                selected = LevelEditor.Instance.selectedObject.GetComponent<LEditor_SelectableObject>();
            }

            if (OnTileClicked != null)
            {
                OnTileClicked(selected, SlotId);
            }
        }
    }

    public void PlaceGameBoardObject(LEdtior_GameBoardObject newObject, int id)
    {
        if (this.SlotId == id)
        {
            if (newObject != null)
            {
                switch (newObject.tag)
                {
                    case "tile":
                        if (containingTile != null)
                        {
                            containingTile.CleanTile(false);
                            Destroy(containingTile.gameObject);
                        }
                        LEditor_TileObject newTile = newObject.GetComponent<LEditor_TileObject>();
                        newTile.Setup(this.transform.position, this.transform, id);
                        containingTile = newTile;
                        break;

                    case "onTileObject":
                        if (newObject.trigger.size.x *
                            newObject.trigger.size.y <= 1)
                        {
                            containingTile.PlaceOnTileObject(newObject, SlotId);
                        }
                        else
                        {
                            containingTile.PlaceOnMutipleTileObject(newObject, SlotId);
                        }
                        break;

                    case "player":
                        containingTile.PlaceOnTileObject(newObject, SlotId);
                        break;
                }
                Debug.Log("PlaceGameBoardObject Called" + SlotId);
            }
        }
        OnTileClicked -= this.PlaceGameBoardObject;
    }

}
