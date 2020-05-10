using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour {


    public Color placeableColor = Color.yellow;

    public Color notPlaceableColor = Color.red;

    public Color defaultColor = Color.white;

    public Color objectOverlappedColor = Color.cyan;


    [SerializeField]
    GameObject edgeObject;

    [SerializeField]
    Editor_TileObject basicTile;

    [SerializeField]
    Editor_OnTileObject playerCharacter;

    public Player player;
    
    int row;
    int column;
    
    public List<Editor_TileObject> tiles = new List<Editor_TileObject>();
    int size;    

    public void GenerateLevelTilesOnEditor(int row, int column, Transform parent)
    {
        this.row = row;
        this.column = column;
        size = row * column;

        Editor_TileObject firstTile = basicTile;
        Editor_TileObject lastTile = basicTile;
        for (int y = 1; y > -column - 2; y--)
        {
            for (int x = -1; x < row + 2; x++)
            {
                if (y > 0 || x < 0 || y == -column -1 || x == row + 1)
                {
                    GameObject edge = Instantiate(edgeObject, new Vector2(x, y), transform.rotation, this.transform);
                }
                else
                {
                    Editor_TileObject tile = Instantiate(basicTile);
                    tile.Setup(new Vector2(x, y), parent);
                    tiles.Add(tile);
                    tile.tileId = tiles.IndexOf(tile);
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
        Editor_OnTileObject player = Instantiate(playerCharacter);
        firstTile.PlaceGameBoardObject(player, firstTile.tileId);
        float rowFloat = row;
        float columnFloat = column;
        Editor_Camera.Instance.startCameraPosition = new Vector3(((rowFloat) / 2), (-(columnFloat) / 2), -column - 2f);
        Editor_Camera.Instance.transform.position = Editor_Camera.Instance.startCameraPosition;

        Editor_Camera.Instance.SetLimit(firstTile.transform.position, lastTile.transform.position);
        TileController.Instance.tiles = this.tiles;
    }


    public void UpgradeTile(Edtior_GameBoardObject tile, int tileId)
    {
        if (tile != null && tile.GetComponent<Editor_TileObject>() != null)
        {
            tiles[tileId] = tile.GetComponent<Editor_TileObject>();
            Debug.Log("GameBoard upgraded tile" + tileId);
        }
    }

    public List<Editor_TileObject> DetectedTiles()
    {
        List<Editor_TileObject> detecteds = new List<Editor_TileObject>();
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].detected)
            {
                detecteds.Add(tiles[i]);
            }
        }

        return detecteds;
    }

    //public void PlaceTile(TileOnEditor placingTile, TileOnEditor overlapedTile)
    //{
    //    if (overlapedTile.objectOn != null)
    //    {
    //        Destroy(overlapedTile.objectOn.gameObject);
    //    }
    //    placingTile.Setup(overlapedTile.transform.position, overlapedTile.transform.parent, overlapedTile.tileId);
    //    tiles[overlapedTile.tileId] = placingTile;
    //    Destroy(overlapedTile.gameObject);
    //}

    public void DestroyLevelTiles()
    {
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        tiles.Clear();
        tiles = new List<Editor_TileObject>();
    }

    private void Awake()
    {
        Editor_TileObject.OnTileClicked += UpgradeTile;
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
                    //Debug.Log("Missing tile" + i);
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
