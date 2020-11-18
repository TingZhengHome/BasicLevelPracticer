using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    float MovingTimeDuration = 1f;
    public bool isLerping = false;

    int horizontal, vertical;

    public int Vertical
    {
        get
        {
            return vertical;
        }

        private set

        {
            vertical = value;
        }
    }

    public int Horizontal
    {
        get
        {
            return horizontal;
        }

        private set
        {
            horizontal = value;
        }
    }

    float startedTime;

    BoxCollider2D collide;
    [SerializeField]
    Vector2 colliderBottomYOffset = new Vector2(0, -0.3f);
    [SerializeField]
    Vector2 colliderUpperOffset = new Vector2(0, 0.15f);
    [SerializeField]
    Vector2 colliderLeftXOffest = new Vector2(0.38f, 0);
    [SerializeField]
    Vector2 colliderRightXOffset = new Vector2(-0.38f, 0);

    [SerializeField]
    BoxCollider2D step;

    SpriteRenderer spritrenderer;

    Vector2 walkStartPosition;
    Vector2 endPosition;

    List<Pickable> playerPickups = new List<Pickable>();

    Pickable playerTaking;

    [SerializeField]
    float sensingBlockingDistance = 0.4f;
    [SerializeField]
    float pickingUpDistance = 0.25f;
    [SerializeField]
    float egdeCheckDistance = 0.75f;
    [SerializeField]
    float onSlidingSenseTileDistance = 0.20f;

    public bool transported;

    [SerializeField]
    RaycastChecker raycastchecker;

    [SerializeField]
    SlidingMachine slidingMachine;


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
        Horizontal = 0;
        Vertical = 0;
        if (Vertical == 0)
            Horizontal = (int)Input.GetAxisRaw("Horizontal");
        if (Horizontal == 0)
            Vertical = (int)Input.GetAxisRaw("Vertical");

        if (!isSliding)
        {
            if (Horizontal != 0 || Vertical != 0)
            {
                StartLerping(Horizontal, Vertical);
                CheckCollideAndInteraction(Horizontal, Vertical);
            }
        }

    }

    public void Initilize()
    {
        LevelManager.Instance.currentGameBoard.player = this;
        collide = GetComponent<BoxCollider2D>();
        collide.enabled = true;
        collide.isTrigger = false;
        spritrenderer = GetComponent<SpriteRenderer>();
        transform.parent = LevelManager.Instance.currentGameBoard.transform;
        raycastchecker = GetComponent<RaycastChecker>();

        if (slidingMachine == null)
        {
            slidingMachine = transform.Find("SlidingMachine").GetComponent<SlidingMachine>();
        }
        slidingMachine.Initialize(this);
    }
    

    void CheckCollideAndInteraction(int horizontal, int vertical)
    {
        OnTileObject pOntile = GetComponent<OnTileObject>();

        collide.enabled = false;
        pOntile.theTileSetOn.GetComponent<BoxCollider2D>().enabled = false;
        slidingMachine.collide.enabled = false;
        GameBoardObject collidingEgde = raycastchecker.StraightChecker<GameBoardObject>(horizontal, vertical, LevelManager.Instance.edgeLayer, sensingBlockingDistance, walkStartPosition);
        GameBoardObject collidingTile = raycastchecker.StraightChecker<GameBoardObject>(horizontal, vertical, LevelManager.Instance.tileLayer, sensingBlockingDistance, walkStartPosition + colliderBottomYOffset);
        GameBoardObject collidingOnTile = raycastchecker.StraightChecker<GameBoardObject>(horizontal, vertical, LevelManager.Instance.onTileLayer, sensingBlockingDistance, walkStartPosition);
        collide.enabled = true;
        pOntile.theTileSetOn.GetComponent<BoxCollider2D>().enabled = true;
        slidingMachine.collide.enabled = true;

        if (collidingEgde != null)
        {
            Debug.Log("BlockingDetectedBehaviour");
        }
        else if (collidingOnTile != null)
        {
            collidingOnTile.Interact(this);
        }
        else if (collidingTile != null)
        {
            collidingTile.Interact(this);
        }
    }

    void StartLerping(int x, int y)
    {
        isLerping = true;
        startedTime = Time.deltaTime;

        walkStartPosition = transform.position;
        endPosition = walkStartPosition + new Vector2(x, y);
    }


    public bool isSliding = false;
    public Vector3 slideStartPostion;
    public Vector3 slideEndPosition;
    float slidingSpeed;

    public void StartSliding(Vector3 targetPosition, float slidingSpeed)
    {
        isSliding = true;
        slideStartPostion = transform.position;
        slideEndPosition = targetPosition;
        this.slidingSpeed = slidingSpeed;
        Debug.Log("Sliding started");
    }

    private void FixedUpdate()
    {
        if (isLerping && !isSliding)
        {
            Lerp();
        }

        if (isSliding)
        {
            Mathf.Clamp(slidingSpeed, 0, 1.5f);
            Slide();
        }
    }

    void Lerp()
    {
        TileObject theTileOn = GetComponent<OnTileObject>().theTileSetOn;

        if (theTileOn != null)
        {
            if (theTileOn.theType == ObjectType.slippery)
            {
                theTileOn.GetComponent<Slippery>().SlideStoppedPlayer(this, Horizontal, Vertical);
            }
        }

        float timeSinceStarted = Time.deltaTime + startedTime;
        float completedMovingPercentage = timeSinceStarted / MovingTimeDuration;

        transform.position = Vector3.Lerp(walkStartPosition, endPosition, completedMovingPercentage);

        if (completedMovingPercentage >= 1f)
        {
            isLerping = false;
        }
    }

    void Slide()
    {
        isLerping = false;

        Vector3 direction = (slideEndPosition - transform.position).normalized;

        slidingMachine.Slide(transform.position, slideEndPosition, slidingSpeed);

        GameBoardObject steppingOn = raycastchecker.StraightChecker<TileObject>(direction.x, direction.y, LevelManager.Instance.tileLayer, onSlidingSenseTileDistance, transform.position);
        OnTileObject collidingOnTile = raycastchecker.StraightChecker<OnTileObject>(direction.x, direction.y, LevelManager.Instance.onTileLayer, onSlidingSenseTileDistance, transform.position) as OnTileObject;

        if (collidingOnTile != null && collidingOnTile!= null)
        {
            collidingOnTile.GetComponent<OnTileObject>().Interact(this);
        }

        if (steppingOn != null && steppingOn.GetComponent<TileObject>() != null)
        {

            if (steppingOn.GetComponent<Slippery>() == null)
            {
                steppingOn.GetComponent<TileObject>().Interact(this);
            }
        }
    }

    public void StopSliding()
    {
        isSliding = false;
        slideEndPosition = transform.position;
        slidingSpeed = 0;
    }

    public void PickUpItem(Pickable pickable)
    {
        playerTaking = pickable;
        playerPickups.Add(pickable);
    }

}
