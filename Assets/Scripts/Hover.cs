using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Hover : Singleton<Hover>
{

    [SerializeField]
    LEdtior_GameBoardObject HovingObject = null;
    [SerializeField]
    SpriteRenderer spriteRenderer;
    [SerializeField]
    BoxCollider2D collide;

    
    public LEditor_TileSelectedUI tileSelectedUI;
    [SerializeField]
    LEditor_TileSelectedUI tileSelectedUIPrefab;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collide = GetComponent<BoxCollider2D>();
    }

    // Use this for initialization
    void Start()
    {
        //Editor_TileObject.OnTileClicked += ClickEventCheck;
        DontDestroyOnLoad(this.gameObject);
        Instance.DeleteRedundancy();

        LEditor_TileContainer.OnTileClicked += SetPlacedObjectRotation;
        spriteRenderer = GetComponent<SpriteRenderer>();
        collide = GetComponent<BoxCollider2D>();
        this.gameObject.SetActive(true);
    }

    //public void CheckTileSelectedUI()
    //{
    //    if (tileSelectedUI != null)
    //    {
    //        DestroyImmediate(tileSelectedUI.gameObject);
    //    }

    //    tileSelectedUI = Instantiate(tileSelectedUIPrefab.gameObject).GetComponent<LEditor_TileSelectedUI>();
    //    tileSelectedUI.transform.SetParent(transform);
    //    if (GameManager.Instance.GetActiveScene().name == "LevelEditor")
    //    {
    //        LEditor_UIManager.Instance.TileSelectedUI = tileSelectedUI.gameObject;
    //    }
    //}


    void Update()
    {
        //CheckTileSelectedUI();
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("LevelEditor"))
        {
            if (GameManager.Instance.editCampaignPanel.loadablButtonsAreaController.draggingLoadableButton == null)
            {
                if (LevelEditor.Instance.clickedBoardObjectButton != null)
                {
                    StartHovering();
                }
                else if (LevelEditor.Instance.isMovingPlacedObject)
                {
                    StartGrabbing();
                }
                else
                {
                    CleanHover();
                }

                if (HovingObject != null)
                {
                    RotateHoveringObject();
                }
                transform.parent = null;
                transform.position = Vector3Extensions.GetWorldPositionOnPlane(new Vector3(), Input.mousePosition, 0f);
            }
            else
            {
                transform.SetParent(LEditor_UIManager.Instance.SaveLevelUI.transform);
                transform.position = Input.mousePosition;
            }
        }
        else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("MainMenu"))
        {
            transform.position = Input.mousePosition;
        }

    }


    public void ClickEventCheck(LEdtior_GameBoardObject nObj, int id)
    {
        if (LevelEditor.Instance.clickedBoardObjectButton != null)
        {
            StartHovering();
        }
        else if (LevelEditor.Instance.isMovingPlacedObject)
        {
            StartGrabbing();
        }
        else
        {
            CleanHover();
        }
    }

    public void StartHovering()
    {
        HovingObject = LevelEditor.Instance.clickedBoardObjectButton.representObject.GetComponent<LEdtior_GameBoardObject>();
        spriteRenderer.sprite = LevelEditor.Instance.clickedBoardObjectButton.sprite;
        SetCollideSize(HovingObject.GetComponent<BoxCollider2D>());
    }

    public void StartGrabbing()
    {
        HovingObject = LevelEditor.Instance.movingObject;
        spriteRenderer.sprite = LevelEditor.Instance.movingObject.GetComponent<SpriteRenderer>().sprite;
        SetCollideSize(HovingObject.GetComponent<BoxCollider2D>());
    }

    public void CleanHover()
    {
        HovingObject = null;
        spriteRenderer.sprite = null;
        collide.size = new Vector2(1f, 1f);
    }

    public void RotateHoveringObject()
    {
        if (HovingObject.tag != "player")
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                this.transform.Rotate(0, 0, -90f, Space.World);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                this.transform.Rotate(0, 0, 90f, Space.World);
            }
        }
    }

    public void SetPlacedObjectRotation(LEdtior_GameBoardObject nObj, int id)
    {
        if (nObj != null && LevelEditor.Instance.currentEditingState == LevelEditor.editingState.mapBuilding)
        {
            nObj.transform.rotation = this.transform.rotation;
        }
    }

    public void SetCollideSize(BoxCollider2D objectCollider)
    {
        collide.size = objectCollider.size;
    }

    public void RevertCollideSize(string currentScene)
    {
        switch (currentScene)
        {
            case "MainMenu":
                collide.size = new Vector2(0.2f, 0.2f);
                break;
            case "LevelEditor":
                collide.size = new Vector2(1f, 1f);
                break;
        }
    }

    public void SetInState(string sceneName)
    {
        switch (sceneName)
        {
            case "MainMenu":
                spriteRenderer.enabled = false;
                transform.SetParent(MainMenu.Instance.transform.parent);
                break;
            case "LevelEditor":
                spriteRenderer.enabled = true;
                LevelEditor.Instance.Hover = this.gameObject;
                //CheckTileSelectedUI();
                LEditor_UIManager.Instance.TileSelectedUI = tileSelectedUI.gameObject;
                break;
        }
    }

    private void OnDrawGizmos()
    {
        if (this != null)
        {
            Gizmos.color = new Color(1f, 1f, 0, 0.3f);
            Gizmos.DrawCube(transform.position, collide.size);
        }
    }

}
