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
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/save.dat");
        
        bf.Serialize(file, saveDat);

        file.Close();
        Debug.Log("Game Saved.");
    }

    public generalSAVE Load()
    {

        string path = Application.persistentDataPath + "/save.dat";
        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/save.dat", FileMode.Open);

            generalSAVE saveDat = (generalSAVE)bf.Deserialize(file);
            file.Close();
            Debug.Log("Game Loaded.");

            return saveDat;
        }
        Debug.LogWarning("No save file found.");
        return null;
    }

    }
