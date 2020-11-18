using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LEdtior_GameBoardObject : MonoBehaviour {

    public int idInFactory;

    public string ObjectName;

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

    public virtual void Setup(Vector2 placedPositon, Transform parent)
    {
        trigger = GetComponent<BoxCollider2D>();
        spriteRender = GetComponent<SpriteRenderer>();
        transform.position = placedPositon;
        transform.parent = parent;
        SetSortingLayer(this.GetComponent<SpriteRenderer>());
    }

    public virtual void SetSortingLayer(SpriteRenderer sprite)
    {
        sprite.sortingOrder = (int)transform.position.y * -10;
    }

    public virtual void TurnColor(Color newColor)
    {
        spriteRender.color = newColor;
    }
}