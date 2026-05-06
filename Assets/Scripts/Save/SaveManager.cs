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
    private static string savePath = Application.persistentDataPath + "/save.dat";
    #endregion


    #region Class Methods
    private static void Load()
    {
        if (File.Exists(savePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(savePath, FileMode.Open);
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
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(savePath);
        bf.Serialize(file, data);
        file.Close();
    }
    public static void DeleteSaved()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
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


}
/*
 * Clase con los datos que se guardaran del juego
*/
public class SaveData
{
    public List<string> eventsKeys = new List<string>();

}