using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : Singleton<TileController> {


    public List<LEditor_TileObject> onEditorTiles = new List<LEditor_TileObject>();

    public List<TileObject> tiles = new List<TileObject>();



	// Use this for initialization
	void Start () {
        LEditor_TileContainer.OnTileClicked += UpgradeTiles;
	}
	
	// Update is called once per frame
	void Update () {

    }

    public void UpgradeTiles(LEdtior_GameBoardObject tile, int tileId)
    {
        if (tile != null && tile.GetComponent<LEditor_TileObject>() != null &&
            LevelEditor.Instance.currentEditingState == LevelEditor.editingState.mapBuilding)
        {
            if (tileId < onEditorTiles.Count)
            {
                onEditorTiles[tileId] = tile.GetComponent<LEditor_TileObject>();
                Debug.Log("Controller upgraded tile" + tileId);
            }
        }
    }

    public void SetActiveTiles()
    {
        GetList();
        //LevelEditor.Instance.EditingGameboard.AddActiveTiles();
        //tiles = LevelEditor.Instance.EditingGameboard.tiles;
        if (onEditorTiles.Count > 2)
        {
            foreach (LEditor_TileObject tile in onEditorTiles)
            {
                if (tile != null)
                {
                    TileObject theTile = tile.GetComponent<TileObject>();
                    theTile.enabled = true;
                    theTile.Initialize(tile);
                    tile.GetComponent<LEditor_TileObject>().enabled = false;
                    tiles.Add(theTile);
                }
            }

            foreach (TileObject tile in tiles)
            {
                if (tile.Property != null)
                {
                    tile.Property.Initialize(tile, tile.GetComponent<LEditor_TileObject>().interactable);
                }
                if (tile.objectOnThis != null)
                {
                    if (tile.objectOnThis.Property != null)
                    {
                        tile.objectOnThis.Property.Initialize(tile.objectOnThis, tile.objectOnThis.GetComponent<LEditor_OnTileObject>().interactable);
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Too few tiles to launch level.");
        }
    }

    public void GetList()
    {
        if (LevelEditor.Instance.EditingGameboard != null)
        {
            onEditorTiles = LevelEditor.Instance.EditingGameboard.OnEditingTiles;
        }
    }
}
