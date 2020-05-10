using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Editor_OnTileObject : Edtior_GameBoardObject {

    public Editor_TileObject theTileSetOn;

    private int size;

    public Editor_Button correspondingButton; 

    public bool pushable;
    public bool pickable;
    public bool isPlayer;
    
    public bool detected;   

    public int Size
    {
        get
        {
            if (size != null)
                return size;
            else
                return (int)(trigger.size.x * trigger.size.y);
        }
           
        private set
        {
             size = value;
        }
    }

    protected virtual void Update()
    {
        if (theTileSetOn != null && theTileSetOn.detected == false)
        {
            spriteRender.color = LevelEditor.Instance.EditingGameboard.defaultColor;
            detected = false;
        }
    }

    public override void Setup(Vector2 worldPosition, Transform parent)
    {
        //this.gridPosition = gridPosition;
        //transform.rotation = Hover.Instance.transform.rotation;
        transform.position = worldPosition;
        transform.parent = parent;
        SetLayer(this.GetComponent<SpriteRenderer>());
        theTileSetOn = parent.GetComponent<Editor_TileObject>();
        if (!isPlayer)
        {
            correspondingButton = GameObject.Find(string.Format(ObjectName + "Button")).GetComponent<Editor_Button>();
            if (correspondingButton == null)
            {
                Debug.LogWarning(this.name + "can't find its corresponding button.");
            }
        }
        else
        {
            correspondingButton = GameObject.Find("PlayerButton").GetComponent<Editor_Button>();
            if (correspondingButton == null)
            {
                Debug.LogWarning(this.name + "can't find its corresponding button.");
            }
        }
        trigger = GetComponent<BoxCollider2D>() ;
        trigger.enabled = false;
        spriteRender = this.GetComponent<SpriteRenderer>();
    }

    public virtual void PickedUp(Edtior_GameBoardObject newO, int id)
    {
        if (newO != null)
        {
            LevelEditor.Instance.clickedBoardObjectButton.CancelButton();
        }
        if (isPlayer)
        {
            LevelEditor.Instance.isPlayerPlaced = false;
        }
        this.correspondingButton.ClickButton();
        LevelEditor.Instance.movingPlacedObject = true;
        theTileSetOn.CleanTile();
        Editor_TileObject.OnTileClicked -= this.PickedUp;
        Hover.Instance.transform.rotation = this.transform.rotation;
    }

    public void TurnColor(Color color)
    {
        spriteRender.color = color;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "hover")
        {
            TurnColor(Color.white);
        }
    }
}
