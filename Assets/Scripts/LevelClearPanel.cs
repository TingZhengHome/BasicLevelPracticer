using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelClearPanel : MonoBehaviour {

    [SerializeField]
    Button nextLevelButton, reTryButton;
    [SerializeField]
    Text messageText;
    string message = "Level Cleared.";

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Show()
    {
        gameObject.SetActive(true);
    }

}
