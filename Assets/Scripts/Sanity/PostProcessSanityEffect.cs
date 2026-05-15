using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessSanityEffect : SanityEffectBase
{
    [Header("Post Processing")]
    [SerializeField] private Volume globalVolume;

    [Header("Vignette")]
    [SerializeField] private float maxVignetteIntensity = 0.6f;

    [Header("Chromatic Aberration")]
    [SerializeField] private float maxChromaticIntensity = 1f;

    [Header("Film Grain (estatica)")]
    [SerializeField] private float maxGrainIntensity = 0.8f;

    private Vignette vignette;
    private ChromaticAberration chromaticAberration;
    private FilmGrain filmGrain;

    private void Awake()
    {
        if (globalVolume == null) return;

        globalVolume.profile.TryGet(out vignette);
        globalVolume.profile.TryGet(out chromaticAberration);
        globalVolume.profile.TryGet(out filmGrain);

        ResetEffects();
    }

    public override void OnSanityChanged(float currentSanity, float maxSanity)
    {
        if (!isEnabled) return;

        float t = 1f - (currentSanity / maxSanity);

        ApplyVignette(t);
        ApplyChromaticAberration(t);
        ApplyGrain(t);
    }

    public override void OnLevelChanged(int newLevel)
    {
        if (!isEnabled) return;
        Debug.Log($"PostProcess: nivel de cordura -> {newLevel}");
    }

    private void ApplyVignette(float t)
    {
        if (vignette == null) return;

        vignette.active = true;
        vignette.intensity.value = Mathf.Lerp(0f, maxVignetteIntensity, t);
        vignette.color.value = Color.Lerp(Color.black, Color.red, t);
    }

    private void ApplyChromaticAberration(float t)
    {
        if (chromaticAberration == null) return;

        chromaticAberration.active = true;
        chromaticAberration.intensity.value = Mathf.Lerp(0f, maxChromaticIntensity, t);
    }

    private void ApplyGrain(float t)
    {
        if (filmGrain == null) return;

        filmGrain.active = true;
        filmGrain.intensity.value = Mathf.Lerp(0f, maxGrainIntensity, t);
    }

    private void ResetEffects()
    {
        if (vignette != null) vignette.active = false;
        if (chromaticAberration != null) chromaticAberration.active = false;
        if (filmGrain != null) filmGrain.active = false;
    }

    public override void Disable()
    {
        base.Disable();
        ResetEffects();
    }
}