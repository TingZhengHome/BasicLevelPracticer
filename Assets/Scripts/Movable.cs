using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movable : InteractableProperty {

    TileObject theTileSetOn;
    
    int squarePerMove;
    float moveSpeed;
    bool isConditioned;

    bool beingMoved;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    public override void GameUpdate()
    {
    }

    public override void Initialize(GameBoardObject basic, InteractableObject interactableO)
    {
        base.Initialize(basic, interactableO);

        theTileSetOn = basic.GetComponent<OnTileObject>().theTileSetOn;

        MovableObject movableObject = interactableO as MovableObject;
        squarePerMove = movableObject.squarePerMove;
        moveSpeed = movableObject.moveSpeed;
        isConditioned = movableObject.isConditioned;

        trigger.enabled = true;
        trigger.isTrigger = false;
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
                //interacter.isLerping = false;
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
        GameBoardObject collidingEgde = MoveCollideCheck<GameBoardObject>(transform.position, x, y, LevelManager.Instance.edgeLayer, 0.65f);
        GameBoardObject collidingObject = MoveCollideCheck<GameBoardObject>(transform.position, x, y, LevelManager.Instance.gameBoardObjectLayer, 0.65f);

        if (collidingObject != null)
        Debug.Log(gameObject.name + " is colliding with " + collidingObject.name);
        if (collidingEgde == null)
        {
            theTileSetOn.GetComponent<BoxCollider2D>().enabled = false;
            if (theTileSetOn.objectOnThis != null && theTileSetOn.objectOnThis != this.GetComponent<OnTileObject>())
            {
                theTileSetOn.objectOnThis.GetComponent<BoxCollider2D>().enabled = false;
            }

            if (collidingObject != null && !collidingObject.isHinderance)
            {
                if (collidingObject.GetComponent<TileObject>() != null)
                {
                    SetNewTile(collidingObject as TileObject);
                }
                else
                {
                    SetNewTile(collidingObject.GetComponent<OnTileObject>().theTileSetOn as TileObject);
                }
                
                while (sqrRemainingDistance > 0)
                {
                    beingMoved = true;
                    transform.position = Vector2.MoveTowards(transform.position, end, moveSpeed * Time.deltaTime);
                    sqrRemainingDistance = (transform.position - end).sqrMagnitude;
                    yield return null;
                }
                beingMoved = false;
            }
            else if (collidingObject != null) 
            {
                Debug.Log("Pushed " + name + " found hinderance.");
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
            Debug.Log(this.name + "will hit " + hit.transform.name + ".");
            return hit.transform.GetComponent<GameBoardObject>();
        }
        else
        {
            return null;
        }
    }

    void SetNewTile(TileObject newTile)
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
