using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveManager : Singleton<SaveManager>
{
    public CampaignData loadedCampaign;
    public LevelData loadedLevel;

    public void SaveGameBoardAsALevel(string inputtedText)
    {
        LevelEditor.Instance.EditingGameboard.Save(loadedCampaign, inputtedText);
        if (loadedCampaign.levelDatas.Count > 0)
            Debug.Log("EditingGameBoard has been saved as a level: " + loadedCampaign.levelDatas[0].levelName + " in current campaign.");

        LevelEditor.Instance.EndSavingAndLoading();
    }

    public void CreateNewCampaign(string inputtedText)
    {
        loadedCampaign = new CampaignData();

        loadedCampaign.campaignName = inputtedText;

        SerializationManagger.Save(inputtedText, loadedCampaign);

        string path = Application.persistentDataPath + "/saves/" + inputtedText + ".save";
        if (File.Exists(path))
            Debug.Log(string.Format("Levels has been saved as a campaign in {0}.", path));

        if (SceneManager.GetActiveScene().name == "LevelEditor")
        {
            LevelEditor.Instance.EndSavingAndLoading();
        }
    }

    public void UpgradeEditingCampaign()
    {
        if (loadedCampaign != null)
        {
            SerializationManagger.Save(loadedCampaign.campaignName, loadedCampaign);
        }

        string path = Application.persistentDataPath + "/saves/" + loadedCampaign.campaignName + ".save";
        if (File.Exists(path))
            Debug.Log(string.Format("Levels has been saved as a campaign in {0}.", path));

        if (SceneManager.GetActiveScene().name == "LevelEditor")
        {
            LevelEditor.Instance.EndSavingAndLoading();
        }
    }

    public void ModifyCampaignInfo1(string changedName, List<LevelData> newLevels)
    {
        CampaignData modifiedCampaignData = new CampaignData();
        modifiedCampaignData.campaignName = changedName;
        modifiedCampaignData.levelDatas = newLevels;
        modifiedCampaignData.theme = loadedCampaign.theme;

        SerializationManagger.Delete(loadedCampaign.campaignName);
        SerializationManagger.Save(changedName, modifiedCampaignData);
        loadedCampaign = modifiedCampaignData;
    }

    public void ModifyCampaignInfo2(string changedName)
    {
        CampaignData modifiedCampaignData = new CampaignData();
        modifiedCampaignData.campaignName = changedName;
        modifiedCampaignData.levelDatas = loadedCampaign.levelDatas;
        modifiedCampaignData.theme = loadedCampaign.theme;

        SerializationManagger.Save(changedName, modifiedCampaignData);
        if (File.Exists(Application.persistentDataPath + "/saves/" + changedName + ".save"))
        {
            SerializationManagger.Replace(changedName, loadedCampaign.campaignName);
        }
    }

    public void LoadLevel(string saveName)
    {
        if (loadedCampaign.levelDatas.Count > 0)
        {
            LevelData levelData = null;

            for (int i = 0; i < loadedCampaign.levelDatas.Count; i++)
            {
                Debug.Log(string.Format("levelData{0} is going to be loaded", i));

                if (loadedCampaign.levelDatas[i] != null)
                {
                    if (loadedCampaign.levelDatas[i].levelName == saveName)
                    {
                        levelData = loadedCampaign.levelDatas[i];
                        Debug.Log(string.Format("levelData{0} is loaded. It is a {1} x {2} level.", i, levelData.row, levelData.column));
                    }
                }
            }

            if (levelData != null)
            {
                LevelEditor.Instance.LoadLevel(levelData);
                loadedLevel = levelData;
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

        if (SceneManager.GetActiveScene().name == "LevelEditor")
        {
            LevelEditor.Instance.EndSavingAndLoading();
        }
    }

    public void LoadCampaign(string saveName)
    {
        loadedCampaign = (CampaignData)SerializationManagger.Load(Application.persistentDataPath + "/saves/" + saveName + ".save");
        loadedLevel = null;

        if (loadedCampaign == null)
        {
            Debug.Log("Failed to load campaign:" + string.Format(Application.persistentDataPath + "/saves/" + saveName + ".save") + ".");
        }
        else
        {
            Debug.Log("Campaign" + saveName + " is loaded.");
        }
        if (SceneManager.GetActiveScene().name == "LevelEditor")
        {
            LevelEditor.Instance.EndSavingAndLoading();
        }
    }



}
