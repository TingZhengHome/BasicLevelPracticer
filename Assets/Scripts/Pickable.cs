using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickable : InteractableProperty {

    bool isConidtioned;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    public override void GameUpdate()
    {
    }

    public override void Initialize(GameBoardObject gameBoardObject, InteractableObject interactableObject)
    {
        base.Initialize(gameBoardObject, interactableObject);

        LEditor_OnTileObject onEditor = gameBoardObject.GetComponent<LEditor_OnTileObject>();

        IsOnTile = true;

        PickableObject pickableObject = interactableObject as PickableObject;
        isConidtioned = pickableObject.isConditioned;
    }

    public override void Interact(Player interacter)
    {
        PickedUp(interacter);
    }

    void PickedUp(Player interacter)
    {
        interacter.PickUpItem(this);
        GetComponent<OnTileObject>().theTileSetOn.objectOnThis = null;
        Destroy(this.gameObject);
    }
}
