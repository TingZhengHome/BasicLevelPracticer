using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OKMessagePanel : Singleton<OKMessagePanel>
{

    [SerializeField]
    GameObject overallMask;
    [SerializeField]
    GameObject panel;
    [SerializeField]
    Button OKButton;
    [SerializeField]
    Text messageText;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this.gameObject);
        Instance.DeleteRedundancy();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DisplayMessage(string message)
    {
        panel.gameObject.SetActive(true);
        messageText.text = message;
        overallMask.gameObject.SetActive(true);
        panel.transform.localPosition = Vector3.zero;
    }

    public void ShutDown()
    {
        messageText.text = null;
        panel.gameObject.SetActive(false);
        overallMask.gameObject.SetActive(false);
    }

}
