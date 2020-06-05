
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LEditor_SelectedTileUI : MonoBehaviour
{
    private static LEditor_SelectedTileUI instance;

    [SerializeField]
    LEditor_SelectableObject attachedObject;

    [SerializeField]
    Button cancelButton;

    [SerializeField]
    Button setConnectionButton;

    [SerializeField]
    Button pickUpButton;

    [SerializeField]
    List<Button> buttons = new List<Button>();

    public static LEditor_SelectedTileUI Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LEditor_SelectedTileUI>();
            }
            return instance;
        }
    }

    private void Awake()
    {

    }

    private void Start()
    {
        Debug.Log("SelectedTileUI starts.");
        PackageButtons();
        UnAttach();
        LEditor_TileObject.OnTileClicked += CheckClickAndAttachTo;
        LevelEditor.LaunchedLevel += DestroySelf;
    }


    private void Update()
    {
        StateCheck();
    }


    void Display()
    {
        if (attachedObject != null)
        {
            gameObject.SetActive(true);
            cancelButton.onClick.AddListener(attachedObject.UnSelectThis);

            if (attachedObject.GetComponent<LEditor_ConnectableObject>() != null ||
                attachedObject.GetComponent<LEditor_PortableObject>() != null)
            {
                setConnectionButton.gameObject.SetActive(true);
                Debug.Log("HeyHeyHey");
            }
            if (attachedObject.GetComponent<LEditor_OnTileObject>() != null)
            {
                LEditor_OnTileObject onTile = attachedObject.GetComponent<LEditor_OnTileObject>();
                pickUpButton.gameObject.SetActive(true);
                pickUpButton.onClick.AddListener(onTile.theTileSetOn.PickUpObjectOnThis);
                pickUpButton.onClick.AddListener(UnAttach);
                Debug.Log("HeyHeyHey");
            }
        }
    }


    public void CheckClickAndAttachTo(Edtior_GameBoardObject newO, int clickedId)
    {
        if (LevelEditor.Instance.currentEditingState == LevelEditor.editingState.mapBuilding &&
            LevelEditor.Instance.currentState == LevelEditor.state.editing)
        {
            if (newO == null)
            {
                LEditor_TileObject clicked = LevelEditor.Instance.EditingGameboard.GetEditingTile(clickedId).GetComponent<LEditor_TileObject>();
                Debug.Log("SelectedUI get the clickedTile" + clicked.TileId);

                if (clicked.GetComponent<LEditor_SelectableObject>() != null &&
                    attachedObject != clicked.GetComponent<LEditor_SelectableObject>())
                {
                    attachedObject = clicked.GetComponent<LEditor_SelectableObject>();
                    transform.position = attachedObject.transform.position;
                    transform.parent = attachedObject.transform;
                    Display();
                    Debug.Log(clicked.name + clicked.TileId + " should be attached.");
                }
                else if (clicked.objectOn != null && clicked.objectOn.GetComponent<LEditor_SelectableObject>() != null &&
                         attachedObject != clicked.objectOn.GetComponent<LEditor_SelectableObject>())
                {
                    attachedObject = clicked.objectOn.GetComponent<LEditor_SelectableObject>();
                    transform.position = attachedObject.transform.position;
                    transform.parent = attachedObject.transform;
                    Display();
                    Debug.Log(clicked.name + clicked.TileId + " should be attached.");
                }
                else
                {
                    if (attachedObject != null)
                    {
                        Debug.Log("Unattach from tile" + clicked.TileId + ".");
                    }
                    UnAttach();
                }
            }
        }
    }

    public void PackageButtons()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<Button>() != null)
            {
                buttons.Add(transform.GetChild(i).GetComponent<Button>());
            }
        }
    }

    public void UnAttach()
    {

        setConnectionButton.onClick.RemoveAllListeners();
        setConnectionButton.gameObject.SetActive(false);

        pickUpButton.onClick.RemoveAllListeners();
        pickUpButton.gameObject.SetActive(false);

        if (attachedObject != null)
        {
            cancelButton.onClick.RemoveListener(attachedObject.UnSelectThis);
        }
        attachedObject = null;
        transform.parent = Hover.Instance.transform;
        this.gameObject.SetActive(false);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }


    void StateCheck()
    {
        if (LevelEditor.Instance.currentEditingState == LevelEditor.editingState.settingConnection ||
            LevelEditor.Instance.currentEditingState == LevelEditor.editingState.settingPortals)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                if (buttons[i] != cancelButton)
                {
                    buttons[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void StartSettingConnection()
    {
        if (LevelEditor.Instance.clickedBoardObjectButton == null)
        {
            if (attachedObject.GetComponent<LEditor_ConnectableObject>() != null)
            {
                LevelEditor.Instance.currentEditingState = LevelEditor.editingState.settingConnection;

            }
            else if (attachedObject.GetComponent<LEditor_PortableObject>() != null)
            {
                LevelEditor.Instance.currentEditingState = LevelEditor.editingState.settingPortals;
            }
        }
    }
}
