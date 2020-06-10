using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LEditor_PortableObject : LEditor_SelectableObject
{

    public bool isExit;
    public LEditor_PortableObject connectedPortable;

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
        if (theTileSetOn.GetComponent <LEditor_PortableObject>() != null)
        {
            isExit = theTileSetOn.isExit;
        }
        else if (attachedOnTileObject.GetComponent<LEditor_PortableObject>() != null)
        {
            isExit = attachedOnTileObject.isExit;
        }
    }

    protected override void SenseHover()
    {
  
        Collider2D hit = Physics2D.OverlapPoint(transform.position, LevelEditor.Instance.hoverLayer);
        this.ColorControl(hit, LevelEditor.Instance.selectedObject);
        GetComponent<LEditor_SelectableObject>().ColorControl(hit, LevelEditor.Instance.selectedObject);

        if (!EventSystem.current.IsPointerOverGameObject() && hit != null)
        {
            LEditor_PortableObject selected = LevelEditor.Instance.selectedObject.GetComponent<LEditor_PortableObject>();


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
                LEditor_PortableObject selected = null;
                if (selectedObject != null)
                {
                    selected = selectedObject.GetComponent<LEditor_PortableObject>();
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

    private void Disconnection(LEditor_PortableObject connected)
    {
        connected.connectedPortable = null;
        connectedPortable = null;
        Debug.Log("Portal" + theTileSetOn.TileId + " disconnected with Portal" + connected.theTileSetOn.TileId);

    }

    //public override void ColorControl(Collider2D hit, LEdtior_GameBoardObject selectedObject)
    //{
    //    LEditor_PortableObject selected = selectedObject.GetComponent<LEditor_PortableObject>();
    //    GameBoard EditingGameBoard = LevelEditor.Instance.EditingGameboard;

    //        if (!isExit || !selected.isExit)
    //        {
    //            if (connectedPortable != selected)
    //            {
    //                TurnColor(EditingGameBoard.connectableColor);
    //            }
    //            else
    //            {
    //                TurnColor(EditingGameBoard.connectedColor);
    //            }
    //        }

    //        base.ColorControl(hit, selectedObject);
    //}
}

