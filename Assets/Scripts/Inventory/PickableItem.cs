using UnityEngine;

public class PickableItem : Interactable
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private bool destroyOnPickup = true;

    public override void Interact()
    {
        if (itemData == null) return;

        bool added = Inventory.Instance.AddItem(itemData);

        if (added)
        {
            // Opcional: mostrar dialogo al recoger
            if (DialogueManager.Instance != null)
            {
                Debug.Log("Agarraste: " + itemData.itemName);
            }

            if (destroyOnPickup)
                Destroy(gameObject);
        }
    }

    public override void OnPlayerEnter()
    {
        Debug.Log("Puedes agarrar: " + itemData.itemName);
    }
}