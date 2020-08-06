using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStorage : MonoBehaviour {

    string savePath;


    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "GameLevel");
    }

    public void Save(GameBoard level, int version)
    {
        using (var writer = new BinaryWriter(File.Open(savePath, FileMode.Create)))
        {
            writer.Write(version);
            //level.Save(writer);
        }
    }

    public void Load(GameBoard level)
    {
        byte[] data = File.ReadAllBytes(savePath);
        var reader = new BinaryReader(new MemoryStream(data));
        //level.Load(reader, -reader.ReadInt32());
    }



}
