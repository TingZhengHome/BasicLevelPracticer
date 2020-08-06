using System;
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

    public InteractableObject InteractableO
    {
        get
        {
            return interactable;
        }
    }

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
        if (interactable != null)
        {
            interactable.IdInFactory = LevelEditor.Instance.EditingGameboard.factory.GetInteractableFactoryId(interactable);
        }
    }

    public void CheckSelectable()
    {
        switch (theType)
        {
            case ObjectType.connectable:
                if (GetComponent<LEditor_ConnectableObject>() == null && InteractableO != null)
                {
                    LEditor_ConnectableObject connectable = gameObject.AddComponent<LEditor_ConnectableObject>();
                    connectable.Setup(this, this, InteractableO);
                    selectableComponent = connectable;
                }
                break;

            case ObjectType.portable:
                if (GetComponent<LEditor_PortableObject>() == null && InteractableO != null)
                {
                    LEditor_PortableObject portable = gameObject.AddComponent<LEditor_PortableObject>();
                    portable.Setup(this, this, InteractableO);
                    selectableComponent = portable;
                }
                break;
        }
    }

    public void GameUpdate()
    {
        SenseHover();

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

        //string interactablePath = string.Format("Asset/Prefabs/ScriptableObjects/{0}", data.interactablePath);
        if (data.interactablePath != string.Empty)
            interactable = (InteractableObject)AssetDatabase.LoadAssetAtPath(data.interactablePath, typeof(InteractableObject));

        //if (selectableComponent != null)
        //selectableComponent.Load(data.selectableData);

    }

}
