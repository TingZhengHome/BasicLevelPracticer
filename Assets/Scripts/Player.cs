using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    float MovingTimeDuration = 1f;
    public bool isLerping = false;

    float startedTime;

    BoxCollider2D collide;
    public Vector2 onGameColliderSize;

    SpriteRenderer spritrenderer;

    Vector2 startPosition;
    Vector2 endPosition;

    List<Pickable> playerPickups = new List<Pickable>();

    Pickable playerTaking;

    [SerializeField]
    float sensingBlockingDistance = 0.4f;
    [SerializeField]
    float pickingUpDistance = 0.25f;
    [SerializeField]
    float egdeCheckDistance = 0.75f;

    public bool transported;

    // Use this for initialization
    void Start()
    {
        collide = GetComponent<BoxCollider2D>();
        spritrenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        SortingLayerSystem.Instance.UpdateLayer(spritrenderer);
        int horizontal = 0;
        int vertical = 0;
        if (vertical == 0)
            horizontal = (int)Input.GetAxisRaw("Horizontal");
        if (horizontal == 0)
            vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            StartLerping(horizontal, vertical);
            CheckCollideAndInteraction(horizontal, vertical);
        }
    }

    public void Initilize()
    {
        LevelManager.Instance.currentGameBoard.player = this;
        collide = GetComponent<BoxCollider2D>();
        collide.enabled = true;
        collide.size = onGameColliderSize;
        collide.isTrigger = false;
        spritrenderer = GetComponent<SpriteRenderer>();
        transform.parent = LevelManager.Instance.currentGameBoard.transform;
    }


    GameBoardObject MoveCollideCheck<T>(float x, float y, LayerMask layer, float sensingDistance) where T : GameBoardObject
    {
        Vector2 RaycastEndPoint = startPosition + new Vector2(x * sensingDistance, y * sensingDistance);

        BoxCollider2D standingTileTrigger = GetComponent<OnTileObject>().theTileSetOn.GetComponent<BoxCollider2D>();

        standingTileTrigger.enabled = false;
        collide.enabled = false;
        RaycastHit2D hit = Physics2D.Linecast(startPosition, RaycastEndPoint, layer);
        collide.enabled = true;
        standingTileTrigger.enabled = true ;

        if (hit.transform != null)
        {
            return hit.transform.GetComponent<GameBoardObject>();
        }
        else
        {
            return null;
        }
    }

    void CheckCollideAndInteraction(int horizontal, int vertical)
    {
        GameBoardObject collidingEgde = MoveCollideCheck<GameBoardObject>(horizontal, vertical, LevelManager.Instance.edgeLayer, sensingBlockingDistance);
        GameBoardObject collidingObject = MoveCollideCheck<GameBoardObject>(horizontal, vertical, LevelManager.Instance.gameBoardObjectLayer, sensingBlockingDistance);
        if (collidingEgde != null)
        {
            GameBoardObject theObject = MoveCollideCheck<GameBoardObject>(horizontal, vertical, LevelManager.Instance.edgeLayer, 0.4f);
            Debug.Log("BlockingDetectedBehaviour");
            isLerping = false;
            if (theObject != null && theObject.Property != null)   
            {
                theObject.GetComponent<OnTileObject>().Interact(this);
            }
        }
        else if (collidingObject != null)
        {
            //if (collidingObject.isHinderance)
            //{
            //    isLerping = false;
            //}
            collidingObject.Interact(this);
        }
    }

    void StartLerping(int x, int y)
    {
        isLerping = true;
        startedTime = Time.deltaTime;

        startPosition = transform.position;
        endPosition = startPosition + new Vector2(x, y);
    }

    private void FixedUpdate()
    {
        if (isLerping)
        {
            float timeSinceStarted = Time.deltaTime + startedTime;
            float completedMovingPercentage = timeSinceStarted / MovingTimeDuration;

            transform.position = Vector3.Lerp(startPosition, endPosition, completedMovingPercentage);

            if (completedMovingPercentage >= 1f)
            {
                isLerping = false;
            }
        }
    }

    public void PickUpItem(Pickable pickable)
    {
        playerTaking = pickable;
        playerPickups.Add(pickable);
    }
}
