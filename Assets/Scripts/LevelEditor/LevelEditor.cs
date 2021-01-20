using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditor : Singleton<LevelEditor>
{
    public enum state { editing, testing, saving };

    public enum editingState { mapBuilding, settingConnection, settingPortals, settingWinningPickables, settingWinningTile }

    public state currentState = state.editing;
    public editingState currentEditingState = editingState.mapBuilding;
    public LEditor_SelectableObject selectedObject;
    public LEditor_OnTileObject movingObject;

    [SerializeField]
    public LEditor_OnTileObjectButton pickUpButton;

    [SerializeField]
    public GameBoard gameBoard;
    public GameBoard EditingGameboard;
    GameBoard tempSavedGBoard;

    private int column = 0, row = 0;

    public LEditor_Button clickedBoardObjectButton;

    public GameObject Hover;
    public LayerMask hoverLayer;

    public bool isPlayerPlaced;
    public bool isMovingPlacedObject;

    [SerializeField]
    float continousPlacingDelay = 0.2f;
    float continousPlacingCounter;

    public delegate void LaunchLevelEvent();
    public static event LaunchLevelEvent LaunchedLevelEvents;

    public delegate void ReturnToEditingEvent();
    public static event ReturnToEditingEvent ReturnToEditingEvents;

    public CampaignData campaignData = new CampaignData();

    public GameBoardFactory boardsFactory;

    void Start()
    {
        isPlayerPlaced = false;
        LaunchedLevelEvents += EscapeSelectingState;
        Hover = FindObjectOfType<Hover>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == state.editing)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if ((movingObject != null && movingObject.tag != "player") ||
                    clickedBoardObjectButton != null)
                {
                    EndCurrentEditingEvent();
                }

                EndSavingAndLoading();
            }
            if (Input.GetMouseButton(0) && !isMovingPlacedObject)
            {
                continousPlacingCounter += Time.deltaTime;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                continousPlacingCounter = 0;
            }
            if (EditingGameboard != null)
                EditingGameboard.GameUpdate();
        }
    }

    public void SetColumn(string input)
    {
        Text waringText = LEditor_UIManager.Instance.ScaleInputWarningText;

        waringText.gameObject.SetActive(false);
        waringText.text = string.Empty;
        int inputNum = int.Parse(input);

        if (inputNum > 0)
        {
            LevelEditor.Instance.column = inputNum;

            if (column == 1 && row == 1)
            {
                waringText.gameObject.SetActive(true);
                waringText.text = "Either Column or Row should be greater than 1.";
            }
        }
        else
        {
            column = inputNum;
            waringText.gameObject.SetActive(true);
            waringText.text = "Neither Column or Row can be zero.";
        }
    }

    public void SetRow(string input)
    {
        Text waringText = LEditor_UIManager.Instance.ScaleInputWarningText;

        waringText.gameObject.SetActive(false);
        waringText.text = string.Empty;
        int inputNum = int.Parse(input);

        if (inputNum > 0)
        {
            row = inputNum;
            if (column == 1 && row == 1)
            {
                waringText.gameObject.SetActive(true);
                waringText.text = "Either Column or Row should be greater than 1.";
            }
        }
        else
        {
            row = inputNum;
            waringText.gameObject.SetActive(true);
            waringText.text = "Neither Column or Row can be zero.";
        }
    }

    public bool MouseHoldThanDelay()
    {
        return continousPlacingCounter > continousPlacingDelay;
    }

    public void GenerateGameBoard()
    {
        LEditor_UIManager.Instance.TileSelectedUI.GetComponent<LEditor_TileSelectedUI>().UnAttach();
        if (selectedObject != null)
        {
            EscapeSelectingState();
        }

        if (column != 0 && row != 0)
        {
            if (column < 1 && row < 1)
            {
                LEditor_UIManager.Instance.ScaleInputWarningText.gameObject.SetActive(true);
                LEditor_UIManager.Instance.ScaleInputWarningText.text = "Either Column or Row should be greater than 1.";
                return;
            }
            if (EditingGameboard != null)
            {
                Destroy(EditingGameboard.gameObject);
            }
            EditingGameboard = Instantiate(gameBoard);
            EditingGameboard.transform.position = Vector3.zero;
            EditingGameboard.GenerateTilesOnEditor(row, column, EditingGameboard.transform);
            EditingGameboard.gameObject.name = "GameBoard";
        }
        else
        {
            LEditor_UIManager.Instance.ScaleInputWarningText.gameObject.SetActive(true);
            LEditor_UIManager.Instance.ScaleInputWarningText.text = "Neither Column or Row can be zero.";
        }
    }

    public void StartMovingObject(LEditor_OnTileObject pickUp)
    {
        isMovingPlacedObject = true;
        movingObject = pickUp;
        pickUpButton.GetComponent<LEditor_OnTileObjectButton>().PickUp(pickUp);
    }

    public void ClickButton(LEditor_Button clicked)
    {
        Instance.clickedBoardObjectButton = clicked;
        Instance.isMovingPlacedObject = false;
    }

    public void EndCurrentEditingEvent()
    {
        if (clickedBoardObjectButton != null)
        {
            CancelButtonClick();
        }
        if (isMovingPlacedObject)
        {
            EndMovingObject();
        }
        if (selectedObject != null)
        {
            EscapeSelectingState();
        }
    }

    public void EndMovingObject()
    {
        isMovingPlacedObject = false;
        movingObject = null;
        CancelButtonClick();
        pickUpButton.Emptize();
    }

    public void CancelButtonClick()
    {
        clickedBoardObjectButton = null;
    }

    public void EscapeSelectingState()
    {
        CancelButtonClick();
        currentEditingState = editingState.mapBuilding;
        selectedObject = null;
        LEditor_UIManager.Instance.TileSelectedUI.GetComponent<LEditor_TileSelectedUI>().UnAttach();

        foreach (LEditor_TileObject tile in EditingGameboard.OnEditingTiles)
        {
            if (tile != null)
            {
                tile.TurnColor(EditingGameboard.defaultColor);
            }
        }
    }

    public void LaunchLevel()
    {
        tempSavedGBoard = Instantiate(EditingGameboard);
        tempSavedGBoard.SetRowAndColumn(EditingGameboard.Row, EditingGameboard.Column);
        tempSavedGBoard.gameObject.SetActive(false);
        CancelButtonClick();
        LaunchedLevelEvents();
        currentState = state.testing;
        LEditor_UIManager.Instance.ShutDownEditingUI();
        EditingGameboard.AddActiveTiles();
    }

    public void ReturnToEditing()
    {
        if (currentState == state.testing)
        {
            EditingGameboard = tempSavedGBoard;
            tempSavedGBoard = null;
            EditingGameboard.gameObject.name = "GameBoard";
            EditingGameboard.gameObject.SetActive(true);
            if (ReturnToEditingEvents != null)
                ReturnToEditingEvents();
            LEditor_UIManager.Instance.ShowEditingUI();
            MainCamera.Instance.GetComponent<MainCamera>().enabled = true;
            currentState = state.editing;
        }
        else
        {
            LEditor_UIManager.Instance.ShutDownEditingUI();
            currentEditingState = editingState.mapBuilding;
            EndSavingAndLoading();
        }
    }

    //public void OnSaveLevelClick()
    //{
    //    if (currentEditingState == editingState.mapBuilding ||
    //        currentEditingState == editingState.settingConnection ||
    //        currentEditingState == editingState.settingPortals)
    //    {
    //        //LEditor_UIManager.Instance.ShowSaveNameAsker(LEditor_UIManager.Instance.LevelSavingText);
    //    }
    //}

    public void StartSettingLevel()
    {
        EndCurrentEditingEvent();
        LevelSetting level = null;
        if (EditingGameboard != null)
        {
            level = EditingGameboard.levelSetting;
        }
        
        LEditor_UIManager.Instance.ShutDownEditingUI();
        LEditor_UIManager.Instance.ShowSettingLevelUI();
    }

    public void SaveGameBoardAsAnotherLevel(string inputtedText)
    {
        if (SaveManager.Instance.loadedCampaign != null && SaveManager.Instance.loadedLevel != null)
        {
            EditingGameboard.Save(SaveManager.Instance.loadedCampaign, inputtedText);
        }
        SaveManager.Instance.UpgradeEditingCampaign();
        if (campaignData.levelDatas.Count > 0)
            Debug.Log("EditingGameBoard has been saved as a level: " + campaignData.levelDatas[0].levelName + " in current campaign.");

        EndSavingAndLoading();
    }

    public void SaveGameBoardAsALevel()
    {
        if (EditingGameboard != null)
        {
            if (EditingGameboard.levelSetting == null)
            {
                OKMessagePanel.Instance.DisplayMessage(OKMessageLibrary.levelNotSetCondition);
                StartSettingLevel();
                return;
            }
            else if (EditingGameboard.levelSetting.neededPickables.Count == 0 && EditingGameboard.levelSetting.winningTile == null)
            {
                OKMessagePanel.Instance.DisplayMessage(OKMessageLibrary.levelNotSetCondition);
                StartSettingLevel();
                return;
            }

            if (SaveManager.Instance.loadedCampaign != null && SaveManager.Instance.loadedLevel != null)
            {
                EditingGameboard.Save(SaveManager.Instance.loadedCampaign, SaveManager.Instance.loadedLevel.levelName);
            }
        }
        

        SaveManager.Instance.UpgradeEditingCampaign();

        EndSavingAndLoading();
    }

    public void LoadLevel(LevelData leveldata)
    {
        if (leveldata != null)
        {
            if (EditingGameboard != null)
            {
                EndCurrentEditingEvent();
                Destroy(EditingGameboard.gameObject);
            }

            EditingGameboard = Instantiate(boardsFactory.GetGameBoard(leveldata.theme));
            EditingGameboard.Load(leveldata);
        }

        EndSavingAndLoading();
    }

    public void EndSavingAndLoading()
    {
        LEditor_UIManager.Instance.EndSavingAndLoading();
        EndCurrentEditingEvent();
        if (currentState == state.saving)
        {
            currentState = state.editing;
        }
        if (currentEditingState == editingState.settingWinningPickables ||
            currentEditingState == editingState.settingWinningTile)
        {
            currentEditingState = editingState.mapBuilding;
        }
    }

    public void BackToMainMenu()
    {
        GameManager.Instance.GoMainMenu();
    }
}
