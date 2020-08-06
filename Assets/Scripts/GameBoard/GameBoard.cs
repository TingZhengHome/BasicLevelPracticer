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

    public GameBoardThem theme;

    [SerializeField]
    GameObject edgeObject;

    [SerializeField]
    LEditor_TileObject basicTile;
    [SerializeField]
    LEditor_TileContainer container;

    [SerializeField]
    LEditor_OnTileObject playerCharacter;

    public LEditor_TileObject BasicTile
    {
        get
        {
            return basicTile;
        }

        private set
        {
            basicTile = value;
        }
    }


    public Player player;

    int row;
    int column;
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

    public void GenerateLevelTilesOnEditor(int row, int column, Transform parent)
    {
        this.row = row;
        this.column = column;
        size = row * column;

        LEditor_TileObject firstTile = BasicTile;
        LEditor_TileContainer firstContainer = container;
        LEditor_TileObject lastTile = BasicTile;
        GameObject edgesCollector = new GameObject();
        edgesCollector.name = "EdgesCollector";
        edgesCollector.transform.parent = this.transform;
        OnEditingTiles.Clear();
        for (int y = 1; y > -column - 2; y--)
        {
            for (int x = -1; x < row + 2; x++)
            {
                if (y > 0 || x < 0 || y == -column - 1 || x == row + 1)
                {
                    GameObject edge = Instantiate(edgeObject, new Vector2(x, y), transform.rotation, edgesCollector.transform);
                }
                else
                {
                    LEditor_TileContainer tContainer = Instantiate(container);
                    containers.Add(tContainer);
                    tContainer.Setup(new Vector2(x, y), parent, containers.IndexOf(tContainer));
                    tContainer.name = "TileContainer" + tContainer.SlotId;

                    LEditor_TileObject tile = Instantiate(BasicTile);
                    OnEditingTiles.Add(tile);
                    tile.ObjectName = BasicTile.name;
                    tile.name = BasicTile.name;

                    tContainer.PlaceGameBoardObject(tile, tContainer.SlotId);

                    if (y == 0 && x == 0)
                    {
                        firstTile = tile;
                        firstContainer = tContainer;
                    }
                    if (y == (-column) && x == (row - 1))
                    {
                        lastTile = tile;
                    }
                    Debug.Log("Generated" + x.ToString() + "," + y.ToString());
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

    public void Save(SaveData save)
    {
        LevelData level = new LevelData(theme, row, column);

        for (int i = 0; i < OnEditingTiles.Count; i++)
        {
            level.tileDatas.Add(OnEditingTiles[i].Save());
        }

        level.settingData = levelSetting.Save();

        save.levelDatas.Add(level);
    }


    public void Load(LevelData data)
    {
        this.row = data.row;
        this.column = data.column;
        size = row * column;

        GenerateLevelTilesOnLoad(row, column, data);
        LoadOnTiles(data);
        LoadSelectables(data);

        levelSetting.Load(data.settingData);
        Debug.Log(string.Format("Level gameboard{0}{1} has been loaded.", row, column));
    }


    public void GenerateLevelTilesOnLoad(int row, int columm, LevelData level)
    {
        LEditor_TileObject firstTile = BasicTile;
        LEditor_TileContainer firstContainer = container;
        LEditor_TileObject lastTile = BasicTile;
        GameObject edgesCollector = new GameObject();
        edgesCollector.name = "EdgesCollector";
        edgesCollector.transform.parent = this.transform;

        int tNum = 0;

        for (int y = 1; y > -column - 2; y--)
        {
            for (int x = -1; x < row + 2; x++)
            {
                if (y > 0 || x < 0 || y == -column - 1 || x == row + 1)
                {
                    GameObject edge = Instantiate(edgeObject, new Vector2(x, y), transform.rotation, edgesCollector.transform);
                }
                else
                {
                    LEditor_TileContainer tContainer = Instantiate(container);
                    containers.Add(tContainer);
                    tContainer.Setup(new Vector2(x, y), transform, containers.IndexOf(tContainer));
                    tContainer.name = "TileContainer" + tContainer.SlotId;


                    LEditor_TileObject tile = basicTile;
                    if (level.tileDatas[tNum].idInFactory >= 0)
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
                        tile = Instantiate(basicTile);
                        tContainer.PlaceGameBoardObject(tile, tContainer.SlotId);
                    }

                    OnEditingTiles.Add(tile);


                    if (y == 0 && x == 0)
                    {
                        firstTile = tile;
                        firstContainer = tContainer;
                    }
                    if (y == (-column) && x == (row - 1))
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
                    if ( onTileData.idsOnBoard != null && onTileData.idsOnBoard.Count <= 1)
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
                    else
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
            if ((data.tileDatas[i].selectableData != null) ||
                (data.tileDatas[i].objectOnData != null && data.tileDatas[i].objectOnData.selectableData != null))
            {
                if (OnEditingTiles[i].theType == ObjectType.connectable)
                {
                    Debug.Log(string.Format("Tile{0} is going to load selectable", data.tileDatas[i].idOnBoard));
                    OnEditingTiles[i].GetComponent<LEditor_ConnectableObject>().Load(data.tileDatas[i].selectableData);
                }
                else if (OnEditingTiles[i].theType == ObjectType.portable)
                {
                    Debug.Log(string.Format("Tile{0} is going to load selectable", data.tileDatas[i].idOnBoard));
                    OnEditingTiles[i].GetComponent<LEditor_PortableObject>().Load(data.tileDatas[i].selectableData);
                }
                else if (OnEditingTiles[i].objectOn != null && OnEditingTiles[i].objectOn.theType == ObjectType.connectable)
                {
                    Debug.Log(string.Format("OnTile{0} is going to load selectable", data.tileDatas[i].idOnBoard));
                    OnEditingTiles[i].objectOn.GetComponent<LEditor_ConnectableObject>().Load(data.tileDatas[i].objectOnData.selectableData);
                }
                else if (OnEditingTiles[i].objectOn != null && OnEditingTiles[i].objectOn.theType == ObjectType.portable)
                {
                    Debug.Log(string.Format("OnTile{0} is going to load selectable", data.tileDatas[i].idOnBoard));
                    OnEditingTiles[i].objectOn.GetComponent<LEditor_PortableObject>().Load(data.tileDatas[i].objectOnData.selectableData);
                }

            }
        }
    }

}
