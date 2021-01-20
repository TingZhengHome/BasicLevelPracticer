using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : Singleton<MainMenu>
{

    string[] saveFilesNames;

    //public LoadableButton selectedLoadableButton;

    public GameObject MainPanel;

    public List<GameObject> subordinatedPanels = new List<GameObject>();

    private void Awake()
    {
        gameObject.SetActive(true);
        SaveManager.Instance.loadedLevel = null;
    }

    // Use this for initialization
    void Start()
    {
        gameObject.SetActive(true);
        
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void ShowPanel(GameObject panelObject)
    {
        switch (panelObject.name)
        {
            case "CampaignListPanel":
                ShowCampaignListPanel(panelObject);
                break;
            case "EditCampaignPanel":
                ShowEditCampaignPanel(string.Empty);
                break;
            case "NewCampaignPanel":
                ShowNewCampaignPanel(panelObject);
                break;
            case "EncyclopediaPanel":
                ShowEncyclopediaPanel(panelObject);
                break;
            case "OptionsPanel":
                ShowOptionsPanel(panelObject);
                break;
        }
    }


    public CampaignListPanel campaignListPanel;

    private void ShowCampaignListPanel(GameObject panelObject)
    {
        ShutDownAllPanelAndShow(campaignListPanel.gameObject);
        //selectedLoadableButton = null;
        saveFilesNames = null;
        GetLoadFilesName("campaigns");
        campaignListPanel.gameObject.SetActive(true);
        campaignListPanel.Show(saveFilesNames);
    }


    public EditCampaignPanel editCampaignPanel;

    public void ShowEditCampaignPanel(string campDataName)
    {
        //selectedLoadableButton = null;

        if (campDataName == string.Empty)
        {

        }
        else
        {
            ShutDownAllPanelAndShow(editCampaignPanel.gameObject);
            editCampaignPanel.gameObject.SetActive(true);
            saveFilesNames = null;
            GetLoadFilesName("levels");
            editCampaignPanel.Show(saveFilesNames, campDataName);
        }

    }

    public NewCampaignPanel newCampaignPanel;

    public void ShowNewCampaignPanel(GameObject panelObject)
    {
        ShutDownAllPanelAndShow(panelObject);
        newCampaignPanel.Show();
    }

    private void ShowEncyclopediaPanel(GameObject panelObject)
    {
        ShutDownAllPanelAndShow(panelObject);
        panelObject.SetActive(true);
    }

    private void ShowOptionsPanel(GameObject panelObject)
    {
        ShutDownAllPanelAndShow(panelObject);
        panelObject.SetActive(true);
    }

    public string[] GetLoadFilesName(string type)
    {
        if (!Directory.Exists(Application.persistentDataPath + "/saves"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves");
        }

        if (type == "campaigns")
        {
            saveFilesNames = Directory.GetFiles(Application.persistentDataPath + "/saves").OrderByDescending(f => new FileInfo(f).CreationTime).ToArray<string>();
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
                else
                {
                    saveFilesNames = new string[0];
                }
            }
            else
            {
                Debug.Log("There is no level to load because there is no loaded campaign.");
            }
        }

        return saveFilesNames;
    }

    //public void SelectAndUnselectLoadableButton(Button button, string dataType, string dataName)
    //{
    //    Debug.Log("SelectAndUnselect");
    //    if (button != null && (campaignListPanel.gameObject.activeSelf || editCampaignPanel.gameObject.activeSelf))
    //    {
    //        if ((selectedLoadableButton != null && selectedLoadableButton != button.GetComponent<LoadableButton>()) || selectedLoadableButton == null)
    //        {
    //            selectedLoadableButton = button.GetComponent<LoadableButton>();
    //            if (dataType == "campaigns")
    //            {
    //                campaignListPanel.editCampaignButton.onClick.AddListener(() => { SaveManager.Instance.LoadCampaign(dataName); });
    //                campaignListPanel.editCampaignButton.onClick.AddListener(() => { GameManager.Instance.StartEditCampaign(); });
    //                campaignListPanel.deleteCampaignButton.onClick.AddListener(() => { DeleteSelectedLoadableButton(dataType, dataName); });
    //                Debug.Log(string.Format("Button{0} is selected", dataName));
    //            }
    //        }
    //        else if (selectedLoadableButton == button.GetComponent<LoadableButton>())
    //        {
    //            selectedLoadableButton = null;
    //            campaignListPanel.editCampaignButton.onClick.RemoveAllListeners();
    //            campaignListPanel.deleteCampaignButton.onClick.RemoveAllListeners();
    //            Debug.Log(string.Format("Button{0} is unselected", dataName));
    //        }
    //    }
    //}

    //public void DeleteSelectedLoadableButton(string dataType, string dataName)
    //{
    //    if (selectedLoadableButton != null)
    //    {
    //        if (dataType == "campaigns")
    //        {
    //            SerializationManagger.Delete(dataName);
    //            Destroy(selectedLoadableButton.gameObject);
    //            campaignListPanel.editCampaignButton.onClick.RemoveAllListeners();
    //            campaignListPanel.deleteCampaignButton.onClick.RemoveAllListeners();
    //        }

    //        if (dataType == "levels")
    //        {
    //            for (int i = 0; i < SaveManager.Instance.campaignData.levelDatas.Count; i++)
    //            {
    //                if (SaveManager.Instance.campaignData.levelDatas[i].levelName == dataName)
    //                {
    //                    SaveManager.Instance.campaignData.levelDatas.Remove(SaveManager.Instance.campaignData.levelDatas[i]);
    //                }
    //            }
    //            Destroy(selectedLoadableButton.gameObject);
    //            campaignListPanel.editCampaignButton.onClick.RemoveAllListeners();
    //            campaignListPanel.playCampaignButton.onClick.RemoveAllListeners();
    //            campaignListPanel.deleteCampaignButton.onClick.RemoveAllListeners();
    //        }
    //    }
    //}

    public void ShutDownPanel(GameObject panelObject)
    {
        panelObject.SetActive(false);
        //selectedLoadableButton = null;
        MainPanel.SetActive(true);
    }

    public void ShutDownAllPanelAndShow(GameObject except)
    {
        foreach (GameObject panel in subordinatedPanels)
        {
            if (panel != except)
                panel.gameObject.SetActive(false);
            else panel.gameObject.SetActive(true);
        }
        MainPanel.gameObject.SetActive(false);
    }

    public void ShutDownAllPanel()
    {
        foreach (GameObject panel in subordinatedPanels)
        {
            panel.gameObject.SetActive(false);
        }
        MainPanel.gameObject.SetActive(false);
    }
}
