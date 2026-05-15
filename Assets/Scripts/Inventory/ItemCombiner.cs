using UnityEngine;

/*
 * ---------------------------------------------------------------
 *                       ITEM COMBINER
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * Maneja la combinacion automatica de items.
 * Cuando el jugador interactua con un objeto del mundo,
 * el sistema verifica si hay un item en el inventario
 * que se combine con ese objeto.
 *
 * La combinacion ocurre automaticamente sin abrir ningun menu. (por ahora asi es en Sally Face)
 * El personaje dice una linea de dialogo y el item cambia
 * su apariencia y descripcion.
 *
 * COMO USARLO DESDE OTROS SCRIPTS:
 *   ItemCombiner.Instance.TryCombine(itemRequerido);
 *
 * SETUP EN UNITY:
 *   Crear un GameObject llamado "ItemCombiner" y agregar
 *   este script. No necesita referencias en el Inspector.
 * ---------------------------------------------------------------
 */

public class ItemCombiner : MonoBehaviour
{
    public static ItemCombiner Instance { get; private set; }

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

    // Intenta combinar un item del inventario con el item objetivo
    public bool TryCombine(ItemData targetItem)
    {
        if (targetItem == null) return false;

        // Buscar en el inventario si hay algo que se combine con targetItem
        foreach (ItemData inventoryItem in Inventory.Instance.GetItems())
        {
            if (inventoryItem.combinesWith == targetItem)
            {
                ExecuteCombine(inventoryItem, targetItem);
                return true;
            }
        }

        return false;
    }

    private void ExecuteCombine(ItemData itemA, ItemData itemB)
    {
        // Mostrar dialogo de combinacion si tiene key
        if (!string.IsNullOrEmpty(itemA.combineDialogueKey) &&
            DialogueManager.Instance != null)
        {
            // TODO: Conectar con sistema de dialogo para mostrar
            // linea de combinacion del personaje
            Debug.Log($"Combinando: {itemA.itemName} + {itemB.itemName}");
        }

        // Quitar los items originales
        Inventory.Instance.RemoveItem(itemA);
        Inventory.Instance.RemoveItem(itemB);

        // Agregar el item resultante
        if (itemA.combineResult != null)
            Inventory.Instance.AddItem(itemA.combineResult);

        Debug.Log($"Resultado: {itemA.combineResult?.itemName}");
    }
}