using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardInfo : ScriptableObject {

    [SerializeField]
    Editor_TileObject basicTile;

    [SerializeField]
    Editor_OnTileObject playerCharacter;

    int row;
    int column;

    public List<Editor_TileObject> tiles = new List<Editor_TileObject>();
    int size;



    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
