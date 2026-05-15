using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("Inventario")]
    [SerializeField] private RectTransform inventoryPanel;
    [SerializeField] private Transform itemsContainer;
    [SerializeField] private GameObject itemSlotPrefab;

    [Header("Mision")]
    [SerializeField] private RectTransform missionPanel;
    [SerializeField] private TextMeshProUGUI missionText;

    [Header("Inspeccion")]
    [SerializeField] private ItemInspectUI itemInspectUI;

    [Header("Animacion Inventario")]
    [SerializeField] private float slideSpeed = 8f;
    [SerializeField] private float hiddenY = 150f;
    [SerializeField] private float visibleY = 0f;

    [Header("Animacion Mision")]
    [SerializeField] private float missionHiddenX = -420f;
    [SerializeField] private float missionVisibleX = 10f;
    //[SerializeField] private float missionDelay = 0.3f; // espera a que baje el inventario

    private bool isOpen = false;
    private bool isAnimating = false;
    private List<GameObject> activeSlots = new List<GameObject>();

    private void Start()
    {
        inventoryPanel.anchoredPosition = new Vector2(0, hiddenY);

        if (missionPanel != null)
            missionPanel.anchoredPosition = new Vector2(missionHiddenX,
                missionPanel.anchoredPosition.y);

        Inventory.Instance.onInventoryChanged.AddListener(RefreshUI);
        RefreshUI();
    }

    // --- TOGGLE --- //
    public void ToggleInventory()
    {
        if (isAnimating) return;

        if (itemInspectUI != null && itemInspectUI.IsOpen)
        {
            itemInspectUI.Hide();
            return;
        }

        isOpen = !isOpen;
        StartCoroutine(AnimateInventory(isOpen));
    }

    private IEnumerator AnimateInventory(bool open)
    {
        isAnimating = true;

        // Lanzar animacion de mision AL MISMO TIEMPO
        if (missionPanel != null)
            StartCoroutine(AnimateMission(open));

        // Animar inventario (arriba/abajo)
        float targetY = open ? visibleY : hiddenY;
        float currentY = inventoryPanel.anchoredPosition.y;

        while (Mathf.Abs(currentY - targetY) > 0.1f)
        {
            currentY = Mathf.Lerp(currentY, targetY, Time.deltaTime * slideSpeed);
            inventoryPanel.anchoredPosition = new Vector2(0, currentY);
            yield return null;
        }

        inventoryPanel.anchoredPosition = new Vector2(0, targetY);
        isAnimating = false;
    }

    private IEnumerator AnimateMission(bool show)
    {
        float targetX = show ? missionVisibleX : missionHiddenX;
        float currentX = missionPanel.anchoredPosition.x;
        float currentY = missionPanel.anchoredPosition.y;

        while (Mathf.Abs(currentX - targetX) > 0.1f)
        {
            currentX = Mathf.Lerp(currentX, targetX, Time.deltaTime * slideSpeed);
            missionPanel.anchoredPosition = new Vector2(currentX, currentY);
            yield return null;
        }
        missionPanel.anchoredPosition = new Vector2(targetX, currentY);
    }

    // --- REFRESH --- //
    public void RefreshUI()
    {
        foreach (var slot in activeSlots)
            Destroy(slot);
        activeSlots.Clear();

        foreach (ItemData item in Inventory.Instance.GetItems())
        {
            if (item.isNote) continue;

            GameObject slot = Instantiate(itemSlotPrefab, itemsContainer);
            activeSlots.Add(slot);

            Image icon = slot.transform.Find("Icon")?.GetComponent<Image>();
            if (icon != null)
            {
                bool isCombined = Inventory.Instance.IsItemCombined(item);
                icon.sprite = (isCombined && item.combinedIcon != null) ?
                    item.combinedIcon : item.icon;
            }

            Button button = slot.GetComponent<Button>();
            if (button != null)
            {
                ItemData capturedItem = item;
                button.onClick.AddListener(() => OnItemClicked(capturedItem));
            }
        }
    }

    // --- CLICK EN ITEM --- //
    private void OnItemClicked(ItemData item)
    {
        isOpen = false;
        StartCoroutine(AnimateInventory(false));

        if (itemInspectUI != null)
            itemInspectUI.Show(item);
    }

    // --- MISION --- //
    public void SetMissionText(string text)
    {
        if (missionText != null)
            missionText.text = text;
    }

    public bool IsOpen => isOpen;
}