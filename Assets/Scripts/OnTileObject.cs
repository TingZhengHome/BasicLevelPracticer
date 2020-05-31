using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTileObject : GameBoardObject {


    Tile theTileSetOn;

    public bool movable;
    [SerializeField]
    float movedSpeed;
    bool beingMoved = false;

    public bool isHinderance;

    public bool pickable;
    public bool isPlayer;


    public bool connectable;
    [SerializeField]
    GameBoardObject[] connectingObjects;
    bool turnOn;


    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (movable || pickable)
        {
            SortingLayerSystem.Instance.UpdateLayer(this.GetComponent<SpriteRenderer>());
        }
    }

    public void Initialize(LEditor_OnTileObject onTileOb)
    {
        theTileSetOn = onTileOb.theTileSetOn.GetComponent<Tile>();
        movable = onTileOb.isPushable;
        pickable = onTileOb.isPickable;
    }

    public void Interact(Player interacter, string action)
    {
        Debug.Log(this.name + "interacted.");

        switch (action)
        {
            case "push":
                if (this.movable)
                {
                    Debug.Log("Pushable object.");
                    Vector3 Direction = (new Vector3(
                        transform.position.x - interacter.transform.position.x,
                        transform.position.y - interacter.transform.position.y,
                        transform.position.z
                        )).normalized;

                    if (Direction != Vector3.zero)
                    {
                        if (!beingMoved)
                        {
                            StartCoroutine(Pushed(Direction.x, Direction.y));
                        }
                    }
                }
                break;
            case "interact":
                if (this.pickable)
                {
                    Debug.Log("Player picked up " + this.name);
                    PickedUp(interacter);
                }
                break;
        }
    }


    public IEnumerator Pushed(float x, float y)
    {
        Vector3 end = transform.position;
        if (Mathf.Abs(x) >= Mathf.Abs(y))
        {
            end = new Vector2(transform.position.x + x/Mathf.Abs(x), transform.position.y);
        }
        else if (Mathf.Abs(x) < Mathf.Abs(y))
        {
            end = new Vector2(transform.position.x, transform.position.y + y / Mathf.Abs(y));
        }
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
        if (!MoveCollideCheck<GameBoardObject>(transform.position, x, y, LevelManager.Instance.blockingLayer, 0.65f) &&
            !MoveCollideCheck<GameBoardObject>(transform.position, x, y, LevelManager.Instance.interactableObjectLayer, 0.65f))
        {
            theTileSetOn.GetComponent<BoxCollider2D>().enabled = false;
            SetNewTile(MoveCollideCheck<Tile>(transform.position, x, y, LevelManager.Instance.emptyTileLayer, 0.65f) as Tile);
            while (sqrRemainingDistance > 0)
            {
                beingMoved = true;
                transform.position = Vector2.MoveTowards(transform.position, end, movedSpeed * Time.deltaTime);
                sqrRemainingDistance = (transform.position - end).sqrMagnitude;
                yield return null;
            }
           
            beingMoved = false;
        }
        else
        {
            Debug.Log("Hinderance found");
        }
    }

    GameBoardObject MoveCollideCheck<T>(Vector2 startPosition, float x, float y, LayerMask layer, float sensingDistance) where T : GameBoardObject
    {
        this.GetComponent<BoxCollider2D>().enabled = false;
        Vector2 RaycastEndPoint = startPosition + new Vector2(x * sensingDistance, y * sensingDistance);
        RaycastHit2D hit = Physics2D.Linecast(startPosition, RaycastEndPoint, layer);
        this.GetComponent<BoxCollider2D>().enabled = true;

        if (hit.transform != null)
        {
            Debug.Log(this.name + "will hit " + hit.transform.name + ".");
            return hit.transform.GetComponent<GameBoardObject>();
        }
        else
        {
            return null;
        }
    }

    void PickedUp(Player interacter)
    {
        interacter.PickUpItem(this);
        theTileSetOn.objectOnThis = null;
        Destroy(this.gameObject);
    }

    public void TurnOnAndOff()
    {
            for (int i = 0; i < connectingObjects.Length; i++)
            {
                if (connectingObjects[i].GetComponent<Tile>() != null && connectingObjects[i].GetComponent<Tile>().button)
                {
                    Tile button = connectingObjects[i].GetComponent<Tile>();
                    if (!button.pressed)
                    {
                        return;
                    }
                }
            }
        turnOn = true;
            Debug.Log("Turn On");
    }


    void SetNewTile(Tile newTile)
    {
        if (newTile != null)
        {
            theTileSetOn.GetComponent<BoxCollider2D>().enabled = true;
            theTileSetOn.objectOnThis = null;
            theTileSetOn = newTile;
            theTileSetOn.SetObjectOn(this);
            transform.SetParent(theTileSetOn.transform);
        }
    }
}

