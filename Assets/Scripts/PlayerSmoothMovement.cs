using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSmoothMovement : MonoBehaviour {

    [SerializeField]
    float movTime = 0.1f;
    public LayerMask blockingLayer;

    SpriteRenderer spritrenderer;

    [SerializeField]
    float movePeriord = 0.5f;

    BoxCollider2D collider2d;
    Rigidbody2D rigi2d;
    private float inverseMoveTime;

    bool moving;
    float timeStartedMoving;

    // Use this for initialization
    void Start () {
        collider2d = GetComponent<BoxCollider2D>();
        rigi2d = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / movTime;
        spritrenderer = GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update () {

        int horizontal = 0;
        int vertical = 0;
        if (!moving)
        {
            horizontal = (int)Input.GetAxisRaw("Horizontal");
            vertical = (int)Input.GetAxisRaw("Vertical");
        }

        SortingLayerSystem.Instance.UpdateLayer(spritrenderer);

        if (horizontal != 0 || vertical != 0)
        {
            AttemptToMove<GameBoardObject>(horizontal, vertical);
        }
	}

    public void Initilize()
    {
        collider2d = GetComponent<BoxCollider2D>();
        collider2d.enabled = true;
        rigi2d = GetComponent<Rigidbody2D>();
        spritrenderer = GetComponent<SpriteRenderer>();
        transform.parent = LevelManager.Instance.currentGameBoard.transform;
    }


    bool Move(int x, int y, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(x, y);

        collider2d.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        collider2d.enabled = true;

        if (hit.transform == null)
        {
            StartCoroutine(SmoothMovement(end));
            return true;
        }
        else
        {
            return false;
        }
    }

    void AttemptToMove<GameBoardObject>(int x, int y)
    {
        RaycastHit2D hit;
        bool canMove = Move(x, y, out hit);

        if (hit.transform == null)
        {
            return;
        }

        GameBoardObject hitObject = hit.transform.GetComponent<GameBoardObject>();

        if (!canMove && hitObject != null)
        {

        }
    }


    IEnumerator SmoothMovement(Vector3 end)
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
        if (!moving)
        {
            moving = true;
            while (sqrRemainingDistance > float.Epsilon)
            {
                Vector3 newPosition = Vector3.MoveTowards(rigi2d.position, end, inverseMoveTime * Time.deltaTime);
                Debug.Log(string.Format("sqrRemainingDistance:{0}", sqrRemainingDistance));
                rigi2d.MovePosition(newPosition);
                sqrRemainingDistance = (transform.position - end).sqrMagnitude;
                yield return null;
            }
            moving = false;
        }
    }



}
