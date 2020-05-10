using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerButton : Editor_Button {

    public override void ClickButton()
    {
        Debug.Log("ButtonClicked");
            LevelEditor.Instance.clickedBoardObjectButton = this;
            LevelEditor.Instance.Hover.SetActive(true);
    }
}
