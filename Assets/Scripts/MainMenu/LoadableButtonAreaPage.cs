using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadableButtonAreaPage : MonoBehaviour
{

    [SerializeField]
    int pageNum;

    [SerializeField]
    int buttonLimit;

    public Transform levelButtonsArea;
    public GameObject buttonSlotsPanel;

    [SerializeField]
    Button addLevelButton;

    //[SerializeField]
    //Button addCampaignButton;

    [SerializeField]
    LoadableButtonSlot buttonSlotPrefab;

    public int PageNum
    {
        get
        {
            return pageNum;
        }

        private set
        {
            pageNum = value;
        }
    }

    public int ButtonLimit
    {
        get
        {
            return buttonLimit;
        }
    }

    private void Awake()
    {
        if (addLevelButton != null && addLevelButton.onClick.GetPersistentEventCount() == 0)
        {
            addLevelButton.onClick.AddListener(() => { GameManager.Instance.editCampaignPanel.ShowLevelNameAsker("Add"); });
        }
        //if (addCampaignButton != null && addCampaignButton.onClick.GetPersistentEventCount() == 0)
        //{
        //    addCampaignButton.onClick.AddListener(() => { MainMenu.Instance.campaignListPanel.AddNewCampaign(); });
        //}
    }

    // Use this for initialization
    void Start()
    {
        //if (addLevelButton.onClick.GetPersistentEventCount() == 0)
        //{
        //    addLevelButton.onClick.AddListener(() => { MainMenu.Instance.editCampaignPanel.AddNewLevel(addLevelButton); });
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf && addLevelButton != null)
        {
            DisplayOrShutDownAddButton();
        }
    }

    public void Initialize(int pageNum, Transform parent)
    {
        CleanUp();
        if (buttonSlotsPanel != null && buttonSlotPrefab != null)
        {
            if (buttonSlotsPanel.transform.childCount == 0)
            {
                for (int i = 0; i < buttonLimit; i++)
                {
                    LoadableButtonSlot slot = Instantiate(buttonSlotPrefab);
                    slot.slotIndex = 6 * pageNum + i;
                    slot.name = "ButtonSlot" + slot.slotIndex;
                    slot.transform.SetParent(buttonSlotsPanel.transform, false);
                }
            }
        }

        this.pageNum = pageNum;
        int realPageNum = pageNum + 1;
        name = "Page" + realPageNum;
        transform.SetParent(parent, false);
        transform.position = parent.GetComponent<LoadableAreaController>().pagePostion.transform.position;
    }

    public void InsertButton(LoadableButton theButton, int newIndex)
    {
        int buttonInPageIndex = newIndex - ButtonLimit * PageNum;

        if (theButton != null)
        {
            theButton.transform.SetParent(levelButtonsArea, false);
            theButton.transform.SetSiblingIndex(buttonInPageIndex);
            Debug.Log("Data" + newIndex + " is inserted.");
        }
        else
        {
            Debug.LogWarning("You lost the being inserted button.");
        }
    }

    public void DisplayOrShutDownAddButton()
    {
        int buttonCount = levelButtonsArea.transform.childCount;
        if (buttonCount > ButtonLimit)
        {
            if (addLevelButton != null)
                addLevelButton.gameObject.SetActive(false);
            //if (addCampaignButton != null)
            //    addCampaignButton.gameObject.SetActive(false);
        }
        else
        {
            if (addLevelButton != null)
            {
                addLevelButton.gameObject.SetActive(true);
                addLevelButton.transform.SetSiblingIndex(buttonCount - 1);
            }
            //if (addCampaignButton != null)
            //{
            //    addCampaignButton.gameObject.SetActive(true);
            //    addCampaignButton.transform.SetSiblingIndex(buttonCount - 1);
            //}
        }
    }

    public void SetActiveButtonSlotPanel()
    {
        buttonSlotsPanel.SetActive(true);
    }

    public List<LoadableButton> containingLoadables()
    {
        List<LoadableButton> loadables = new List<LoadableButton>();

        foreach (LoadableButton button in levelButtonsArea.GetComponentsInChildren<LoadableButton>())
        {
            if (button != null)
            {
                loadables.Add(button);
            }
        }
        return loadables;
    }

    public void RefreshButtonIndexes()
    {
        List<LoadableButton> buttons = containingLoadables();
        int i = 0;
        foreach (LoadableButton button in buttons)
        {
            button.slotIndex = i + ButtonLimit * PageNum;
            i++;
        }
    }

    public void CleanUp()
    {
        if (levelButtonsArea != null)
        {
            foreach (Transform button in levelButtonsArea)
            {
                if (button.GetComponent<LoadableButton>() != null)
                {
                    Destroy(button.gameObject);
                }
            }
        }

        if (buttonSlotsPanel != null)
        {
            foreach (Transform slot in buttonSlotsPanel.transform)
            {
                if (slot.GetComponent<LoadableButtonSlot>() != null)
                {
                    Destroy(slot.gameObject);
                }
            }
        }
    }

}
