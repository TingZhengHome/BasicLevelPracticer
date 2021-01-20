using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LoadableAreaController : MonoBehaviour
{
    public LoadableButton selectedLoadableButton;

    [SerializeField]
    LoadableButtonAreaPage buttonPagePrefab;
    public List<LoadableButtonAreaPage> buttonPages = new List<LoadableButtonAreaPage>();
    public LoadableButtonAreaPage currentPage;
    [SerializeField]
    int pageLimit;
    [SerializeField]
    Text LoadablePageNumtext;

    public Transform pagePostion;

    [SerializeField]
    Button addCampaignButton;

    public List<LoadableButton> GetAllLoadable()
    {
        List<LoadableButton> allLoadableButtons = new List<LoadableButton>();
        foreach (LoadableButtonAreaPage page in buttonPages)
        {
            int p = 0;
            foreach (LoadableButton button in page.containingLoadables())
            {
                if (button != null)
                {
                    allLoadableButtons.Add(button);
                    p++;
                }
            }
            Debug.Log(p + "button in page" + page.PageNum);
        }

        return allLoadableButtons;
    }

    // Use this for initialization
    void Start()
    {
        if (addCampaignButton != null && addCampaignButton.onClick.GetPersistentEventCount() == 0)
        {
            addCampaignButton.onClick.AddListener(() => { MainMenu.Instance.campaignListPanel.AddNewCampaign(); });
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (draggingLoadableButton != null)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                DropBackDraggingButton();
            }

            if (Input.GetMouseButtonUp(0))
            {
                EndDraggingEvents();
            }
        }
    }

    public void Initialize(int totalFilesNum)
    {
        selectedLoadableButton = null;
        for (int i = 0; i <= totalFilesNum / buttonPagePrefab.ButtonLimit; i++)
        {
            CreateNewPage(i);
        }

        if (buttonPages.Count == 0)
        {
            CreateNewPage(0);
        }
        currentPage = buttonPages[0];
        buttonPages[0].gameObject.SetActive(true);
        LoadablePageNumtext.text = (currentPage.PageNum + 1).ToString();
        PageObjectsControll();
    }

    public void CreateNewPage(int i)
    {
        LoadableButtonAreaPage newPage = Instantiate(buttonPagePrefab).GetComponent<LoadableButtonAreaPage>();
        if (currentPage == null)
        {
            newPage.Initialize(i, this.transform);
        }
        else
        {
            newPage.Initialize(currentPage.PageNum + 1, this.transform);
        }

        buttonPages.Add(newPage);
        newPage.gameObject.SetActive(false);
    }

    [SerializeField]
    Button PageUp, PageDown;

    public void TurnLevelAreaPage(string UpOrDown)
    {
        int currentPageNum = currentPage.PageNum;

        if (UpOrDown == "Up")
        {
            if (currentPageNum > 0)
            {
                buttonPages[currentPageNum].gameObject.SetActive(false);
                currentPage = buttonPages[currentPageNum - 1];
                buttonPages[currentPage.PageNum].gameObject.SetActive(true);
            }
        }
        else if (UpOrDown == "Down")
        {
            if (currentPageNum < pageLimit - 1)
            {
                if (buttonPages[currentPageNum + 1] != null)
                {
                    buttonPages[currentPageNum].gameObject.SetActive(false);
                    currentPage = buttonPages[currentPageNum + 1];
                    buttonPages[currentPage.PageNum].gameObject.SetActive(true);
                }
            }
        }
        PageObjectsControll();
        LoadablePageNumtext.text = (currentPage.PageNum + 1).ToString();
    }

    public void PageObjectsControll()
    {
        int currentPageNum = currentPage.PageNum;
        int totalSlots = currentPage.ButtonLimit * buttonPages.Count;

        if (currentPageNum > 0)
        {
            PageUp.gameObject.SetActive(true);
        }
        else
        {
            PageUp.gameObject.SetActive(false);
        }

        if (currentPage.containingLoadables().Count < currentPage.ButtonLimit && draggingLoadableButton == null)
        {
            if (draggingLoadableButton == null)
            {
                PageDown.gameObject.SetActive(false);
            }
            else
            {
                if (currentPage.ButtonLimit - currentPage.containingLoadables().Count > 1)
                    PageDown.gameObject.SetActive(false);
            }
        }
        //else if (GetAllLoadable().Count >= totalSlots && buttonPages.Count < pageLimit)
        //{
        //    CreateNewPage(currentPageNum + 1);
        //}

        if (currentPageNum + 1 < buttonPages.Count && buttonPages[currentPageNum + 1] != null)
        {
            PageDown.gameObject.SetActive(true);
        }
        else
        {
            PageDown.gameObject.SetActive(false);
        }
    }

    public void InsertLoadableButton(LoadableButton insertingButton, int insertingIndex)
    {

        for (int i = 0; i < buttonPages.Count; i++)
        {
            Debug.Log("Controller is inserting data" + insertingIndex + " to Page" + i + ", whose limit number is " + buttonPagePrefab.ButtonLimit * (i + 1) + ".");
            if (insertingIndex < buttonPagePrefab.ButtonLimit * (i + 1))
            {
                buttonPages[i].InsertButton(insertingButton, insertingIndex);
                if (GetAllLoadable().Count >= currentPage.ButtonLimit * buttonPages.Count && buttonPages.Count < pageLimit)
                {
                    CreateNewPage(currentPage.PageNum + 1);
                }
                PageObjectsControll();
                return;
            }
        }
    }

    public LoadableButton draggingLoadableButton = null;

    public void StartDraggingButton(LoadableButton theButton)
    {
        draggingLoadableButton = theButton;
        if (selectedLoadableButton != null)
        {
            SelectAndUnselectLoadableButton(selectedLoadableButton, "levels", selectedLoadableButton.containingLevelData.levelName);
        }
        currentPage.buttonSlotsPanel.SetActive(true);
    }

    [SerializeField]
    GameObject previewButtonPrefab;
    GameObject previewButton;
    public LoadableButtonSlot previewingSlot;


    public void PreviewDraggedButtonPosition(LoadableButtonSlot overlappedSlot, Button overlappedButton)
    {
        if (previewingSlot == null && overlappedSlot != null)
        {
            int overlapingIndex = overlappedSlot.transform.GetSiblingIndex();
            previewingSlot = overlappedSlot;

            previewButton = Instantiate(previewButtonPrefab);
            previewButton.transform.SetParent(currentPage.levelButtonsArea);
            previewButton.transform.SetSiblingIndex(overlapingIndex);
            previewButton.GetComponentInChildren<Text>().text = draggingLoadableButton.GetComponentInChildren<Text>().text;

            Debug.Log(overlapingIndex);
        }
    }

    public void EndDraggingEvents()
    {
        if (previewButton != null && previewingSlot != null)
        {
            PutDownDraggedButton();
            StopPreviewingDraggedButtonPostion();
        }
        else if (previewingSlot == null)
        {
            DropBackDraggingButton();
        }
    }

    public void DropBackDraggingButton()
    {
        InsertLoadableButton(draggingLoadableButton, draggingLoadableButton.slotIndex);

        draggingLoadableButton.BePutDown(draggingLoadableButton.slotIndex);
        if (previewingSlot != null)
        {
            StopPreviewingDraggedButtonPostion();
        }
        draggingLoadableButton = null;
    }

    public void PutDownDraggedButton()
    {
        FillEmptyByDragging(draggingLoadableButton, previewingSlot.slotIndex);

        if (currentPage.containingLoadables().Count <= currentPage.ButtonLimit)
        {
            InsertLoadableButton(draggingLoadableButton, previewingSlot.slotIndex);

            draggingLoadableButton.BePutDown(previewingSlot.slotIndex);
            draggingLoadableButton = null;
        }
        RefreshIndexes();
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            if (MainMenu.Instance.editCampaignPanel.gameObject.activeSelf)
            {
                MainMenu.Instance.editCampaignPanel.RefreshLevelDatas();
            }
        }
        if (SceneManager.GetActiveScene().name == "LevelEditor")
        {
            if (LEditor_UIManager.Instance.editCampaignPanel.gameObject.activeSelf)
            {
                LEditor_UIManager.Instance.editCampaignPanel.RefreshLevelDatas();
            }
        }

    }

    public void FillEmptyByDragging(LoadableButton button, int insertingIndex)
    {
        if (currentPage.containingLoadables().Count >= currentPage.ButtonLimit)
        {
            if (button.slotIndex < insertingIndex)
            {
                for (int i = 0; i < currentPage.PageNum; i++)
                {
                    List<LoadableButton> buttons = buttonPages[i].containingLoadables();
                    List<LoadableButton> nextPageButtons = buttonPages[i + 1].containingLoadables();
                    if (buttons.Count < buttonPages[0].ButtonLimit)
                    {
                        if (buttonPages[i + 1].containingLoadables().Count > 1)
                        {
                            InsertLoadableButton(nextPageButtons[0], (i + 1) * buttonPages[0].ButtonLimit - 1);
                        }
                    }
                }
            }
            else if (button.slotIndex > insertingIndex)
            {
                for (int i = currentPage.PageNum; i < buttonPages.Count; i++)
                {
                    List<LoadableButton> buttons = buttonPages[i].containingLoadables();
                    int limit = buttonPages[i].ButtonLimit;

                    if (buttons.Count >= limit)
                    {
                        InsertLoadableButton(buttons[buttons.Count - 1], (i + 1) * 6);
                    }
                }
            }
        }
    }

    public void FillEmptyByDelete()
    {
        for (int i = 0; i < buttonPages.Count - 1; i++)
        {
            Debug.Log("Page" + i + " contains " + buttonPages[i].containingLoadables().Count + " buttons.");
            if (buttonPages[i].containingLoadables().Count < buttonPages[i].ButtonLimit)
            {
                Debug.Log("Page" + i + " has a hole.");
                if (buttonPages[i + 1].containingLoadables().Count >= 1)
                {
                    Debug.Log("Going to pull one in the next page to fill it.");
                    InsertLoadableButton(buttonPages[i + 1].containingLoadables()[0], buttonPagePrefab.ButtonLimit * (i + 1) - 1);
                }
            }
        }
        DeleteRedundantPages();
    }

    public void DeleteRedundantPages()
    {
        for (int i = 1; i < buttonPages.Count; i++)
        {
            if (buttonPages[i].containingLoadables().Count == 0)
            {
                if (buttonPages[i - 1].containingLoadables().Count < buttonPagePrefab.ButtonLimit)
                {
                    Destroy(buttonPages[i].gameObject);
                    buttonPages.Remove(buttonPages[i]);
                }
            }
        }
    }

    public void StopPreviewingDraggedButtonPostion()
    {
        previewingSlot = null;
        Destroy(previewButton);
        Debug.Log("Destory!");
    }

    public void ShutDownAllPageSlotPanel()
    {
        foreach (LoadableButtonAreaPage page in buttonPages)
        {
            page.buttonSlotsPanel.SetActive(false);
        }
    }

    public void SelectAndUnselectLoadableButton(LoadableButton button, string dataType, string dataName)
    {
        EditCampaignPanel editCampaignPanel = null;
        CampaignListPanel campaignListPanel = null;

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            editCampaignPanel = MainMenu.Instance.editCampaignPanel;
            campaignListPanel = MainMenu.Instance.campaignListPanel;
        }

        if (SceneManager.GetActiveScene().name == "LevelEditor")
        {
            editCampaignPanel = LEditor_UIManager.Instance.editCampaignPanel;
        }


        Debug.Log("SelectAndUnselect");
        if (button != null)
        {
            if ((selectedLoadableButton != null && selectedLoadableButton != button) || selectedLoadableButton == null)
            {
                selectedLoadableButton = button;
                if (dataType == "campaigns" && campaignListPanel != null)
                {
                    //selectedLoadableButton.GetComponent<Button>().onClick;
                    campaignListPanel.editCampaignButton.onClick.AddListener(() => { SaveManager.Instance.LoadCampaign(dataName); });
                    campaignListPanel.editCampaignButton.onClick.AddListener(() => { GameManager.Instance.StartEditCampaign(); });
                    //campaignListPanel.deleteCampaignButton.onClick.AddListener(() => { DeleteSelectedLoadableButton(dataType, dataName); });
                    Debug.Log(string.Format("Button{0} is selected", dataName));
                }

                if (dataType == "levels" && editCampaignPanel != null)
                {
                    editCampaignPanel.loadLevelButton.onClick.AddListener(() => { GameManager.Instance.LoadSelectedLevel(selectedLoadableButton.containingLevelData); });
                    //editCampaignPanel.deleteLevelButton.onClick.AddListener(() => { editCampaignPanel.loadablButtonsAreaController.DeleteSelectedLoadableButton(dataType, dataName); });
                    Debug.Log(string.Format("Button{0} is selected", dataName));
                }
            }
            else if (selectedLoadableButton == button.GetComponent<LoadableButton>())
            {
                selectedLoadableButton = null;
                if (campaignListPanel != null)
                {
                    campaignListPanel.editCampaignButton.onClick.RemoveAllListeners();
                    //campaignListPanel.deleteCampaignButton.onClick.RemoveAllListeners();
                }
                if (editCampaignPanel != null)
                {
                    editCampaignPanel.loadLevelButton.onClick.RemoveAllListeners();
                    //editCampaignPanel.deleteLevelButton.onClick.RemoveAllListeners();
                }
                
                Debug.Log(string.Format("Button{0} is unselected", dataName));
            }
        }
    }

    public void DeleteSelectedLoadableButton(string dataType, string dataName)
    {
        EditCampaignPanel editCampaignPanel = null;
        CampaignListPanel campaignListPanel = null;

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            editCampaignPanel = MainMenu.Instance.editCampaignPanel;
            campaignListPanel = MainMenu.Instance.campaignListPanel;
        }

        if (SceneManager.GetActiveScene().name == "LevelEditor")
        {
            editCampaignPanel = LEditor_UIManager.Instance.editCampaignPanel;
        }

        if (selectedLoadableButton != null)
        {
            if (dataType == "campaigns")
            {
                campaignListPanel.UnregisterCheckSelected(selectedLoadableButton);
                SerializationManagger.Delete(dataName);
                DestroyImmediate(selectedLoadableButton.gameObject);
                campaignListPanel.editCampaignButton.onClick.RemoveAllListeners();
                campaignListPanel.deleteCampaignButton.onClick.RemoveAllListeners();
            }

            if (dataType == "levels")
            {
                if (SaveManager.Instance.loadedLevel != selectedLoadableButton.containingLevelData)
                {
                    editCampaignPanel.UnregisterCheckSelected(selectedLoadableButton);
                    DestroyImmediate(selectedLoadableButton.gameObject);
                    editCampaignPanel.loadLevelButton.onClick.RemoveAllListeners();
                    //MainMenu.Instance.editCampaignPanel.playLevelButton.onClick.RemoveAllListeners();
                    editCampaignPanel.deleteLevelButton.onClick.RemoveAllListeners();
                }
                else
                {
                    OKMessagePanel.Instance.DisplayMessage(OKMessageLibrary.cannotDeleteOnEditing);
                }
            }
        }
        FillEmptyByDelete();
        RefreshIndexes();
        editCampaignPanel.RefreshLevelDatas();
        PageObjectsControll();
        DeleteAskerPanel.Instance.ShutDown();
    }

    public void DeleteDraggedLoadableButton()
    {
        if (draggingLoadableButton != null)
        {
            Destroy(draggingLoadableButton.gameObject);
            //ShutDownAllPageSlotPanel();
            MainMenu.Instance.editCampaignPanel.RefreshLevelDatas();
        }
    }


    public void CleanUp()
    {
        selectedLoadableButton = null;
        draggingLoadableButton = null;
        foreach (LoadableButtonAreaPage page in buttonPages)
        {
            page.CleanUp();
            Destroy(page.gameObject);
        }
        buttonPages.Clear();
    }

    public void RefreshIndexes()
    {
        foreach (LoadableButtonAreaPage page in buttonPages)
        {
            page.RefreshButtonIndexes();
        }
    }

}
