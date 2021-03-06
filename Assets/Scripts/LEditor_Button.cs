﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LEditor_Button : MonoBehaviour {

    public enum ButtonKind {tile, objectOnTile}

    public Edtior_GameBoardObject representObject;
    public Sprite sprite;

    public virtual void ClickButton()
    {
        LevelEditor.Instance.clickedBoardObjectButton = this;
        //LevelEditor.Instance.Hover.SetActive(true);
        LevelEditor.Instance.isMovingPlacedObject = false;
    }

    public void CancelButton()
    {
        LevelEditor.Instance.clickedBoardObjectButton = null;
        //LevelEditor.Instance.Hover.SetActive(false);
    }
}
