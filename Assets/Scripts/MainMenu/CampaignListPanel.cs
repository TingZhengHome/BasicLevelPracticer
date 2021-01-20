using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CampaignListPanel : MonoBehaviour {

    
    public LoadableAreaController loadableButtonsController;

    //public Transform campaignButtonsArea;
    [SerializeField]
    GameObject loadableCampaignPrefab;
    public Button playCampaignButton, editCampaignButton, deleteCampaignButton;

    public CampaignInfoPanel campaignInfoPanel;

    public static event Action<LoadableAreaController> checkSelected;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (loadableButtonsController.selectedLoadableButton == null)
        {
            campaignInfoPanel.ShutDown();
        }
	}

    public void Show(string[] saveFilesNames)
    {
        CleanUp();

        loadableButtonsController.Initialize(saveFilesNames.Length);

        for (int i = 0; i < saveFilesNames.Length; i++)
        {
            LoadableButton campaignButton = Instantiate(loadableCampaignPrefab).GetComponent<LoadableButton>();
            campaignButton.GetComponent<LoadableButton>().type = loadableType.campaign;

            loadableButtonsController.InsertLoadableButton(campaignButton, i);

            string path = Application.persistentDataPath + "/saves/";
            string campaignName = saveFilesNames[i].Remove(0, path.ToCharArray().Length).Replace(".save", "");
            string theme = SaveManager.Instance.loadedCampaign.theme.ToString();
            int totalLevels = SaveManager.Instance.loadedCampaign.levelDatas.Count;
            Debug.Log("A button is named:" + campaignName);
            campaignButton.GetComponent<Button>().onClick.AddListener(() => { loadableButtonsController.SelectAndUnselectLoadableButton(campaignButton, "campaigns", campaignName); });
            checkSelected += campaignButton.CheckSelected;
            campaignButton.GetComponent<Button>().onClick.AddListener(() => { CheckAllLoadableSelected(loadableButtonsController); });
            campaignButton.GetComponent<Button>().onClick.AddListener(() => { ShowCampaignInfo(campaignName, theme, totalLevels); });
            campaignButton.GetComponentInChildren<Text>().text = campaignName;
        }
    }

    public void CheckAllLoadableSelected(LoadableAreaController controller)
    {
        checkSelected(controller);
    }

    public void UnregisterCheckSelected(LoadableButton button)
    {
        checkSelected -= button.CheckSelected;
    }

    public void AddNewCampaign()
    {
        ShutDown();

        MainMenu.Instance.ShowNewCampaignPanel(MainMenu.Instance.newCampaignPanel.gameObject);
    }

    public void ShowCampaignInfo(string name, string theme, int totalLevels)
    {
        campaignInfoPanel.Show(name, theme, totalLevels);
    }

    public void CleanUp()
    {
        playCampaignButton.onClick.RemoveAllListeners();
        editCampaignButton.onClick.RemoveAllListeners();
        deleteCampaignButton.onClick.RemoveAllListeners();

        loadableButtonsController.CleanUp();
    }


    public void ShutDown()
    {
        CleanUp();
        gameObject.SetActive(false);
        MainMenu.Instance.MainPanel.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        CleanUp();
        checkSelected = null;
    }

    public void ShowDeleteAskerPanel()
    {
        DeleteAskerPanel.Instance.Show(this, loadableButtonsController.selectedLoadableButton);
    }
}
