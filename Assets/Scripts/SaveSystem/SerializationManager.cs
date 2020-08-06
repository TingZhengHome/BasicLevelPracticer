using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SerializationManagger
{

    static string path = Application.persistentDataPath + "/save.save";

    public static string Path
    {
        get
        {
            return path;
        }

        private set
        {
            path = value;
        }
    }

    public static bool Save(string saveName, object saveData)
    {
        BinaryFormatter formatter = GetBinaryFormatter();

        if (!Directory.Exists(Application.persistentDataPath + "/save"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/save");
        }

        //string path = Application.persistentDataPath + "/save" + saveName + ".save";

        FileStream file = File.Create(Path);

        formatter.Serialize(file, saveData);

        file.Close();

        return true;
    }

    public static object Load()
    {
        if (!File.Exists(Path))
        {
            return null;
        }

        BinaryFormatter formatter = GetBinaryFormatter();

        FileStream file = File.Open(Path, FileMode.Open);

        try
        {
            object save = formatter.Deserialize(file);
            file.Close();
            return save;

        }
        catch
        {
            Debug.LogErrorFormat("Failed to load file at {0}", Path);

            return null;
        }
    }


    public static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        return formatter;
    }


}
