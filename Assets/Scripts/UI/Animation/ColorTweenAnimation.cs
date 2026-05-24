using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/*
 * ---------------------------------------------------------------
 *                   COLOR TWEEN ANIMATION
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * Animacion de color para elementos de UI.
 * Hace transicion suave entre dos colores.
 *
 * USOS COMUNES:
 *   - Cambio de color al hover de botones
 *   - Efecto de cordura en UI (se vuelve rojo)
 *   - Feedback visual de estado
 *   - Resaltar elementos importantes
 * ---------------------------------------------------------------
 */

public class ColorTweenAnimation : UIAnimationBase
{
    public override IEnumerator Animate(RectTransform target, UIAnimationPreset preset, System.Action onComplete)
    {
        yield return WaitDelay(preset.delay);

        // Obtener el componente de color
        Image image = target.GetComponent<Image>();
        UnityEngine.UI.Text text = target.GetComponent<UnityEngine.UI.Text>();
        TMPro.TextMeshProUGUI tmp = target.GetComponent<TMPro.TextMeshProUGUI>();

        if (image == null && text == null && tmp == null)
        {
            Debug.LogWarning($"ColorTweenAnimation: {target.name} no tiene Image, Text o TMP");
            onComplete?.Invoke();
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < preset.duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / preset.duration);
            float eased = EaseCalculator.Evaluate(preset.easeType, t);
            Color color = Color.Lerp(preset.colorFrom, preset.colorTo, eased);

            if (image != null) image.color = color;
            if (text != null) text.color = color;
            if (tmp != null) tmp.color = color;

            yield return null;
        }

        // Asegurar color final
        Color finalColor = preset.colorTo;
        if (image != null) image.color = finalColor;
        if (text != null) text.color = finalColor;
        if (tmp != null) tmp.color = finalColor;

        onComplete?.Invoke();
    }
}