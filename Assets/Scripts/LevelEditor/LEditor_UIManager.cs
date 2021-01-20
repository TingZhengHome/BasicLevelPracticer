using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LEditor_UIManager : Singleton<LEditor_UIManager> {

    public EditCampaignPanel editCampaignPanel;
    string[] saveFilesNames;

    [SerializeField]
    public GameObject SaveLevelUI;
    [SerializeField]
    public GameObject EditorButtonUI;
    
    public GameObject BoardScaleAskerUI;

    [SerializeField]
    Text campaignNameText, levelNameText;

    public GameObject SettingLevelUI;
    public GameObject Mask;
    public Button startSetLevelButton;
    public Button endSettingButton;
    public Button allPickablesButton;
    public Button certainPointButton;
    public Button returnToEditingButton;
    public Button saveOnSettingLevelButton;
    public Button saveNewOnSettingLevelButoon;

    public Button saveLevelButton;

    public LoadableButton selectedLoadableButton;


    public GameObject TileSelectedUI;

    [SerializeField]
    public LEditor_OnTileObjectButton pickUpButton;

    [SerializeField]
    public Text ScaleInputWarningText;

    // Use this for initialization
    void Start()
    {
        BoardScaleAskerUI.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            if (editCampaignPanel.gameObject.activeSelf)
            {
                editCampaignPanel.ShutDown();
            }

            //if (saveNameAsker.activeSelf)
            //{
            //    saveNameAsker.SetActive(false);
            //}
        }

        if (SaveManager.Instance.loadedCampaign != null)
        {
            campaignNameText.text = "Campaign Name: " + SaveManager.Instance.loadedCampaign.campaignName;
        }
        if (LevelEditor.Instance.EditingGameboard != null)
        {
            levelNameText.text = "Level Name: " + LevelEditor.Instance.EditingGameboard.levelName;
        }
        ButtonOnSettingLevelControl();
    }

    ////[SerializeField]
    ////GameObject saveNameAsker;
    ////public Text saveNameAskerText;
    ////[SerializeField]
    ////InputField askerInputField;
    //const string levelSavingText = "Type the level name:";
    //const string campaignSavingText = "Type the campaign name:";
    //public string LevelSavingText
    //{
    //    get
    //    {
    //        return levelSavingText;
    //    }
    //}

    //public string CampaignSavingText
    //{
    //    get
    //    {
    //        return campaignSavingText;
    //    }
    //}

    //[SerializeField]
    //Button saveButtonOnAsker;
    //public void ShowSaveNameAsker(string textToDisplay)
    //{
    //    LevelEditor.Instance.EndCurrentEditingEvent();
    //    LevelEditor.Instance.EndSavingAndLoading();
    //    saveButtonOnAsker.onClick.RemoveAllListeners();

    //    if (textToDisplay == LevelSavingText)
    //    {
    //        if (LevelEditor.Instance.EditingGameboard != null)
    //        {
    //            askerInputField.text = LevelEditor.Instance.EditingGameboard.levelName;
    //            Debug.Log("EditingGameBoard's Name is " + LevelEditor.Instance.EditingGameboard.levelName);
    //            Debug.Log("inputtedText = " + askerInputField.text);
    //        }

    //        saveButtonOnAsker.onClick.AddListener(() => { CheckInputtedName("level", askerInputField.text); });
    //    }
    //    else if (textToDisplay == CampaignSavingText)
    //    {
    //        if (SaveManager.Instance.loadedCampaign != null)
    //        {
    //            askerInputField.text = SaveManager.Instance.loadedCampaign.campaignName;
    //            Debug.Log("Campaign's Name is " + SaveManager.Instance.loadedCampaign.campaignName);
    //            Debug.Log("inputtedText = " + askerInputField.text);
    //        }

    //        saveButtonOnAsker.onClick.AddListener(() => { CheckInputtedName("campaign", askerInputField.text); });
    //    }
    //    saveNameAsker.SetActive(true);
    //    saveNameAskerText.text = textToDisplay;
    //    Mask.SetActive(true);
    //}

    //public void CheckInputtedName(string levelOrCampaign, string inputtedText)
    //{
    //    if (levelOrCampaign == "level")
    //    {
    //        for (int i = 0; i < SaveManager.Instance.loadedCampaign.levelDatas.Count; i++)
    //        {
    //            LevelData level = SaveManager.Instance.loadedCampaign.levelDatas[i];
    //            if (level.levelName == inputtedText)
    //            {
    //                ShowOverwriteAsker(levelOrCampaign, inputtedText);
    //                return;
    //            }
    //        }
    //        SaveManager.Instance.SaveGameBoardAsALevel(inputtedText);
    //    }
    //    if (levelOrCampaign == "campaign")
    //    {
    //        string path = Application.persistentDataPath + "/saves";
    //        foreach (string fileName in Directory.GetFiles(path))
    //        {
    //            if (fileName == inputtedText)
    //            {
    //                ShowOverwriteAsker(levelOrCampaign, inputtedText);
    //                return;
    //            }
    //        }
    //        SaveManager.Instance.CreateNewCampaign(inputtedText);
    //    }
    //}

    //[SerializeField]
    //GameObject overwriteAskerUI;
    //[SerializeField]
    //Text overwriteAskerText;
    //[SerializeField]
    //Button yesButton, noButton;

    //public void ShowOverwriteAsker(string levelOrCampaign, string inputtedText)
    //{
    //    overwriteAskerUI.SetActive(true);

    //    if (levelOrCampaign == "level")
    //    {
    //        overwriteAskerText.text = string.Format("There is a level's name same with the inputted name: {0}.\r\n Do you want to overwrite it?", inputtedText);
    //        yesButton.onClick.AddListener(() => { SaveManager.Instance.SaveGameBoardAsALevel(inputtedText); });
    //        noButton.onClick.AddListener(() => { ShowSaveNameAsker(LevelSavingText); });
    //    }

    //    if (levelOrCampaign == "campaign")
    //    {
    //        overwriteAskerText.text = string.Format("There is a campaign's name same with the inputted name: {0}.\r\n Do you want to overwrite it?", inputtedText);
    //        yesButton.onClick.AddListener(() => { SaveManager.Instance.CreateNewCampaign(inputtedText); });
    //        noButton.onClick.AddListener(() => { ShowSaveNameAsker(CampaignSavingText); });
    //    }
    //}
    
    public void ShowEditCampaignPanel()
    {
        saveFilesNames = null;
        GetLoadFilesName("levels");
        editCampaignPanel.gameObject.SetActive(true);
        editCampaignPanel.Show(saveFilesNames, SaveManager.Instance.loadedCampaign.campaignName);
        Mask.SetActive(true);
    }

    public void GetLoadFilesName(string type)
    {
        if (!Directory.Exists(Application.persistentDataPath + "/saves"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves");
        }

        if (type == "campaigns")
        {
            saveFilesNames = Directory.GetFiles(Application.persistentDataPath + "/saves");
        }
        if (type == "levels")
        {
            if (SaveManager.Instance.loadedCampaign != null)
            {
                if (SaveManager.Instance.loadedCampaign.levelDatas.Count > 0)
                {
                    saveFilesNames = new string[SaveManager.Instance.loadedCampaign.levelDatas.Count];
                    for (int i = 0; i < SaveManager.Instance.loadedCampaign.levelDatas.Count; i++)
                    {
                        saveFilesNames[i] = SaveManager.Instance.loadedCampaign.levelDatas[i].levelName;
                    }
                }
            }
            else
            {
                Debug.Log("There is no level to load because there is no loaded campaign.");
            }
        }
    }
    [SerializeField]
    GameObject SaveButtons;

    public void ShowEditingUI()
    {
        BoardScaleAskerUI.SetActive(true);
        EditorButtonUI.SetActive(true);
        SaveButtons.SetActive(true);
    }

    public void ShutDownEditingUI()
    {
        BoardScaleAskerUI.SetActive(false);
        EditorButtonUI.SetActive(false);
        SaveButtons.SetActive(false);
        editCampaignPanel.gameObject.SetActive(false);
    }

    public void ShowSettingLevelUI()
    {
        SettingLevelUI.SetActive(true);
        allPickablesButton.gameObject.SetActive(true);
        certainPointButton.gameObject.SetActive(true);
        allPickablesButton.onClick.AddListener(LevelEditor.Instance.EditingGameboard.levelSetting.StartChoosingPickables);
        certainPointButton.onClick.AddListener(LevelEditor.Instance.EditingGameboard.levelSetting.StartChoosingtheTargetedTile);
        returnToEditingButton.gameObject.SetActive(true);
        
        Mask.SetActive(true);
        ShutDownEditingUI();
    }

    public void ShutDowSettingLevelUI()
    {
        SettingLevelUI.SetActive(false);
        allPickablesButton.gameObject.SetActive(false);
        certainPointButton.gameObject.SetActive(false);
        returnToEditingButton.gameObject.SetActive(false);
        allPickablesButton.onClick.RemoveAllListeners();
        certainPointButton.onClick.RemoveAllListeners();
        Mask.SetActive(false);
        ShowEditingUI();
    }

    public void ButtonOnSettingLevelControl()
    {
        if (LevelEditor.Instance.currentEditingState == LevelEditor.editingState.settingWinningPickables ||
            LevelEditor.Instance.currentEditingState == LevelEditor.editingState.settingWinningTile)
        {
            if (LevelEditor.Instance.EditingGameboard.levelSetting.neededPickables.Count == 0 &&
                LevelEditor.Instance.EditingGameboard.levelSetting.winningTile == null)
            {
                saveOnSettingLevelButton.gameObject.SetActive(false);
                saveNewOnSettingLevelButoon.gameObject.SetActive(false);
            }
            else
            {
                saveOnSettingLevelButton.gameObject.SetActive(true);
                saveNewOnSettingLevelButoon.gameObject.SetActive(true);
            }
        }
        else
        {
            saveOnSettingLevelButton.gameObject.SetActive(false);
            saveNewOnSettingLevelButoon.gameObject.SetActive(false);
        }

    }

    public void EndSavingAndLoading()
    {
        //saveNameAsker.SetActive(false);
        //overwriteAskerUI.SetActive(false);
        //saveButtonOnAsker.onClick.RemoveAllListeners();

        editCampaignPanel.ShutDown();

        //loadPanel.SetActive(false);
        //selectedLoadableButton = null;
        //loadThisButton.onClick.RemoveAllListeners();
        //deleteThisButton.onClick.RemoveAllListeners();

        ShowEditingUI();
        ShutDowSettingLevelUI();
    }

}
