using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Collections.Generic;

/*
 * Clase estatica para el guardado de informacion del juego
 */

public static class SaveManager 
{
    #region Parameters
    private static SaveData data;
    private static string savePath = Application.persistentDataPath + "/save";
    public static string saveSlotConst= "SaveSlot";
    #endregion


    #region Class Methods
    public static bool SaveExist(string _slotName)
    {

        string newPath = savePath + _slotName+".dat";
       
        return File.Exists(newPath);

    }
    private static void Load()
    {
        string newPath = savePath;
        if (PlayerPrefs.HasKey(saveSlotConst))
        {
            newPath = savePath+ PlayerPrefs.GetString(saveSlotConst)+".dat";
        }
        if (File.Exists(newPath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(newPath, FileMode.Open);
            data = (SaveData)bf.Deserialize(file);
            file.Close();
        }
        else
        {
            data = new SaveData();
        }
    }
    private static void Save()
    {
        string newPath = savePath; ;
        if (PlayerPrefs.HasKey(saveSlotConst))
        {
            newPath = savePath + PlayerPrefs.GetString(saveSlotConst) + ".dat";
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(newPath);
        bf.Serialize(file, data);
        file.Close();
    }
    public static void DeleteSaved()
    {
        string newPath = savePath;
        if (PlayerPrefs.HasKey(saveSlotConst))
        {
            newPath = savePath + PlayerPrefs.GetString(saveSlotConst) + ".dat";
        }


        if (File.Exists(newPath))
        {
            File.Delete(newPath);
        }
        data = null;
    }

    #endregion


    #region EventKeys
    //Ejemplo de momento
    public static void SaveEventKey(string _eventKey)
    {
        if (data == null) Load();
        if (!data.eventsKeys.Contains(_eventKey))
        {
            data.eventsKeys.Add(_eventKey);
            Save();
        }
    }
    public static bool IsEventKeySaved(string eventKey)
    {
        if (data == null) Load();
        return data.eventsKeys.Contains(eventKey);
    }
    #endregion

    public static void SaveSceneName(string _sceneName)
    {
        if (data == null) Load();
        data.sceneName = _sceneName;
        Save();
    }   
    public static string GetSceneName()
    {
        if (data == null) Load();
        return data.sceneName;
    }

}
/*
 * Clase con los datos que se guardaran del juego
*/
[Serializable]
public class SaveData
{
    public List<string> eventsKeys = new List<string>();
    public string sceneName;
    public int playerPositionX, playerPositionY, playerPositionZ;



}