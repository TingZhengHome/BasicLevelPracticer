using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : InteractableProperty
{
    bool isExit;

    bool isConditioned;

    Portal connectedPortal;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    public override void GameUpdate()
    {
        if (spriterenderer.sortingLayerName == "Tile")
        {
            spriterenderer.sortingOrder = (int)transform.position.y * -10 + 2;
        }
    }

    public override void Initialize(GameBoardObject gameBoardObject)
    {
        base.Initialize(gameBoardObject);

        LEditor_PortalObject onEditorPortal = GetComponent<LEditor_PortalObject>();
        if (onEditorPortal.connectedPortable != null)
        {
            isExit = onEditorPortal.isExit;
            connectedPortal = GetComponent<LEditor_PortalObject>().connectedPortable.GetComponent<Portal>();
        }

        if (spriterenderer.sortingLayerName == "Tile")
        {
            spriterenderer.sortingOrder = (int)transform.position.y * -10 + 2;
        }
    }

    public override void Interact(Player interacter)
    {
        float distance = (transform.position - interacter.transform.position).magnitude;

        Debug.Log("Distance to interacted portal is " + distance);

        if (distance <= 0.37f && interacter.transported == false)
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
            interacter.isSliding = false;
            interacter.GetComponent<OnTileObject>().theTileSetOn.objectOnThis = null;
            interacter.GetComponent<OnTileObject>().theTileSetOn = connectedPortal.GetComponent<OnTileObject>().theTileSetOn;
        }
    }
}

