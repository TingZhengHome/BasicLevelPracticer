using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeleteAskerPanel : Singleton<DeleteAskerPanel>
{
    [SerializeField]
    Text askerText;
    [SerializeField]
    Button deleteButton, cancleButton;
    [SerializeField]
    GameObject panel;

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        DeleteRedundancy();
        panel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Show(EditCampaignPanel editingCampaignPanel, LoadableButton selectedButton)
    {
        if (editingCampaignPanel.loadablButtonsAreaController.selectedLoadableButton != null)
        {
            panel.SetActive(true);
            deleteButton.onClick.AddListener(() => { editingCampaignPanel.loadablButtonsAreaController.DeleteSelectedLoadableButton("levels", selectedButton.containingLevelData.levelName); });
            askerText.text = askerMessage("level", selectedButton.containingLevelData.levelName);
        }
    }

    public void Show(CampaignListPanel openCampaignListPanel, LoadableButton selectedButton)
    {
        if (openCampaignListPanel.loadableButtonsController.selectedLoadableButton != null)
        {
            panel.SetActive(true);
            deleteButton.onClick.AddListener(() => { openCampaignListPanel.loadableButtonsController.DeleteSelectedLoadableButton("campaigns", selectedButton.GetComponentInChildren<Text>().text); });
            askerText.text = askerMessage("campaign", selectedButton.GetComponentInChildren<Text>().text);
        }
    }

    string askerMessage(string type, string dataName)
    {
        return string.Format("Are you sure to delete {0}: {1}?", type, dataName);
    }

    public void ShutDown()
    {
        panel.SetActive(false);
        deleteButton.onClick.RemoveAllListeners();
        askerText.text = null;
    }

}
