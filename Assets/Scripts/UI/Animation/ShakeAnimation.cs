using System.Collections;
using UnityEngine;

/*
 * ---------------------------------------------------------------
 *                      SHAKE ANIMATION
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * Animacion de shake para elementos de UI.
 * El objeto vibra aleatoriamente y regresa a su posicion original.
 * Usa anchoredPosition para compatibilidad con canvas scaling.
 *
 * USOS COMUNES:
 *   - Feedback de error
 *   - Impacto o dano
 *   - Alertas importantes
 * ---------------------------------------------------------------
 */

public class ShakeAnimation : UIAnimationBase
{
    public override IEnumerator Animate(
        RectTransform target,
        UIAnimationPreset preset,
        System.Action onComplete)
    {
        yield return WaitDelay(preset.delay);

        Vector2 originalPos = target.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < preset.duration)
        {
            elapsed += Time.deltaTime;

            // Fade out del shake si esta configurado
            float strength = preset.shakeStrength;
            if (preset.shakeFadeOut)
                strength *= 1f - (elapsed / preset.duration);

            // Posicion aleatoria
            float x = Random.Range(-1f, 1f) * strength;
            float y = Random.Range(-1f, 1f) * strength;

            target.anchoredPosition = originalPos + new Vector2(x, y);

            // Esperar segun vibrato
            yield return new WaitForSeconds(1f / preset.shakeVibrato);
        }

        // Restaurar posicion original
        target.anchoredPosition = originalPos;
        onComplete?.Invoke();
    }
}