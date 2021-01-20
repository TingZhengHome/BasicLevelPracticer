using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SerializationManagger
{
    public static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        return formatter;
    }

    public static bool Save(string saveName, object campaignData)
    {
        BinaryFormatter formatter = GetBinaryFormatter();

        if (!Directory.Exists(Application.persistentDataPath + "/saves"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves");
        }

        string path = Application.persistentDataPath + "/saves/" + saveName + ".save";

        if (File.Exists(path))
        {
            Debug.Log("Same name file already exists in the path.");
            File.Delete(path);

            if (!File.Exists(path))
            {
                Debug.Log(string.Format("Old save file {0} has been deleted", saveName));
            }
        }
        FileStream file = File.Create(path);

        formatter.Serialize(file, campaignData);

        file.Close();

        return true;
    }

    public static object Load(string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        BinaryFormatter formatter = GetBinaryFormatter();

        FileStream file = File.Open(path, FileMode.Open);

        try
        {
            object save = formatter.Deserialize(file);
            file.Close();
            return save;
        }
        catch
        {
            Debug.LogErrorFormat("Failed to load file at {0}", path);

            return null;
        }
    }

    public static void Replace(string newFileName, string destinationFileName)
    {
        string oldFilePath = Application.persistentDataPath + "/saves/" + destinationFileName + ".save";
        string newFilePath = Application.persistentDataPath + "/saves/" + newFileName + ".save";
        string backUpPath = Application.persistentDataPath + "/backups/" + destinationFileName + ".save";

        if (File.Exists(oldFilePath) && File.Exists(newFilePath) )
        {
            if (oldFilePath != newFilePath)
            {
                File.Replace(newFilePath, oldFilePath, backUpPath);
                File.Delete(newFilePath);
            }
        }
    }

    public static void Delete(string dataName)
    {
        string path = Application.persistentDataPath + "/saves/" + dataName + ".save";

        Debug.Log(dataName + " is deleted.");

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log(dataName + " is deleted.");
        }
    }
}
