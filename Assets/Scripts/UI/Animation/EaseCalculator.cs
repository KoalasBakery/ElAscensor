using UnityEngine;
using static UIAnimationPreset;

/*
 * ---------------------------------------------------------------
 *                      EASE CALCULATOR
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * Calcula el valor de ease para un tiempo dado.
 * Centraliza todos los tipos de ease del sistema.
 *
 * COMO USAR:
 *   float value = EaseCalculator.Evaluate(EaseType.Bounce, t);
 * ---------------------------------------------------------------
 */

public static class EaseCalculator
{
    public static float Evaluate(EaseType easeType, float t,
        AnimationCurve customCurve = null)
    {
        t = Mathf.Clamp01(t);

        switch (easeType)
        {
            case EaseType.Linear:
                return t;

            case EaseType.EaseIn:
                return t * t;

            case EaseType.EaseOut:
                return t * (2f - t);

            case EaseType.EaseInOut:
                return t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t;

            case EaseType.Bounce:
                return BounceOut(t);

            case EaseType.Elastic:
                return ElasticOut(t);

            case EaseType.BackIn:
                return BackIn(t);

            case EaseType.BackOut:
                return BackOut(t);

            default:
                return customCurve != null ? customCurve.Evaluate(t) : t;
        }
    }

    // --- FORMULAS --- //
    private static float BounceOut(float t)
    {
        if (t < 1f / 2.75f)
            return 7.5625f * t * t;
        else if (t < 2f / 2.75f)
        {
            t -= 1.5f / 2.75f;
            return 7.5625f * t * t + 0.75f;
        }
        else if (t < 2.5f / 2.75f)
        {
            t -= 2.25f / 2.75f;
            return 7.5625f * t * t + 0.9375f;
        }
        else
        {
            t -= 2.625f / 2.75f;
            return 7.5625f * t * t + 0.984375f;
        }
    }

    private static float ElasticOut(float t)
    {
        if (t == 0f || t == 1f) return t;
        return Mathf.Pow(2f, -10f * t) *
               Mathf.Sin((t - 0.075f) * (2f * Mathf.PI) / 0.3f) + 1f;
    }

    private static float BackIn(float t)
    {
        float s = 1.70158f;
        return t * t * ((s + 1f) * t - s);
    }

    private static float BackOut(float t)
    {
        float s = 1.70158f;
        t -= 1f;
        return t * t * ((s + 1f) * t + s) + 1f;
    }
}