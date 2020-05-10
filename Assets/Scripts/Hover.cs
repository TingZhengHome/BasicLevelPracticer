using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hover : Singleton<Hover> {

    SpriteRenderer spriteRenderer;
    [SerializeField]
    BoxCollider2D collide;


    //public event Action<GameBoardObjectOnEditor> onClickedTileEvent;

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
            HovingObject = LevelEditor.Instance.clickedBoardObjectButton.representObject.GetComponent<Edtior_GameBoardObject>();
            spriteRenderer.sprite = HovingObject.GetComponent<SpriteRenderer>().sprite;
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
        //DetectAndPlaceGameBoardObject();
    }


    public void SetPlacedObjectRotation(Edtior_GameBoardObject nObj, int id)
    {
        if (nObj != null)
        {
            nObj.transform.rotation = this.transform.rotation;
        }
        if (LevelEditor.Instance.movingPlacedObject)
        {
            transform.rotation = Quaternion.identity;
        }
    }


    //private void FixedUpdate()
    //{
    //    this.gameObject.SetActive(true);
    //    GameBoardObjectOnEditor representingObject = null;

    //    if (LevelEditor.Instance.clickedBoardObjectButton != null)
    //    {
    //        representingObject = LevelEditor.Instance.clickedBoardObjectButton.representObject.GetComponent<GameBoardObjectOnEditor>();
    //        spriteRenderer.sprite = representingObject.GetComponent<SpriteRenderer>().sprite;
    //        SetCollideSize(representingObject.GetComponent<BoxCollider2D>());
    //    }
    //    else
    //    {
    //        representingObject = null;
    //        spriteRenderer.sprite = null;
    //        collide.size = new Vector2(1f, 1f);
    //    }

    //    if (representingObject != null)
    //    {
    //        if (Input.GetKeyDown(KeyCode.Q))
    //        {
    //            this.transform.Rotate(0, 0, -90f, Space.World);
    //        }
    //        else if (Input.GetKeyDown(KeyCode.E))
    //        {
    //            this.transform.Rotate(0, 0, 90f, Space.World);
    //        }
    //        if (Input.GetMouseButtonDown(0))
    //        {
    //            PlaceSingleGameBoardObject(representingObject);
    //        }
    //    }
    //    transform.position = GetWorldPositionOnPlane(Input.mousePosition, -1f);
    //}


    //public void ClickTileEvent()
    //{
    //      ("Clicked");
    //    GameBoardObjectOnEditor newO = null;
    //    if (LevelEditor.Instance.clickedBoardObjectButton != null)
    //    {
    //        newO = Instantiate(LevelEditor.Instance.clickedBoardObjectButton.representObject);
    //    }

    //    if (onClickedTileEvent != null)
    //    {
    //        if (newO != null)
    //        {
    //            Debug.Log(newO.name + "instantiated.");
    //            newO.transform.rotation = this.transform.rotation;
    //            newO.name = LevelEditor.Instance.clickedBoardObjectButton.representObject.name;
    //            onClickedTileEvent(newO);
    //            Debug.Log("And we get Here");
    //        }
    //        else
    //        {
    //            onClickedTileEvent(newO);
    //        }
    //    }
    //    //onClickedTileEvent = null;
    //}

    //public void DetectAndPlaceGameBoardObject()
    //{
    //    Collider2D[] overlapeds = Physics2D.OverlapBoxAll(transform.position, collide.size, 0f, LevelManager.Instance.emptyTileLayer);
    //    List<TileOnEditor> placeableTiles = new List<TileOnEditor>();
    //    GameBoardObjectOnEditor representingObject = LevelEditor.Instance.clickedBoardObjectButton.representObject.GetComponent<GameBoardObjectOnEditor>();

    //    int represetingObjectSize = 0;
    //    if (representingObject != null)
    //    {
    //        representingObject.trigger = representingObject.GetComponent<BoxCollider2D>();
    //        represetingObjectSize = (int)(representingObject.trigger.size.x * representingObject.trigger.size.y);
    //    }

    //    if (overlapeds != null)
    //    {
    //        for (int i = 0; i < overlapeds.Length; i++)
    //        {
    //            TileOnEditor overlapingTile = null;
    //            if (overlapeds[i] != null)
    //            {
    //                if (Mathf.Abs(overlapeds[i].transform.position.y - transform.position.y) > collide.size.y * 1 / 2 ||
    //                    Mathf.Abs(overlapeds[i].transform.position.x - transform.position.x) > collide.size.x * 1 / 2)
    //                {
    //                    continue;
    //                }
    //                overlapingTile = overlapeds[i].GetComponent<TileOnEditor>();
    //            }

    //            if (!EventSystem.current.IsPointerOverGameObject() && overlapingTile != null && representingObject != null)
    //            {
    //                if (represetingObjectSize <= 1)
    //                {
    //                    if (!overlapingTile.isPlaceable)
    //                    {
    //                        overlapingTile.TurnColor(notPlaceableColor);
    //                        continue;
    //                    }
    //                    else if (overlapingTile.objectOn != null)
    //                    {
    //                        overlapingTile.TurnColor(overlapingObjectsColor);
    //                    }
    //                    else
    //                    {
    //                        overlapingTile.TurnColor(placeableColor);
    //                    }

    //                    if (Input.GetMouseButtonDown(0))
    //                    {
    //                        GameBoardObjectOnEditor newObject = Instantiate(representingObject);
    //                        newObject.transform.rotation = this.transform.rotation;
    //                        newObject.name = representingObject.name;
    //                        if (newObject.tag != "tile")
    //                        {
    //                            overlapingTile.PlaceTileObject((OnTileObjectOnEditor)newObject);
    //                        }
    //                        else
    //                        {
    //                            LevelEditor.Instance.EditingGameboard.PlaceTile((TileOnEditor)newObject, overlapingTile);
    //                        }
    //                    }
    //                    else if (LevelEditor.Instance.MouseHoldThanDelay())
    //                    {
    //                        GameBoardObjectOnEditor newObject = Instantiate(representingObject);
    //                        newObject.transform.rotation = this.transform.rotation;
    //                        newObject.name = representingObject.name;
    //                        if (newObject.tag != "tile")
    //                        {
    //                            overlapingTile.PlaceTileObject((OnTileObjectOnEditor)newObject);
    //                        }
    //                        else
    //                        {
    //                            LevelEditor.Instance.EditingGameboard.PlaceTile((TileOnEditor)newObject, overlapingTile);
    //                        }
    //                    }
    //                }
    //                else
    //                {
    //                    if (!overlapingTile.isPlaceable)
    //                    {
    //                        overlapingTile.TurnColor(notPlaceableColor);
    //                        continue;
    //                    }
    //                    else if (overlapingTile.objectOn != null)
    //                    {
    //                        overlapingTile.TurnColor(overlapingObjectsColor);
    //                        placeableTiles.Add(overlapingTile);
    //                    }
    //                    else
    //                    {
    //                        overlapingTile.TurnColor(placeableColor);
    //                        placeableTiles.Add(overlapingTile);
    //                    }

    //                    if (Input.GetMouseButtonDown(0))
    //                    {
    //                        if (placeableTiles.Count == represetingObjectSize)
    //                        {
    //                            int OverlapingTileObjectCount = 0;
    //                            OnTileObjectOnEditor temp = null;

    //                            for (int t = 0; t < placeableTiles.Count; t++)
    //                            {
    //                                if (placeableTiles[t].objectOn != null)
    //                                {
    //                                    if (temp == null)
    //                                    {
    //                                        temp = placeableTiles[t].objectOn;
    //                                        OverlapingTileObjectCount += 1;
    //                                    }
    //                                    else if (placeableTiles[t].objectOn != temp)
    //                                    {
    //                                        OverlapingTileObjectCount += 1;
    //                                        temp = placeableTiles[t].objectOn;
    //                                    }
    //                              }
    //                            }

    //                            Debug.Log("OverlapingTileObjectCount:"+ OverlapingTileObjectCount);
    //                            if (OverlapingTileObjectCount <= 1)
    //                            {
    //                                OnMutipleTileObjectOnEditor newOntiles = Instantiate(representingObject) as OnMutipleTileObjectOnEditor;
    //                                if (newOntiles != null)
    //                                    Debug.Log("We get here.");
    //                                foreach (TileOnEditor tile in placeableTiles)
    //                                {
    //                                    tile.PlaceTileObject(newOntiles);
    //                                }
    //                                if (OverlapingTileObjectCount == 0 && LevelEditor.Instance.movingPlacedObject)
    //                                {
    //                                    LevelEditor.Instance.movingPlacedObject = false;
    //                                    LevelEditor.Instance.CancelButtonClick();
    //                                }

    //                                if (newOntiles != null)
    //                                    Debug.Log("We get here2.");
    //                                newOntiles.Setup(placeableTiles[0].transform, placeableTiles);
    //                            }
    //                            else
    //                            {
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //            else if (!EventSystem.current.IsPointerOverGameObject() && overlapingTile != null && representingObject == null)
    //            {
    //                if (overlapingTile.objectOn != null)
    //                {
    //                    Debug.Log("We can pick");
    //                    if (Input.GetMouseButtonDown(0))
    //                    {
    //                        overlapingTile.objectOn.PickedUp();
    //                        Debug.Log("Picked.");
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}

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
