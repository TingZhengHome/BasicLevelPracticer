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
    public LEditor_TileObject containingTile;

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
        SenseHoverOnBuildingTheBoard();
        SenseHoverOnSettingLevelGoal();
    }

    void SenseHoverOnBuildingTheBoard()
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

                        if (Input.GetMouseButtonDown(0) || LevelEditor.Instance.MouseHoldThanDelay())
                        {
                            OnTileClicked += this.PlaceGameBoardObject;
                            TileClicked();
                        }
                        if (Input.GetMouseButtonDown(1))
                        {
                            QuickPlaceBasicTile();
                        }
                    }
                    else
                    {
                        if (Input.GetMouseButtonDown(0) || LevelEditor.Instance.MouseHoldThanDelay())
                        {
                            containingTile.EmptyHoverClickedEvents(grabbingObject, SlotId);
                            TileClicked();
                        }
                        if (Input.GetMouseButtonDown(1))
                        {
                            QuickPlaceBasicTile();
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


    void SenseHoverOnSettingLevelGoal()
    {
        Collider2D hit = Physics2D.OverlapPoint(this.transform.position, LevelEditor.Instance.hoverLayer);

        if (LevelEditor.Instance.currentEditingState == LevelEditor.editingState.settingWinningPickables)
        {
            containingTile.ColorControl(LevelEditor.editingState.settingWinningPickables);
            if (containingTile != null && containingTile.objectOn != null)
            {
                if (containingTile.objectOn.theType == ObjectType.pickable)
                {
                    OnTileObject thePickableOn = containingTile.objectOn.GetComponent<OnTileObject>();
                    if (!EventSystem.current.IsPointerOverGameObject() && hit != null)
                    {
                        if (Input.GetMouseButtonDown(0))
                        {

                            if (!LevelEditor.Instance.EditingGameboard.levelSetting.neededPickables.Exists(x => x == thePickableOn))
                            {
                                LevelEditor.Instance.EditingGameboard.levelSetting.neededPickables.Add(thePickableOn);
                                containingTile.detected = true; //use "detected" to discriminate whether the object is added
                                Debug.Log("New needed pickable added: " + thePickableOn.name + SlotId);
                            }
                            else
                            {
                                LevelEditor.Instance.EditingGameboard.levelSetting.neededPickables.Remove(thePickableOn);
                                containingTile.detected = false;
                                Debug.Log("Needed pickable removed: " + thePickableOn.name + SlotId);
                            }
                        }
                    }
                }
            }
        }

        if (LevelEditor.Instance.currentEditingState == LevelEditor.editingState.settingWinningTile)
        {
            containingTile.ColorControl(LevelEditor.editingState.settingWinningTile);
            if (containingTile != null && !containingTile.isHinderance)
            {
                if (containingTile.objectOn == null || (containingTile.objectOn != null && !containingTile.isHinderance && containingTile.objectOn.tag != "player") ||
                    (containingTile.objectOn != null && containingTile.objectOn.theType == ObjectType.movable && containingTile.isHinderance))
                {
                    if (!EventSystem.current.IsPointerOverGameObject() && hit != null)
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            if (LevelEditor.Instance.EditingGameboard.levelSetting.winningTile == null)
                            {
                                LevelEditor.Instance.EditingGameboard.levelSetting.winningTile = containingTile.GetComponent<TileObject>();
                                LevelEditor.Instance.EditingGameboard.levelSetting.winningTile = containingTile.GetComponent<TileObject>();
                                containingTile.detected = true; //use "detected" to discriminate whether the object is added
                                Debug.Log("Set target Tile:" + LevelEditor.Instance.EditingGameboard.levelSetting.winningTile.name + LevelEditor.Instance.EditingGameboard.levelSetting.winningTile.GetComponent<LEditor_TileObject>().TileId);
                            }
                            else
                            {
                                if (LevelEditor.Instance.EditingGameboard.levelSetting.winningTile != containingTile.GetComponent<TileObject>())
                                {
                                    LevelEditor.Instance.EditingGameboard.levelSetting.winningTile.GetComponent<LEditor_TileObject>().detected = false;
                                    Debug.Log("Cancle target Tile:" + LevelEditor.Instance.EditingGameboard.levelSetting.winningTile.name + LevelEditor.Instance.EditingGameboard.levelSetting.winningTile.GetComponent<LEditor_TileObject>().TileId);
                                    LevelEditor.Instance.EditingGameboard.levelSetting.winningTile = containingTile.GetComponent<TileObject>();
                                    containingTile.detected = true;
                                    Debug.Log("Set target Tile:" + LevelEditor.Instance.EditingGameboard.levelSetting.winningTile.name + LevelEditor.Instance.EditingGameboard.levelSetting.winningTile.GetComponent<LEditor_TileObject>().TileId);
                                }
                                else if (LevelEditor.Instance.EditingGameboard.levelSetting.winningTile == containingTile.GetComponent<TileObject>())
                                {
                                    containingTile.detected = false;
                                    Debug.Log("Cancle target Tile:" + LevelEditor.Instance.EditingGameboard.levelSetting.winningTile.name + LevelEditor.Instance.EditingGameboard.levelSetting.winningTile.GetComponent<LEditor_TileObject>().TileId);
                                    LevelEditor.Instance.EditingGameboard.levelSetting.winningTile = null;
                                }
                            }

                        }

                    }

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
                    if (LevelEditor.Instance.clickedBoardObjectButton.name == "BasicTileButton")
                    {
                        handlingObject = Instantiate(LevelEditor.Instance.EditingGameboard.GetBasicTile(slotId));
                        handlingObject.name = LevelEditor.Instance.EditingGameboard.GetBasicTile(slotId).name;
                    }
                    else
                    {
                        handlingObject = Instantiate(LevelEditor.Instance.clickedBoardObjectButton.representObject);
                        handlingObject.name = LevelEditor.Instance.clickedBoardObjectButton.representObject.name;
                    }
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
                            if (containingTile.theType == ObjectType.connectable)
                            {
                                LEditor_ConnectableObject connectableTile = containingTile.GetComponent<LEditor_ConnectableObject>();
                                connectableTile.DisAllConnection();
                            }
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


    public void QuickPlaceBasicTile()
    {
        LevelEditor.Instance.EndCurrentEditingEvent();
        LEditor_TileObject basicTile = Instantiate(LevelEditor.Instance.EditingGameboard.GetBasicTile(SlotId));
        OnTileClicked += PlaceGameBoardObject;
        OnTileClicked(basicTile, SlotId);
    }
}
