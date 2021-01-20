using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T :MonoBehaviour {

    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
            }
            

            return instance;
        }
    }

    public void DeleteRedundancy()
    {
        if (FindObjectsOfType<T>().Length >= 2)
        {
            for (int i = 0; i < FindObjectsOfType<T>().Length - 1; i++)
            {
                Destroy(FindObjectsOfType<T>()[i].gameObject);
            }
        }
    }
}
