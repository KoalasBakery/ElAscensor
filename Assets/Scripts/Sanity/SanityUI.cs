using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
 * ---------------------------------------------------------------
 *                        SANITY UI
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * Muestra visualmente el nivel de cordura del jugador.
 * Consiste en una barra que se llena/vacia segun la cordura.
 * Cambia de color segun el nivel actual.
 *
 * NIVELES DE COLOR:
 *   Nivel 4 - Verde  (completamente cuerdo)
 *   Nivel 3 - Amarillo (algo estresado)
 *   Nivel 2 - Naranja (perdiendo cordura)
 *   Nivel 1 - Rojo   (al limite)
 *
 * SETUP EN UNITY:
 *   Este script va en el Canvas.
 *   Asignar en el Inspector:
 *     · Sanity Bar Fill -> Image con fill method horizontal
 *     · Sanity Text     -> TMP opcional con numero
 *     · Level Indicator -> TMP opcional con nivel
 * ---------------------------------------------------------------
 */

public class SanityUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Image sanityBarFill;
    [SerializeField] private TextMeshProUGUI sanityText;
    [SerializeField] private TextMeshProUGUI levelIndicator;

    [Header("Colores por nivel")]
    [SerializeField] private Color level4Color = new Color(0.4f, 0.9f, 0.4f); // verde
    [SerializeField] private Color level3Color = new Color(1f, 0.9f, 0.2f);   // amarillo
    [SerializeField] private Color level2Color = new Color(1f, 0.5f, 0.1f);   // naranja
    [SerializeField] private Color level1Color = new Color(1f, 0.2f, 0.2f);   // rojo

    [Header("Animacion")]
    [SerializeField] private float smoothSpeed = 5f;

    private float targetFill;

    private void Start()
    {
        // Suscribirse a eventos del SanityManager
        SanityManager.Instance.onSanityChanged.AddListener(OnSanityChanged);
        SanityManager.Instance.onSanityLevelChanged.AddListener(OnLevelChanged);

        // Inicializar
        targetFill = SanityManager.Instance.GetSanityPercentage();
        if (sanityBarFill != null)
            sanityBarFill.fillAmount = targetFill;

        UpdateColor(SanityManager.Instance.CurrentLevel);
    }

    private void Update()
    {
        // Animar la barra suavemente
        if (sanityBarFill != null)
        {
            sanityBarFill.fillAmount = Mathf.Lerp(
                sanityBarFill.fillAmount,
                targetFill,
                Time.deltaTime * smoothSpeed);
        }
    }

    private void OnSanityChanged(float newSanity)
    {
        targetFill = SanityManager.Instance.GetSanityPercentage();

        if (sanityText != null)
            sanityText.text = $"{Mathf.RoundToInt(newSanity)}/50";
    }

    private void OnLevelChanged(int newLevel)
    {
        UpdateColor(newLevel);

        if (levelIndicator != null)
            levelIndicator.text = $"Nivel {newLevel}";
    }

    private void UpdateColor(int level)
    {
        if (sanityBarFill == null) return;

        sanityBarFill.color = level switch
        {
            4 => level4Color,
            3 => level3Color,
            2 => level2Color,
            1 => level1Color,
            _ => level4Color
        };
    }
}