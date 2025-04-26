using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance == null) //singleton
        {
            DontDestroyOnLoad(gameObject); //protects data between scenes
            instance = this;
        }
        else if (instance != this) Destroy(gameObject); //destroys new instances
    }

    public void Save(generalSAVE saveDat) //save by reading/writing file
    {
        if (File.Exists(Application.persistentDataPath + "/save.dat")) //clears existing saveDat
            File.Delete(Application.persistentDataPath + "/save.dat");
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/save.dat");
        
        bf.Serialize(file, saveDat);

        file.Close();
        Debug.Log("Success: Save");
    }

    public generalSAVE Load()
    {
        if (!File.Exists(Application.persistentDataPath + "/save.dat")) return null; //no file to load
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/save.dat", FileMode.Open);

        generalSAVE saveDat = (generalSAVE)bf.Deserialize(file);
        file.Close();

        Debug.Log("Success: Load");
        return saveDat;
    }
    public bool SaveExists()//make sure there is a save
    {
        return File.Exists(Application.persistentDataPath + "/save.dat");
    }
    public void DeleteSave()
    {
        string savePath = Application.persistentDataPath + "/save.dat";

        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Save file deleted.");
        }
        else
        {
            Debug.LogWarning("No save file to delete.");
        }
    }

}
