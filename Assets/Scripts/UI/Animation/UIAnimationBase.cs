using System.Collections;
using UnityEngine;

/*
 * ---------------------------------------------------------------
 *                     UI ANIMATION BASE
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * Clase base abstracta para todas las animaciones de UI.
 * Hereda de MonoBehaviour para poder usarse como componente
 * e implementa IUIAnimation para garantizar la interfaz.
 *
 * COMO CREAR UNA NUEVA ANIMACION:
 *   public class MiAnimacion : UIAnimationBase
 *   {
 *       public override IEnumerator Animate(
 *           RectTransform target,
 *           UIAnimationPreset preset,
 *           System.Action onComplete)
 *       {
 *           // tu logica aqui
 *       }
 *   }
 * ---------------------------------------------------------------
 */

public abstract class UIAnimationBase : MonoBehaviour, IUIAnimation
{
    public abstract IEnumerator Animate(
        RectTransform target,
        UIAnimationPreset preset,
        System.Action onComplete);

    // Helper para evaluar la curva de animacion
    protected float Evaluate(UIAnimationPreset preset, float elapsed)
    {
        float t = Mathf.Clamp01(elapsed / preset.duration);
        return EaseCalculator.Evaluate(preset.easeType, t, preset.customCurve);
    }

    // Helper para esperar el delay
    protected IEnumerator WaitDelay(float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);
    }
}