using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriggerableByDraggeds : MonoBehaviour
{

    [SerializeField]
    float triggerButtonTime = 0.8f;
    [SerializeField]
    float pointerStayCounter = 0;
    [SerializeField]
    bool isDraggingMouseOverAButton = false;
    Button button;


    private void Awake()
    {
        button = GetComponent<Button>();
        isDraggingMouseOverAButton = false;
    }

    // Use this for initialization
    void Start()
    {
        button = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDraggingMouseOverAButton)
        {
            pointerStayCounter += Time.deltaTime;
            if (pointerStayCounter >= triggerButtonTime)
            {
                button.onClick.Invoke();
                pointerStayCounter = 0;
            }
        }
    }

    public void isDraggingObMouseOverButton()
    {
        Debug.Log(name + " feel the pointer.");
        if (GameManager.Instance.editCampaignPanel.loadablButtonsAreaController.draggingLoadableButton != null)
        {
            Debug.Log(name + " feel the draggedOb.");
            isDraggingMouseOverAButton = true;
        }
    }

    public void isDraggingObMouseExitButton()
    {
        Debug.Log(name + " lose the pointer.");

        pointerStayCounter = 0;
        isDraggingMouseOverAButton = false;
    }

    private void OnDisable()
    {
        isDraggingMouseOverAButton = false;
    }
}
