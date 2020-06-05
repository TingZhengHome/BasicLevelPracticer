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
    public Color unconnectableColor = Color.grey;


    [SerializeField]
    GameObject edgeObject;

    [SerializeField]
    LEditor_TileObject basicTile;

    [SerializeField]
    LEditor_OnTileObject playerCharacter;

    public Player player;

    int row;
    int column;

    public List<LEditor_TileObject> tiles = new List<LEditor_TileObject>();
    int size;

    public void GenerateLevelTilesOnEditor(int row, int column, Transform parent)
    {
        this.row = row;
        this.column = column;
        size = row * column;

        LEditor_TileObject firstTile = basicTile;
        LEditor_TileObject lastTile = basicTile;
        for (int y = 1; y > -column - 2; y--)
        {
            for (int x = -1; x < row + 2; x++)
            {
                if (y > 0 || x < 0 || y == -column - 1 || x == row + 1)
                {
                    GameObject edge = Instantiate(edgeObject, new Vector2(x, y), transform.rotation, this.transform);
                }
                else
                {
                    LEditor_TileObject tile = Instantiate(basicTile);
                    tiles.Add(tile);
                    tile.Setup(new Vector2(x, y), parent, tiles.IndexOf(tile));
                    tile.ObjectName = basicTile.name;
                    if (y == 0 && x == 0)
                    {
                        firstTile = tile;
                    }
                    if (y == (-column + 1) && x == (row - 1))
                    {
                        lastTile = tile;
                    }
                    Debug.Log("Generated" + x.ToString() + "," + y.ToString());
                }

            }
        }
        LEditor_OnTileObject player = Instantiate(playerCharacter);
        firstTile.PlaceGameBoardObject(player, firstTile.TileId);
        float rowFloat = row;
        float columnFloat = column;
        LEditor_Camera.Instance.startCameraPosition = new Vector3(((rowFloat) / 2), (-(columnFloat) / 2), -column - 2f);
        LEditor_Camera.Instance.transform.position = LEditor_Camera.Instance.startCameraPosition;

        LEditor_Camera.Instance.SetLimit(firstTile.transform.position, lastTile.transform.position);
        TileController.Instance.tiles = this.tiles;

        //Editor_SelectedTileUI selectedUI = Instantiate(LevelEditor.Instance.TileSelectedUI).GetComponent<Editor_SelectedTileUI>();
    } 


    public void UpgradeTile(Edtior_GameBoardObject tile, int tileId)
    {
        if (tile != null && tile.GetComponent<LEditor_TileObject>() != null &&
            LevelEditor.Instance.currentEditingState == LevelEditor.editingState.mapBuilding)
        {
            tiles[tileId] = tile.GetComponent<LEditor_TileObject>();
            Debug.Log("GameBoard upgraded tile" + tileId);
        }
    }

    public List<LEditor_TileObject> GetDetectedTiles()
    {
        List<LEditor_TileObject> detecteds = new List<LEditor_TileObject>();
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].detected)
            {
                detecteds.Add(tiles[i]);
            }
        }

        return detecteds;
    }

    public List<LEditor_OnTileObject> GetConnectableOnTileObject()
    {
        List<LEditor_OnTileObject> connetacblesOTs = new List<LEditor_OnTileObject>();
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].objectOn != null)
            {
                LEditor_OnTileObject onT = tiles[i].objectOn;
                if (onT.thisType == LEditor_OnTileObject.types.connectable)
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
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].objectOn != null)
            {
                LEditor_OnTileObject onT = tiles[i].objectOn;
                if (onT.thisType == LEditor_OnTileObject.types.connectable)
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
            return tiles[id];
        }

        return null;
    }

    public void DestroyLevelTiles()
    {
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        tiles.Clear();
        tiles = new List<LEditor_TileObject>();
    }

    private void Awake()
    {
        LEditor_TileObject.OnTileClicked += UpgradeTile;
    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    public void GameUpdate()
    {
        if (LevelEditor.Instance.currentState == LevelEditor.state.editing)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i] != null)
                {
                    tiles[i].GameUpdate();
                }
                else
                {

                }
            } 
        }
    }

    //public void SetActiveTiles()
    //{
    //    if (tiles.Count > 2)
    //    {
    //        foreach (TileOnEditor tile in tiles)
    //        {
    //            tile.GetComponent<Tile>().enabled = true;
    //            tile.GetComponent<Tile>().Initialize(tile);
    //            tile.GetComponent<TileOnEditor>().enabled = false;
    //        }
    //    }
    //}



    //public void SetInactiveTiles()
    //{
    //    if (tiles.Count > 2)
    //    {
    //        foreach (TileOnEditor tile in tiles)
    //        {
    //            tile.GetComponent<TileOnEditor>().enabled = true;
    //            tile.GetComponent<Tile>().enabled = false;
    //        }
    //    }

    //}
}
