using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInspectUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private RectTransform inspectPanel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private Button closeButton;

    [Header("Animacion")]
    [SerializeField] private float animSpeed = 6f;
    [SerializeField] private float offsetY = 80f; // cuanto sube desde abajo
    [SerializeField] private float centerY = 0f;  // posicion final en centro

    public bool IsOpen { get; private set; }

    private Vector2 hiddenPos;
    private Vector2 visiblePos;

    private void Start()
    {
        // Posicion oculta = un poco abajo del centro
        hiddenPos = new Vector2(0, centerY - offsetY);
        visiblePos = new Vector2(0, centerY);

        inspectPanel.anchoredPosition = hiddenPos;

        // Empieza transparente e invisible
        if (canvasGroup == null)
            canvasGroup = inspectPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = inspectPanel.gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        inspectPanel.gameObject.SetActive(false);

        if (closeButton != null)
            closeButton.onClick.AddListener(Hide);
    }

    public void Show(ItemData item)
    {
        if (item == null) return;

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

        StopAllCoroutines();
        StartCoroutine(AnimateShow());
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
    }
}