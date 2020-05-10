using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditor : Singleton<LevelEditor> {

    public enum state {editing, testing};

    public state currentState = state.editing;

    [SerializeField]
    GameObject EditorButtonUI;
    [SerializeField]
    GameObject BoardScaleAskerUI;


    [SerializeField]
    public GameBoard gameBoard;
    public GameBoard EditingGameboard;
    GameBoard tempSavedGBoard;

    private int column = 0, row = 0;

    [SerializeField]
    Text InputWarningText;

    public Editor_Button clickedBoardObjectButton;
    public GameObject Hover;

    public bool isPlayerPlaced;
    public bool movingPlacedObject;

    [SerializeField]
    float continousPlacingDelay = 0.2f;
    float continousPlacingCounter;

    public LayerMask hoverLayer;

    public delegate void LaunchedLevelEvent();
    public event LaunchedLevelEvent LaunchedLevel;

    void Start () {
        BoardScaleAskerUI.SetActive(true);
        isPlayerPlaced = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (currentState == state.editing)
        {
            if (Input.GetKeyDown(KeyCode.Escape) && clickedBoardObjectButton != null)
            {
                if (clickedBoardObjectButton.gameObject.name != "PlayerButton")
                {
                    CancelButtonClick();
                }
            }
            if (Input.GetMouseButton(0) && !movingPlacedObject)
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


    public void ClickButton(Editor_Button clicked)
    {
        Instance.clickedBoardObjectButton = clicked;
        Instance.movingPlacedObject = false;
    }

    public void CancelButtonClick()
    {
        clickedBoardObjectButton = null;
        movingPlacedObject = false;
    }

    public void LaunchLevel()
    {
        tempSavedGBoard = Instantiate(EditingGameboard);
        tempSavedGBoard.gameObject.SetActive(false);
        CancelButtonClick();
        //LevelManager.Instance.LaunchLevel();
        //EditorCamera.Instance.transform.position = EditorCamera.Instance.startCameraPosition;
        //EditorCamera.Instance.GetComponent<EditorCamera>().enabled = false;
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
        Editor_Camera.Instance.GetComponent<Editor_Camera>().enabled = true;
        EditingGameboard.gameObject.name = "GameBoard";
        currentState = state.editing;
    }
}
