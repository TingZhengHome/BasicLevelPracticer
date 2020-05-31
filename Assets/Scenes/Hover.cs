using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hover : Singleton<Hover> {

    SpriteRenderer spriteRenderer;
    [SerializeField]
    BoxCollider2D collide;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collide = GetComponent<BoxCollider2D>();
    }

    // Use this for initialization
    void Start() {
        Editor_TileObject.OnTileClicked += SetPlacedObjectRotation;
        spriteRenderer = GetComponent<SpriteRenderer>();
        collide = GetComponent<BoxCollider2D>();
    }


    void Update()
    {
        this.gameObject.SetActive(true);
        Edtior_GameBoardObject HovingObject = null;

        if (LevelEditor.Instance.clickedBoardObjectButton != null)
        {
            if (!LevelEditor.Instance.isMovingPlacedObject)
            {
                HovingObject = LevelEditor.Instance.clickedBoardObjectButton.representObject.GetComponent<Edtior_GameBoardObject>();
            }
            else
            {
                HovingObject = LevelEditor.Instance.movingObject;
            }
            spriteRenderer.sprite = LevelEditor.Instance.clickedBoardObjectButton.sprite;
            SetCollideSize(HovingObject.GetComponent<BoxCollider2D>());
        }
        else
        {
            HovingObject = null;
            spriteRenderer.sprite = null;
            collide.size = new Vector2(1f, 1f);
        }

        if (HovingObject != null)
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
        transform.position = GetWorldPositionOnPlane(Input.mousePosition, -1f);
    }


    public void SetPlacedObjectRotation(Edtior_GameBoardObject nObj, int id)
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

    public Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float z)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, z));
        float distance;
        xy.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }
}
