using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour {

    public Color placeableColor = Color.yellow;
    public Color notPlaceableColor = Color.red;
    public Color defaultColor = Color.white;
    public Color objectOverlappedColor = Color.cyan;
    public Color selectedColor = Color.grey;
    public Color connectableColor = Color.blue;
    public Color connectedColor = Color.magenta;
    public Color connectorColor = Color.green;
    public Color unclickableColor = Color.grey;

    [SerializeField]
    string GameBoardTheme;

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


    public List<LEditor_TileContainer> containers = new List<LEditor_TileContainer>();
    public List<LEditor_TileObject> OnEditorTiles = new List<LEditor_TileObject>();
    int size;

    public List<TileObject> tiles = new List<TileObject>();

    public LevelSetting levelSetting = new LevelSetting();

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
        OnEditorTiles.Clear();
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
                    OnEditorTiles.Add(tile);
                    tile.ObjectName = BasicTile.name;

                    tContainer.PlaceGameBoardObject(tile, tContainer.SlotId);

                    if (y == 0 && x == 0)
                    {
                        firstTile = tile;
                        firstContainer = tContainer;
                    }
                    if (y == (-column ) && x == (row - 1))
                    {
                        lastTile = tile;
                    }
                    Debug.Log("Generated" + x.ToString() + "," + y.ToString());
                }

            }
        }
        LEditor_OnTileObject player = Instantiate(playerCharacter);
        this.player = player.GetComponent<Player>();
        firstTile.PlaceOnTileObject(player, firstTile.TileId);
        firstTile.transform.parent = firstContainer.transform;
        float rowFloat = row;
        float columnFloat = column;
        MainCamera.Instance.Initialize(rowFloat, columnFloat, firstTile, lastTile, column);
        TileController.Instance.onEditorTiles = this.OnEditorTiles;
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
            if (tileId < OnEditorTiles.Count)
            {
                OnEditorTiles[tileId] = tile.GetComponent<LEditor_TileObject>();
                Debug.Log("GameBoard upgraded tile" + tileId);
            }
        }
    }

    public List<LEditor_TileObject> GetDetectedTiles()
    {
        List<LEditor_TileObject> detecteds = new List<LEditor_TileObject>();
        for (int i = 0; i < OnEditorTiles.Count; i++)
        {
            if (OnEditorTiles[i].detected)
            {
                detecteds.Add(OnEditorTiles[i]);
            }
        }

        return detecteds;
    }

    public List<LEditor_OnTileObject> GetConnectableOnTileObject()
    {
        List<LEditor_OnTileObject> connetacblesOTs = new List<LEditor_OnTileObject>();
        for (int i = 0; i < OnEditorTiles.Count; i++)
        {
            if (OnEditorTiles[i].objectOn != null)
            {
                LEditor_OnTileObject onT = OnEditorTiles[i].objectOn;
                if (onT.theType == condition.connectable)
                {
                    connetacblesOTs.Add(onT);
                }
            }
        }
        return connetacblesOTs;
    }

    public List<LEditor_OnTileObject> GetPortableOnTileObject()
    {
        List<LEditor_OnTileObject> portableOTs = new List<LEditor_OnTileObject>();
        for (int i = 0; i < OnEditorTiles.Count; i++)
        {
            if (OnEditorTiles[i].objectOn != null)
            {
                LEditor_OnTileObject onT = OnEditorTiles[i].objectOn;
                if (onT.theType == condition.connectable)
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
            return OnEditorTiles[id];
        }

        return null;
    }

    public void AddActiveTiles()
    {
        for (int i = 0; i < OnEditorTiles.Count; i++)
        {
            if (OnEditorTiles[i] != null)
            {
                tiles.Add(OnEditorTiles[i].GetComponent<TileObject>());
            }
        }
        LevelEditor.LaunchedLevelEvents -= AddActiveTiles;
        LevelEditor.ReturnToEditingEvents -= AddActiveTiles;
    }


    public void DestroyLevelTiles()
    {
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        OnEditorTiles.Clear();
        OnEditorTiles = new List<LEditor_TileObject>();
    }



    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
	}

  
}
