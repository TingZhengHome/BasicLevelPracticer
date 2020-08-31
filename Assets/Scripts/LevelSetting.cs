using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum levelClearCondition { getPickables, reachCertainTile }
[System.Serializable]
public class LevelSetting
{
    //string levelName;

    //public string LevelName
    //{
    //    get
    //    {
    //        return levelName;
    //    }

    //    private set
    //    {
    //        levelName = value;
    //    }
    //}

    public levelClearCondition winningCondition;

    public List<OnTileObject> neededPickables = new List<OnTileObject>();

    public TileObject TargetTile;

    public void StartChoosingPickables()
    {
        winningCondition = levelClearCondition.getPickables;

        LevelEditor.Instance.currentEditingState = LevelEditor.editingState.settingWinningPickables;

        LEditor_UIManager.Instance.Mask.SetActive(false);
        LEditor_UIManager.Instance.EditorButtonUI.SetActive(false);
        LEditor_UIManager.Instance.allPickablesButton.gameObject.SetActive(false);
        LEditor_UIManager.Instance.certainPointButton.gameObject.SetActive(false);
    }


    public void StartChoosingtheTargetedTile()
    {
        winningCondition = levelClearCondition.reachCertainTile;

        LevelEditor.Instance.currentEditingState = LevelEditor.editingState.settingWinningTile;

        LEditor_UIManager.Instance.Mask.SetActive(false);
        LEditor_UIManager.Instance.EditorButtonUI.SetActive(false);
        LEditor_UIManager.Instance.allPickablesButton.gameObject.SetActive(false);
        LEditor_UIManager.Instance.certainPointButton.gameObject.SetActive(false);
    }

    public LevelSettingData Save()
    {
        LevelSettingData settingData = new LevelSettingData();

        if (winningCondition == levelClearCondition.getPickables)
        {
            if (neededPickables.Count > 0)
            {
                for (int i = 0; i < neededPickables.Count; i++)
                {
                    settingData.neededPickablesIds.Add(neededPickables[i].GetComponent<LEditor_OnTileObject>().theTileSetOn.TileId);
                }
            }
        }
        else if (winningCondition == levelClearCondition.reachCertainTile)
        {
            settingData.TargetedTileId = TargetTile.GetComponent<LEditor_TileObject>().TileId;
        }

        return settingData;
    }

    public void Load(LevelSettingData settingData)
    {
        winningCondition = settingData.winningCondition;

        if (winningCondition == levelClearCondition.getPickables)
        {
            for (int i = 0; i < settingData.neededPickablesIds.Count; i++)
            {
                neededPickables.Add(LevelEditor.Instance.EditingGameboard.GetEditingTile(settingData.neededPickablesIds[i]).objectOn.GetComponent<OnTileObject>());
            }
        }
        else if (winningCondition == levelClearCondition.reachCertainTile)
        {
            TargetTile = LevelEditor.Instance.EditingGameboard.GetEditingTile(settingData.TargetedTileId).GetComponent<TileObject>();
        }
    }
}
