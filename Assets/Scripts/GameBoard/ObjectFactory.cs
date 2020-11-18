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
    List<Sprite> themeSprites;

    public LEditor_TileObject GetTile(int factoryId)
    {
        return themeTiles[factoryId];
    }

    public LEditor_TileObject GetTileByType(string type)
    {
        LEditor_TileObject returningTile = null;

        for (int i = 0; i < themeTiles.Count; i++)
        {
            if (themeTiles[i].theType.ToString() == type)
            {
                returningTile = themeTiles[i];
            }
        }
        return returningTile;
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

    public Sprite GetObjectSprite(int factoryId)
    {
        return themeSprites[factoryId];
    }

    public int GetObjectSprite(Sprite sprite)
    {
        for (int i = 0; i < themeSprites.Count; i++)
        {
            if (sprite == themeSprites[i])
            {
                return i;
            }
        }

        return -1;
    }

}
