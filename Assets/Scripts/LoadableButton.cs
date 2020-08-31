using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadableButton : MonoBehaviour {

    public Image image;

    [SerializeField]
    Color defaultColor, selectedColor;

    [SerializeField]
    Text text;


    private void Awake()
    {
        image = GetComponent<Image>();
    }


    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (LEditor_UIManager.Instance.selectedLoadableButton != this)
        {
            image.color = defaultColor;
        }
        else
        {
            image.color = selectedColor;
        }
	}
}
