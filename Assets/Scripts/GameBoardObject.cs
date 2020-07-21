
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum condition { connectable, portable, pickable, movable, normal, edge}

public abstract class GameBoardObject : MonoBehaviour {

    public bool isHinderance;

    protected SpriteRenderer spriterenderer;
    protected BoxCollider2D trigger;

    protected InteractableProperty property;

    public InteractableProperty Property
    {
        get
        {
            if (property != null)
            {
                return property;
            }
            else
            {
                return null;
            }
        }

        protected set
        {
            property = value;
        }
    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual void Initilize(GameBoardObject gameBoardObject)
    {


    }

    public virtual void Interact(Player interacter)
    {
    }

}
