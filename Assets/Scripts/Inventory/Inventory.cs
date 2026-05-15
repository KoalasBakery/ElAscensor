using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * ---------------------------------------------------------------
 *                         INVENTORY
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * Singleton que guarda los items del jugador.
 * Soporta items normales y notas por separado.
 * Trackea que items han sido combinados.
 * ---------------------------------------------------------------
 */

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    private List<ItemData> items = new List<ItemData>();
    private List<ItemData> notes = new List<ItemData>();
    private List<ItemData> combinedItems = new List<ItemData>();

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

    // --- ITEMS --- //
    public bool AddItem(ItemData item)
    {
        if (item == null) return false;

        if (item.isUnique && items.Contains(item))
        {
            Debug.Log("Ya tienes: " + item.itemName);
            return false;
        }

        if (item.isNote)
        {
            notes.Add(item);
            Debug.Log("Nota agregada: " + item.itemName);
        }
        else
        {
            items.Add(item);
            Debug.Log("Item agregado: " + item.itemName);
        }

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

        if (notes.Contains(item))
        {
            notes.Remove(item);
            onInventoryChanged?.Invoke();
            return true;
        }

        return false;
    }

    public bool HasItem(ItemData item)
    {
        return items.Contains(item) || notes.Contains(item);
    }

    // --- COMBINADOS --- //
    public void MarkAsCombined(ItemData item)
    {
        if (!combinedItems.Contains(item))
        {
            combinedItems.Add(item);
            onInventoryChanged?.Invoke();
        }
    }

    public bool IsItemCombined(ItemData item)
    {
        return combinedItems.Contains(item);
    }

    // --- GETTERS --- //
    public List<ItemData> GetItems() => items;
    public List<ItemData> GetNotes() => notes;
}