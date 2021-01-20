using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class EditCampaignPanel : MonoBehaviour
{

    [SerializeField]
    GameObject loadableLevelButtonPrefab = null;
    public Button loadLevelButton, deleteLevelButton, playLevelButton;
    public InputField campaignName;
    [SerializeField]
    Text themeText;
    [SerializeField]
    Text totalLevelsText;

    public LoadableAreaController loadablButtonsAreaController;

    List<LevelData> levelDatas = new List<LevelData>();

    public static event Action<LoadableAreaController> checkSelected;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (loadablButtonsAreaController.selectedLoadableButton == null)
        {
            ShutDownLevelInfo();
        }
    }

    public void Show(string[] saveFilesNames, string campDataName)
    {
        loadLevelButton.onClick.RemoveAllListeners();
        deleteLevelButton.onClick.RemoveAllListeners();

        ShutDownLevelInfo();

        CampaignData loadedCamp = SaveManager.Instance.loadedCampaign;

        SetCampaignInfo(loadedCamp.campaignName, loadedCamp.theme.ToString(), saveFilesNames.Length);


        loadablButtonsAreaController.Initialize(saveFilesNames.Length);

        for (int i = 0; i < saveFilesNames.Length; i++)
        {
            LevelData leveldata = SaveManager.Instance.loadedCampaign.levelDatas[i];
            LoadableButton levelButton = Instantiate(loadableLevelButtonPrefab).GetComponent<LoadableButton>();
            levelButton.InitializeLevelButton(i, loadableType.level, leveldata);
            loadablButtonsAreaController.InsertLoadableButton(levelButton, i);
            levelDatas.Add(leveldata);

            string levelName = saveFilesNames[i];
            Debug.Log("A button is named:" + levelName);
            levelButton.GetComponent<Button>().onClick.AddListener(() => { loadablButtonsAreaController.SelectAndUnselectLoadableButton(levelButton, "levels", levelName); });
            checkSelected += levelButton.CheckSelected;
            levelButton.GetComponent<Button>().onClick.AddListener(() => { CheckAllLoadableSelected(loadablButtonsAreaController); });
            levelButton.GetComponent<Button>().onClick.AddListener(() => { DisplayLevelInfo(levelButton.containingLevelData.levelName, levelButton.containingLevelData.row.ToString(), levelButton.containingLevelData.column.ToString(), levelButton.containingLevelData.settingData.winningCondition.ToString()); });
            levelButton.GetComponentInChildren<Text>().text = levelName;
        }
    }

    [SerializeField]
    LevelNameAskerPanel levelNameAsker;

    public void ShowLevelNameAsker(string action)
    {
        levelNameAsker.Show(action);
    }

    public void CheckAllLoadableSelected(LoadableAreaController controller)
    {
        checkSelected(controller);
    }

    public void UnregisterCheckSelected(LoadableButton button)
    {
        checkSelected -= button.CheckSelected;
    }

    public void SetCampaignInfo(string name, string theme, int levelCount)
    {
        campaignName.text = name;
        themeText.text = "Theme: " + theme;
        totalLevelsText.text = "Total Levels: " + levelCount;
    }

    public void AddNewLevel(Button button)
    {
        CampaignData loadedCamp = SaveManager.Instance.loadedCampaign;
        LoadableButtonAreaPage currentPage = loadablButtonsAreaController.currentPage;
        int newButtonIndex = button.transform.GetSiblingIndex();

        LoadableButton levelButton = Instantiate(loadableLevelButtonPrefab).GetComponent<LoadableButton>();
        levelButton.InitializeLevelButton(newButtonIndex + 6 * currentPage.PageNum, loadableType.level, null);
        loadablButtonsAreaController.InsertLoadableButton(levelButton, newButtonIndex + 6 * currentPage.PageNum);
        levelButton.GetComponent<Button>().onClick.AddListener(() => { loadablButtonsAreaController.SelectAndUnselectLoadableButton(levelButton, "levels", "emptyLevel"); });
        checkSelected += levelButton.CheckSelected;
        levelButton.GetComponent<Button>().onClick.AddListener(() => { CheckAllLoadableSelected(loadablButtonsAreaController); });
        levelButton.GetComponent<Button>().onClick.AddListener(() => { DisplayLevelInfo(string.Empty, "0", "0", string.Empty); });

        RefreshLevelDatas();
    }

    public void AddNewLevel(string newLevelName)
    {
        CampaignData loadedCamp = SaveManager.Instance.loadedCampaign;
        LoadableButtonAreaPage currentPage = loadablButtonsAreaController.currentPage;
        int newButtonIndex = loadablButtonsAreaController.currentPage.containingLoadables().Count;
        LevelData levelData = new LevelData(SaveManager.Instance.loadedCampaign.theme, 0, 0, newLevelName);

        LoadableButton levelButton = Instantiate(loadableLevelButtonPrefab).GetComponent<LoadableButton>();
        levelButton.InitializeLevelButton(newButtonIndex + 6 * currentPage.PageNum, loadableType.level, levelData);
        loadablButtonsAreaController.InsertLoadableButton(levelButton, newButtonIndex + 6 * currentPage.PageNum);
        levelButton.GetComponent<Button>().onClick.AddListener(() => { loadablButtonsAreaController.SelectAndUnselectLoadableButton(levelButton, "levels", "emptyLevel"); });
        checkSelected += levelButton.CheckSelected;
        levelButton.GetComponent<Button>().onClick.AddListener(() => { CheckAllLoadableSelected(loadablButtonsAreaController); });
        levelButton.GetComponent<Button>().onClick.AddListener(() => { DisplayLevelInfo(newLevelName, "0", "0", string.Empty); });

        RefreshLevelDatas();
        if (levelNameAsker.gameObject.activeSelf)
        {
            levelNameAsker.ShutDown();
        }
    }

    [SerializeField]
    GameObject levelInfoPanel;
    [SerializeField]
    Text levelNameText;
    [SerializeField]
    InputField levelNameInput;
    [SerializeField]
    Text levelSizeText;
    [SerializeField]
    Text winConditionText;

    public void DisplayLevelInfo(string levelName, string row, string column, string winCondition)
    {
        levelInfoPanel.SetActive(true);

        levelNameText.text = "Level Name: " + levelName;
        if (levelName == "New Level")
        {
            levelNameInput.text = string.Empty;
        }
        else
        {
            levelNameInput.text = levelName;
        }

        levelSizeText.text = "Level Size: " + row + "x" + column;
        winConditionText.text = "Condition: " + winCondition;

        if (loadablButtonsAreaController.selectedLoadableButton != null)
        {
            levelNameInput.onEndEdit.AddListener((x) => { x = levelNameInput.text; loadablButtonsAreaController.selectedLoadableButton.ModifyLevelName(levelNameInput.text); });
        }
    }

    public void ShutDownLevelInfo()
    {
        if (levelInfoPanel.activeSelf)
        {
            levelNameText.text = "Level Name: ";
            levelSizeText.text = "Level Size: ";
            winConditionText.text = "Condition: ";
            levelNameInput.onEndEdit.RemoveAllListeners();
            levelInfoPanel.SetActive(false);
        }
    }

    public void SaveOnUIEditingCampaignChange()
    {
        RefreshLevelDatas();
        if (SaveManager.Instance != null && SaveManager.Instance.loadedCampaign != null)
        {
            if (gameObject.activeSelf && campaignName.text != string.Empty)
            {
                SaveManager.Instance.ModifyCampaignInfo1(campaignName.text, levelDatas);
            }
        }
        Debug.Log("Campagin modification saved. New level count: " + levelDatas.Count);
    }

    public void RefreshLevelDatas()
    {
        List<LevelData> newDatas = new List<LevelData>();
        List<LoadableButton> allLoadables = loadablButtonsAreaController.GetAllLoadable();

        for (int i = 0; i < allLoadables.Count; i++)
        {
            newDatas.Add(allLoadables[i].containingLevelData);
        }

        levelDatas = newDatas;
        totalLevelsText.text = "Total Levels: " + allLoadables.Count;

        Debug.Log("Level datas refreshed.");
    }

    public void ResetPanel()
    {
        loadablButtonsAreaController.CleanUp();
        ShutDownLevelInfo();
        checkSelected = null;
    }

    public void ShutDown()
    {
        ResetPanel();
        gameObject.SetActive(false);

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            MainMenu.Instance.MainPanel.gameObject.SetActive(true);
        }
        if (SceneManager.GetActiveScene().name == "LevelEditor")
        {
            LEditor_UIManager.Instance.Mask.SetActive(false);
        }
    }

    private void OnDisable()
    {
        ResetPanel();
    }

    public void ShowDeleteAskerPanel()
    {
        DeleteAskerPanel.Instance.Show(this, loadablButtonsAreaController.selectedLoadableButton);
    }
}
