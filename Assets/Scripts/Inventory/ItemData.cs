using UnityEngine;

/*
 * ---------------------------------------------------------------
 *                         ITEM DATA
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * ScriptableObject que define un item del inventario.
 * Soporta dos estados: normal y combinado.
 * Las combinaciones ocurren automaticamente al interactuar
 * con objetos del mundo que requieren cierto item.
 *
 * SETUP:
 * Clic derecho en Assets -> Inventory -> Item
 * ---------------------------------------------------------------
 */

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("Info Base")]
    [Tooltip("Nombre del item que aparece en la UI")]
    public string itemName;

    [Tooltip("Descripcion que aparece al inspeccionar")]
    [TextArea(2, 4)]
    public string description;

    [Tooltip("Icono del item en el inventario")]
    public Sprite icon;

    [Tooltip("Imagen grande que aparece al inspeccionar")]
    public Sprite inspectImage;

    [Header("Combinacion")]
    [Tooltip("Item con el que se combina este item")]
    public ItemData combinesWith;

    [Header("Consumible")]
    [Tooltip("Si el item se puede consumir")]
    public bool isConsumable = false;

    [Tooltip("Key de localizacion del mensaje al consumir")]
    public string consumeMessageKey;

    [Tooltip("Cuanta cordura da al consumirse (puede ser negativo)")]
    public float sanityEffect = 0f;

    [Tooltip("Usos disponibles, -1 = usos infinitos")]
    public int maxUses = 1;

    [Tooltip("Flag que activa al consumirse")]
    public string consumeFlagKey;

    [Tooltip("Flag que activa cuando se acaban los usos")]
    public string depletedflagKey;

    [Tooltip("Item resultante de la combinacion")]
    public ItemData combineResult;

    [Tooltip("Key de dialogo que dice el personaje al combinar")]
    public string combineDialogueKey;

    [Header("Estado Combinado")]
    [Tooltip("Icono cuando el item ya fue combinado")]
    public Sprite combinedIcon;

    [Tooltip("Descripcion cuando el item ya fue combinado")]
    [TextArea(2, 4)]
    public string combinedDescription;

    [Header("Settings")]
    [Tooltip("Si solo puede haber uno en el inventario")]
    public bool isUnique = true;

    [Tooltip("Si es una nota/documento va a pestana especial")]
    public bool isNote = false;
}