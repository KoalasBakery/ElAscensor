using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * ---------------------------------------------------------------
 *                    STAGGER ANIMATION
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * Anima una lista de elementos uno tras otro con un delay
 * entre cada uno. Util para listas, menus y journals.
 *
 * USOS COMUNES:
 *   - Entradas del journal apareciendo una por una
 *   - Items del inventario
 *   - Opciones de menu
 *   - Notificaciones en secuencia
 *
 * COMO USARLO:
 *   UIAnimationManager.Instance.PlayStagger(
 *       misElementos, miPreset, delay: 0.1f);
 * ---------------------------------------------------------------
 */

public class StaggerAnimation : UIAnimationBase
{
    public override IEnumerator Animate(RectTransform target, UIAnimationPreset preset, System.Action onComplete)
    {
        // Este metodo no se usa directamente
        // Usar PlayStagger en UIAnimationManager
        onComplete?.Invoke();
        yield break;
    }

    public IEnumerator AnimateStagger(List<RectTransform> targets, UIAnimationPreset preset, float staggerDelay, System.Action onComplete)
    {
        if (targets == null || targets.Count == 0)
        {
            onComplete?.Invoke();
            yield break;
        }

        int completed = 0;

        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] == null) continue;

            int capturedIndex = i;
            RectTransform target = targets[i];

            // Crear preset con delay acumulado
            UIAnimationPreset staggerPreset = Instantiate(preset);
            staggerPreset.delay = preset.delay + (i * staggerDelay);

            StartCoroutine(AnimateTarget(target, staggerPreset, () =>
            {
                completed++;
                if (completed >= targets.Count)
                    onComplete?.Invoke();
            }));
        }

        // Esperar a que todos terminen
        float totalTime = preset.duration + preset.delay +
            (targets.Count - 1) * staggerDelay;
        yield return new WaitForSeconds(totalTime);
    }

    private IEnumerator AnimateTarget(RectTransform target,
        UIAnimationPreset preset, System.Action onComplete)
    {
        yield return WaitDelay(preset.delay);

        float elapsed = 0f;
        Vector2 startPos = target.anchoredPosition +
            new Vector2(0, -preset.slideDistance);
        Vector2 endPos = target.anchoredPosition;

        target.anchoredPosition = startPos;

        CanvasGroup cg = target.GetComponent<CanvasGroup>();
        if (cg == null) cg = target.gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 0f;

        while (elapsed < preset.duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / preset.duration);
            float eased = EaseCalculator.Evaluate(preset.easeType, t);

            target.anchoredPosition = Vector2.Lerp(startPos, endPos, eased);
            cg.alpha = Mathf.Lerp(0f, 1f, eased);

            yield return null;
        }

        target.anchoredPosition = endPos;
        cg.alpha = 1f;
        onComplete?.Invoke();
    }
}