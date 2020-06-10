using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditor : Singleton<LevelEditor> {

    public enum state { editing, testing };

    public enum editingState { mapBuilding, settingConnection, settingPortals }

    public state currentState = state.editing;
    public editingState currentEditingState = editingState.mapBuilding;
    public LEditor_SelectableObject selectedObject;
    public LEditor_OnTileObject movingObject;

    [SerializeField]
    GameObject EditorButtonUI;
    [SerializeField]
    GameObject BoardScaleAskerUI;

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

    public bool isPlayerPlaced;
    public bool isMovingPlacedObject;

    [SerializeField]
    float continousPlacingDelay = 0.2f;
    float continousPlacingCounter;

    public LayerMask hoverLayer;

    public delegate void LaunchedLevelEvent();
    public static event LaunchedLevelEvent LaunchedLevel;

    void Start () {
        BoardScaleAskerUI.SetActive(true);
        isPlayerPlaced = false;
    }
	
	// Update is called once per frame
	void Update () {
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

        if (inputNum > 0 )
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
        else if (isMovingPlacedObject)
        {
            EndMovingObject();
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

        foreach (LEditor_TileObject tile in EditingGameboard.tiles)
        {
            tile.TurnColor(EditingGameboard.defaultColor);
        }
    }

    public void LaunchLevel()
    {
        tempSavedGBoard = Instantiate(EditingGameboard);
        tempSavedGBoard.gameObject.SetActive(false);
        CancelButtonClick();
        LaunchedLevel();
        currentState = state.testing;
        SetOnAndOffEditingUI();
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
        LevelManager.Instance.ShutDownLevel();
        tempSavedGBoard.gameObject.SetActive(true);
        EditingGameboard = tempSavedGBoard;
        SetOnAndOffEditingUI();
        LEditor_Camera.Instance.GetComponent<LEditor_Camera>().enabled = true;
        EditingGameboard.gameObject.name = "GameBoard";
        currentState = state.editing;
    }
}
