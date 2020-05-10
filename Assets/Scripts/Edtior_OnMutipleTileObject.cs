using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edtior_OnMutipleTileObject : Editor_OnTileObject {

    public List<Editor_TileObject> theTilesSetOn = new List<Editor_TileObject>();

    public bool isHinderance;


    protected override void Update()
    {
        ColorControll();
    }


    public void ColorControll()
    {
        if (detected)
        {
            bool clear = true;
            for (int i = 0; i < theTilesSetOn.Count; i++)
            {
                if (theTilesSetOn[i].detected == true)
                {
                    clear = false;
                }
            }

            if (clear)
            {
                detected = false;
                spriteRender.color = LevelEditor.Instance.EditingGameboard.defaultColor;
            }
            else if (LevelEditor.Instance.clickedBoardObjectButton == null)
            {
                spriteRender.color = LevelEditor.Instance.EditingGameboard.placeableColor;
            }
            else if (LevelEditor.Instance.clickedBoardObjectButton != null)
            {
                spriteRender.color = LevelEditor.Instance.EditingGameboard.objectOverlappedColor;
            }
        }
    }

    public void Setup(Transform parent, List<Editor_TileObject> theTilesSetOn)
    {
        transform.position = PlacePosition(LevelEditor.Instance.EditingGameboard.DetectedTiles());
        transform.parent = parent;
        SetLayer(this.GetComponent<SpriteRenderer>());
        this.theTilesSetOn = theTilesSetOn;
        if (!isPlayer)
        {
            correspondingButton = GameObject.Find(string.Format(ObjectName + "Button")).GetComponent<Editor_Button>();
        }
        else
        {
            correspondingButton = GameObject.Find("PlayerButton").GetComponent<Editor_Button>();
        }
        detected = false;
    }

    Vector2 PlacePosition(List<Editor_TileObject> theTilesSetOn)
    {
        float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;
        for (int t = 0; t < theTilesSetOn.Count; t++)
        {
            if (maxY < theTilesSetOn[t].transform.position.y)
            {
                maxY = theTilesSetOn[t].transform.position.y;
            }
            if (minY > theTilesSetOn[t].transform.position.y)
            {
                minY = theTilesSetOn[t].transform.position.y;
            }
            if (maxX < theTilesSetOn[t].transform.position.x)
            {
                maxX = theTilesSetOn[t].transform.position.x;
            }
            if (minX > theTilesSetOn[t].transform.position.x)
            {
                minX = theTilesSetOn[t].transform.position.x;
            }
            if (theTilesSetOn[t].objectOn != null)
            {
                theTilesSetOn[t].objectOn.PickedUp(this, theTilesSetOn[t].tileId);
            }
            theTilesSetOn[t].objectOn = this;
        }
        return new Vector2((maxX + minX) * 0.5f, (maxY + minY) * 0.5f);
    }

    public override void PickedUp(Edtior_GameBoardObject newO, int id)
    {
        if (LevelEditor.Instance.clickedBoardObjectButton != null)
        {
            LevelEditor.Instance.CancelButtonClick();
        }
        this.correspondingButton.ClickButton();
        LevelEditor.Instance.movingPlacedObject = true;
        for (int i = 0; i < theTilesSetOn.Count; i++)
        {
            if (theTilesSetOn[i].objectOn != null)
            {
                theTilesSetOn[i].CleanTile();
            }
        }
        Hover.Instance.transform.rotation = this.transform.rotation;
        Editor_TileObject.OnTileClicked -= this.PickedUp;
        Destroy(this.gameObject);
    }
}
