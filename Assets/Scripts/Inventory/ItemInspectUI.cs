using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;

/*
 * ---------------------------------------------------------------
 *                      ITEM INSPECT UI
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * Muestra la vista de inspeccion de un item al clickearlo
 * en el inventario. Igual que Sally Face — imagen grande
 * del item y descripcion a la derecha.
 * Si el item es consumible muestra el boton de Consumir.
 * ---------------------------------------------------------------
 */

public class ItemInspectUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private RectTransform inspectPanel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private TextMeshProUGUI usesText;
    [SerializeField] private Button consumeButton;
    [SerializeField] private Button closeButton;

    [Header("Animacion")]
    [SerializeField] private float animSpeed = 6f;
    [SerializeField] private float offsetY = 80f;
    [SerializeField] private float centerY = 0f;

    private Vector2 hiddenPos;
    private Vector2 visiblePos;
    private ItemData currentItem;

    public bool IsOpen { get; private set; }

    private void Start()
    {
        hiddenPos = new Vector2(0, centerY - offsetY);
        visiblePos = new Vector2(0, centerY);

        inspectPanel.anchoredPosition = hiddenPos;

        if (canvasGroup == null)
            canvasGroup = inspectPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = inspectPanel.gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        inspectPanel.gameObject.SetActive(false);

        closeButton?.onClick.AddListener(Hide);
        consumeButton?.onClick.AddListener(OnConsumePressed);
    }

    public void Show(ItemData item)
    {
        if (item == null) return;

        currentItem = item;
        IsOpen = true;
        inspectPanel.gameObject.SetActive(true);
        canvasGroup.blocksRaycasts = true;

        bool isCombined = Inventory.Instance.IsItemCombined(item);

        // Imagen
        Sprite displayImage = item.inspectImage != null ? item.inspectImage : item.icon;
        if (isCombined && item.combinedIcon != null)
            displayImage = item.combinedIcon;
        if (itemImage != null)
            itemImage.sprite = displayImage;

        // Nombre
        if (itemNameText != null)
            itemNameText.text = item.itemName;

        // Descripcion
        if (itemDescriptionText != null)
            itemDescriptionText.text =
                (isCombined && !string.IsNullOrEmpty(item.combinedDescription)) ?
                item.combinedDescription : item.description;

        // Usos restantes
        UpdateUsesText(item);

        // Boton consumir
        if (consumeButton != null)
            consumeButton.gameObject.SetActive(item.isConsumable);

        StopAllCoroutines();
        StartCoroutine(AnimateShow());
    }

    private void UpdateUsesText(ItemData item)
    {
        if (usesText == null) return;

        if (!item.isConsumable)
        {
            usesText.gameObject.SetActive(false);
            return;
        }

        usesText.gameObject.SetActive(true);

        int uses = Inventory.Instance.GetRemainingUses(item);

        if (item.maxUses == -1)
            usesText.text = "Usos: Infinitos";
        else
            usesText.text = $"Usos: {uses}/{item.maxUses}";
    }

    private void OnConsumePressed()
    {
        if (currentItem == null) return;

        bool consumed = Inventory.Instance.ConsumeItem(currentItem);

        if (consumed)
        {
            // Si se agoto el item cerramos el panel
            if (!Inventory.Instance.HasItem(currentItem))
            {
                Hide();
                return;
            }

            // Si quedan usos actualizamos el texto
            UpdateUsesText(currentItem);
        }
    }

    public void Hide()
    {
        StopAllCoroutines();
        StartCoroutine(AnimateHide());
    }

    private IEnumerator AnimateShow()
    {
        inspectPanel.anchoredPosition = hiddenPos;
        canvasGroup.alpha = 0f;

        while (canvasGroup.alpha < 0.99f)
        {
            canvasGroup.alpha = Mathf.Lerp(
                canvasGroup.alpha, 1f, Time.deltaTime * animSpeed);

            inspectPanel.anchoredPosition = Vector2.Lerp(
                inspectPanel.anchoredPosition, visiblePos,
                Time.deltaTime * animSpeed);

            yield return null;
        }

        canvasGroup.alpha = 1f;
        inspectPanel.anchoredPosition = visiblePos;
    }

    private IEnumerator AnimateHide()
    {
        while (canvasGroup.alpha > 0.01f)
        {
            canvasGroup.alpha = Mathf.Lerp(
                canvasGroup.alpha, 0f, Time.deltaTime * animSpeed);

            inspectPanel.anchoredPosition = Vector2.Lerp(
                inspectPanel.anchoredPosition, hiddenPos,
                Time.deltaTime * animSpeed);

            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        inspectPanel.anchoredPosition = hiddenPos;
        inspectPanel.gameObject.SetActive(false);
        IsOpen = false;
        currentItem = null;
    }
}