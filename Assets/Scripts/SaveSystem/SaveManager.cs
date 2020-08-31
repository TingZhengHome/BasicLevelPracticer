using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveManager : MonoBehaviour {

    public Text saveName;
    public GameObject loadButton;


    public string[] saveFiles;
    public void GetLoadFiles()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/saves"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves");
        }

        saveFiles = Directory.GetFiles(Application.persistentDataPath + "/saves");
    }


}
