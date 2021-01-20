using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelNameAskerPanel : MonoBehaviour
{

    [SerializeField]
    GameObject overwriteAsker;
    [SerializeField]
    Text overwriteAskerText;

    [SerializeField]
    InputField inputField;

    [SerializeField]
    Button addButton, saveButton;

    // Use this for initialization
    void Start()
    {
        //ShutDown();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void Show(string action)
    {
        ShutDown();
        gameObject.SetActive(true);

        if (action == "Add")
        {
            addButton.gameObject.SetActive(true);
        }
        else if (action == "SaveNew")
        {
            saveButton.gameObject.SetActive(true);
        }
    }

    public void CheckAddNewInput(InputField input)
    {
        Debug.Log("Add is clicked");
        string inputtingName = input.text;
        List<LevelData> levelDatas = SaveManager.Instance.loadedCampaign.levelDatas;
        if (SaveManager.Instance.loadedCampaign != null)
        {
            for (int i = 0; i < levelDatas.Count; i++)
            {
                if (inputtingName == levelDatas[i].levelName)
                {
                    OKMessagePanel.Instance.DisplayMessage(OKMessageLibrary.levelNameAlready);
                    Debug.Log("Level with same name already exists.");
                    return;
                }
                //else
                //{
                //    Debug.Log("Going to add a new level.");
                //    GameManager.Instance.editCampaignPanel.AddNewLevel(inputtingName);
                //    return;
                //}
            }
            //if (SaveManager.Instance.loadedCampaign.levelDatas.Count == 0)
            //{
                GameManager.Instance.editCampaignPanel.AddNewLevel(inputtingName);
            //}
        }
    }

    public void CheckSaveNewInput(InputField input)
    {
        Debug.Log("SaveAsNew is clicked");
        string inputtingName = input.text;
        List<LevelData> levelDatas = SaveManager.Instance.loadedCampaign.levelDatas;
        if (SaveManager.Instance.loadedCampaign != null)
        {
            for (int i = 0; i < levelDatas.Count; i++)
            {
                if (inputtingName == levelDatas[i].levelName)
                {
                    ShowOverwriteAsker(inputtingName);
                    return;
                }
            }
            //if (SaveManager.Instance.loadedCampaign.levelDatas.Count == 0)
            //{
                LevelEditor.Instance.SaveGameBoardAsAnotherLevel(inputtingName);
                ShutDown();
            //}
        }
    }

    public void OverwriteLevel(InputField input)
    {
        string inputtingName = input.text;
        LevelEditor.Instance.SaveGameBoardAsAnotherLevel(inputtingName);
        ShutDown();
    }

    public void ShowOverwriteAsker(string levelName)
    {
        overwriteAsker.SetActive(true);
        overwriteAskerText.text = string.Format("Level {0} already exists. Do you want to overwrite it?", levelName);
    }

    public void ShutDownOverwriteAsker()
    {
        overwriteAskerText.text = null;
        overwriteAsker.SetActive(false);
    }

    public void ShutDown()
    {
        inputField.text = null;
        ShutDownOverwriteAsker();
        addButton.gameObject.SetActive(false);
        saveButton.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
