using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class LEditor_TileObject : Edtior_GameBoardObject
{
    public int tileId;
        
    public bool changeable = true;

    public bool isPlaceable;

    public bool isConnectable;
    public bool isButton;

    public bool isWalkable;
    public List<LEditor_SelectableObject> selectableCompnents = new List<LEditor_SelectableObject>();

    public LEditor_OnTileObject objectOn;
    public LEditor_TileButton correspondingButton;

    public bool detected;
    public bool selected; //to Selectable

    public static event Action<Edtior_GameBoardObject, int> OnTileClicked;

    public void Setup(Vector2 worldPosition, Transform parent, int id)
    {
        base.Setup(worldPosition, parent);
        this.tileId = id;
        CheckSelectable();
    }

    public void CheckSelectable()
    {
        if (isConnectable && GetComponent<LEditor_ConnectableObject>() == null)
        {
            LEditor_ConnectableObject connectable = gameObject.AddComponent<LEditor_ConnectableObject>();
            connectable.Setup(this, this);
            selectableCompnents.Add(connectable);
        }
    }

    public void GameUpdate()
    {
        if (LevelEditor.Instance.currentEditingState == LevelEditor.editingState.mapBuilding)
        {
            SenseHover();
        }

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

        if (LevelEditor.Instance.currentEditingState == LevelEditor.editingState.settingConnection)
        {
            if (objectOn == null)
            {
                if (!isConnectable)
                {
                    TurnColor(LevelEditor.Instance.EditingGameboard.unconnectableColor);
                    Debug.Log(this.name + this.tileId + " should become grey.");
                }
            }
            else
            {
                if (!objectOn.isConnectable)
                {
                    TurnColor(LevelEditor.Instance.EditingGameboard.unconnectableColor);
                    Debug.Log(objectOn.name + this.tileId + " should become grey.");
                }
            }
        }
    }

    void SenseHover()
    {
        Collider2D hit = Physics2D.OverlapPoint(this.transform.position, LevelEditor.Instance.hoverLayer);
        Edtior_GameBoardObject grabbingObject = null;
        int objectSize = 0;
        if (LevelEditor.Instance.clickedBoardObjectButton != null)
        {
            grabbingObject = LevelEditor.Instance.clickedBoardObjectButton.representObject;
        }
        else if (LevelEditor.Instance.movingObject != null)
        {
            grabbingObject = LevelEditor.Instance.movingObject;
        }

        ColorControl(hit, grabbingObject);
        if (!EventSystem.current.IsPointerOverGameObject() && hit != null)
        {
            detected = true;
            if (grabbingObject != null)
            {
                objectSize = (int)(grabbingObject.trigger.size.x *
                                   grabbingObject.trigger.size.y);
                if (objectSize <= 1)
                {
                    if (Input.GetMouseButtonDown(0) || LevelEditor.Instance.MouseHoldThanDelay())
                    {
                        OnTileClicked += this.PlaceGameBoardObject;
                        TileClicked();
                    }
                }
                else
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        OnTileClicked += this.PlaceMutipleTileObject;
                        TileClicked();
                    }
                }
            }
            else //holdingObject == null
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (objectOn != null)
                    {
                        if (objectOn.GetComponent<LEditor_SelectableObject>() == null)
                        {
                            OnTileClicked += objectOn.PickUp;
                            TileClicked();
                        }
                        else
                        { 
                            if (objectOn.GetComponent<LEditor_SelectableObject>().selected == false)
                            {
                                OnTileClicked += objectOn.GetComponent<LEditor_SelectableObject>().SelectObject;
                            }
                            else
                            {
                                OnTileClicked += objectOn.GetComponent<LEditor_SelectableObject>().UnSelectObject;
                            }
                            TileClicked();
                        }
                    }
                    else if (GetComponent<LEditor_SelectableObject>() != null)
                    {
                        if (GetComponent<LEditor_SelectableObject>().selected == false)
                        {
                            OnTileClicked += GetComponent<LEditor_SelectableObject>().SelectObject;
                        }
                        else
                        {
                            OnTileClicked += GetComponent<LEditor_SelectableObject>().UnSelectObject;
                        }
                        TileClicked();
                    }
                    else
                    {
                        TileClicked();
                    }
                }
            }
        }
        else
        {
            detected = false;
            OnTileClicked -= PlaceGameBoardObject;
        }
    }


    public void TileClicked()
    {
        if (LevelEditor.Instance.currentEditingState == LevelEditor.editingState.mapBuilding)
        {
            Debug.Log("Clicked tile" + tileId);
            Edtior_GameBoardObject newO = null;
            if (LevelEditor.Instance.clickedBoardObjectButton != null)
            {
                if (!LevelEditor.Instance.isMovingPlacedObject)
                {
                    newO = Instantiate(LevelEditor.Instance.clickedBoardObjectButton.representObject);
                    newO.name = LevelEditor.Instance.clickedBoardObjectButton.representObject.name;
                }
                else
                {
                    newO = LevelEditor.Instance.movingObject;
                    newO.name = LevelEditor.Instance.movingObject.name;
                }
            }
            else if (LevelEditor.Instance.isMovingPlacedObject)
            {
                newO = LevelEditor.Instance.movingObject;
                newO.name = LevelEditor.Instance.movingObject.name;
            }

            if (OnTileClicked != null)
            {
                OnTileClicked(newO, tileId);
            }
        }

        else if (LevelEditor.Instance.currentEditingState == LevelEditor.editingState.settingConnection)
        {
            LEditor_ConnectableObject selected = null;
            if (LevelEditor.Instance.selectedObject != null)
            {
                selected = LevelEditor.Instance.selectedObject.GetComponent<LEditor_ConnectableObject>();
            }

            if (OnTileClicked != null)
            {
                OnTileClicked(selected, tileId);
            }
        }
    }

    void ColorControl(Collider2D hit, Edtior_GameBoardObject handlingObject)
    {
        if (LevelEditor.Instance.currentEditingState == LevelEditor.editingState.mapBuilding)
        {
            if (hit != null)
            {
                if (handlingObject != null)
                {
                    if (!isPlaceable && handlingObject.tag != "tile")
                    {
                        TurnColor(LevelEditor.Instance.EditingGameboard.notPlaceableColor);
                        return;
                    }
                    else if (objectOn != null)
                    {
                        objectOn.detected = true;
                        TurnColor(LevelEditor.Instance.EditingGameboard.objectOverlappedColor);
                        if (objectOn.trigger.size.x *
                            objectOn.trigger.size.y
                            > 1)
                        {
                            for (int i = 0; i < objectOn.GetComponent<LEdtior_OnMutipleTileObject>().theTilesSetOn.Count; i++)
                            {
                                if (objectOn.GetComponent<LEdtior_OnMutipleTileObject>().theTilesSetOn[i].detected == false)
                                {
                                    objectOn.GetComponent<LEdtior_OnMutipleTileObject>().theTilesSetOn[i].TurnColor(LevelEditor.Instance.EditingGameboard.defaultColor);
                                }
                            }
                        }
                    }
                    else
                    {
                        TurnColor(LevelEditor.Instance.EditingGameboard.placeableColor);
                    }
                }
                else
                {
                    TurnColor(LevelEditor.Instance.EditingGameboard.placeableColor);
                    if (objectOn != null)
                    {
                        objectOn.detected = true;
                        if (objectOn.trigger.size.x *
                            objectOn.trigger.size.y
                            <= 1)
                        {
                            TurnColor(LevelEditor.Instance.EditingGameboard.placeableColor);
                        }
                        else
                        {
                            for (int i = 0; i < objectOn.GetComponent<LEdtior_OnMutipleTileObject>().theTilesSetOn.Count; i++)
                            {
                                objectOn.GetComponent<LEdtior_OnMutipleTileObject>().theTilesSetOn[i].TurnColor(LevelEditor.Instance.EditingGameboard.placeableColor);
                            }
                        }
                    }
                }
            }
            else
            {
                if (objectOn != null)
                {
                    if (objectOn.detected == false)
                        TurnColor(LevelEditor.Instance.EditingGameboard.defaultColor);
                }
                else
                {
                    TurnColor(LevelEditor.Instance.EditingGameboard.defaultColor);
                }
            }
        }
    }

    public override void TurnColor(Color color)
    {
        if (spriteRender.color != color)
        {
            spriteRender.color = color;
        }

        if (objectOn != null)
        {
            if (LevelEditor.Instance.clickedBoardObjectButton == null ||
                selected || LevelEditor.Instance.currentEditingState == LevelEditor.editingState.settingConnection)
            {
                objectOn.spriteRender.color = color;
            }

        }

        if (objectOn != null && LevelEditor.Instance.currentEditingState == LevelEditor.editingState.settingConnection)
        {

        }
    }

    public void PickUpObjectOnThis()
    {
        if (objectOn != null)
        {
            objectOn.PickUp(null, tileId);
        }
    }


    public void PlaceGameBoardObject(Edtior_GameBoardObject newObject, int id)
    {
        if (this.tileId == id)
        {
            if (newObject != null)
            {
                switch (newObject.tag)
                {
                    case "tile":
                        if (objectOn != null)
                        {
                            Destroy(objectOn.gameObject);
                        }
                        LEditor_TileObject newTile = newObject.GetComponent<LEditor_TileObject>();
                        newTile.Setup(this.transform.position, this.transform.parent, id);
                        Destroy(this.gameObject);
                        break;

                    case "onTileObject":
                        if (isPlaceable == false)
                        {
                            if (!LevelEditor.Instance.isMovingPlacedObject)
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
                                objectOn.PickUp(newObject, tileId);
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
                                        objectOn.PickUp(newObject, this.tileId);
                                    }
                                }
                                else
                                {
                                    objectOn.PickUp(newObject, this.tileId);
                                }
                            }
                            objectOn = newOnTile;
                        }
                        isPlaceable = true;
                        break;

                    case "player":
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
                        break;
                }
                Debug.Log("Function Called" + tileId);
            }
        }
        OnTileClicked -= this.PlaceGameBoardObject;
    }

    public void PlaceMutipleTileObject(Edtior_GameBoardObject newObject, int id)
    {
        if (this.tileId == id)
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
        OnTileClicked -= this.PlaceMutipleTileObject;
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
