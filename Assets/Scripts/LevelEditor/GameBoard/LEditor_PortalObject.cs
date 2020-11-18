using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LEditor_PortalObject : LEditor_SelectableObject
{

    public bool isExit;
    public LEditor_PortalObject connectedPortable;

    public override void GameUpdate()
    {
        base.SenseHover();
        if (LevelEditor.Instance.currentEditingState == LevelEditor.editingState.settingPortals)
        {
            if (this.GetComponent<LEditor_OnTileObject>() != null)
            {
                GetComponent<BoxCollider2D>().enabled = true;
            }
            SenseHover();
        }
        else
        {
            if (this.GetComponent<LEditor_OnTileObject>() != null)
            {
                GetComponent<BoxCollider2D>().enabled = false;
            }
        }
    }

    public override void Setup(LEditor_TileObject theTileSetOn, LEdtior_GameBoardObject attachedObject)
    {
        base.Setup(theTileSetOn, attachedObject);
        LevelEditor.Instance.EditingGameboard.UpgradeTile(theTileSetOn, theTileSetOn.TileId);
        if (spriteRender.sortingLayerName == "Tile")
        {
            spriteRender.sortingOrder = (int)transform.position.y * -10 + 2;
        }
    }

    protected override void SenseHover()
    {
  
        Collider2D hit = Physics2D.OverlapPoint(transform.position, LevelEditor.Instance.hoverLayer);
        this.ColorControl(hit, LevelEditor.Instance.selectedObject);
        GetComponent<LEditor_SelectableObject>().ColorControl(hit, LevelEditor.Instance.selectedObject);

        if (!EventSystem.current.IsPointerOverGameObject() && hit != null)
        {
            LEditor_PortalObject selected = LevelEditor.Instance.selectedObject.GetComponent<LEditor_PortalObject>();


            if (!selected.isExit || !isExit)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    LEditor_TileContainer.OnTileClicked += SetConnection;
                    theTileSetOn.TileClicked();
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
                LEditor_PortalObject selected = null;
                if (selectedObject != null)
                {
                    selected = selectedObject.GetComponent<LEditor_PortalObject>();
                }

                if (selected != null)
                {

                    if (!selected.isExit || !isExit)
                    {
                        if (connectedPortable == null)
                        {
                            connectedPortable = selected;
                            Debug.Log("Portal" + theTileSetOn.TileId + " is set connection to Portal " + connectedPortable.theTileSetOn.TileId);
                            if (selected.connectedPortable != this)
                            {
                                selected.SetConnection(this, selected.theTileSetOn.TileId);
                            }
                        }
                        else if (connectedPortable != selected)
                        {
                            connectedPortable.Disconnection(this);
                            connectedPortable = selected;
                            Debug.Log("Portal" + theTileSetOn.TileId + " is set connection to Portal " + connectedPortable.theTileSetOn.TileId);
                            if (selected.connectedPortable != this)
                            {
                                selected.SetConnection(this, selected.theTileSetOn.TileId);
                            }
                        }
                        else if (connectedPortable == selected)
                        {
                            connectedPortable.Disconnection(this);
                        }

                    }
                }
            }

            LEditor_TileContainer.OnTileClicked -= SetConnection;
        }
    }

    private void Disconnection(LEditor_PortalObject connected)
    {
        connected.connectedPortable = null;
        connectedPortable = null;
        Debug.Log("Portal" + theTileSetOn.TileId + " disconnected with Portal" + connected.theTileSetOn.TileId);
    }

    public PortalData Save()
    {
        //base.Save();
        PortalData data = null;
        if (connectedPortable != null)
        {
            data = new PortalData(isOnTile, isExit, connectedPortable.theTileSetOn.TileId);
        }
        else
        {
            data = new PortalData(isOnTile, isExit, -1);
        }

        return data;
    }


    public override void Load(SelectableData selectableData)
    {
        base.Load(selectableData);

        PortalData data = (PortalData)selectableData;

        isExit = data.isExit;
        if (data.connectedPortalId != -1)
        {
            connectedPortable = LevelEditor.Instance.EditingGameboard.GetEditingTile(data.connectedPortalId).objectOn.GetComponent<LEditor_PortalObject>();
        }
    }

}

