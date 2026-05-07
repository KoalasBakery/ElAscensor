using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    private List<ItemData> items = new List<ItemData>();

    // Evento que avisa a la UI cuando el inventario cambia
    public UnityEvent onInventoryChanged;

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

    public bool AddItem(ItemData item)
    {
        // Si es unico(el bool ese) y ya lo tenemos, no se agrega
        if (item.isUnique && items.Contains(item))
        {
            Debug.Log("Ya tienes este ítem: " + item.itemName);
            return false;
        }

        items.Add(item);
        Debug.Log("Ítem agregado: " + item.itemName);
        onInventoryChanged?.Invoke();
        return true;
    }

    public bool RemoveItem(ItemData item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            onInventoryChanged?.Invoke();
            return true;
        }
        return false;
    }

    public bool HasItem(ItemData item)
    {
        return items.Contains(item);
    }

    public List<ItemData> GetItems()
    {
        return items;
    }
}