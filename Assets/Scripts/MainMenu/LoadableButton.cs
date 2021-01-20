using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public enum loadableType { campaign, level };
public class LoadableButton : MonoBehaviour
{
    RectTransform rectTransform;

    [SerializeField]
    GameObject editingMark;

    EditCampaignPanel editCampaignPanel;
    CampaignListPanel campaignListPanel;

    public LevelData containingLevelData;

    public loadableType type;

    Button button;

    public Image image;

    [SerializeField]
    Color defaultColor, selectedColor;

    [SerializeField]
    Text text;

    BoxCollider2D trigger;

    bool isBeingDragged;

    public bool isDetected;

    public int slotIndex;

    private void Awake()
    {
        image = GetComponent<Image>();
        trigger = GetComponent<BoxCollider2D>();
        button = GetComponent<Button>();
        rectTransform = GetComponent<RectTransform>();
    }


    // Use this for initialization
    void Start()
    {
        button = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
     
        if (type == loadableType.level)
        {
            if (isBeingDragged)
            {
                BeingDragged();
            }

            if (!isBeingDragged && GameManager.Instance.editCampaignPanel.loadablButtonsAreaController.draggingLoadableButton != null)
            {
            }

            if (!isDetected)
            {
                if (!isBeingDragged && GameManager.Instance.editCampaignPanel.loadablButtonsAreaController.previewingSlot == this)
                {
                }
            }

            if (SaveManager.Instance.loadedLevel == containingLevelData)
            {
                editingMark.SetActive(true);
            }
            else
            {
                editingMark.SetActive(false);
            }
        }
    }

    public void CheckSelected(LoadableAreaController controller)
    {
        LoadableButton selectedButton = controller.selectedLoadableButton;

        if (selectedButton == this)
        {
            image.color = selectedColor;
        }
        else
        {
            image.color = defaultColor;
        }

    }

    public void InitializeLevelButton(int index, loadableType type, LevelData data)
    {
        if (type == loadableType.level)
        {
            slotIndex = index;
            this.type = type;
            if (data != null)
            {
                containingLevelData = data;
            }
            else
            {
                containingLevelData = new LevelData(SaveManager.Instance.loadedCampaign.theme, 0, 0, "New Level");
            }
        }

        transform.GetComponentInChildren<Text>().text = containingLevelData.levelName;
    }

    public void GetDragged()
    {
        if (!isBeingDragged && type == loadableType.level)
        {
            isBeingDragged = true;
            transform.SetParent(GameManager.Instance.hover.transform);
            GameManager.Instance.editCampaignPanel.loadablButtonsAreaController.StartDraggingButton(this);
            GetComponent<Image>().raycastTarget = false;
            text.raycastTarget = false;
            button.interactable = false;
            GameManager.Instance.hover.SetCollideSize(trigger);
        }
    }

    public void BeingDragged()
    {
        transform.localPosition = Vector3.zero;
    }

    public void BePutDown(int newSlotIndex)
    {
        slotIndex = newSlotIndex;
        isBeingDragged = false;
        GetComponent<Image>().raycastTarget = true;
        text.raycastTarget = true;
        button.interactable = true;
        GameManager.Instance.hover.RevertCollideSize(SceneManager.GetActiveScene().name);
    }

    public void ModifyLevelName(string newName)
    {
        if (newName != string.Empty)
        {
            containingLevelData.levelName = newName;
            transform.GetComponentInChildren<Text>().text = containingLevelData.levelName;
        }
        GameManager.Instance.editCampaignPanel.RefreshLevelDatas();
    }


    //public void SetUpperLayerPanels()
    //{
    //    if (GameManager.Instance.GetActiveScene().name == "MainMenu")
    //    {
    //        editCampaignPanel = Leditor;
    //    }
    //}
}
