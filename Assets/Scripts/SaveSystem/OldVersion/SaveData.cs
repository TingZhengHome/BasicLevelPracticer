using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class SaveData {

    int version;

    public List<LevelData> levelDatas = new List<LevelData>();
}

[System.Serializable]
public class LevelData
{
    public GameBoardThem theme;

    public int row;
    public int column;

    public List<TileData> tileDatas = new List<TileData>();

    //public List<OnTileData> onTileDatas;

    public LevelSettingData settingData = new LevelSettingData();

    public LevelData(GameBoardThem theTheme, int row, int column)
    {
        theme = theTheme;
        this.row = row;
        this.column = column;
        //tileDatas = new List<TileData>();
    }
}

[System.Serializable]
public class TileData
{
    public int idInFactory;

    public int idOnBoard;

    //public ObjectType type;

    public bool isPlaceable;
    public bool isHinderance;

    public string interactablePath;

    public SelectableData selectableData;

    public OnTileData objectOnData;

    public TileData(LEditor_TileObject tile)
    {
        this.idInFactory = tile.idInFactory;
        this.idOnBoard = tile.TileId;
        this.isPlaceable = tile.isPlaceable;
        this.isHinderance = tile.isHinderance;
        if (tile.interactable != null)
        {
            this.interactablePath = AssetDatabase.GetAssetPath(tile.interactable);
        }
        if (tile.selectableComponent != null)
        {
            this.selectableData = tile.selectableComponent.Save(tile.theType, tile );
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

    //public ObjectType type;

    public bool isHinderance;

    public SelectableData selectableData;

    public OnTileData(int idInFactory, bool isHinderance, SelectableData selectable)
    {
        this.idInFactory = idInFactory;
        this.isHinderance = isHinderance;
        this.selectableData = selectable;
    }
}


[System.Serializable]
public class SelectableData
{
    //public bool isOnTile;
}

[System.Serializable]
public class ConnectableData : SelectableData
{
    public bool isOnTile;
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
public class PortableData : SelectableData
{
    public bool isOnTile;
    public bool isExit;
    public int connectedPortalId;

    public PortableData(bool isOnTile, bool isExit, int connectedId)
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

