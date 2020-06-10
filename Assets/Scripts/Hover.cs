using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hover : Singleton<Hover>
{

    [SerializeField]
    LEdtior_GameBoardObject HovingObject = null;
    [SerializeField]
    SpriteRenderer spriteRenderer;
    [SerializeField]
    BoxCollider2D collide;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collide = GetComponent<BoxCollider2D>();
    }

    // Use this for initialization
    void Start()
    {
        //Editor_TileObject.OnTileClicked += ClickEventCheck;
        LEditor_TileContainer.OnTileClicked += SetPlacedObjectRotation;
        spriteRenderer = GetComponent<SpriteRenderer>();
        collide = GetComponent<BoxCollider2D>();
        this.gameObject.SetActive(true);
    }


    void Update()
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
        transform.position = Vector3Extensions.GetWorldPositionOnPlane(new Vector3(), Input.mousePosition, -1f);
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

    void SetCollideSize(BoxCollider2D objectCollider)
    {
        collide.size = objectCollider.size;
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
