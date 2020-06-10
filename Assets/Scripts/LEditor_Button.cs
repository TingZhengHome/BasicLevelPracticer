using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LEditor_Button : MonoBehaviour {

    public enum ButtonKind {tile, objectOnTile}

    public LEdtior_GameBoardObject representObject;
    public Sprite sprite;

    public virtual void ClickButton()
    {
        if (LevelEditor.Instance.movingObject == null && LevelEditor.Instance.selectedObject == null &&
            LevelEditor.Instance.currentEditingState == LevelEditor.editingState.mapBuilding)
        {
            LevelEditor.Instance.clickedBoardObjectButton = this;
            LevelEditor.Instance.isMovingPlacedObject = false;
        }
        else
        {
            return;
        }
    }

    public void CancelButton()
    {
        LevelEditor.Instance.clickedBoardObjectButton = null;
    }
}
