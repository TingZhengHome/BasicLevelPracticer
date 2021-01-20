using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CampaignInfoPanel : MonoBehaviour {


    [SerializeField]
    Text campaignNameText, themeText, levelsText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Show(string name, string theme, int totalLevelsNum)
    {
        gameObject.SetActive(true);
        campaignNameText.text = "Name: " + name;
        themeText.text = "Theme: " + theme;
        levelsText.text = "Levels: " + totalLevelsNum;
    }

    public void ShutDown()
    {
        campaignNameText.text = "Name:";
        themeText.text = "Theme: ";
        levelsText.text = "Levels: ";
        gameObject.SetActive(false);
    }
}
