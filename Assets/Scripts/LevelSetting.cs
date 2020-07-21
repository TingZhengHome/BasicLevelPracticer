using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSetting{

    [SerializeField]
    string levelName;

    public enum levelClearCondition { getPickables, reachCertainTile}

    public levelClearCondition winningCondition;

    public List<OnTileObject> neededPickables = new List<OnTileObject>();

    public TileObject TileToReach;

    //public LevelSetting(levelClearCondition winningCondition, List<OnTileObject> pickablesToPass, TileObject tileToWin)
    //{
    //    levelName = "GameBoard";
    //    this.winningCondition = winningCondition;
    //    this.neededPickables = pickablesToPass;
    //    this.TileToReach = tileToWin;
    //}


    public void StartChoosingPickables()
    {
        winningCondition = levelClearCondition.getPickables;

        LevelEditor.Instance.currentEditingState = LevelEditor.editingState.settingWinningPickables;
        
        LevelEditor.Instance.EditorButtonUI.SetActive(false);
        LevelEditor.Instance.allPickablesButton.gameObject.SetActive(false);
        LevelEditor.Instance.certainPointButton.gameObject.SetActive(false);
    }


    public void StartChoosingtheTargetedTile()
    {
        winningCondition = levelClearCondition.reachCertainTile;

        LevelEditor.Instance.currentEditingState = LevelEditor.editingState.settingWinningTile;

        LevelEditor.Instance.EditorButtonUI.SetActive(false);
        LevelEditor.Instance.allPickablesButton.gameObject.SetActive(false);
        LevelEditor.Instance.certainPointButton.gameObject.SetActive(false);
    }
}
