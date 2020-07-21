using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager> {

    public LayerMask blockingLayer;
    public LayerMask interactableObjectLayer;


    public LayerMask edgeLayer;
    public LayerMask gameBoardObjectLayer;


    public LayerMask emptyTileLayer;

    public GameBoard currentGameBoard;

    TileObject[] tiles;

    public Player player;

    public GameObject TestingUI;

    // Use this for initialization
    void Start () { 
        LevelEditor.LaunchedLevelEvents += LaunchLevel;
        LevelEditor.ReturnToEditingEvents += ShutDownLevel;
	}

    // Update is called once per frame
    private void Update()
    {
        if (LevelEditor.Instance.currentState == LevelEditor.state.testing)
        {
            currentGameBoard.GameUpdate();
        }
    }


    public void GameUpdate()
    {
        
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
        TestingUI.SetActive(true);
    }

    public void ShutDownLevel()
    {
        if (currentGameBoard != null)
        {
            //currentGameBoard.SetInactiveTiles();
            MainCamera.Instance.SetBackToEditing();
            Destroy(currentGameBoard.gameObject);
            player = null;
        }
        TestingUI.SetActive(false);
    }
}
