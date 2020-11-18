using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveManager : Singleton<SaveManager> {

    public CampaignData campaignData = new CampaignData();

    public void SaveGameBoardAsALevel(string inputtedText)
    {
        LevelEditor.Instance.EditingGameboard.Save(campaignData, inputtedText);
        if (campaignData.levelDatas.Count > 0)
            Debug.Log("EditingGameBoard has been saved as a level: " + campaignData.levelDatas[0].levelName + " in current campaign.");

        LevelEditor.Instance.EndSavingAndLoading();
    }

    public void SaveLevelsAsACampaign(string inputtedText)
    {
        campaignData.campaignName = inputtedText;

        SerializationManagger.Save(inputtedText, campaignData);

        string path = Application.persistentDataPath + "/saves/" + inputtedText + ".save";
        if (File.Exists(path))
            Debug.Log(string.Format("Levels has been saved as a campaign in {0}.", path));

        LevelEditor.Instance.EndSavingAndLoading();
    }

    public void LoadLevel(string saveName)
    {
        if (campaignData.levelDatas.Count > 0)
        {
            LevelData levelData = null;

            for (int i = 0; i < campaignData.levelDatas.Count; i++)
            {
                Debug.Log(string.Format("levelData{0} is going to be loaded", i));

                if (campaignData.levelDatas[i] != null)
                {
                    if (campaignData.levelDatas[i].levelName == saveName)
                    {
                        levelData = campaignData.levelDatas[i];
                        Debug.Log(string.Format("levelData{0} is loaded.", i));
                    }
                }
            }

            if (levelData != null)
            {
                if (LevelEditor.Instance.EditingGameboard != null)
                {
                    LevelEditor.Instance.EndCurrentEditingEvent();
                    Destroy(LevelEditor.Instance.EditingGameboard.gameObject);
                }

                LevelEditor.Instance.EditingGameboard = Instantiate(LevelEditor.Instance.boardsFactory.GetGameBoard(levelData.theme));
                LevelEditor.Instance.EditingGameboard.Load(levelData);
            }
            else
            {
                Debug.LogError("The selected level is not saved in the campaign.");
            }
        }
        else
        {
            Debug.LogError("Loaded campaign data stores zero level.");
        }

        LevelEditor.Instance.EndSavingAndLoading();
    }

    public void LoadCampaign(string saveName)
    {
        campaignData = (CampaignData)SerializationManagger.Load(Application.persistentDataPath + "/saves/" + saveName + ".save");

        if (campaignData == null)
        {
            Debug.Log("Failed to load campaign:" + string.Format(Application.persistentDataPath + "/saves/" + saveName + ".save") + ".");
        }
        else
        {
            Debug.Log("Campaign" + saveName + " is loaded.");
        }

         LevelEditor.Instance.EndSavingAndLoading();
    }


}
