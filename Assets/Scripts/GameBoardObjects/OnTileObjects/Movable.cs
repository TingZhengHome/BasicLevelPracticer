using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movable : InteractableProperty
{

    TileObject theTileSetOn;

    [SerializeField]
    int squarePerMove;
    [SerializeField]
    float moveSpeed;
    [SerializeField]
    bool isConditioned;

    bool beingMoved;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    public override void GameUpdate()
    {
    }

    public override void Initialize(GameBoardObject basic)
    {
        base.Initialize(basic);

        theTileSetOn = basic.GetComponent<OnTileObject>().theTileSetOn;

        collide.enabled = true;
        collide.isTrigger = false;
    }

    public override void Interact(Player interacter)
    {
        Debug.Log("Interacted with Pushable object:" + gameObject.name + theTileSetOn.TileId + ".");
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

    public IEnumerator Pushed(float x, float y)
    {
        Vector3 end = transform.position;
        if (Mathf.Abs(x) >= Mathf.Abs(y))
        {
            end = new Vector2(transform.position.x + (x / Mathf.Abs(x)) * squarePerMove, transform.position.y);
        }
        else if (Mathf.Abs(x) < Mathf.Abs(y))
        {
            end = new Vector2(transform.position.x, transform.position.y + (y / Mathf.Abs(y)) * squarePerMove);
        }
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
        GameBoardObject collidingEgde = MoveCollideCheck<GameBoardObject>(transform.position, x, y, LevelManager.Instance.edgeLayer, 0.7f);
        GameBoardObject collidingTile = MoveCollideCheck<GameBoardObject>(transform.position, x, y, LevelManager.Instance.tileLayer, 0.7f);
        GameBoardObject collidingOnTile = MoveCollideCheck<GameBoardObject>(transform.position, x, y, LevelManager.Instance.onTileLayer, 0.7f);

        if (collidingTile != null)
            Debug.Log(gameObject.name + " is colliding with " + collidingTile.name);
        if (collidingEgde == null)
        {
            theTileSetOn.GetComponent<BoxCollider2D>().enabled = false;
            if (theTileSetOn.objectOnThis != null && theTileSetOn.objectOnThis != this.GetComponent<OnTileObject>())
            {
                theTileSetOn.objectOnThis.GetComponent<BoxCollider2D>().enabled = false;
            }

            if (collidingTile != null)
            {
                if (collidingTile.GetComponent<TileObject>() != null && collidingTile.isHindrance)
                {
                    Debug.Log(name + " is going to interact with " + collidingTile.name);                   
                    collidingTile.GetComponent<TileObject>().Interact(GetComponent<GameBoardObject>());
                }

                if (collidingTile.GetComponent<BoxCollider2D>().isTrigger)
                {
                    if (collidingTile.GetComponent<TileObject>() != null)
                    {
                        GetOnNewTile(collidingTile as TileObject);
                    }

                    while (sqrRemainingDistance > 0)
                    {
                        beingMoved = true;
                        transform.position = Vector2.MoveTowards(transform.position, end, moveSpeed * Time.deltaTime);
                        sqrRemainingDistance = (transform.position - end).sqrMagnitude;
                        yield return null;
                    }
                    beingMoved = false;
                    if (collidingTile.GetComponent<Hollow>() != null && collidingTile.GetComponent<Hollow>().FillingObject.name == this.name)
                    {
                        //trigger.isTrigger = true;
                    }
                }
            }
            if (collidingOnTile != null)
            {
                if (collidingOnTile.GetComponent<OnTileObject>() != null)
                {
                    OnTileObject cOT = collidingOnTile.GetComponent<OnTileObject>();

                    if (collidingOnTile.isHindrance)
                    {
                        Debug.Log(name + " is going to interact with " + collidingOnTile.name);
                        cOT.GetComponent<OnTileObject>().Interact(GetComponent<GameBoardObject>());
                    }

                    if (collidingOnTile.GetComponent<BoxCollider2D>().isTrigger)
                    {
                        Debug.Log(name + " is going to interact with " + collidingOnTile.name);
                        cOT.GetComponent<OnTileObject>().Interact(GetComponent<GameBoardObject>());

                        GetOnNewTile(cOT.theTileSetOn);

                        while (sqrRemainingDistance > 0)
                        {
                            beingMoved = true;
                            transform.position = Vector2.MoveTowards(transform.position, end, moveSpeed * Time.deltaTime);
                            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
                            yield return null;
                        }
                        beingMoved = false;
                    }
                }
                
            }
        }
        else
        {
            Debug.Log("Edge found.");
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
            Debug.Log(this.name + " will hit " + hit.transform.name + ".");
            return hit.transform.GetComponent<GameBoardObject>();
        }
        else
        {
            return null;
        }
    }

    void GetOnNewTile(TileObject newTile)
    {
        if (newTile != null)
        {
            theTileSetOn.GetComponent<BoxCollider2D>().enabled = true;
            if (theTileSetOn.objectOnThis != null && theTileSetOn.objectOnThis == this.GetComponent<OnTileObject>())
            {
                theTileSetOn.objectOnThis = null;
            }
            else
            {
                theTileSetOn.objectOnThis.GetComponent<BoxCollider2D>().enabled = true;
            }

            theTileSetOn = newTile;

            if (newTile.objectOnThis != null)
            {
                if (newTile.objectOnThis.GetComponent<Connectable>() != null &&
                    !newTile.objectOnThis.GetComponent<Connectable>().isButton)
                {

                }
                else
                {
                    theTileSetOn.SetObjectOn(this.GetComponent<OnTileObject>());
                }
            }
            else
            {
                theTileSetOn.SetObjectOn(this.GetComponent<OnTileObject>());
            }
            transform.SetParent(theTileSetOn.transform);
        }
    }
}
