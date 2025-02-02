using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class SaveData
{
    public bool[] isActive;
    public int[] highScores;
    public int[] stars;
}

public class GameData : MonoBehaviour
{
    public static GameData gameData;

    public SaveData saveData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        
        if (gameData == null)
        {
            DontDestroyOnLoad(gameObject);
            gameData = this;
        }
        else
        {
            Destroy(gameObject);
        }
        Load();
    }

    private void Start()
    {
      
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnDisable()
    {
        Save();
    }

    public void Save()
    {
        var formatter = new BinaryFormatter();
        var file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.OpenOrCreate);
        SaveData data = new SaveData();
        data = saveData;
        formatter.Serialize(file, data);
        file.Close();
        Debug.Log("saved");
    }


    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/player.dat"))
        {
            var formatter = new BinaryFormatter();
            var file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.OpenOrCreate);
            saveData = formatter.Deserialize(file) as SaveData;
            file.Close();
        }
        else
        {
            saveData = new SaveData();
            saveData.isActive = new bool[100];
            saveData.stars = new int[100];
            saveData.highScores = new int[100];
            saveData.isActive[0] = true;
        }
        
    }

    private void OnApplicationQuit()
    {
        Save();
    }
}