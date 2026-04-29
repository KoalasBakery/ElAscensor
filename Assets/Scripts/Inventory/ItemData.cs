using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("Item Info")]
    public string itemName;
    [TextArea(2, 4)]
    public string description;
    public Sprite icon;

    [Header("Settings")]
    public bool isUnique = true; // Si solo puede haber uno en el inventario
}