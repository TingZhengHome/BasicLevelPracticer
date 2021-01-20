using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingMachine : MonoBehaviour {

    [SerializeField]
    Player player;


    [SerializeField]
    public BoxCollider2D collide;

    public float colliderDefaultXSize;
    public float colliderDefaultYSize;
    

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void Initialize(Player player)
    {
        this.player = player;
        collide = GetComponent<BoxCollider2D>();

        colliderDefaultYSize = collide.size.y;
        colliderDefaultXSize = collide.size.x;
    }

    public void Slide(Vector3 startPosition, Vector3 slideEndPosition, float slidingSpeed)
    {
        Debug.Log("SlidingMachine is going to start sliding, with slidingSpeed: " + slidingSpeed + ".");

        float sqrRemainingDistance = (startPosition - slideEndPosition).magnitude;
        Vector3 direction = (slideEndPosition - transform.position).normalized;

        player.transform.position = Vector3.MoveTowards(transform.position, slideEndPosition, slidingSpeed * Time.deltaTime);

        if (sqrRemainingDistance <= 0)
        {
            player.StopSliding();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (player.isSliding)
        {
            if (collision != null && (collision.transform.GetComponent<GameBoardObject>() != null ))
            {
                if (collision.transform.GetComponent<GameBoardObject>().isHindrance || collision.transform.tag == "edge")
                {
                    Debug.Log("SlidingMachine sensed a hinderance: " + collision.transform.name);
                    player.StopSliding();
                }
            }
        }
    }
}
