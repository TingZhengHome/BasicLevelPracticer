using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : Singleton<TileController> {


    public List<Editor_TileObject> tiles = new List<Editor_TileObject>();

	// Use this for initialization
	void Start () {
        Editor_TileObject.OnTileClicked += UpgradeTile;
	}
	
	// Update is called once per frame
	void Update () {

    }

    public void UpgradeTile(Edtior_GameBoardObject tile, int tileId)
    {
        if (tile != null && tile.GetComponent<Editor_TileObject>() != null)
        {
            tiles[tileId] = tile.GetComponent<Editor_TileObject>();
            Debug.Log("Controller upgraded tile" + tileId);
        }
    }



    public void SetActiveTiles()
    {
        GetList();
        if (tiles.Count > 2)
        {
            foreach (Editor_TileObject tile in tiles)
            {
                tile.GetComponent<Tile>().enabled = true;
                tile.GetComponent<Tile>().Initialize(tile);
                tile.GetComponent<Editor_TileObject>().enabled = false;
            }
        }
    }

    public void GetList()
    {
        if (LevelEditor.Instance.EditingGameboard != null)
        {
            tiles = LevelEditor.Instance.EditingGameboard.tiles;
        }
    }
}
