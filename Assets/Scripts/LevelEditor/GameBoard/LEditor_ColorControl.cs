using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class LEditor_ColorControl
{
    public static void ColorControl(this LEditor_TileObject tile, Collider2D hit, LEdtior_GameBoardObject handlingObject)
    {
        if (LevelEditor.Instance.currentEditingState == LevelEditor.editingState.mapBuilding && tile != null)
        {
            if (!EventSystem.current.IsPointerOverGameObject() && hit != null)
            {
                if (handlingObject != null)
                {
                    if (!tile.isPlaceable && handlingObject.tag != "tile")
                    {
                        tile.TurnColor(LevelEditor.Instance.EditingGameboard.notPlaceableColor);
                        return;
                    }
                    else if (tile.objectOn != null)
                    {
                        tile.objectOn.detected = true;
                        tile.TurnColor(LevelEditor.Instance.EditingGameboard.objectOverlappedColor);
                        if (tile.objectOn.trigger.size.x *
                            tile.objectOn.trigger.size.y
                            > 1)
                        {
                            for (int i = 0; i < tile.objectOn.GetComponent<LEdtior_OnMutipleTileObject>().theTilesSetOn.Count; i++)
                            {
                                if (tile.objectOn.GetComponent<LEdtior_OnMutipleTileObject>().theTilesSetOn[i].detected == false)
                                {
                                    tile.objectOn.GetComponent<LEdtior_OnMutipleTileObject>().theTilesSetOn[i].TurnColor(LevelEditor.Instance.EditingGameboard.defaultColor);
                                }
                            }
                        }
                    }
                    else
                    {
                        tile.TurnColor(LevelEditor.Instance.EditingGameboard.placeableColor);
                    }
                }
                else
                {
                    if (tile.GetComponent<LEditor_SelectableObject>() != null &&
                        LevelEditor.Instance.selectedObject == tile.GetComponent<LEditor_SelectableObject>())
                    {
                        tile.TurnColor(LevelEditor.Instance.EditingGameboard.selectedColor);
                    }
                    else
                    {
                        tile.TurnColor(LevelEditor.Instance.EditingGameboard.placeableColor);
                    }

                    if (tile.objectOn != null)
                    {
                        tile.objectOn.detected = true;
                        if (tile.objectOn.trigger.size.x *
                            tile.objectOn.trigger.size.y
                            <= 1)
                        {
                            if (tile.objectOn.GetComponent<LEditor_SelectableObject>() != null &&
                            LevelEditor.Instance.selectedObject == tile.objectOn.GetComponent<LEditor_SelectableObject>())
                            {
                                tile.TurnColor(LevelEditor.Instance.EditingGameboard.selectedColor);

                            }
                            else
                            {
                                tile.TurnColor(LevelEditor.Instance.EditingGameboard.placeableColor);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < tile.objectOn.GetComponent<LEdtior_OnMutipleTileObject>().theTilesSetOn.Count; i++)
                            {
                                if (tile.objectOn.GetComponent<LEditor_SelectableObject>() != null &&
                                    LevelEditor.Instance.selectedObject == tile.objectOn.GetComponent<LEditor_SelectableObject>())
                                {
                                    tile.objectOn.GetComponent<LEdtior_OnMutipleTileObject>().theTilesSetOn[i].TurnColor(LevelEditor.Instance.EditingGameboard.selectedColor);
                                }
                                else
                                {
                                    tile.objectOn.GetComponent<LEdtior_OnMutipleTileObject>().theTilesSetOn[i].TurnColor(LevelEditor.Instance.EditingGameboard.placeableColor);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (tile.objectOn != null)
                {
                    if (tile.objectOn.detected == false)
                    {
                        if (tile.objectOn.GetComponent<LEditor_SelectableObject>() != null &&
                            LevelEditor.Instance.selectedObject == tile.objectOn.GetComponent<LEditor_SelectableObject>())
                        {
                            tile.TurnColor(LevelEditor.Instance.EditingGameboard.selectedColor);
                        }
                        else
                        {
                            tile.TurnColor(LevelEditor.Instance.EditingGameboard.defaultColor);

                        }
                    }
                }
                else
                {
                    if (tile.GetComponent<LEditor_SelectableObject>() != null &&
                        LevelEditor.Instance.selectedObject == tile.GetComponent<LEditor_SelectableObject>())
                    {
                        tile.TurnColor(LevelEditor.Instance.EditingGameboard.selectedColor);
                    }
                    else
                    {
                        tile.TurnColor(LevelEditor.Instance.EditingGameboard.defaultColor);
                    }
                }
            }
        }

        ColorControlOnSettingConnection(tile, hit, handlingObject);

        ColorControlOnSettingPortable(tile, hit, handlingObject);
    }

    public static void ColorControlOnSettingConnection(LEditor_TileObject tile, Collider2D hit, LEdtior_GameBoardObject handlingObject)
    {
        if (LevelEditor.Instance.currentEditingState == LevelEditor.editingState.settingConnection)
        {
            if (tile.objectOn == null)
            {
                if (tile.theType != ObjectType.connectable)
                {
                    tile.TurnColor(LevelEditor.Instance.EditingGameboard.unclickableColor);
                }
            }
            else
            {
                if (tile.objectOn.theType != ObjectType.connectable)
                {
                    tile.TurnColor(LevelEditor.Instance.EditingGameboard.unclickableColor);
                }
            }
        }
    }

    public static void ColorControlOnSettingPortable(LEditor_TileObject tile, Collider2D hit, LEdtior_GameBoardObject handlingObject)
    {
        if (LevelEditor.Instance.currentEditingState == LevelEditor.editingState.settingPortals)
        {
            if (tile.objectOn == null)
            {
                if (tile.theType != ObjectType.portal)
                {
                    tile.TurnColor(LevelEditor.Instance.EditingGameboard.unclickableColor);
                }
            }
            else
            {
                if (tile.objectOn.theType != ObjectType.portal)
                {
                    tile.TurnColor(LevelEditor.Instance.EditingGameboard.unclickableColor);
                }
            }
        }
    }

    public static void ColorControl(this LEditor_TileObject tile, LevelEditor.editingState currentstate)
    {
        if (currentstate == LevelEditor.editingState.settingWinningPickables)
        {
            if ((tile.objectOn == null) || (tile.objectOn != null && tile.objectOn.theType != ObjectType.pickable))
            {
                tile.TurnColor(LevelEditor.Instance.EditingGameboard.unclickableColor);
            }

            if (tile.objectOn != null && tile.objectOn.theType == ObjectType.pickable)
            {
                if (tile.detected == true)
                {
                    tile.TurnColor(LevelEditor.Instance.EditingGameboard.selectedColor);
                }
                else
                {
                    tile.TurnColor(LevelEditor.Instance.EditingGameboard.defaultColor);
                }
            }
        }

        if (currentstate == LevelEditor.editingState.settingWinningTile)
        {
            if (tile.detected == true)
            {
                tile.TurnColor(LevelEditor.Instance.EditingGameboard.selectedColor);
            }
            else if (tile.detected == false)
            {
                if (tile.isHinderance)
                {
                    tile.TurnColor(LevelEditor.Instance.EditingGameboard.unclickableColor);
                }
                else if ((tile.objectOn != null && tile.objectOn.isHinderance && (tile.objectOn.theType != ObjectType.movable && tile.objectOn.theType != ObjectType.connectable)) ||
                          tile.objectOn != null && !tile.objectOn.isHinderance && tile.objectOn.tag == "player")
                {
                    tile.TurnColor(LevelEditor.Instance.EditingGameboard.unclickableColor);
                }
                else
                {
                    tile.TurnColor(LevelEditor.Instance.EditingGameboard.defaultColor);
                }
            }
        }
    }

    public static void ColorControl(this LEditor_OnTileObject onTile)
    {
        if (LevelEditor.Instance.currentEditingState == LevelEditor.editingState.mapBuilding)
        {
            if (onTile.theTileSetOn != null && onTile.theTileSetOn.detected == false)
            {
                if (onTile.GetComponent<LEditor_SelectableObject>() != null)
                {
                    if (LevelEditor.Instance.selectedObject != onTile.GetComponent<LEditor_SelectableObject>())
                    {
                        onTile.spriteRender.color = LevelEditor.Instance.EditingGameboard.defaultColor;
                    }
                    else
                    {
                        onTile.spriteRender.color = LevelEditor.Instance.EditingGameboard.selectedColor;
                    }
                }
                else
                {
                    onTile.spriteRender.color = LevelEditor.Instance.EditingGameboard.defaultColor;
                }
                onTile.detected = false;
            }
        }
    }

    public static void ColorControl(this LEditor_SelectableObject selectable, Collider2D hit, LEdtior_GameBoardObject selectedObject)
    {
        if (LevelEditor.Instance.selectedObject == selectable)
        {
            selectable.TurnColor(LevelEditor.Instance.EditingGameboard.selectedColor);
        }
    }

    public static void ColorControl(this LEditor_ConnectableObject connectable, Collider2D hit, LEdtior_GameBoardObject selectedObject)
    {
        LEditor_ConnectableObject selected = selectedObject.GetComponent<LEditor_ConnectableObject>();
        GameBoard EditingGameBoard = LevelEditor.Instance.EditingGameboard;

        if (selected.isButton)
        {
            if (!connectable.isButton)
            {
                if (!connectable.connecteds.Exists(x => x.GetComponent<LEditor_ConnectableObject>() == selected))
                {
                    connectable.TurnColor(EditingGameBoard.connectableColor);
                }
                else
                {
                    connectable.TurnColor(EditingGameBoard.connectedColor);
                }
            }
        }
        else
        {
            if (connectable.isButton)
            {
                if (connectable.connectedObject != selected)
                {
                    connectable.TurnColor(EditingGameBoard.connectableColor);
                }
                else
                {
                    connectable.TurnColor(EditingGameBoard.connectedColor);
                }
            }
        }

        connectable.GetComponent<LEditor_SelectableObject>().ColorControl(hit, LevelEditor.Instance.selectedObject);
    }

    public static void ColorControl(this LEditor_PortalObject portable, Collider2D hit, LEdtior_GameBoardObject selectedObject)
    {
        LEditor_PortalObject selected = selectedObject.GetComponent<LEditor_PortalObject>();
        GameBoard EditingGameBoard = LevelEditor.Instance.EditingGameboard;

        if (!portable.isExit || !selected.isExit)
        {
            if (portable.connectedPortable != selected)
            {
                portable.TurnColor(EditingGameBoard.connectableColor);
            }
            else
            {
                portable.TurnColor(EditingGameBoard.connectedColor);
            }
        }

        portable.GetComponent<LEditor_SelectableObject>().ColorControl(hit, LevelEditor.Instance.selectedObject);
    }



}
