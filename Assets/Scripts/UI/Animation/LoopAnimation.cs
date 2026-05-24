using System.Collections;
using UnityEngine;

/*
 * ---------------------------------------------------------------
 *                      LOOP ANIMATION
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * Wrapper que permite hacer loop de cualquier animacion.
 *
 * TIPOS DE LOOP:
 *   Restart  -> vuelve al inicio cada vez
 *   PingPong -> va y viene
 *
 * USOS COMUNES:
 *   - Animaciones de idle en UI
 *   - Indicadores pulsantes
 *   - Efectos ambientales de UI
 * ---------------------------------------------------------------
 */

public class LoopAnimation : UIAnimationBase
{
    public override IEnumerator Animate(
        RectTransform target,
        UIAnimationPreset preset,
        System.Action onComplete)
    {
        yield return WaitDelay(preset.delay);

        int count = 0;
        bool reverse = false;

        while (preset.loopCount == -1 || count < preset.loopCount)
        {
            Vector2 originalPos = target.anchoredPosition;
            Vector3 originalScale = target.localScale;
            float elapsed = 0f;

            while (elapsed < preset.duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / preset.duration;

                if (preset.loopType == LoopType.PingPong)
                    t = reverse ? 1f - t : t;

                float evaluated = EaseCalculator.Evaluate(preset.easeType, t);

                // Aplicar efecto de float (sube y baja)
                float offsetY = Mathf.Sin(evaluated * Mathf.PI) * 10f;
                target.anchoredPosition = originalPos + new Vector2(0, offsetY);

                yield return null;
            }

            // Restaurar posicion
            target.anchoredPosition = originalPos;

            if (preset.loopType == LoopType.PingPong)
                reverse = !reverse;

            count++;
        }

        onComplete?.Invoke();
    }
}