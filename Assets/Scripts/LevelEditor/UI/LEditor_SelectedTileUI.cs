﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LEditor_TileSelectedUI : MonoBehaviour
{
    private static LEditor_TileSelectedUI instance;

    [SerializeField]
    LEditor_SelectableObject attachedObject;

    [SerializeField]
    Button cancelButton;

    [SerializeField]
    Button setConnectionButton;

    [SerializeField]
    Button pickUpButton;

    [SerializeField]
    List<Button> buttons = new List<Button>();

    public static LEditor_TileSelectedUI Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LEditor_TileSelectedUI>();
            }
            return instance;
        }
    }

    private void Awake()
    {

    }

    private void Start()
    {
        Debug.Log("SelectedTileUI starts.");
        PackUpButtons();
        UnAttach();
        LEditor_TileContainer.OnTileClicked += CheckClickAndAttachTo;

        LevelEditor.LaunchedLevelEvents += UnAttach;
        LevelEditor.ReturnToEditingEvents += UnAttach;
    }


    private void Update()
    {
        StateCheck();
    }


    void SetupAndDisplay()
    {
        if (attachedObject != null)
        {
            cancelButton.gameObject.SetActive(true);
            cancelButton.onClick.AddListener(attachedObject.UnSelectThis);

            if (attachedObject.GetComponent<LEditor_ConnectableObject>() != null ||
                attachedObject.GetComponent<LEditor_PortalObject>() != null)
            {
                setConnectionButton.gameObject.SetActive(true);
            }
            if (attachedObject.GetComponent<LEditor_OnTileObject>() != null)
            {
                LEditor_OnTileObject onTile = attachedObject.GetComponent<LEditor_OnTileObject>();
                pickUpButton.gameObject.SetActive(true);
                pickUpButton.onClick.AddListener(onTile.theTileSetOn.PickUpObjectOnThis);
                pickUpButton.onClick.AddListener(UnAttach);
            }
        }
    }


    public void CheckClickAndAttachTo(LEdtior_GameBoardObject newO, int clickedId)
    {
        if (LevelEditor.Instance.currentEditingState == LevelEditor.editingState.mapBuilding &&
            LevelEditor.Instance.currentState == LevelEditor.state.editing)
        {
            
            if (newO == null)
            {
                LEditor_TileObject clicked = LevelEditor.Instance.EditingGameboard.GetEditingTile(clickedId).GetComponent<LEditor_TileObject>();
                Debug.Log("SelectedUI get the clickedTile" + clicked.TileId);

                if (clicked.GetComponent<LEditor_SelectableObject>() != null &&
                    attachedObject != clicked.GetComponent<LEditor_SelectableObject>())
                {
                    UnAttach();
                    attachedObject = clicked.GetComponent<LEditor_SelectableObject>();
                    transform.position = attachedObject.transform.position;
                    transform.parent = attachedObject.transform;
                    SetupAndDisplay();
                    Debug.Log(clicked.name + clicked.TileId + " should be attached.");
                }
                else if (clicked.objectOn != null && clicked.objectOn.GetComponent<LEditor_SelectableObject>() != null &&
                         attachedObject != clicked.objectOn.GetComponent<LEditor_SelectableObject>())
                {
                    UnAttach();
                    attachedObject = clicked.objectOn.GetComponent<LEditor_SelectableObject>();
                    transform.position = attachedObject.transform.position;
                    transform.parent = attachedObject.transform;
                    SetupAndDisplay();
                    Debug.Log(clicked.name + clicked.TileId + " should be attached.");
                }
                else
                {
                    if (attachedObject != null)
                    {
                        Debug.Log("Unattach from tile" + clicked.TileId + ".");
                    }
                    UnAttach();
                }
            }
        }
    }

    public void PackUpButtons()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<Button>() != null)
            {
                buttons.Add(transform.GetChild(i).GetComponent<Button>());
            }
        }
    }

    public void UnAttach()
    {
        setConnectionButton.onClick.RemoveAllListeners();
        setConnectionButton.gameObject.SetActive(false);

        pickUpButton.onClick.RemoveAllListeners();
        pickUpButton.gameObject.SetActive(false);

        if (attachedObject != null)
        {
            cancelButton.onClick.RemoveListener(attachedObject.UnSelectThis);
        }
        attachedObject = null;
        transform.parent = Hover.Instance.transform;
        cancelButton.gameObject.SetActive(false);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }


    void StateCheck()
    {
        if (LevelEditor.Instance.currentEditingState == LevelEditor.editingState.settingConnection ||
            LevelEditor.Instance.currentEditingState == LevelEditor.editingState.settingPortals)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                if (buttons[i] != cancelButton)
                {
                    buttons[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void StartSettingConnection()
    {
        if (LevelEditor.Instance.clickedBoardObjectButton == null)
        {
            if (attachedObject.GetComponent<LEditor_ConnectableObject>() != null)
            {
                LevelEditor.Instance.currentEditingState = LevelEditor.editingState.settingConnection;
            }
            else if (attachedObject.GetComponent<LEditor_PortalObject>() != null)
            {
                LevelEditor.Instance.currentEditingState = LevelEditor.editingState.settingPortals;
            }
        }
    }
}
