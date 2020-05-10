using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField]
    float MovingTimeDuration = 1f;
    bool isLerping = false;

    float startedTime;

    BoxCollider2D collide;
    public Vector2 onGameColliderSize;

    SpriteRenderer spritrenderer;

    Vector2 startPosition;
    Vector2 endPosition;

    OnTileObject playerTaking;

    [SerializeField]
    float blockingSensingDistatnce = 0.4f;
    [SerializeField]
    float pickingUpDistance = 0.25f;
    [SerializeField]
    float egdeCheckDistance = 0.75f;

	// Use this for initialization
	void Start () {
        collide = GetComponent<BoxCollider2D>();
        spritrenderer = GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
        SortingLayerSystem.Instance.UpdateLayer(spritrenderer);
        int horizontal = 0;
        int vertical = 0;
        if (vertical == 0)
            horizontal = (int)Input.GetAxisRaw("Horizontal");
        if (horizontal == 0)
            vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            //if (!EdgeCollideCheck<GameBoardObject>(horizontal, vertical, 0.55f))
            //{
                StartLerping(horizontal, vertical);
            //}
            if (MoveCollideCheck<GameBoardObject>(horizontal, vertical, LevelManager.Instance.blockingLayer, blockingSensingDistatnce) != null)
            {
                GameBoardObject theObject = MoveCollideCheck<GameBoardObject>(horizontal, vertical, LevelManager.Instance.blockingLayer, 0.4f);
                Debug.Log("BlockingDetectedBehaviour");
                isLerping = false;
                if (theObject.GetComponent<OnTileObject>() != null)
                {
                    theObject.GetComponent<OnTileObject>().Interact(this, "push");
                }
            }
            else if (MoveCollideCheck<GameBoardObject>(horizontal, vertical, LevelManager.Instance.interactableObjectLayer, pickingUpDistance) != null)
            {
                GameBoardObject theObject = MoveCollideCheck<GameBoardObject>(horizontal, vertical, LevelManager.Instance.interactableObjectLayer, pickingUpDistance);
                Debug.Log("MeetingInteractableObject");
                if (theObject.GetComponent<OnTileObject>() != null)
                {
                    theObject.GetComponent<OnTileObject>().Interact(this, "interact");
                }
            }
        }
	}

    public void Initilize()
    {
        collide = GetComponent<BoxCollider2D>();
        collide.enabled = true;
        collide.size = onGameColliderSize;
        spritrenderer = GetComponent<SpriteRenderer>();
        transform.parent = LevelManager.Instance.currentGameBoard.transform;
    }


    GameBoardObject MoveCollideCheck<T>(float x, float y, LayerMask layer, float sensingDistance) where T : GameBoardObject
    {

        Vector2 RaycastEndPoint = startPosition + new Vector2(x * sensingDistance, y * sensingDistance);

        collide.enabled = false;
        RaycastHit2D hit = Physics2D.Linecast(startPosition, RaycastEndPoint, layer);
        collide.enabled = true;

        if (hit.transform != null)
        {
            Debug.Log("Player collide :"+ hit.transform.name);
            return hit.transform.GetComponent<GameBoardObject>();
        }
        else
        {
            return null;
        }
    }

    //bool EdgeCollideCheck<T>(float x, float y, float sensingDistance) where T : GameBoardObject
    //{

    //    Vector2 RaycastEndPoint = startPosition + new Vector2(x * sensingDistance, y * sensingDistance);

    //    collide.enabled = false;
    //    RaycastHit2D hit = Physics2D.Linecast(startPosition, RaycastEndPoint);
    //    collide.enabled = true;

    //    if (hit.transform != null)
    //    {

    //        return false;
    //    }
    //    else
    //    {
    //        Debug.Log("Player is colliding edge.");
    //        return true;
    //    }
    //}

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

    public void PickUpItem(OnTileObject pickable)
    {
        playerTaking = pickable;
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{

    //    switch (collision.gameObject.layer)
    //    {
    //        case 8:
    //            break;
    //    }
    //}
}
