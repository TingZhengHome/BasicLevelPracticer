using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewCampaignPanel : MonoBehaviour
{

    public InputField campaignNameInput;

    public Dropdown themeSelector;

    [SerializeField]
    Button createButton;

    [SerializeField]
    GameObject overwriteAsker;
    [SerializeField]
    Button yes, no;
    [SerializeField]
    Text overwriteAskerText;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void Show()
    {
        gameObject.SetActive(true);
        overwriteAsker.gameObject.SetActive(false);
        overwriteAsker.gameObject.SetActive(false);
        overwriteAskerText.text = string.Empty;
    }


    public void CreateCampaign()
    {
        GameManager.Instance.StartEditNewCampaign(campaignNameInput.text, themeSelector.captionText.text);
        gameObject.SetActive(false);

    }

    public void CheckNameExist()
    {
        if (campaignNameInput.text != string.Empty)
        {
            if (isNameAlreadyExist(MainMenu.Instance.GetLoadFilesName("campaigns"), campaignNameInput.text))
            {
                ShowOverwriteAsker(campaignNameInput.text);
            }
            else
            {
                CreateCampaign();
            }
        }
    }

    bool isNameAlreadyExist(string[] existCampaigns, string newCampaignName)
    {
        foreach (string name in existCampaigns)
        {
            string path = Application.persistentDataPath + "/saves/";
            string campaignName = name.Remove(0, path.ToCharArray().Length).Replace(".save", "");
            if (newCampaignName == campaignName)
            {
                return true;
            }
        }
        return false;
    }

    public void ShowOverwriteAsker(string overwrittingName)
    {
        overwriteAsker.gameObject.SetActive(true);
        overwriteAskerText.text = string.Format("Campaign {0} is already exist. Do you want to overwrite it?", overwrittingName);
    }

    public void ShutDownOverwriteAsker()
    {
        overwriteAsker.gameObject.SetActive(false);
        overwriteAskerText.text = string.Empty;
    }

    public void SelectTheme()
    {

    }

    public void ShutDown()
    {
        gameObject.SetActive(false);
        overwriteAsker.gameObject.SetActive(false);
        overwriteAsker.gameObject.SetActive(false);
        overwriteAskerText.text = string.Empty;
        MainMenu.Instance.MainPanel.gameObject.SetActive(true);
    }
}
