using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform itemsContainer;
    [SerializeField] private GameObject itemSlotPrefab;

    private bool isOpen = false;

    private void Start()
    {
        inventoryPanel.SetActive(false);
        Inventory.Instance.onInventoryChanged.AddListener(RefreshUI);
    }

    public void ToggleInventory()
    {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);

        if (isOpen)
        {
            RefreshUI();
            InputManager.Instance.SwitchToUI();
            PlayerController player = FindAnyObjectByType<PlayerController>();
            player?.SetMovementEnabled(false);
        }
        else
        {
            InputManager.Instance.SwitchToGameplay();
            PlayerController player = FindAnyObjectByType<PlayerController>();
            player?.SetMovementEnabled(true);
        }
    }

    private void RefreshUI()
    {
        // Se lImpian los slots anteriores
        foreach (Transform child in itemsContainer)
            Destroy(child.gameObject);

        // sE crea un slot por cada ítem
        foreach (ItemData item in Inventory.Instance.GetItems())
        {
            GameObject slot = Instantiate(itemSlotPrefab, itemsContainer);

            // Icono
            Image icon = slot.transform.Find("Icon").GetComponent<Image>();
            if (icon != null && item.icon != null)
                icon.sprite = item.icon;

            // Nombre
            TextMeshProUGUI nameText = slot.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
            if (nameText != null)
                nameText.text = item.itemName;

            // Descripcion
            TextMeshProUGUI descText = slot.transform.Find("Description").GetComponent<TextMeshProUGUI>();
            if (descText != null)
                descText.text = item.description;
        }
    }
}