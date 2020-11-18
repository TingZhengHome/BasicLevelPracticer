using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{

    public Color placeableColor = Color.yellow;
    public Color notPlaceableColor = Color.red;
    public Color defaultColor = Color.white;
    public Color objectOverlappedColor = Color.cyan;
    public Color selectedColor = Color.grey;
    public Color connectableColor = Color.blue;
    public Color connectedColor = Color.magenta;
    public Color connectorColor = Color.green;
    public Color unclickableColor = Color.grey;

    public string levelName;
    public GameBoardThem theme;

    [SerializeField]
    GameObject edgeObject;

    [SerializeField]
    LEditor_TileContainer container;

    [SerializeField]
    LEditor_OnTileObject playerCharacter;

    public Player player;

    int row;
    public int Row
    {
        get
        {
            return row;
        }
    }
    int column;
    public int Column
    {
        get
        {
            return column;
        }
    }
    int size;

    public List<LEditor_TileContainer> containers = new List<LEditor_TileContainer>();
    public List<LEditor_TileObject> OnEditingTiles = new List<LEditor_TileObject>();
    public List<TileObject> tiles = new List<TileObject>();

    public LevelSetting levelSetting = new LevelSetting();

    public ObjectFactory factory;


    private void Awake()
    {
        LEditor_TileContainer.OnTileClicked += UpgradeTile;
    }

    public void SetRowAndColumn(int row, int column)
    {
        this.row = row;
        this.column = column;
    }

    public void GenerateTilesOnEditor(int row, int column, Transform parent)
    {
        this.row = row;
        this.column = column;
        size = row * column;

        LEditor_TileObject basicTile1 = factory.GetTile(0);
        LEditor_TileObject basicTile2 = factory.GetTile(1);

        LEditor_TileObject firstTile = basicTile1;
        LEditor_TileContainer firstContainer = container;
        LEditor_TileObject lastTile = basicTile2;
        GameObject edgesCollector = new GameObject();
        edgesCollector.name = "EdgesCollector";
        edgesCollector.transform.parent = this.transform;
        OnEditingTiles.Clear();
        for (int y = 1; y > -column - 1; y--)
        {
            for (int x = -1; x < row + 1; x++)
            {
                if (y > 0 || x < 0 || y == -column || x == row)
                {
                    GameObject edge = Instantiate(edgeObject, new Vector2(x, y), transform.rotation, edgesCollector.transform);
                }
                else
                {
                    LEditor_TileContainer tContainer = Instantiate(container);
                    containers.Add(tContainer);
                    tContainer.Setup(new Vector2(x, y), parent, containers.IndexOf(tContainer));
                    tContainer.name = "TileContainer" + tContainer.SlotId;

                    LEditor_TileObject tile = null;
                    if (x % 2 == 0)
                    {
                        tile = Instantiate(basicTile1);
                        tile.ObjectName = basicTile1.name;
                        tile.name = tile.ObjectName;
                    }
                    else if (x % 2 == 1)
                    {
                        tile = Instantiate(basicTile2);
                        tile.ObjectName = basicTile2.name;
                        tile.name = tile.ObjectName;
                    }

                    if (tile != null)
                    {
                        OnEditingTiles.Add(tile);

                        tContainer.PlaceGameBoardObject(tile, tContainer.SlotId);

                        if (y == 0 && x == 0)
                        {
                            firstTile = tile;
                            firstContainer = tContainer;
                        }
                        if (y == (-column + 1) && x == (row - 1))
                        {
                            lastTile = tile;
                        }
                        Debug.Log("Generated" + x.ToString() + "," + y.ToString());
                    }
                }

            }
        }
        LEditor_OnTileObject player = Instantiate(playerCharacter);
        player.name = playerCharacter.name;
        this.player = player.GetComponent<Player>();
        firstTile.PlaceOnTileObject(player, firstTile.TileId);
        firstTile.transform.parent = firstContainer.transform;
        float rowFloat = row;
        float columnFloat = column;
        MainCamera.Instance.Initialize(rowFloat, columnFloat, firstTile, lastTile, column);
        TileController.Instance.onEditorTiles = this.OnEditingTiles;
        //LevelEditor.LaunchedLevelEvents += AddActiveTiles;
    }

    public void GameUpdate()
    {
        if (LevelEditor.Instance.currentState == LevelEditor.state.editing)
        {
            for (int i = 0; i < containers.Count; i++)
            {
                if (containers[i] != null)
                {
                    containers[i].GameUpdate();
                }
            }
        }

        if (LevelEditor.Instance.currentState == LevelEditor.state.testing)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i] != null)
                    tiles[i].GameUpdate();
            }
        }
    }

    public void UpgradeTile(LEdtior_GameBoardObject tile, int tileId)
    {
        if (tile != null && tile.GetComponent<LEditor_TileObject>() != null &&
            LevelEditor.Instance.currentEditingState == LevelEditor.editingState.mapBuilding)
        {
            if (tileId < OnEditingTiles.Count)
            {
                OnEditingTiles[tileId] = tile.GetComponent<LEditor_TileObject>();
                Debug.Log("GameBoard upgraded tile" + tileId);
            }
        }
    }

    #region ObjectGetters

    public List<LEditor_TileObject> GetDetectedTiles()
    {
        List<LEditor_TileObject> detecteds = new List<LEditor_TileObject>();
        for (int i = 0; i < OnEditingTiles.Count; i++)
        {
            if (OnEditingTiles[i].detected)
            {
                detecteds.Add(OnEditingTiles[i]);
            }
        }
        return detecteds;
    }

    public List<LEditor_OnTileObject> GetAllConnectableOnTileObject()
    {
        List<LEditor_OnTileObject> connetacblesOTs = new List<LEditor_OnTileObject>();
        for (int i = 0; i < OnEditingTiles.Count; i++)
        {
            if (OnEditingTiles[i].objectOn != null)
            {
                LEditor_OnTileObject onT = OnEditingTiles[i].objectOn;
                if (onT.theType == ObjectType.connectable)
                {
                    connetacblesOTs.Add(onT);
                }
            }
        }
        return connetacblesOTs;
    }

    public List<LEditor_OnTileObject> GetAllPortableOnTileObject()
    {
        List<LEditor_OnTileObject> portableOTs = new List<LEditor_OnTileObject>();
        for (int i = 0; i < OnEditingTiles.Count; i++)
        {
            if (OnEditingTiles[i].objectOn != null)
            {
                LEditor_OnTileObject onT = OnEditingTiles[i].objectOn;
                if (onT.theType == ObjectType.connectable)
                {
                    portableOTs.Add(onT);
                }
            }
        }
        return portableOTs;
    }

    public LEditor_TileObject GetEditingTile(int id)
    {
        if (LevelEditor.Instance.EditingGameboard == this)
        {
            return OnEditingTiles[id];
        }

        return null;
    }

    public LEditor_TileObject GetBasicTile(int id)
    {
        if (LevelEditor.Instance.EditingGameboard == this)
        {
            return factory.GetTile(id % 2);
        }

        return null;
    }


    public TileObject GetTile(int id)
    {
        if (LevelManager.Instance.currentGameBoard == this)
        {
            return OnEditingTiles[id].GetComponent<TileObject>();
        }
        return null;
    }

    public void AddActiveTiles()
    {
        for (int i = 0; i < OnEditingTiles.Count; i++)
        {
            if (OnEditingTiles[i] != null)
            {
                tiles.Add(OnEditingTiles[i].GetComponent<TileObject>());
            }
        }
        LevelEditor.LaunchedLevelEvents -= AddActiveTiles;
        LevelEditor.ReturnToEditingEvents -= AddActiveTiles;
    }

    #endregion


    public void DestroyLevelTiles()
    {
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        OnEditingTiles.Clear();
        OnEditingTiles = new List<LEditor_TileObject>();
    }

    public void Save(CampaignData campaignData, string inputtedText)
    {
        LevelData level = new LevelData(theme, Row, Column, inputtedText);

        levelName = inputtedText;

        for (int i = 0; i < OnEditingTiles.Count; i++)
        {
            level.tileDatas.Add(OnEditingTiles[i].Save());
        }

        level.settingData = levelSetting.Save();

        if (campaignData.levelDatas.Count > 0)
        {
            for (int i = 0; i < campaignData.levelDatas.Count; i++)
            {
                if (campaignData.levelDatas[i].levelName == level.levelName)
                {
                    campaignData.levelDatas[i] = level;
                }
                else if (i == campaignData.levelDatas.Count - 1)
                {
                    campaignData.levelDatas.Add(level);
                }
            }
        }
        else
        {
            campaignData.levelDatas.Add(level);
        }
    }


    public void Load(LevelData data)
    {
        this.row = data.row;
        this.column = data.column;
        size = Row * Column;
        levelName = data.levelName;

        GenerateTilesOnLoad(Row, Column, data);
        LoadOnTiles(data);
        LoadSelectables(data);

        levelSetting.Load(data.settingData);
        Debug.Log(string.Format("Level{2} gameboard{0}{1} has been loaded.", Row, Column, data.levelName));
    }


    public void GenerateTilesOnLoad(int row, int columm, LevelData level)
    {
        LEditor_TileObject basicTile1 = factory.GetTile(0);
        LEditor_TileObject basicTile2 = factory.GetTile(1);


        LEditor_TileObject firstTile = basicTile1;
        LEditor_TileContainer firstContainer = container;
        LEditor_TileObject lastTile = basicTile2;
        GameObject edgesCollector = new GameObject();
        edgesCollector.name = "EdgesCollector";
        edgesCollector.transform.parent = this.transform;

        int tNum = 0;

        for (int y = 1; y > -Column - 1; y--)
        {
            for (int x = -1; x < row + 1; x++)
            {
                if (y > 0 || x < 0 || y == -Column|| x == row)
                {
                    GameObject edge = Instantiate(edgeObject, new Vector2(x, y), transform.rotation, edgesCollector.transform);
                }
                else
                {
                    LEditor_TileContainer tContainer = Instantiate(container);
                    containers.Add(tContainer);
                    tContainer.Setup(new Vector2(x, y), transform, containers.IndexOf(tContainer));
                    tContainer.name = "TileContainer" + tContainer.SlotId;


                    LEditor_TileObject tile = basicTile1;
                    if (level.tileDatas[tNum] != null && level.tileDatas[tNum].idInFactory >= 0)
                    {
                        tile = Instantiate(factory.GetTile(level.tileDatas[tNum].idInFactory));
                        tile.ObjectName = factory.GetTile(level.tileDatas[tNum].idInFactory).name;
                        tile.name = factory.GetTile(level.tileDatas[tNum].idInFactory).name;
                        tContainer.PlaceGameBoardObject(tile, tContainer.SlotId);
                        tile.Load(level.tileDatas[tNum]);
                        tNum += 1;
                    }
                    else
                    {
                        Debug.Log(string.Format("Saved TileObject{0} cannot be loaded because it is not match level theme.", tNum));
                        if (tNum % 2 == 0)
                        {
                            tile = Instantiate(basicTile1);
                        }
                        else if (tNum % 2 == 1)
                        {
                            tile = Instantiate(basicTile2);
                        }
                        tContainer.PlaceGameBoardObject(tile, tContainer.SlotId);
                        tNum += 1;
                    }

                    OnEditingTiles.Add(tile);


                    if (y == 0 && x == 0)
                    {
                        firstTile = tile;
                        firstContainer = tContainer;
                    }
                    if (y == (-Column) && x == (row - 1))
                    {
                        lastTile = tile;
                    }
                    Debug.Log("Generated" + x.ToString() + "," + y.ToString());
                }
            }
        }
    }


    public void LoadOnTiles(LevelData data)
    {
        for (int i = 0; i < data.tileDatas.Count; i++)
        {
            OnTileData onTileData = data.tileDatas[i].objectOnData;
            LEditor_OnTileObject onTile = null;
            if (onTileData != null)
            {
                if (onTileData.idInFactory >= 0)
                {
                    if (onTileData.idsOnBoard != null && onTileData.idsOnBoard.Count <= 1)
                    {
                        onTile = Instantiate(LevelEditor.Instance.EditingGameboard.factory.GetOnTile(onTileData.idInFactory));
                        onTile.name = LevelEditor.Instance.EditingGameboard.factory.GetOnTile(onTileData.idInFactory).name;
                        if (onTile.GetComponent<LEdtior_OnMutipleTileObject>() == null)
                        {
                            containers[i].PlaceGameBoardObject(onTile, data.tileDatas[i].idOnBoard);
                        }
                        else
                        {
                            Debug.LogError(string.Format("Loading onTileObject:{0} is found error: an OnMutipleTileObject stored less than 1 occupied tile ids", i));
                        }
                    }
                    else if (onTileData.idsOnBoard != null)
                    {
                        if (i == onTileData.idsOnBoard[onTileData.idsOnBoard.Count - 1])
                        {
                            onTile = Instantiate(LevelEditor.Instance.EditingGameboard.factory.GetOnTile(onTileData.idInFactory));
                            onTile.name = LevelEditor.Instance.EditingGameboard.factory.GetOnTile(onTileData.idInFactory).name;
                            if (onTile.GetComponent<LEdtior_OnMutipleTileObject>() != null)
                            {
                                containers[i].containingTile.PlaceOnMutipleTileObjectOnLoad(onTile, data.tileDatas[i].objectOnData.idsOnBoard);
                            }
                            else
                            {
                                Debug.LogError(string.Format("Loading onTileObject:{0} is found error: an OnSingleTileObject stored more than 1 occupied tile ids", i));
                            }
                        }
                    }

                }
                else if (onTileData.idInFactory < 0)
                {
                    Debug.Log("Saved OnTileObject cannot be loaded because it is not match level theme.");
                }
            }
            if (onTile != null)
            {
                onTile.Load(onTileData);
            }
        }
    }

    public void LoadSelectables(LevelData data)
    {
        for (int i = 0; i < OnEditingTiles.Count; i++)
        {
            if (data.tileDatas[i] != null)
            {
                if ((data.tileDatas[i].selectableData != null) ||
                (data.tileDatas[i].objectOnData != null && data.tileDatas[i].objectOnData.selectableData != null))
                {
                    if (OnEditingTiles[i].theType == ObjectType.connectable)
                    {
                        Debug.Log(string.Format("Tile{0} is going to load selectable", data.tileDatas[i].idOnBoard));
                        OnEditingTiles[i].GetComponent<LEditor_ConnectableObject>().Load(data.tileDatas[i].selectableData);
                    }
                    else if (OnEditingTiles[i].theType == ObjectType.portal)
                    {
                        Debug.Log(string.Format("Tile{0} is going to load selectable", data.tileDatas[i].idOnBoard));
                        OnEditingTiles[i].GetComponent<LEditor_PortalObject>().Load(data.tileDatas[i].selectableData);
                    }
                    else if (OnEditingTiles[i].objectOn != null && OnEditingTiles[i].objectOn.theType == ObjectType.connectable)
                    {
                        Debug.Log(string.Format("OnTile{0} is going to load selectable", data.tileDatas[i].idOnBoard));
                        OnEditingTiles[i].objectOn.GetComponent<LEditor_ConnectableObject>().Load(data.tileDatas[i].objectOnData.selectableData);
                    }
                    else if (OnEditingTiles[i].objectOn != null && OnEditingTiles[i].objectOn.theType == ObjectType.portal)
                    {
                        Debug.Log(string.Format("OnTile{0} is going to load selectable", data.tileDatas[i].idOnBoard));
                        OnEditingTiles[i].objectOn.GetComponent<LEditor_PortalObject>().Load(data.tileDatas[i].objectOnData.selectableData);
                    }

                }

            }
        }
    }

}
