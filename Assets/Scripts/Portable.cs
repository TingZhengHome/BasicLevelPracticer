using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portable : InteractableProperty
{
    bool isExit;

    bool isConditioned;

    Portable connectedPortal;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    public override void GameUpdate()
    {
    }

    public override void Initialize(GameBoardObject gameBoardObject, InteractableObject interactable)
    {
        base.Initialize(gameBoardObject, interactable);
        PortableObject portableO = interactable as PortableObject;

        isExit = portableO.isExit;
        isConditioned = portableO.isConditioned;

        LEditor_PortableObject onEditorPortable = GetComponent<LEditor_PortableObject>();
        if (onEditorPortable.connectedPortable != null)
        {
            connectedPortal = GetComponent<LEditor_PortableObject>().connectedPortable.GetComponent<Portable>();
        }
    }

    public override void Interact(Player interacter)
    {
        float distance = (transform.position - interacter.transform.position).magnitude;

        Debug.Log("Distance to interacted portal is " + distance);

        if (distance <= 0.2f && interacter.transported == false)
        {
            Transport(interacter);
        }
        else if (distance >= 0.6f)
        {
            interacter.transported = false;
        }
    }

    public void Transport(Player interacter)
    {
        if (connectedPortal != null)
        {
            interacter.transform.position = connectedPortal.transform.position;
            interacter.transported = true;
            interacter.isLerping = false;
        }
    }
}

