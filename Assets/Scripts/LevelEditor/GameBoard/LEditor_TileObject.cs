﻿using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;

public class LEditor_TileObject : LEdtior_GameBoardObject
{
    public LEditor_TileContainer container;

    public ObjectType theType;

    [SerializeField]
    private int tileId;
    public int TileId
    {
        get
        {
            return tileId;
        }
    }

    [SerializeField]
    Text idText;

    public bool isPlaceable;
    public bool isHinderance;

    [SerializeField]
    bool isEdged;

    [SerializeField] //0 = top, 1 = right, 2 = left, 3 = bottom.
    List<GameObject> edgeObjects = new List<GameObject>();

    public LEditor_SelectableObject selectableComponent;

    public LEditor_OnTileObject objectOn;
    
    public LEditor_TileButton correspondingButton;

    public bool detected;

    public void Setup(Vector2 worldPosition, Transform parent, int id)
    {
        base.Setup(worldPosition, parent);
        tileId = id;
        idText.text = id.ToString();
        container = parent.GetComponent<LEditor_TileContainer>();
        CheckSelectable();
        idInFactory = LevelEditor.Instance.EditingGameboard.factory.GetTileFactoryId(this);

        gameObject.layer = LayerMask.NameToLayer("TileObject");

        if (isEdged)
        {
            SetEdgeLayer();
        }
    }

    public void CheckSelectable()
    {
    }

    private void Awake()
    {
        if (isEdged)
        {
            for (int i = 0; i < edgeObjects.Count; i++)
            {
                edgeObjects[i].SetActive(true);
            }
        }
    }

    public void GameUpdate()
    {

        if (selectableComponent != null)
        {
            selectableComponent.GameUpdate();
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

        EdgeDisplayControl();

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
                    LEditor_TileContainer.OnTileClicked += objectOn.BePickUp;
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

    public override void TurnColor(Color color)
    {
        if (spriteRender!= null && spriteRender.color != color)
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
            objectOn.BePickUp(null, TileId);
        }
    }

    public void PlaceOnTileObject(LEdtior_GameBoardObject newObject, int id)
    {
        Debug.Log("123321");
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
                    objectOn.BePickUp(newObject, TileId);
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
                            objectOn.BePickUp(newObject, this.TileId);
                        }
                    }
                    else
                    {
                        objectOn.BePickUp(newObject, this.TileId);
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
                objectOn.BePickUp(newObject, this.tileId);
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

    public void PlaceOnMutipleTileObjectOnLoad(LEditor_OnTileObject onTile, List<int> idsOnBoard)
    {
        LEdtior_OnMutipleTileObject onMutipleTile = onTile.GetComponent<LEdtior_OnMutipleTileObject>();
        List<LEditor_TileObject> theTilesSetOn = new List<LEditor_TileObject>();
        for (int i = 0; i < idsOnBoard.Count; i++)
        {
            theTilesSetOn.Add(LevelEditor.Instance.EditingGameboard.GetEditingTile(idsOnBoard[i]));
        }

        if (onMutipleTile != null && theTilesSetOn.Count > 1)
        {
            onMutipleTile.Setup(this.transform, theTilesSetOn);
            isPlaceable = true;
        }
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



    void EdgeDisplayControl()
    {
        if (isEdged)
        {
            GameBoard editingGameBoard = LevelEditor.Instance.EditingGameboard;


            Vector2 upperRaycastEndPoint = (Vector2)transform.position + new Vector2(0, 0.7f);
            Vector2 rightRaycastEndPoint = (Vector2)transform.position + new Vector2(0.7f, 0);
            Vector2 leftRaycastEndPoint = (Vector2)transform.position + new Vector2(-0.7f, 0);
            Vector2 belowRaycastEndPoint = (Vector2)transform.position + new Vector2(0, -0.7f);


            trigger.enabled = false;
            RaycastHit2D upperHit = Physics2D.Linecast(transform.position, upperRaycastEndPoint, LevelManager.Instance.gameBoardObjectLayer);
            RaycastHit2D rightHit = Physics2D.Linecast(transform.position, rightRaycastEndPoint, LevelManager.Instance.gameBoardObjectLayer);
            RaycastHit2D leftHit = Physics2D.Linecast(transform.position, leftRaycastEndPoint, LevelManager.Instance.gameBoardObjectLayer);
            RaycastHit2D belowHit = Physics2D.Linecast(transform.position, belowRaycastEndPoint, LevelManager.Instance.gameBoardObjectLayer);
            trigger.enabled = true;

            if (upperHit.transform != null)
            {
                if (upperHit.transform.GetComponent<LEditor_TileObject>() != null)
                {
                    if (upperHit.transform.name != this.name)
                    {
                        edgeObjects[0].SetActive(true);
                    }
                    else
                    {
                        edgeObjects[0].SetActive(false);
                    }
                }
            }
            else
            {
                edgeObjects[0].SetActive(true);
            }

            if (rightHit.transform != null)
            {
                if (rightHit.transform.GetComponent<LEditor_TileObject>() != null)
                {
                    if (rightHit.transform.name != this.name)
                    {
                        edgeObjects[1].SetActive(true);
                    }
                    else
                    {
                        edgeObjects[1].SetActive(false);
                    }
                }
            }
            else
            {
                edgeObjects[1].SetActive(true);
            }

            if (leftHit.transform != null)
            {
                if (leftHit.transform.GetComponent<LEditor_TileObject>() != null)
                {
                    if (leftHit.transform.name != this.name)
                    {
                        edgeObjects[2].SetActive(true);
                    }
                    else
                    {
                        edgeObjects[2].SetActive(false);
                    }
                }
            }
            else
            {
                edgeObjects[2].SetActive(true);
            }

            if (belowHit.transform != null)
            {
                if (belowHit.transform.GetComponent<LEditor_TileObject>() != null)
                {
                    if (belowHit.transform.GetComponent<LEditor_TileObject>().name != this.name)
                    {
                        edgeObjects[3].SetActive(true);
                    }
                    else
                    {
                        edgeObjects[3].SetActive(false);
                    }
                }
            }
            else
            {
                edgeObjects[3].SetActive(true);
            }
        }
    }

    public void SetEdgeLayer()
    {
        edgeObjects[0].GetComponent<SpriteRenderer>().sortingOrder = -(int)transform.position.y * 10 + 3;
        edgeObjects[1].GetComponent<SpriteRenderer>().sortingOrder = -(int)transform.position.y * 10 + 4;
        edgeObjects[2].GetComponent<SpriteRenderer>().sortingOrder = -(int)transform.position.y * 10 + 4;
        edgeObjects[3].GetComponent<SpriteRenderer>().sortingOrder = -(int)transform.position.y * 10 + 2;
    }

    public TileData Save()
    {
        TileData data = new TileData(this);

        return data;
    }


    public void Load(TileData data)
    {
        this.tileId = data.idOnBoard;

        this.isPlaceable = data.isPlaceable;
        this.isHinderance = data.isHinderance;

        if (selectableComponent != null)
            selectableComponent.Load(data.selectableData);
    }

}
