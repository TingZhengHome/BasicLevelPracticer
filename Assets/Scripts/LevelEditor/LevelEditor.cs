using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditor : Singleton<LevelEditor>
{

    public enum state { editing, testing };

    public enum editingState { mapBuilding, settingConnection, settingPortals, settingWinningPickables, settingWinningTile }

    public state currentState = state.editing;
    public editingState currentEditingState = editingState.mapBuilding;
    public LEditor_SelectableObject selectedObject;
    public LEditor_OnTileObject movingObject;

    [SerializeField]
    public GameObject EditorButtonUI;
    [SerializeField]
    GameObject BoardScaleAskerUI;
    [SerializeField]
    public GameObject SettingLevelButtons;
    public Button allPickablesButton;
    public Button certainPointButton;
    public Button saveLevelButton;
    public Button returnToEditingButton;


    public GameObject TileSelectedUI;

    [SerializeField]
    public LEditor_OnTileObjectButton pickUpButton;


    [SerializeField]
    public GameBoard gameBoard;
    public GameBoard EditingGameboard;
    GameBoard tempSavedGBoard;

    private int column = 0, row = 0;

    [SerializeField]
    Text InputWarningText;

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



    void Start()
    {
        BoardScaleAskerUI.SetActive(true);
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


    public bool MouseHoldThanDelay()
    {
        return continousPlacingCounter > continousPlacingDelay;
    }

    public void SetColumn(string input)
    {
        InputWarningText.gameObject.SetActive(false);
        InputWarningText.text = string.Empty;
        int inputNum = int.Parse(input);

        if (inputNum > 0)
        {
            column = inputNum;

            if (column == 1 && row == 1)
            {
                InputWarningText.gameObject.SetActive(true);
                InputWarningText.text = "Either Column or Row should be greater than 1.";
            }
        }
        else
        {
            column = inputNum;
            InputWarningText.gameObject.SetActive(true);
            InputWarningText.text = "Neither Column or Row can be zero.";
        }
    }

    public void SetRow(string input)
    {
        InputWarningText.gameObject.SetActive(false);
        InputWarningText.text = string.Empty;
        int inputNum = int.Parse(input);

        if (inputNum > 0)
        {
            row = inputNum;
            if (column == 1 && row == 1)
            {
                InputWarningText.gameObject.SetActive(true);
                InputWarningText.text = "Either Column or Row should be greater than 1.";
            }
        }
        else
        {
            row = inputNum;
            InputWarningText.gameObject.SetActive(true);
            InputWarningText.text = "Neither Column or Row can be zero.";
        }
    }

    public void GenerateGameBoard()
    {
        TileSelectedUI.GetComponent<LEditor_SelectedTileUI>().UnAttach();
        if (selectedObject != null)
        {
            EscapeSelectingState();
        }

        if (column != 0 && row != 0)
        {

            if (column < 1 && row < 1)
            {
                InputWarningText.gameObject.SetActive(true);
                InputWarningText.text = "Either Column or Row should be greater than 1.";
                return;
            }
            if (EditingGameboard != null)
            {
                Destroy(EditingGameboard.gameObject);
            }
            EditingGameboard = Instantiate(gameBoard);
            EditingGameboard.transform.position = Vector3.zero;
            EditingGameboard.GenerateLevelTilesOnEditor(row, column, EditingGameboard.transform);
            EditingGameboard.gameObject.name = "GameBoard";
        }
        else
        {
            InputWarningText.gameObject.SetActive(true);
            InputWarningText.text = "Neither Column or Row can be zero.";
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
        LEditor_SelectedTileUI.Instance.UnAttach();

        foreach (LEditor_TileObject tile in EditingGameboard.OnEditorTiles)
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
        tempSavedGBoard.gameObject.SetActive(false);
        CancelButtonClick();
        LaunchedLevelEvents();
        currentState = state.testing;
        SetOnAndOffEditingUI();
        EditingGameboard.AddActiveTiles();
    }


    public void SetOnAndOffEditingUI()
    {
        if (BoardScaleAskerUI.gameObject.activeInHierarchy)
        {
            BoardScaleAskerUI.SetActive(false);
            EditorButtonUI.SetActive(false);
        }
        else
        {
            BoardScaleAskerUI.SetActive(true);
            EditorButtonUI.SetActive(true);
        }
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
            SetOnAndOffEditingUI();
            MainCamera.Instance.GetComponent<MainCamera>().enabled = true;
            currentState = state.editing;
        }
        else
        {
            SetOnAndOffEditingUI();
            currentEditingState = editingState.mapBuilding;
            allPickablesButton.onClick.RemoveAllListeners();
            certainPointButton.onClick.RemoveAllListeners();
            SettingLevelButtons.SetActive(false);
        }
    }

    public void OnSaveLevelClick()
    {
        if (currentEditingState == editingState.mapBuilding ||
            currentEditingState == editingState.settingConnection ||
            currentEditingState == editingState.settingPortals)
        {
            StartSettingLevel();
        }
        else
        {
            SaveGameBoardAsALevel();
        }
    }

    public void StartSettingLevel()
    {
        EndCurrentEditingEvent();
        LevelSetting level = EditingGameboard.levelSetting;
        //SetOnAndOffEditingUI();
        BoardScaleAskerUI.SetActive(false);
        EditorButtonUI.SetActive(false);
        SettingLevelButtons.SetActive(true);
        allPickablesButton.gameObject.SetActive(true);
        certainPointButton.gameObject.SetActive(true);
        returnToEditingButton.gameObject.SetActive(true);
        allPickablesButton.onClick.AddListener(level.StartChoosingPickables);
        certainPointButton.onClick.AddListener(level.StartChoosingtheTargetedTile);
    }

    public void SaveGameBoardAsALevel()
    {
        string levelPrefabPath = "Assets/Prefabs/GoodBoards/GoodBoards.prefab";

        if (LevelEditor.Instance.EditingGameboard.levelSetting.winningCondition == LevelSetting.levelClearCondition.getPickables)
        {
            Debug.Log(String.Format("Created level prefab:{0}  Winning Codition:{1}  Pickables Count:{2}"
            , EditingGameboard.name, (EditingGameboard.levelSetting.winningCondition).ToString(), EditingGameboard.levelSetting.neededPickables.Count));
        }

        if (EditingGameboard.levelSetting.winningCondition == LevelSetting.levelClearCondition.reachCertainTile && EditingGameboard.levelSetting.TileToReach != null)
        {
            Debug.Log(String.Format("Created level prefab:{0}  Winning Condition:{1}  The Targeted Tile ID:{2}"
                , EditingGameboard.name, (EditingGameboard.levelSetting.winningCondition).ToString(), EditingGameboard.levelSetting.TileToReach.GetComponent<LEditor_TileObject>().TileId));
        }
        else
        {
            Debug.Log("Fail to create level: no tile being selected");
        }
            
    }
}
