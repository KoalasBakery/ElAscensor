using System.Collections;
using UnityEngine;

/*
 * ---------------------------------------------------------------
 *                      ROTATE ANIMATION
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * Animacion de rotacion para elementos de UI.
 * Puede rotar de un angulo a otro o hacer loop infinito.
 *
 * USOS COMUNES:
 *   - Iconos de carga (loop infinito)
 *   - Efectos de entrada con rotacion
 *   - Indicadores de estado
 * ---------------------------------------------------------------
 */

public class RotateAnimation : UIAnimationBase
{
    public override IEnumerator Animate(
        RectTransform target,
        UIAnimationPreset preset,
        System.Action onComplete)
    {
        yield return WaitDelay(preset.delay);

        if (preset.rotateLoop)
            yield return StartCoroutine(RotateLoop(target, preset));
        else
        {
            yield return StartCoroutine(RotateOnce(target, preset));
            onComplete?.Invoke();
        }
    }

    private IEnumerator RotateOnce(RectTransform target, UIAnimationPreset preset)
    {
        float elapsed = 0f;
        float startAngle = preset.rotateFrom;
        float endAngle = preset.rotateTo;

        while (elapsed < preset.duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / preset.duration);
            float eased = EaseCalculator.Evaluate(preset.easeType, t);
            float angle = Mathf.Lerp(startAngle, endAngle, eased);

            target.localEulerAngles = new Vector3(0f, 0f, angle);
            yield return null;
        }

        target.localEulerAngles = new Vector3(0f, 0f, endAngle);
    }

    private IEnumerator RotateLoop(RectTransform target, UIAnimationPreset preset)
    {
        while (true)
        {
            float elapsed = 0f;
            while (elapsed < preset.duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / preset.duration);
                float angle = Mathf.Lerp(preset.rotateFrom, preset.rotateTo, t);
                target.localEulerAngles = new Vector3(0f, 0f, angle);
                yield return null;
            }
        }
    }
}