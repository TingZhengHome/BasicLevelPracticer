using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LEdtior_OnMutipleTileObject : LEditor_OnTileObject
{

    public List<LEditor_TileObject> theTilesSetOn = new List<LEditor_TileObject>();

    public override void GameUpdate()
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

    public void Setup(Transform parent, List<LEditor_TileObject> theTilesSetOn)
    {
        transform.position = GetCenterPosition(theTilesSetOn);
        
        SetSortingLayer(this.GetComponent<SpriteRenderer>());
        this.theTilesSetOn = theTilesSetOn;
        theTileSetOn = theTilesSetOn[theTilesSetOn.Count - 1];
        transform.parent = theTilesSetOn[theTilesSetOn.Count-1].transform;
        correspondingButton = GameObject.Find("PlayerButton").GetComponent<LEditor_Button>();
        idInFactory = LevelEditor.Instance.EditingGameboard.factory.GetOnTileFactoryId(this);

        detected = false;
    }

    Vector2 GetCenterPosition(List<LEditor_TileObject> theTilesSetOn)
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
                theTilesSetOn[t].objectOn.BePickUp(this, theTilesSetOn[t].TileId);
            }
            theTilesSetOn[t].objectOn = this;
            Debug.Log(string.Format("A OnMutipleTileObject is set on tile{0}", theTilesSetOn[t].TileId));
        }
        return new Vector2((maxX + minX) * 0.5f, (maxY + minY) * 0.5f);
    }

    public override void BePickUp(LEdtior_GameBoardObject newO, int id)
    {
        if (LevelEditor.Instance.clickedBoardObjectButton != null)
        {
            LevelEditor.Instance.CancelButtonClick();
        }
        LevelEditor.Instance.StartMovingObject(this);
        for (int i = 0; i < theTilesSetOn.Count; i++)
        {
            if (theTilesSetOn[i].objectOn == this)
            {
                theTilesSetOn[i].CleanTile(true);
            }
        }
        Hover.Instance.transform.rotation = this.transform.rotation;
        LEditor_TileContainer.OnTileClicked -= this.BePickUp;
    }

    public override OnTileData Save()
    {
        OnTileData data = base.Save();
        for (int i = 0; i < theTilesSetOn.Count; i++)
        {
            if (theTilesSetOn[i] != null)
            {
                data.idsOnBoard.Add(theTilesSetOn[i].TileId);
                Debug.Log(string.Format("the Tile id:{0} set on a OnMutipleTileObject is saved.", theTilesSetOn[i].TileId));
            }
        }
        data.idOnBoard = -2;

        return data;
    }

    public override void Load(OnTileData data)
    {
        base.Load(data);
    }

}
