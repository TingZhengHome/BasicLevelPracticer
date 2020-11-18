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
        LEditor_TileSelectedUI.Instance.UnAttach();

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

    public void OnSaveLevelClick()
    {
        if (currentEditingState == editingState.mapBuilding ||
            currentEditingState == editingState.settingConnection ||
            currentEditingState == editingState.settingPortals)
        {
            LEditor_UIManager.Instance.ShowSaveNameAsker(LEditor_UIManager.Instance.LevelSavingText);
        }
    }

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

    public void SaveGameBoardAsALevel(string inputtedText)
    {
        EditingGameboard.Save(campaignData, inputtedText);
        if (campaignData.levelDatas.Count > 0)
            Debug.Log("EditingGameBoard has been saved as a level: " + campaignData.levelDatas[0].levelName + " in current campaign.");

        EndSavingAndLoading();
    }

    public void SaveLevelsAsACampaign(string inputtedText)
    {
        campaignData.campaignName = inputtedText;

        SerializationManagger.Save(inputtedText, campaignData);

        string path = Application.persistentDataPath + "/saves/" + inputtedText + ".save";
        if (File.Exists(path))
            Debug.Log(string.Format("Levels has been saved as a campaign in {0}.", path));

        EndSavingAndLoading();
    }

    public void LoadLevel(string saveName)
    {
        if (campaignData.levelDatas.Count > 0)
        {
            LevelData levelData = null;

            for (int i = 0; i < campaignData.levelDatas.Count; i++)
            {
                Debug.Log(string.Format("levelData{0} is going to be loaded", i));

                if (campaignData.levelDatas[i] != null)
                {
                    if (campaignData.levelDatas[i].levelName == saveName)
                    {
                        levelData = campaignData.levelDatas[i];
                        Debug.Log(string.Format("levelData{0} is loaded.", i));
                    }
                }
            }

            if (levelData != null)
            {
                if (EditingGameboard != null)
                {
                    EndCurrentEditingEvent();
                    Destroy(EditingGameboard.gameObject);
                }

                EditingGameboard = Instantiate(boardsFactory.GetGameBoard(levelData.theme));
                EditingGameboard.Load(levelData);
            }
            else
            {
                Debug.LogError("The selected level is not saved in the campaign.");
            }
        }
        else
        {
            Debug.LogError("Loaded campaign data stores zero level.");
        }

        EndSavingAndLoading();
    }

    public void LoadCampaign(string saveName)
    {
        campaignData = (CampaignData)SerializationManagger.Load(Application.persistentDataPath + "/saves/" + saveName + ".save");

        if (campaignData == null)
        {
            Debug.Log("Failed to load campaign:" + string.Format(Application.persistentDataPath + "/saves/" + saveName + ".save") + ".");
        }
        else
        {
            Debug.Log("Campaign" + saveName + " is loaded.");
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
}
