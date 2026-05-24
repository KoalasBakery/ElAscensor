using System.Collections;
using UnityEngine;

/*
 * ---------------------------------------------------------------
 *                      SCALE ANIMATION
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * Animacion de escala para elementos de UI.
 * El objeto crece o se encoge desde su centro.
 *
 * TIPOS:
 *   ScaleIn  -> crece de scaleFrom a scaleTo
 *   ScaleOut -> se encoge de scaleTo a scaleFrom
 * ---------------------------------------------------------------
 */

public class ScaleAnimation : UIAnimationBase
{
    public override IEnumerator Animate(
        RectTransform target,
        UIAnimationPreset preset,
        System.Action onComplete)
    {
        yield return WaitDelay(preset.delay);

        bool isScaleIn = preset.animationType == UIAnimationType.ScaleIn;
        float startScale = isScaleIn ? preset.scaleFrom : preset.scaleTo;
        float endScale = isScaleIn ? preset.scaleTo : preset.scaleFrom;

        target.localScale = Vector3.one * startScale;
        target.gameObject.SetActive(true);

        float elapsed = 0f;
        while (elapsed < preset.duration)
        {
            elapsed += Time.deltaTime;
            float t = Evaluate(preset, elapsed);
            float scale = Mathf.Lerp(startScale, endScale, t);
            target.localScale = Vector3.one * scale;
            yield return null;
        }

        target.localScale = Vector3.one * endScale;

        if (!isScaleIn)
            target.gameObject.SetActive(false);

        onComplete?.Invoke();
    }
}