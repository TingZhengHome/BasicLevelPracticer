using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastChecker : MonoBehaviour {

    [SerializeField]
    Player player;

    [SerializeField]
    BoxCollider2D collide;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void Initilize()
    {
        if (player != null)
        {
            collide = player.GetComponent<BoxCollider2D>();
        }
    }

    public GameBoardObject StraightChecker<T>(float x, float y, LayerMask layer, float sensingDistance, Vector2 startPosition) where T : GameBoardObject
    {
        Vector2 raycastEndPoint = startPosition + new Vector2(x * sensingDistance, y * sensingDistance);

        //Vector2 bottomStartPoint = new Vector2(startPosition.x, startPosition.y - 0.3f);
        //Vector2 bottomEndPoint = raycastEndPoint - new Vector2(0, 0.3f);

        //Vector2 rightEndPoint = raycastEndPoint + colliderRightXOffset;
        //Vector2 leftEndPoint = raycastEndPoint + colliderLeftXOffest;


        BoxCollider2D standingTileTrigger = player.GetComponent<OnTileObject>().theTileSetOn.GetComponent<BoxCollider2D>();

        standingTileTrigger.enabled = false;
        collide.enabled = false;
        RaycastHit2D hit = Physics2D.Linecast(startPosition, raycastEndPoint, layer);
        //RaycastHit2D bottomHit = Physics2D.Linecast(bottomStartPoint, bottomEndPoint, layer);
        //RaycastHit2D rightHit = Physics2D.Linecast(startPosition, rightEndPoint, layer);
        //RaycastHit2D leftHit = Physics2D.Linecast(startPosition, leftEndPoint, layer);
        collide.enabled = true;
        standingTileTrigger.enabled = true;

        if (hit.transform != null)
        {
            Debug.DrawLine(startPosition, raycastEndPoint, Color.blue);
            return hit.transform.GetComponent<T>();
        }
        else
        {
            return null;
        }
    }
}
