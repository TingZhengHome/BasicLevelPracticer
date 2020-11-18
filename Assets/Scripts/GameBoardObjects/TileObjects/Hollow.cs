using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hollow : InteractableProperty
{

    bool isFilled;

    [SerializeField]
    GameObject fillingObject;
    public GameObject FillingObject
    {
        get
        {
            return fillingObject;
        }

        private set
        {
            fillingObject = value;
        }
    }

    [SerializeField]
    GameObject filledObject;

    // Use this for initialization
    void Start()
    {
        collide = GetComponent<BoxCollider2D>();
        collide.enabled = true;
        spriterenderer = GetComponent<SpriteRenderer>();
    }

    public override void Initialize(GameBoardObject gameBoardObject)
    {
        base.Initialize(gameBoardObject);

        if (filledObject == null)
        {
            filledObject = transform.Find("Filling").gameObject;
        }

        collide.isTrigger = false;
        isHindrance = true;
    }

    public override void GameUpdate()
    {
        if (GetComponent<TileObject>().objectOnThis != null && GetComponent<TileObject>().objectOnThis.name == fillingObject.name)
        {
            StartFilling(GetComponent<TileObject>().objectOnThis.GetComponent<Movable>());
        }
    }

    public override void Interact(GameBoardObject interacter)
    { 
        if (interacter.name == fillingObject.name)
        {
            collide.isTrigger = true;
            Debug.Log("Interacted.");
        }
    }

    public void StartFilling(Movable fillingObject)
    {
            Invoke("BeFilled", 1f);

            Destroy(fillingObject.gameObject, 0.7f);
    }

    public void BeFilled()
    {
        isFilled = true;
        filledObject.SetActive(true);
        filledObject.GetComponent<SpriteRenderer>().sortingOrder = (int)transform.position.y * -10 + 1;
        isHindrance = false;
        collide.enabled = true;
        collide.isTrigger = true;
    }
}
