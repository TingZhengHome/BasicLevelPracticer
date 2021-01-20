using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{

    CampaignTheme selectedTheme;
    CampaignData selectedCampaign;

    public LayerMask hoverLayer;
    public Hover hover;
    public Hover hoverPrefab;


    public EditCampaignPanel editCampaignPanel;

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        Instance.DeleteRedundancy();
        CheckHover();
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            editCampaignPanel = MainMenu.Instance.editCampaignPanel;
        }
        else if (SceneManager.GetActiveScene().name == "LevelEditor")
        {
            editCampaignPanel = LEditor_UIManager.Instance.editCampaignPanel;
        }

    }

    public void StartEditCampaign()
    {
        CampaignData campaignData = SaveManager.Instance.loadedCampaign;


        if (campaignData == null)
        {
            MainMenu.Instance.ShowEditCampaignPanel(string.Empty);
        }
        else
        {
            MainMenu.Instance.ShowEditCampaignPanel(campaignData.campaignName);
        }

    }

    public void StartEditNewCampaign(string campaignName, string theme)
    {
        CampaignData newCamp = new CampaignData();

        newCamp.campaignName = campaignName;
        newCamp.theme = (CampaignTheme)Enum.Parse(typeof(CampaignTheme), theme);

        SaveManager.Instance.loadedCampaign = newCamp;
        SaveManager.Instance.CreateNewCampaign(campaignName);

        MainMenu.Instance.ShowEditCampaignPanel(campaignName);
    }

    public void StartPlaySelectedCampaign()
    {

    }

    public void StartEditNewLevel()
    {

    }

    public Scene GetActiveScene()
    {
        return SceneManager.GetActiveScene();
    }


    public void GoMainMenu()
    {
        StartCoroutine(LoadMainMenu());
    }

    IEnumerator LoadMainMenu()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            hover.transform.SetParent(this.transform);
            hover.tileSelectedUI.UnAttach();
            SceneManager.LoadScene("MainMenu");
            yield return null;
        }

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            CheckHover();
            //hover.CheckTileSelectedUI();
        }

    }


    public void LoadSelectedLevel(LevelData data)
    {
        if (data != null)
        {
            if (editCampaignPanel.gameObject.activeSelf == true)
            {
                editCampaignPanel.SaveOnUIEditingCampaignChange();
            }
            StartCoroutine(LoadLevelEditor(data));
        }
    }

    IEnumerator LoadLevelEditor(LevelData data)
    {
        if (SceneManager.GetActiveScene().name != "LevelEditor")
        {
            hover.transform.SetParent(this.transform);
            SceneManager.LoadScene("LevelEditor");
            yield return null;
        }

        if (LevelEditor.Instance != null)
        {
            SaveManager.Instance.LoadLevel(data.levelName);
            CheckHover();
            hover.SetInState("LevelEditor");
        }
    }

    [SerializeField]
    GameObject cannotDeleteOnEditingPanel;

    public void ShowCannotDeleteOnEditing()
    {
        cannotDeleteOnEditingPanel.SetActive(true);
    }

    public void ShutDownCannotDeleteOnEditing()
    {
        cannotDeleteOnEditingPanel.SetActive(false);
    }

    public void CheckHover()
    {
        if (hover == null)
        {
            hover = FindObjectOfType<Hover>();
            if (hover == null)
            {
                hover = Instantiate(hoverPrefab).GetComponent<Hover>();
            }
            hover.SetInState(GetActiveScene().name);
        }
    }

}
