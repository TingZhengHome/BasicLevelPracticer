using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class CampaignData
{
    public string campaignName;

    public List<LevelData> levelDatas = new List<LevelData>();
}

[System.Serializable]
public class LevelData
{
    public GameBoardThem theme;

    public string levelName = null;

    public int row;
    public int column;

    public List<TileData> tileDatas = new List<TileData>();

    public LevelSettingData settingData = new LevelSettingData();

    public LevelData(GameBoardThem theTheme, int row, int column, string name)
    {
        theme = theTheme;
        this.row = row;
        this.column = column;
        levelName = name;
    }
}

[System.Serializable]
public class TileData
{
    public int idInFactory;

    public int idOnBoard;


    public bool isPlaceable;
    public bool isHinderance;

    public string interactablePath;


    public SelectableData selectableData;
    public ConnectableData connectableData;
    public PortalData portableData;

    public OnTileData objectOnData;

    public TileData(LEditor_TileObject tile)
    {
        this.idInFactory = tile.idInFactory;
        this.idOnBoard = tile.TileId;
        this.isPlaceable = tile.isPlaceable;
        this.isHinderance = tile.isHinderance;

        if (tile.selectableComponent != null)
        {
            if (tile.theType == ObjectType.connectable)
            {
                selectableData = tile.GetComponent<LEditor_ConnectableObject>().Save();
            }
            else if (tile.theType == ObjectType.portal)
            {
                selectableData = tile.GetComponent<LEditor_PortalObject>().Save();
            }
        }
        if (tile.objectOn != null)
        {
            this.objectOnData = tile.objectOn.Save();
        }
    }
}


[System.Serializable]
public class OnTileData
{
    public int idInFactory;

    public int idOnBoard;
    public List<int> idsOnBoard = new List<int>();

    public bool isHinderance;

    public SelectableData selectableData;

    public OnTileData(int idInFactory, int idOnBoard, List<int> idsOnBoard, bool isHinderance, LEditor_OnTileObject onTile)
    {
        this.idInFactory = idInFactory;
        this.idOnBoard = idOnBoard;
        this.idsOnBoard = idsOnBoard;
        this.isHinderance = isHinderance;

        if (onTile.selectableComponents != null)
        {
            if (onTile.theType == ObjectType.connectable)
            {
                selectableData = onTile.GetComponent<LEditor_ConnectableObject>().Save();
            }
            else if (onTile.theType == ObjectType.portal)
            {
                selectableData = onTile.GetComponent<LEditor_PortalObject>().Save();
            }
        }
    }
}


[System.Serializable]
public class SelectableData
{
    public bool isOnTile;
}

[System.Serializable]
public class ConnectableData : SelectableData
{
    //public bool isOnTile;
    public bool isButton;
    public int connectedObjectId;
    public List<int> connectedsIds;

    public ConnectableData(bool isOnTile, bool isButton, int connectedId)
    {
        this.isOnTile = isOnTile;
        this.isButton = isButton;
        this.connectedObjectId = connectedId;
        this.connectedsIds = new List<int>();
    }
}

[System.Serializable]
public class PortalData : SelectableData
{
    //public bool isOnTile;
    public bool isExit;
    public int connectedPortalId;

    public PortalData(bool isOnTile, bool isExit, int connectedId)
    {
        this.isOnTile = isOnTile;
        this.isExit = isExit;
        connectedPortalId = connectedId;
    }
}

[System.Serializable]
public class LevelSettingData
{
    public string levelName;

    public levelClearCondition winningCondition;

    public List<int> neededPickablesIds = new List<int>();
    public int TargetedTileId;
}

