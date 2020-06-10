using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LEditor_ConnectableObject : LEditor_SelectableObject
{
    public bool isButton;
    public LEditor_ConnectableObject connectedObject;
    public List<LEditor_ConnectableObject> connecteds = new List<LEditor_ConnectableObject>();


    public override void GameUpdate()
    {
        base.SenseHover();
        if (LevelEditor.Instance.currentEditingState == LevelEditor.editingState.settingConnection)
        {
            if (this.GetComponent<LEditor_OnTileObject>() != null)
            {
                GetComponent<BoxCollider2D>().enabled = true;
            }
            else
            {
                GetComponent<BoxCollider2D>().enabled = false;

            }   
            SenseHover();
        }
    }

    public override void Setup(LEditor_TileObject theTileSetOn, LEdtior_GameBoardObject attachedObject)
    {
        base.Setup(theTileSetOn, attachedObject);
        if (theTileSetOn.GetComponent<LEditor_ConnectableObject>() != null)
        {
            isButton = theTileSetOn.isButton;
        }
        else if (attachedOnTileObject.GetComponent<LEditor_ConnectableObject>() != null)
        {
            isButton = attachedOnTileObject.isButton;
        }
    }

    protected override void SenseHover()
    {


        Collider2D hit = Physics2D.OverlapPoint(transform.position, LevelEditor.Instance.hoverLayer);
        this.ColorControl(hit, LevelEditor.Instance.selectedObject);

        if (!EventSystem.current.IsPointerOverGameObject() && hit != null)
        {
            LEditor_ConnectableObject selected = LevelEditor.Instance.selectedObject.GetComponent<LEditor_ConnectableObject>();

            if (selected.isButton)
            {
                if (!isButton)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        LEditor_TileContainer.OnTileClicked += SetConnection;
                        theTileSetOn.TileClicked();
                    }
                }
            }
            else
            {
                if (isButton)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        LEditor_TileContainer.OnTileClicked += SetConnection;
                        theTileSetOn.TileClicked();
                    }
                }
            }
        }
    }

    public void SetConnection(LEdtior_GameBoardObject selectedObject, int id)
    {
        if (theTileSetOn != null)
        {
            if (theTileSetOn.TileId == id)
            {
                LEditor_ConnectableObject selected = null;
                if (selectedObject != null)
                {
                    selected = selectedObject.GetComponent<LEditor_ConnectableObject>();
                }

                if (selected != null)
                {
                    if (isButton && !selected.isButton)
                    {
                        if (connectedObject == null)
                        {
                            connectedObject = selected;
                            Debug.Log("ButtonTile" + theTileSetOn.TileId + " is set connection to connectable " + connectedObject.theTileSetOn.TileId);
                            if (!selected.connecteds.Exists(x => x.GetComponent<LEditor_ConnectableObject>() == this))
                            {
                                selected.SetConnection(this, selected.theTileSetOn.TileId);
                            }
                        }
                        else if (connectedObject != selected)
                        {
                            connectedObject.Disconnection(this);
                            connectedObject = selected;
                            Debug.Log("ButtonTile" + theTileSetOn.TileId + " is set connection to connectable " + connectedObject.theTileSetOn.TileId);
                            if (!selected.connecteds.Exists(x => x.GetComponent<LEditor_ConnectableObject>() == this))
                            {
                                selected.SetConnection(this, selected.theTileSetOn.TileId);
                            }
                        }
                        else if (connectedObject == selected)
                        {
                            connectedObject.Disconnection(this);
                        }
                    }
                    else if (!isButton && selected.isButton)
                    {
                        if (!connecteds.Exists(x => x.GetComponent<LEditor_ConnectableObject>() == selected))
                        {
                            connecteds.Add(selected);
                            Debug.Log("ConnectableObject" + theTileSetOn.TileId + " is set connection to button " + selected.theTileSetOn.TileId);
                            if (selected.connectedObject != this)
                            {
                                selected.SetConnection(this, selected.theTileSetOn.TileId);
                            }
                        }
                        else
                        {
                            Disconnection(selected);
                        }
                    }
                }
            }

            LEditor_TileContainer.OnTileClicked -= SetConnection;
        }
    }

    public void Disconnection(LEditor_ConnectableObject connected)
    {
        if (isButton)
        {
            connected.connecteds.Remove(this);
            connectedObject = null;
            Debug.Log("ButtonTile" + theTileSetOn.TileId + "disconnected with ButtonTile" + connected.theTileSetOn.TileId);
        }
        else
        {
            connecteds.Remove(connected);
            connected.connectedObject = null;
            Debug.Log("ConnectableObject" + theTileSetOn.TileId + " disconnected with ButtonTile" + connected.theTileSetOn.TileId);
        }
    }

}
