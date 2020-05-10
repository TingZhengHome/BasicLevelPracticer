using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edtior_GameBoardObject : MonoBehaviour {

    public string ObjectName;

    //public Point gridPosition;

    public BoxCollider2D trigger;

    public SpriteRenderer spriteRender;


    private void Awake()
    {
        trigger = GetComponent<BoxCollider2D>();
        spriteRender = GetComponent<SpriteRenderer>();
    }

    // Use this for initialization
    public virtual void Start () {
        trigger = GetComponent<BoxCollider2D>();
        spriteRender = GetComponent<SpriteRenderer>();
    }
	

	// Update is called once per frame
	void Update () {
		
	}

    public virtual void Setup(Vector2 worldPosition, Transform parent)
    {
        trigger = GetComponent<BoxCollider2D>();
        spriteRender = GetComponent<SpriteRenderer>();
        //this.gridPosition = gridPosition;
        transform.position = worldPosition;
        transform.parent = parent;
        SetLayer(this.GetComponent<SpriteRenderer>());
    }

    void SetID(Vector2 worldPosition)
    {
        
    }

    public virtual void SetLayer(SpriteRenderer sprite)
    {
        sprite.sortingOrder = (int)transform.position.y * -10;
    }
}