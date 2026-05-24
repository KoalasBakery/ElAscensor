using System.Collections;
using UnityEngine;

/*
 * ---------------------------------------------------------------
 *                      FADE ANIMATION
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * Animacion de fade in/out para elementos de UI.
 * Requiere un CanvasGroup en el objeto objetivo.
 *
 * TIPOS:
 *   FadeIn  -> alpha de fadeFrom a fadeTo
 *   FadeOut -> alpha de fadeTo a fadeFrom
 * ---------------------------------------------------------------
 */

public class FadeAnimation : UIAnimationBase
{
    public override IEnumerator Animate(
        RectTransform target,
        UIAnimationPreset preset,
        System.Action onComplete)
    {
        yield return WaitDelay(preset.delay);

        // Obtener o agregar CanvasGroup
        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = target.gameObject.AddComponent<CanvasGroup>();

        bool isFadeIn = preset.animationType == UIAnimationType.FadeIn;
        float startAlpha = isFadeIn ? preset.fadeFrom : preset.fadeTo;
        float endAlpha = isFadeIn ? preset.fadeTo : preset.fadeFrom;

        canvasGroup.alpha = startAlpha;
        target.gameObject.SetActive(true);

        float elapsed = 0f;
        while (elapsed < preset.duration)
        {
            elapsed += Time.deltaTime;
            float t = Evaluate(preset, elapsed);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;

        // Si es fade out desactivar el objeto
        if (!isFadeIn)
        {
            canvasGroup.blocksRaycasts = false;
            target.gameObject.SetActive(false);
        }
        else
        {
            canvasGroup.blocksRaycasts = true;
        }

        onComplete?.Invoke();
    }
}