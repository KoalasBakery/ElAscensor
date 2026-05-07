using System.Collections.Generic;
using UnityEngine;

//Esta verga es un diccionario global de bools con nombre (para desbloquear dialogos, puertas o progresion no se)
public class FlagManager : MonoBehaviour
{
    public static FlagManager Instance { get; private set; }

    private Dictionary<string, bool> flags = new Dictionary<string, bool>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetFlag(string key, bool value)
    {
        flags[key] = value;
        Debug.Log($"Flag '{key}' = {value}");
    }

    public bool GetFlag(string key)
    {
        return flags.TryGetValue(key, out bool value) && value;
    }

    public bool HasFlag(string key)
    {
        return flags.ContainsKey(key);
    }
}