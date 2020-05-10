using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingLayerSystem : Singleton<SortingLayerSystem>
{

    void Update()
    {

    }

    public void UpdateLayer(SpriteRenderer objectSprite)
    {
        if (objectSprite.tag == "player")
            objectSprite.sortingOrder = ((int)objectSprite.transform.position.y * -10) + 1;
        else
            objectSprite.sortingOrder = (int)objectSprite.transform.position.y * -10;
    }
}
