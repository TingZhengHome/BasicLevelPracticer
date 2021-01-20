using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadableButtonSlot : MonoBehaviour
{

    EditCampaignPanel editCampaignPanel;

    public int slotIndex;

    public bool isDetected;

    Image image;

    //[SerializeField]
    //Button addLevelButton;


    private void Awake()
    {
        //if (addLevelButton.onClick.GetPersistentEventCount() == 0)
        //{
        //    addLevelButton.onClick.AddListener(() => { MainMenu.Instance.editCampaignPanel.AddNewLevel(slotIndex); });
        //}

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            editCampaignPanel = MainMenu.Instance.editCampaignPanel;
        }
        else if (SceneManager.GetActiveScene().name == "LevelEditor")
        {
            editCampaignPanel = LEditor_UIManager.Instance.editCampaignPanel;
        }

        image = GetComponent<Image>();
    }


    // Use this for initialization
    void Start()
    {
        //if (addLevelButton.onClick.GetPersistentEventCount() == 0)
        //{
        //    addLevelButton.onClick.AddListener(() => { MainMenu.Instance.editCampaignPanel.AddNewLevel(slotIndex); });
        //}
        //image = GetComponent<Image>();
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            editCampaignPanel = MainMenu.Instance.editCampaignPanel;
        }
        else if (SceneManager.GetActiveScene().name == "LevelEditor")
        {
            editCampaignPanel = LEditor_UIManager.Instance.editCampaignPanel;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (editCampaignPanel.loadablButtonsAreaController.draggingLoadableButton != null)
        {
            GetComponent<Image>().raycastTarget = true;
            SenseHover();
        }
        else
        {
            GetComponent<Image>().raycastTarget = false;
        }

        if (editCampaignPanel.loadablButtonsAreaController.previewingSlot == this && !isDetected)
        {
            StopBeingPreviewed();
        }

        //ShowAndShutDownAddLevel();
    }

    public void SenseHover()
    {
        Collider2D hit = Physics2D.OverlapPoint(this.transform.position, GameManager.Instance.hoverLayer);
        LoadableButton draggingButton = GameManager.Instance.editCampaignPanel.loadablButtonsAreaController.draggingLoadableButton;


        if (draggingButton != null && hit != null)
        {
            isDetected = true;

            if (ContainingButton() != null)
            {
                GameManager.Instance.editCampaignPanel.loadablButtonsAreaController.PreviewDraggedButtonPosition(this, ContainingButton());
            }
        }

        if (hit == null)
        {
            isDetected = false;
        }
    }

    //public void ShowAndShutDownAddLevel()
    //{
    //    int belongingPageNum = slotIndex / 6;
    //    LoadableButtonAreaPage belongingPage = MainMenu.Instance.editCampaignPanel.loadablButtonsAreaController.levelButtonsPages[belongingPageNum];
    //    if (belongingPage != null)
    //    {
    //        if (transform.GetSiblingIndex() > belongingPage.transform.childCount)
    //        {
    //            addLevelButton.gameObject.SetActive(true);
    //        }
    //        else
    //        {
    //            addLevelButton.gameObject.SetActive(false);
    //        }
    //    }

    //    if (MainMenu.Instance.editCampaignPanel.loadablButtonsAreaController.draggingLoadableButton != null)
    //    {
    //        addLevelButton.GetComponent<Image>().raycastTarget = false;
    //    }
    //}

    public Button ContainingButton()
    {
        LoadableButtonAreaPage currentPage = GameManager.Instance.editCampaignPanel.loadablButtonsAreaController.currentPage;

        Button theButton = null;

        if (transform.GetSiblingIndex() < currentPage.levelButtonsArea.transform.childCount)
        {
            if (currentPage.levelButtonsArea.transform.GetChild(transform.GetSiblingIndex()) != null)
            theButton = currentPage.levelButtonsArea.transform.GetChild(transform.GetSiblingIndex()).GetComponent<Button>();
        }
        

        return theButton;
    }

    public void StopBeingPreviewed()
    {
        GameManager.Instance.editCampaignPanel.loadablButtonsAreaController.StopPreviewingDraggedButtonPostion();
    }

}
