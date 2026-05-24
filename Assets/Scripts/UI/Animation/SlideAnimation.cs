using System.Collections;
using UnityEngine;

/*
 * ---------------------------------------------------------------
 *                      SLIDE ANIMATION
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * Animacion de slide para elementos de UI.
 * Usa anchoredPosition para compatibilidad con canvas scaling,
 * diferentes resoluciones, anchors y safe areas.
 * ---------------------------------------------------------------
 */

public class SlideAnimation : UIAnimationBase
{
    public override IEnumerator Animate(
        RectTransform target,
        UIAnimationPreset preset,
        System.Action onComplete)
    {
        yield return WaitDelay(preset.delay);

        // Guardar posicion original en anchoredPosition
        Vector2 originalPos = target.anchoredPosition;
        Vector2 offsetPos = GetOffsetPosition(originalPos, preset);
        bool isSlideIn = IsSlideIn(preset.animationType);

        Vector2 startPos = isSlideIn ? offsetPos : originalPos;
        Vector2 endPos = isSlideIn ? originalPos : offsetPos;

        target.anchoredPosition = startPos;
        target.gameObject.SetActive(true);

        float elapsed = 0f;
        while (elapsed < preset.duration)
        {
            elapsed += Time.deltaTime;
            float t = Evaluate(preset, elapsed);
            target.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        // Asegurar posicion final exacta
        target.anchoredPosition = endPos;

        if (!isSlideIn)
        {
            target.gameObject.SetActive(false);
            // Restaurar posicion original al desactivar
            target.anchoredPosition = originalPos;
        }

        onComplete?.Invoke();
    }

    private Vector2 GetOffsetPosition(Vector2 originalPos, UIAnimationPreset preset)
    {
        switch (preset.animationType)
        {
            case UIAnimationType.SlideInRight:
            case UIAnimationType.SlideOutRight:
                return originalPos + new Vector2(preset.slideDistance, 0);

            case UIAnimationType.SlideInLeft:
            case UIAnimationType.SlideOutLeft:
                return originalPos - new Vector2(preset.slideDistance, 0);

            case UIAnimationType.SlideInUp:
            case UIAnimationType.SlideOutUp:
                return originalPos + new Vector2(0, preset.slideDistance);

            case UIAnimationType.SlideInDown:
            case UIAnimationType.SlideOutDown:
                return originalPos - new Vector2(0, preset.slideDistance);

            default:
                return originalPos;
        }
    }

    private bool IsSlideIn(UIAnimationType type)
    {
        return type == UIAnimationType.SlideInRight ||
               type == UIAnimationType.SlideInLeft ||
               type == UIAnimationType.SlideInUp ||
               type == UIAnimationType.SlideInDown;
    }
}