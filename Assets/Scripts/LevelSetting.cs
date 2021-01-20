using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum levelClearCondition { getPickables, reachCertainTile }
[System.Serializable]
public class LevelSetting
{

    public levelClearCondition winningCondition;

    public List<OnTileObject> neededPickables = new List<OnTileObject>();

    public TileObject winningTile;

    public int winningTileId = -1;

    public void StartChoosingPickables()
    {
        winningCondition = levelClearCondition.getPickables;

        winningTile = null;

        LevelEditor.Instance.currentEditingState = LevelEditor.editingState.settingWinningPickables;

        LEditor_UIManager.Instance.Mask.SetActive(false);
        LEditor_UIManager.Instance.EditorButtonUI.SetActive(false);
        LEditor_UIManager.Instance.allPickablesButton.gameObject.SetActive(false);
        LEditor_UIManager.Instance.certainPointButton.gameObject.SetActive(false);
    }


    public void StartChoosingtheTargetedTile()
    {
        winningCondition = levelClearCondition.reachCertainTile;

        neededPickables = new List<OnTileObject>();

        LevelEditor.Instance.currentEditingState = LevelEditor.editingState.settingWinningTile;

        LEditor_UIManager.Instance.Mask.SetActive(false);
        LEditor_UIManager.Instance.EditorButtonUI.SetActive(false);
        LEditor_UIManager.Instance.allPickablesButton.gameObject.SetActive(false);
        LEditor_UIManager.Instance.certainPointButton.gameObject.SetActive(false);
    }

    public LevelSettingData Save()
    {
        LevelSettingData settingData = new LevelSettingData();

        settingData.winningCondition = winningCondition;

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
            settingData.TargetedTileId = winningTileId;
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
            winningTile = LevelEditor.Instance.EditingGameboard.GetEditingTile(settingData.TargetedTileId).GetComponent<TileObject>();
            winningTileId = settingData.TargetedTileId;
        }
    }
}
