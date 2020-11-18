using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LEditor_UIManager : Singleton<LEditor_UIManager> {

    [SerializeField]
    public GameObject EditorButtonUI;
    [SerializeField]
    GameObject BoardScaleAskerUI;

    [SerializeField]
    Text campaignNameText, levelNameText;

    public GameObject SettingLevelUI;
    public GameObject Mask;
    public Button StartSetLevelButton;
    public Button endSettingButton;
    public Button allPickablesButton;
    public Button certainPointButton;
    public Button returnToEditingButton;

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
            if (loadPanel.activeSelf)
            {
                loadPanel.SetActive(false);
            }

            if (saveNameAsker.activeSelf)
            {
                saveNameAsker.SetActive(false);
            }
        }

        if (SaveManager.Instance.campaignData != null)
        {
            campaignNameText.text = "Campaign Name: " + SaveManager.Instance.campaignData.campaignName;
        }
        if (LevelEditor.Instance.EditingGameboard != null)
        {
            levelNameText.text = "Level Name: " + LevelEditor.Instance.EditingGameboard.levelName;
        }
    }

    [SerializeField]
    GameObject saveNameAsker;
    public Text saveNameAskerText;
    [SerializeField]
    InputField askerInputField;
    const string levelSavingText = "Type the level name:";
    const string campaignSavingText = "Type the campaign name:";
    public string LevelSavingText
    {
        get
        {
            return levelSavingText;
        }
    }

    public string CampaignSavingText
    {
        get
        {
            return campaignSavingText;
        }
    }
    [SerializeField]
    Button saveButtonOnAsker;
    public void ShowSaveNameAsker(string textToDisplay)
    {
        LevelEditor.Instance.EndCurrentEditingEvent();
        LevelEditor.Instance.EndSavingAndLoading();
        saveButtonOnAsker.onClick.RemoveAllListeners();

        if (textToDisplay == LevelSavingText)
        {
            if (LevelEditor.Instance.EditingGameboard != null)
            {
                askerInputField.text = LevelEditor.Instance.EditingGameboard.levelName;
                Debug.Log("EditingGameBoard's Name is " + LevelEditor.Instance.EditingGameboard.levelName);
                Debug.Log("inputtedText = " + askerInputField.text);
            }

            saveButtonOnAsker.onClick.AddListener(() => { CheckInputtedName("level", askerInputField.text); });
        }
        else if (textToDisplay == CampaignSavingText)
        {
            if (SaveManager.Instance.campaignData != null)
            {
                askerInputField.text = SaveManager.Instance.campaignData.campaignName;
                Debug.Log("Campaign's Name is " + SaveManager.Instance.campaignData.campaignName);
                Debug.Log("inputtedText = " + askerInputField.text);
            }

            saveButtonOnAsker.onClick.AddListener(() => { CheckInputtedName("campaign", askerInputField.text); });
        }
        saveNameAsker.SetActive(true);
        saveNameAskerText.text = textToDisplay;
        Mask.SetActive(true);
    }

    public void CheckInputtedName(string levelOrCampaign, string inputtedText)
    {
        if (levelOrCampaign == "level")
        {
            for (int i = 0; i < SaveManager.Instance.campaignData.levelDatas.Count; i++)
            {
                LevelData level = SaveManager.Instance.campaignData.levelDatas[i];
                if (level.levelName == inputtedText)
                {
                    ShowOverwriteAsker(levelOrCampaign, inputtedText);
                    return;
                }
            }
            SaveManager.Instance.SaveGameBoardAsALevel(inputtedText);
        }
        if (levelOrCampaign == "campaign")
        {
            string path = Application.persistentDataPath + "/saves";
            foreach (string fileName in Directory.GetFiles(path))
            {
                if (fileName == inputtedText)
                {
                    ShowOverwriteAsker(levelOrCampaign, inputtedText);
                    return;
                }
            }
            SaveManager.Instance.SaveLevelsAsACampaign(inputtedText);
        }
    }

    [SerializeField]
    GameObject overwriteAskerUI;
    [SerializeField]
    Text overwriteAskerText;
    [SerializeField]
    Button yesButton, noButton;

    public void ShowOverwriteAsker(string levelOrCampaign, string inputtedText)
    {
        overwriteAskerUI.SetActive(true);

        if (levelOrCampaign == "level")
        {
            overwriteAskerText.text = string.Format("There is a level's name same with the inputted name: {0}.\r\n Do you want to overwrite it?", inputtedText);
            yesButton.onClick.AddListener(() => { SaveManager.Instance.SaveGameBoardAsALevel(inputtedText); });
            noButton.onClick.AddListener(() => { ShowSaveNameAsker(LevelSavingText); });
        }

        if (levelOrCampaign == "campaign")
        {
            overwriteAskerText.text = string.Format("There is a campaign's name same with the inputted name: {0}.\r\n Do you want to overwrite it?", inputtedText);
            yesButton.onClick.AddListener(() => { SaveManager.Instance.SaveLevelsAsACampaign(inputtedText); });
            noButton.onClick.AddListener(() => { ShowSaveNameAsker(CampaignSavingText); });
        }
    }

    [SerializeField]
    GameObject loadPanel;
    [SerializeField]
    Text panelText;
    string[] saveFilesNames;
    [SerializeField]
    GameObject loadButtonPrefab;
    [SerializeField]
    Transform loadArea;
    [SerializeField]
    Button loadThisButton, deleteThisButton;

    public void ShowLoadPanel(string dataType)
    {
        saveFilesNames = null;
        GetLoadFilesName(dataType);
        loadPanel.SetActive(true);

        foreach (Transform button in loadArea)
        {
            Destroy(button.gameObject);
        }

        if (dataType == "campaigns")
        {
            for (int i = 0; i < saveFilesNames.Length; i++)
            {
                GameObject button = Instantiate(loadButtonPrefab);
                button.transform.SetParent(loadArea.transform, false);

                string path = Application.persistentDataPath + "/saves/";
                string campaignName = saveFilesNames[i].Remove(0, path.ToCharArray().Length).Replace(".save", "");
                Debug.Log("A button is named:" + campaignName);
                button.GetComponent<Button>().onClick.AddListener(() => { SelectAndUnslecteLoadableButton(button.GetComponent<Button>(), dataType, campaignName); });
                button.GetComponentInChildren<Text>().text = campaignName;
            }
            panelText.text = "Select the campaign";
        }

        if (dataType == "levels")
        {
            if (saveFilesNames != null && saveFilesNames.Length > 0)
            {
                Debug.Log(string.Format("There are {0} save files to be load.", saveFilesNames.Length));
                for (int i = 0; i < saveFilesNames.Length; i++)
                {
                    GameObject button = Instantiate(loadButtonPrefab);
                    button.transform.SetParent(loadArea.transform, false);

                    Debug.Log(i);
                    string levelName = saveFilesNames[i];
                    button.GetComponent<Button>().onClick.AddListener(() => { SelectAndUnslecteLoadableButton(button.GetComponent<Button>(), dataType, levelName); });
                    Debug.Log("saveFile " + levelName + " is connected to a loadButton.");

                    button.GetComponentInChildren<Text>().text = levelName;
                }
            }

            panelText.text = "Select the level";
        }
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
            if (SaveManager.Instance.campaignData != null)
            {
                if (SaveManager.Instance.campaignData.levelDatas.Count > 0)
                {
                    saveFilesNames = new string[SaveManager.Instance.campaignData.levelDatas.Count];
                    for (int i = 0; i < SaveManager.Instance.campaignData.levelDatas.Count; i++)
                    {
                        saveFilesNames[i] = SaveManager.Instance.campaignData.levelDatas[i].levelName;
                    }
                }
            }
            else
            {
                Debug.Log("There is no level to load because there is no loaded campaign.");
            }
        }
    }

    public void SelectAndUnslecteLoadableButton(Button button, string dataType, string dataName)
    {
        if (button != null && loadPanel.activeSelf)
        {
            if ((selectedLoadableButton != null && selectedLoadableButton != button.GetComponent<LoadableButton>()) || selectedLoadableButton == null)
            {
                selectedLoadableButton = button.GetComponent<LoadableButton>();
                if (dataType == "campaigns")
                {
                    loadThisButton.onClick.AddListener(() => { SaveManager.Instance.LoadCampaign(dataName); });
                    deleteThisButton.onClick.AddListener(() => { DeleteSelectedLoadableButton(dataType, dataName); });
                    Debug.Log(string.Format("Button{0} is selected", dataName));
                }

                if (dataType == "levels")
                {
                    loadThisButton.onClick.AddListener(() => { SaveManager.Instance.LoadLevel(dataName); });
                    deleteThisButton.onClick.AddListener(() => { DeleteSelectedLoadableButton(dataType, dataName); });
                    Debug.Log(string.Format("Button{0} is selected", dataName));
                }
            }
            else if (selectedLoadableButton == button.GetComponent<LoadableButton>())
            {
                selectedLoadableButton = null;
                loadThisButton.onClick.RemoveAllListeners();
                deleteThisButton.onClick.RemoveAllListeners();
                Debug.Log(string.Format("Button{0} is unselected", dataName));
            }
        }
    }

    public void DeleteSelectedLoadableButton(string dataType, string dataName)
    {
        if (selectedLoadableButton != null)
        {
            if (dataType == "campaigns")
            {
                SerializationManagger.Delete(dataName);
                Destroy(selectedLoadableButton.gameObject);
                loadThisButton.onClick.RemoveAllListeners();
                deleteThisButton.onClick.RemoveAllListeners();
            }

            if (dataType == "levels")
            {
                for (int i = 0; i < SaveManager.Instance.campaignData.levelDatas.Count; i++)
                {
                    if (SaveManager.Instance.campaignData.levelDatas[i].levelName == dataName)
                    {
                        SaveManager.Instance.campaignData.levelDatas.Remove(SaveManager.Instance.campaignData.levelDatas[i]);
                    }
                }
                Destroy(selectedLoadableButton.gameObject);
                loadThisButton.onClick.RemoveAllListeners();
                deleteThisButton.onClick.RemoveAllListeners();
            }
        }
    }

    public void ShowEditingUI()
    {
        BoardScaleAskerUI.SetActive(true);
        EditorButtonUI.SetActive(true);
    }

    public void ShutDownEditingUI()
    {
        BoardScaleAskerUI.SetActive(false);
        EditorButtonUI.SetActive(false);
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
    }

    public void EndSavingAndLoading()
    {
        saveNameAsker.SetActive(false);
        overwriteAskerUI.SetActive(false);
        saveButtonOnAsker.onClick.RemoveAllListeners();

        loadPanel.SetActive(false);
        selectedLoadableButton = null;
        loadThisButton.onClick.RemoveAllListeners();
        deleteThisButton.onClick.RemoveAllListeners();

        ShowEditingUI();
        ShutDowSettingLevelUI();
    }

}
