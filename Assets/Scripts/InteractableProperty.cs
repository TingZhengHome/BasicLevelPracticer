using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableProperty : GameBoardObject {


    bool isOnTile;

    public bool IsOnTile
    {
        get
        {
            return isOnTile;
        }

        protected set
        {
            isOnTile = value;
        }
    }

    public bool initialized = false;

    private void Awake()
    {
        trigger = GetComponent<BoxCollider2D>();
        spriterenderer = GetComponent<SpriteRenderer>();
    }


    // Use this for initialization
    void Start() {
        trigger = GetComponent<BoxCollider2D>();
        spriterenderer = GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update() {

    }

    public abstract void GameUpdate();

    public virtual void Initialize(GameBoardObject gameBoardObject, InteractableObject interactableObject)
    {
        trigger = GetComponent<BoxCollider2D>();
        spriterenderer = GetComponent<SpriteRenderer>();

        IsOnTile = gameBoardObject.GetComponent<OnTileObject>();
        initialized = true;
    }

    public virtual void Interact(Player interacter)
    {
        Debug.Log("Interacted with " + gameObject.name +  ".");

    }
}
