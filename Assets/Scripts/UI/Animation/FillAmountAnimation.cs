using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/*
 * ---------------------------------------------------------------
 *                   FILL AMOUNT ANIMATION
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * Animacion de fill amount para Images de tipo Filled.
 * Util para barras de progreso, cordura, carga, etc. (osea la de cordura we)
 *
 * USOS COMUNES:
 *   - Barra de cordura
 *   - Barra de carga
 *   - Indicadores de progreso
 *   - Timers visuales
 * ---------------------------------------------------------------
 */

public class FillAmountAnimation : UIAnimationBase
{
    public override IEnumerator Animate(RectTransform target, UIAnimationPreset preset, System.Action onComplete)
    {
        yield return WaitDelay(preset.delay);

        Image image = target.GetComponent<Image>();
        if (image == null)
        {
            Debug.LogWarning($"FillAmountAnimation: {target.name} no tiene Image");
            onComplete?.Invoke();
            yield break;
        }

        if (image.type != Image.Type.Filled)
        {
            Debug.LogWarning($"FillAmountAnimation: {target.name} no es de tipo Filled");
            onComplete?.Invoke();
            yield break;
        }

        float elapsed = 0f;
        float startFill = preset.fillFrom;
        float endFill = preset.fillTo;

        image.fillAmount = startFill;

        while (elapsed < preset.duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / preset.duration);
            float eased = EaseCalculator.Evaluate(preset.easeType, t);
            image.fillAmount = Mathf.Lerp(startFill, endFill, eased);
            yield return null;
        }

        image.fillAmount = endFill;
        onComplete?.Invoke();
    }
}