﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{

    public LayerMask blockingLayer;
    public LayerMask interactableObjectLayer;


    public LayerMask edgeLayer;
    public LayerMask gameBoardObjectLayer;
    public LayerMask tileLayer;
    public LayerMask onTileLayer;


    public LayerMask emptyTileLayer;

    public GameBoard currentGameBoard;

    TileObject[] tiles;

    public Player player;

    public GameObject TestingUI;

    // Use this for initialization
    void Start()
    {
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
            MainCamera.Instance.SetBackToEditing();
            Destroy(currentGameBoard.gameObject);
            player = null;
        }
        TestingUI.SetActive(false);
    }

    [SerializeField]
    LevelClearPanel levelClearPanel;
    public void ShowLevelClearPanel()
    {
        levelClearPanel.Show();
    }

    public void LoadNextLevel()
    {

        CampaignData loadedCampaign = SaveManager.Instance.loadedCampaign;

        int currentIndex = loadedCampaign.levelDatas.FindIndex(x => x == SaveManager.Instance.loadedLevel);

        for (int i = currentIndex; i < loadedCampaign.levelDatas.Count -1; i++)
        {
            LevelData nextLevel = loadedCampaign.levelDatas[currentIndex + i];
            if (nextLevel.row * nextLevel.column > 1 && 
                (nextLevel.settingData.neededPickablesIds.Count > 0 || nextLevel.settingData.TargetedTileId != -1))
            {
                LevelEditor.Instance.ReturnToEditing();
                SaveManager.Instance.LoadLevel(nextLevel.levelName);
                if (SaveManager.Instance.loadedLevel == nextLevel)
                    LevelEditor.Instance.LaunchLevel();
                levelClearPanel.gameObject.SetActive(false);
                return;
            }
        }

        OKMessagePanel.Instance.DisplayMessage(OKMessageLibrary.noLevelToLoad);

    }

    public void ReplayCurrentLevel()
    {
        LevelEditor.Instance.ReturnToEditing();
        LevelEditor.Instance.LaunchLevel();
        levelClearPanel.gameObject.SetActive(false);
    }
}
