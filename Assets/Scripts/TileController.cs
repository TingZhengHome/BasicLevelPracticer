using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : Singleton<TileController> {


    public List<LEditor_TileObject> tiles = new List<LEditor_TileObject>();

	// Use this for initialization
	void Start () {
        LEditor_TileObject.OnTileClicked += UpgradeTiles;
	}
	
	// Update is called once per frame
	void Update () {

    }

    public void UpgradeTiles(Edtior_GameBoardObject tile, int tileId)
    {
        if (tile != null && tile.GetComponent<LEditor_TileObject>() != null && 
            LevelEditor.Instance.currentEditingState == LevelEditor.editingState.mapBuilding)
        {
            tiles[tileId] = tile.GetComponent<LEditor_TileObject>();
            Debug.Log("Controller upgraded tile" + tileId);
        }
    }

    public void SetActiveTiles()
    {
        GetList();
        if (tiles.Count > 2)
        {
            foreach (LEditor_TileObject tile in tiles)
            {
                tile.GetComponent<Tile>().enabled = true;
                tile.GetComponent<Tile>().Initialize(tile);
                tile.GetComponent<LEditor_TileObject>().enabled = false;
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
