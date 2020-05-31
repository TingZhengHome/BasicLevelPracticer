using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager> {

    public LayerMask blockingLayer;
    public LayerMask interactableObjectLayer;
    public LayerMask emptyTileLayer;


    public GameBoard currentGameBoard;

    Tile[] tiles;

    public Player player;

    public GameObject TestingUI;

    // Use this for initialization
    void Start () {
        LevelEditor.LaunchedLevel += LaunchLevel;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LaunchLevel()
    {
        if (currentGameBoard == null)
        {
            currentGameBoard = LevelEditor.Instance.EditingGameboard;
        }
        else
        {
            Destroy(currentGameBoard.gameObject);
            currentGameBoard = LevelEditor.Instance.EditingGameboard;
        }
        TileController.Instance.SetActiveTiles();
        player = currentGameBoard.player;
        TestingUI.SetActive(true);
    }

    public void ShutDownLevel()
    {
        if (currentGameBoard != null)
        {
            //currentGameBoard.SetInactiveTiles();
            Destroy(currentGameBoard.gameObject);
            player = null;
        }
        TestingUI.SetActive(false);
    }
}
