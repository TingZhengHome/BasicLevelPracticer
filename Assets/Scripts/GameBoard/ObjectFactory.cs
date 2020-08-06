using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFactory : MonoBehaviour
{

    [SerializeField]
    List<LEditor_TileObject> themeTiles;

    [SerializeField]
    List<LEditor_OnTileObject> themeOnTiles;

    [SerializeField]
    List<InteractableObject> themeInteractableObjects;

    public LEditor_TileObject GetTile(int factoryId)
    {
        return themeTiles[factoryId];
    }

    public int GetTileFactoryId(LEditor_TileObject theTile)
    {
        for (int i = 0; i < themeTiles.Count; i++)
        {
            if (theTile.name == themeTiles[i].name)
            {
                return i;
            }
        }

        return -1;
    }


    public LEditor_OnTileObject GetOnTile(int factoryId)
    {
        return themeOnTiles[factoryId];
    }

    public int GetOnTileFactoryId(LEditor_OnTileObject theOnTile)
    {
        for (int i = 0; i < themeOnTiles.Count; i++)
        {
            if (theOnTile.name == themeOnTiles[i].name)
            {
                return i;
            }
        }

        return -1;
    }


    public InteractableObject GetInteractable(int factoryId)
    {
        return themeInteractableObjects[factoryId];
    }

    public int GetInteractableFactoryId(InteractableObject theInteractable)
    {
        for (int i = 0; i < themeInteractableObjects.Count; i++)
        {
            if (theInteractable == themeInteractableObjects[i])
            {
                return i;
            }
        }

        return -1;
    }


}
