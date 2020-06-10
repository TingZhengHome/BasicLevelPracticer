using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class LEditor_TileObject : LEdtior_GameBoardObject
{
    public enum types { connectable, portable, wall, normal }

    public LEditor_TileContainer container;


    [SerializeField]
    private int tileId;

    public int TileId
    {
        get
        {
            return tileId;
        }
    }

    public types thisType;

    public bool isPlaceable;

    public List<LEditor_SelectableObject> selectableCompnents = new List<LEditor_SelectableObject>();
    public bool isButton;
    public bool isExit;

    public LEditor_OnTileObject objectOn;

    public bool isWalkable;

    public LEditor_TileButton correspondingButton;

    public bool detected;
            
    //public static event Action<LEdtior_GameBoardObject, int> OnTileClicked;

    public void Setup(Vector2 worldPosition, Transform parent, int id)
    {
        base.Setup(worldPosition, parent);
        tileId = id;
        container = parent.GetComponent<LEditor_TileContainer>();
        CheckSelectable();
    }

    public void CheckSelectable()
    {
        switch (thisType)
        {
            case types.connectable:
                if (GetComponent<LEditor_ConnectableObject>() == null)
                {
                    LEditor_ConnectableObject connectable = gameObject.AddComponent<LEditor_ConnectableObject>();
                    connectable.Setup(this, this);
                    selectableCompnents.Add(connectable);
                }
                break;
            case types.portable:
                if (GetComponent<LEditor_PortableObject>() == null)
                {
                    LEditor_PortableObject portable = gameObject.AddComponent<LEditor_PortableObject>();
                    portable.Setup(this, this);
                    selectableCompnents.Add(portable);
                }
                break;
        }
    }

    public void GameUpdate()
    {
        SenseHover();

        if (selectableCompnents.Count > 0)
        {
            for (int i = 0; i < selectableCompnents.Count; i++)
            {
                selectableCompnents[i].GameUpdate();
            }
        }

        if (objectOn != null)
        {
            objectOn.GameUpdate();
        }

        if (LevelEditor.Instance.currentEditingState == LevelEditor.editingState.mapBuilding)
        {
            trigger.enabled = false;
        }
        else
        {
            trigger.enabled = true;
        }
    }

    void SenseHover()
    {

    }

    public void TileClicked()
    {
        container.TileClicked();
    }

    public void EmptyHoverClickedEvents(LEdtior_GameBoardObject noObject, int id)
    {
        if (TileId == id)
        {
            if (objectOn != null)
            {
                if (objectOn.GetComponent<LEditor_SelectableObject>() == null)
                {
                    LEditor_TileContainer.OnTileClicked += objectOn.PickUp;
                }
                else
                {
                    if (objectOn.GetComponent<LEditor_SelectableObject>().selected == false)
                    {
                        LEditor_TileContainer.OnTileClicked += objectOn.GetComponent<LEditor_SelectableObject>().SelectObject;
                    }
                    else
                    {
                        LEditor_TileContainer.OnTileClicked += objectOn.GetComponent<LEditor_SelectableObject>().UnSelectObject;
                    }
                }
            }
            else if (GetComponent<LEditor_SelectableObject>() != null)
            {
                if (GetComponent<LEditor_SelectableObject>().selected == false)
                {
                    LEditor_TileContainer.OnTileClicked += GetComponent<LEditor_SelectableObject>().SelectObject;
                }
                else
                {
                    LEditor_TileContainer.OnTileClicked += GetComponent<LEditor_SelectableObject>().UnSelectObject;
                }
            }
        }
        LEditor_TileContainer.OnTileClicked -= EmptyHoverClickedEvents;
    }

    //void ColorControl(Collider2D hit, LEdtior_GameBoardObject handlingObject)
    //{
    //    if (LevelEditor.Instance.currentEditingState == LevelEditor.editingState.mapBuilding)
    //    {
    //        if (hit != null)
    //        {
    //            if (handlingObject != null)
    //            {
    //                if (!isPlaceable && handlingObject.tag != "tile")
    //                {
    //                    TurnColor(LevelEditor.Instance.EditingGameboard.notPlaceableColor);
    //                    return;
    //                }
    //                else if (objectOn != null)
    //                {
    //                    objectOn.detected = true;
    //                    TurnColor(LevelEditor.Instance.EditingGameboard.objectOverlappedColor);
    //                    if (objectOn.trigger.size.x *
    //                        objectOn.trigger.size.y
    //                        > 1)
    //                    {
    //                        for (int i = 0; i < objectOn.GetComponent<LEdtior_OnMutipleTileObject>().theTilesSetOn.Count; i++)
    //                        {
    //                            if (objectOn.GetComponent<LEdtior_OnMutipleTileObject>().theTilesSetOn[i].detected == false)
    //                            {
    //                                objectOn.GetComponent<LEdtior_OnMutipleTileObject>().theTilesSetOn[i].TurnColor(LevelEditor.Instance.EditingGameboard.defaultColor);
    //                            }
    //                        }
    //                    }
    //                }
    //                else
    //                {
    //                    TurnColor(LevelEditor.Instance.EditingGameboard.placeableColor);
    //                }
    //            }
    //            else
    //            {
    //                TurnColor(LevelEditor.Instance.EditingGameboard.placeableColor);
    //                if (objectOn != null)
    //                {
    //                    objectOn.detected = true;
    //                    if (objectOn.trigger.size.x *
    //                        objectOn.trigger.size.y
    //                        <= 1)
    //                    {
    //                        TurnColor(LevelEditor.Instance.EditingGameboard.placeableColor);
    //                    }
    //                    else
    //                    {
    //                        for (int i = 0; i < objectOn.GetComponent<LEdtior_OnMutipleTileObject>().theTilesSetOn.Count; i++)
    //                        {
    //                            objectOn.GetComponent<LEdtior_OnMutipleTileObject>().theTilesSetOn[i].TurnColor(LevelEditor.Instance.EditingGameboard.placeableColor);
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        else
    //        {
    //            if (objectOn != null)
    //            {
    //                if (objectOn.detected == false)
    //                    TurnColor(LevelEditor.Instance.EditingGameboard.defaultColor);
    //            }
    //            else
    //            {
    //                TurnColor(LevelEditor.Instance.EditingGameboard.defaultColor);
    //            }
    //        }
    //    }


    //    if (LevelEditor.Instance.currentEditingState == LevelEditor.editingState.settingConnection)
    //    {
    //        if (objectOn == null)
    //        {
    //            if (thisType != types.connectable)
    //            {
    //                TurnColor(LevelEditor.Instance.EditingGameboard.unconnectableColor);
    //            }
    //        }
    //        else
    //        {
    //            if (objectOn.thisType != LEditor_OnTileObject.types.connectable)
    //            {
    //                TurnColor(LevelEditor.Instance.EditingGameboard.unconnectableColor);
    //            }
    //        }
    //    }
    //    if (LevelEditor.Instance.currentEditingState == LevelEditor.editingState.settingPortals)
    //    {
    //        if (objectOn == null)
    //        {
    //            if (thisType != types.portable)
    //            {
    //                TurnColor(LevelEditor.Instance.EditingGameboard.unconnectableColor);
    //            }
    //        }
    //        else
    //        {
    //            if (objectOn.thisType != LEditor_OnTileObject.types.portable)
    //            {
    //                TurnColor(LevelEditor.Instance.EditingGameboard.unconnectableColor);
    //            }
    //        }
    //    }
    //}

    public override void TurnColor(Color color)
    {
        if (spriteRender.color != color)
        {
            spriteRender.color = color;
        }

        if (objectOn != null)
        {
            if (LevelEditor.Instance.clickedBoardObjectButton == null || LevelEditor.Instance.currentEditingState == LevelEditor.editingState.settingConnection)
            {
                objectOn.spriteRender.color = color;
            }
        }
    }

    public void PickUpObjectOnThis()
    {
        if (objectOn != null)
        {
            objectOn.PickUp(null, TileId);
        }
    }

    public void PlaceOnTileObject(LEdtior_GameBoardObject newObject, int id)
    {
        if (newObject.GetComponent<Player>() == false)
        {
            if (!isPlaceable)
            {
                if (!LevelEditor.Instance.movingObject)
                {
                    Destroy(newObject.gameObject);
                }
                return;
            }

            if (LevelEditor.Instance.movingObject != null)
            {
                LEditor_OnTileObject newOnTile = LevelEditor.Instance.movingObject.GetComponent<LEditor_OnTileObject>();
                newOnTile.Setup(transform.position, transform);

                if (objectOn != null)
                {
                    Debug.Log(objectOn.name + " should be pick.");
                    objectOn.PickUp(newObject, TileId);
                }
                else
                {
                    LevelEditor.Instance.EndMovingObject();
                }
                objectOn = newOnTile;
            }
            else
            {
                LEditor_OnTileObject newOnTile = newObject.GetComponent<LEditor_OnTileObject>();
                newOnTile.Setup(transform.position, transform);
                if (objectOn != null)
                {
                    if (objectOn.trigger.size.x * objectOn.trigger.size.y <= 1)
                    {
                        if (objectOn.tag != "player")
                        {
                            Destroy(objectOn.gameObject);
                        }
                        else
                        {
                            objectOn.PickUp(newObject, this.TileId);
                        }
                    }
                    else
                    {
                        objectOn.PickUp(newObject, this.TileId);
                    }
                }
                objectOn = newOnTile;
            }
            isPlaceable = true;
        }
        else
        {
            if (isPlaceable == false)
            {
                return;
            }
            if (objectOn != null)
            {
                objectOn.PickUp(newObject, this.tileId);
            }
            else if (objectOn == null)
            {
                LevelEditor.Instance.EndCurrentEditingEvent();
            }
            LEditor_OnTileObject newOnTile2 = newObject.GetComponent<LEditor_OnTileObject>();
            newOnTile2.Setup(transform.position, transform);
            objectOn = newOnTile2;
            LevelEditor.Instance.isPlayerPlaced = true;
            isPlaceable = true;
        }
    }

    public void PlaceOnMutipleTileObject(LEdtior_GameBoardObject newObject, int id)
    {
        if (this.TileId == id)
        {
            List<LEditor_TileObject> placeableDetecteds = new List<LEditor_TileObject>();
            List<LEditor_TileObject> detecteds = LevelEditor.Instance.EditingGameboard.GetDetectedTiles();
            if (this.gameObject == detecteds[detecteds.Count - 1].gameObject)
            {
                for (int i = 0; i < detecteds.Count; i++)
                {
                    if (detecteds[i].isPlaceable)
                    {
                        placeableDetecteds.Add(detecteds[i]);
                    }
                }
            }
            if (placeableDetecteds.Count == newObject.trigger.size.x * newObject.trigger.size.y &&
                placeableDetecteds.Count == LevelEditor.Instance.EditingGameboard.GetDetectedTiles().Count)
            {
                int OverlapingTileObjectCount = 0;
                LEditor_OnTileObject temp = null;
                for (int i = 0; i < placeableDetecteds.Count; i++)
                {
                    if (placeableDetecteds[i].objectOn != null)
                    {
                        if (temp == null)
                        {
                            temp = placeableDetecteds[i].objectOn;
                            OverlapingTileObjectCount += 1;
                        }
                        else if (placeableDetecteds[i].objectOn != temp)
                        {
                            temp = placeableDetecteds[i].objectOn;
                            OverlapingTileObjectCount += 1;
                        }
                    }
                }
                if (OverlapingTileObjectCount <= 1)
                {
                    LEdtior_OnMutipleTileObject newOnMuTile = newObject.GetComponent<LEdtior_OnMutipleTileObject>();
                    newOnMuTile.Setup(this.transform, placeableDetecteds);
                    isPlaceable = true;
                    if (OverlapingTileObjectCount == 0)
                    {
                        if (LevelEditor.Instance.isMovingPlacedObject)
                        {
                            LevelEditor.Instance.EndMovingObject();
                        }
                    }
                }
                else
                {
                    if (LevelEditor.Instance.clickedBoardObjectButton != null)
                    {
                        Destroy(newObject.gameObject);
                    }
                    return;
                }
            }
            else
            {
                if (LevelEditor.Instance.clickedBoardObjectButton != null)
                {
                    Destroy(newObject.gameObject);
                }
                else if (LevelEditor.Instance.movingObject)
                {
                    return;
                }
            }
        }
        LEditor_TileContainer.OnTileClicked -= this.PlaceOnMutipleTileObject;
    }

    public void CleanTile(bool pickingUp)
    {
        if (objectOn != null)
        {
            if (!pickingUp)
            {
                Destroy(objectOn.gameObject);
            }
            objectOn = null;
        }
        isPlaceable = true;
    }
}
