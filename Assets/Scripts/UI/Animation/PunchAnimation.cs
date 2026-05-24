using System.Collections;
using UnityEngine;

/*
 * ---------------------------------------------------------------
 *                      PUNCH ANIMATION
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * Animacion de punch/bounce para elementos de UI.
 * El objeto crece rapidamente y rebota de vuelta a su tamanio original.
 *
 * USOS COMUNES:
 *   - Feedback al clickear botones
 *   - Aparicion de notificaciones
 *   - Confirmacion de acciones
 * ---------------------------------------------------------------
 */

public class PunchAnimation : UIAnimationBase
{
    public override IEnumerator Animate(
        RectTransform target,
        UIAnimationPreset preset,
        System.Action onComplete)
    {
        yield return WaitDelay(preset.delay);

        Vector3 originalScale = target.localScale;
        float elapsed = 0f;

        while (elapsed < preset.duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / preset.duration;

            // Formula de punch — sube rapido y baja con rebotes
            float punch = PunchFormula(t, preset.punchStrength, preset.punchVibrato);
            target.localScale = originalScale + Vector3.one * punch;

            yield return null;
        }

        // Restaurar escala original
        target.localScale = originalScale;
        onComplete?.Invoke();
    }

    private float PunchFormula(float t, float strength, int vibrato)
    {
        if (t == 0f || t == 1f) return 0f;

        float decay = 4f / vibrato;
        return strength *
               Mathf.Sin(vibrato * Mathf.PI * t) *
               Mathf.Exp(-decay * t);
    }
}