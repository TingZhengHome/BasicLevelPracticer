using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class Editor_TileObject : Edtior_GameBoardObject
{
    public int tileId;

    public bool changeable = true;

    public bool isPlaceable;

    public bool notWalkable;

    public Editor_OnTileObject objectOn;
    public TileButton correspondingButton;

    public bool detected;

    public static event Action<Edtior_GameBoardObject, int> OnTileClicked;


    public void Setup(Vector2 worldPosition, Transform parent, int id)
    {
        base.Setup(worldPosition, parent);
        this.tileId = id;
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public void GameUpdate()
    {
        SenseHover();
    }

    void SenseHover()
    {
        Collider2D hit = Physics2D.OverlapPoint(this.transform.position, LevelEditor.Instance.hoverLayer);
        Edtior_GameBoardObject grabbingObject = null;
        int objectSize = 0;
        if (LevelEditor.Instance.clickedBoardObjectButton != null)
        {
            grabbingObject = LevelEditor.Instance.clickedBoardObjectButton.representObject;
            objectSize = (int)(grabbingObject.trigger.size.x *
                               grabbingObject.trigger.size.y);
        }
        if (!EventSystem.current.IsPointerOverGameObject() && hit != null)
        {
            detected = true;
            if (grabbingObject != null)
            {
                ColorControl(true, grabbingObject);
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
                    List<Editor_TileObject> placeableDetecteds = new List<Editor_TileObject>();
                    List<Editor_TileObject> detecteds = LevelEditor.Instance.EditingGameboard.DetectedTiles();
                    if (this.gameObject == detecteds[detecteds.Count - 1].gameObject)
                    {
                        for (int i = 0; i < detecteds.Count; i++)
                        {
                            if (detecteds[i].isPlaceable)
                            {
                                placeableDetecteds.Add(detecteds[i]);
                            }
                        }

                        if (Input.GetMouseButtonDown(0))
                        {
                            if (placeableDetecteds.Count == objectSize && placeableDetecteds.Count == detecteds.Count)
                            {
                                int OverlapingTileObjectCount = 0;
                                Editor_OnTileObject temp = null;
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
                                            return;
                                        }
                                    }
                                }
                                if (OverlapingTileObjectCount <= 1)
                                {
                                    OnTileClicked += PlaceGameBoardObject;
                                    TileClicked();
                                    if (OverlapingTileObjectCount == 0 && LevelEditor.Instance.movingPlacedObject)
                                    {
                                        LevelEditor.Instance.CancelButtonClick();
                                    }
                                }
                            }
                            else
                            {
                                return;
                            }
                        }

                    }
                }
            }
            else //holdingObject == null
            {
                ColorControl(true, grabbingObject);
                if (objectOn != null)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        OnTileClicked += objectOn.PickedUp;
                        TileClicked();
                    }
                }
            }
        }
        else
        {
            ColorControl(false, grabbingObject);
            detected = false;
            OnTileClicked -= PlaceGameBoardObject;
        }
    }

    public void TileClicked()
    {
        Debug.Log("Clicked tile" + tileId);
        Edtior_GameBoardObject newO = null;
        if (LevelEditor.Instance.clickedBoardObjectButton != null)
        {
            newO = Instantiate(LevelEditor.Instance.clickedBoardObjectButton.representObject);
        }

        if (OnTileClicked != null)
        {
            if (newO != null)
            {
                Debug.Log(newO.name + "instantiated.");
                newO.transform.rotation = this.transform.rotation;
                newO.name = LevelEditor.Instance.clickedBoardObjectButton.representObject.name;
                OnTileClicked(newO, tileId);
                Debug.Log("And we get Here");
            }
            else
            {
                OnTileClicked(newO, tileId);
            }
        }
    }


    void ColorControl(bool hit, Edtior_GameBoardObject grabbingObject)
    {
        if (hit == true)
        {
            if (grabbingObject != null)
            {
                if (!isPlaceable && grabbingObject.tag != "tile")
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
                        for (int i = 0; i < objectOn.GetComponent<Edtior_OnMutipleTileObject>().theTilesSetOn.Count; i++)
                        {
                            if (objectOn.GetComponent<Edtior_OnMutipleTileObject>().theTilesSetOn[i].detected == false)
                            {
                                objectOn.GetComponent<Edtior_OnMutipleTileObject>().theTilesSetOn[i].TurnColor(LevelEditor.Instance.EditingGameboard.defaultColor);
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
                        for (int i = 0; i < objectOn.GetComponent<Edtior_OnMutipleTileObject>().theTilesSetOn.Count; i++)
                        {
                            objectOn.GetComponent<Edtior_OnMutipleTileObject>().theTilesSetOn[i].TurnColor(LevelEditor.Instance.EditingGameboard.placeableColor);
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

    public void TurnColor(Color color)
    {
        if (spriteRender.color != color)
        {
            spriteRender.color = color;
        }

        if (objectOn != null && LevelEditor.Instance.clickedBoardObjectButton != null)
        {
            objectOn.spriteRender.color = color;
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
                        Editor_TileObject newTile = newObject.GetComponent<Editor_TileObject>();
                        newTile.Setup(this.transform.position, this.transform.parent, id);
                        Destroy(this.gameObject);
                        break;

                    case "onTileObject":
                        if (newObject.trigger.size.x * newObject.trigger.size.y <= 1)
                        {
                            if (objectOn != null && LevelEditor.Instance.movingPlacedObject)
                            {
                                objectOn.PickedUp(newObject, this.tileId);
                            }
                            else if (objectOn != null && !LevelEditor.Instance.movingPlacedObject)
                            {
                                if (objectOn.trigger.size.x * objectOn.trigger.size.y <= 1)
                                {
                                    if (objectOn.tag != "player")
                                    {
                                        Destroy(objectOn.gameObject);
                                    }
                                    else
                                    {
                                        objectOn.PickedUp(newObject, this.tileId);
                                    }
                                }
                                else
                                {
                                    objectOn.PickedUp(newObject, this.tileId);
                                }
                            }
                            else if (objectOn == null && LevelEditor.Instance.movingPlacedObject)
                            {
                                LevelEditor.Instance.movingPlacedObject = false;
                                LevelEditor.Instance.CancelButtonClick();
                            }
                            Editor_OnTileObject newOnTile = newObject.GetComponent<Editor_OnTileObject>();
                            newObject.Setup(transform.position, transform);
                            objectOn = newOnTile;
                            isPlaceable = true;
                            break;
                        }
                        else
                        {
                            Edtior_OnMutipleTileObject newMuOnTile = newObject.GetComponent<Edtior_OnMutipleTileObject>();
                            newMuOnTile.Setup(this.transform, LevelEditor.Instance.EditingGameboard.DetectedTiles());
                            isPlaceable = true;
                            break;
                        }

                    case "player":
                        if (objectOn != null)
                        {
                            objectOn.PickedUp(newObject, this.tileId);
                        }
                        else if (objectOn == null)
                        {
                            LevelEditor.Instance.movingPlacedObject = false;
                            LevelEditor.Instance.CancelButtonClick();
                        }
                        Editor_OnTileObject newOnTile2 = newObject.GetComponent<Editor_OnTileObject>();
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

    public void CleanTile()
    {
        if (objectOn != null)
        {
            Destroy(objectOn.gameObject);
            objectOn = null;
        }
        isPlaceable = true;
    }
}
