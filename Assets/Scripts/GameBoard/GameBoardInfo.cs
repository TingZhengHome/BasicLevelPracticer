using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardInfo : ScriptableObject {

    [SerializeField]
    LEditor_TileObject basicTile;

    [SerializeField]
    LEditor_OnTileObject playerCharacter;

    int row;
    int column;

    public List<LEditor_TileObject> tiles = new List<LEditor_TileObject>();
    int size;



    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
