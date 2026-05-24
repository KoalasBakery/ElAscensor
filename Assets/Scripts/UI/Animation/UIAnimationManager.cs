using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * ---------------------------------------------------------------
 *                   UI ANIMATION MANAGER
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * Singleton que coordina todas las animaciones de UI del juego.
 * Usa un sistema modular donde cada tipo de animacion es una
 * clase separada que hereda de UIAnimationBase.
 *
 * COMO USARLO:
 *   // Con tipo de animacion
 *   UIAnimationManager.Instance.Play(miPanel, UIAnimationType.FadeIn);
 *
 *   // Con preset personalizado
 *   UIAnimationManager.Instance.Play(miPanel, miPreset);
 *
 *   // Con callback al terminar
 *   UIAnimationManager.Instance.Play(miPanel, UIAnimationType.FadeIn,
 *       onComplete: () => Debug.Log("Listo"));
 *
 *   // Detener animacion en curso
 *   UIAnimationManager.Instance.Stop(miPanel);
 *
 * COMO AGREGAR UNA NUEVA ANIMACION:
 *   1. Crear clase que herede de UIAnimationBase
 *   2. Agregar su tipo en UIAnimationType
 *   3. Registrarla en RegisterAnimations()
 *
 * DEPENDENCIAS:
 *   - UIAnimationPreset (SO de configuracion)
 *   - UIAnimationBase   (clase base de animaciones)
 *   - UIAnimationType   (enum de tipos)
 * ---------------------------------------------------------------
 */

public class UIAnimationManager : MonoBehaviour
{
    public static UIAnimationManager Instance { get; private set; }

    [Header("Presets por defecto")]
    [Tooltip("Preset que se usa cuando no se especifica uno")]
    [SerializeField] private UIAnimationPreset defaultFadeIn;
    [SerializeField] private UIAnimationPreset defaultFadeOut;
    [SerializeField] private UIAnimationPreset defaultSlideIn;
    [SerializeField] private UIAnimationPreset defaultSlideOut;
    [SerializeField] private UIAnimationPreset defaultScaleIn;
    [SerializeField] private UIAnimationPreset defaultScaleOut;

    // Registro de animaciones disponibles
    private Dictionary<UIAnimationType, UIAnimationBase> animations
        = new Dictionary<UIAnimationType, UIAnimationBase>();

    // Corrutinas activas por objeto
    private Dictionary<RectTransform, Coroutine> activeAnimations
        = new Dictionary<RectTransform, Coroutine>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        RegisterAnimations();
    }

    // --- REGISTRO DE ANIMACIONES --- //
    private void RegisterAnimations()
    {
        // Fade
        FadeAnimation fade = gameObject.AddComponent<FadeAnimation>();
        animations[UIAnimationType.FadeIn] = fade;
        animations[UIAnimationType.FadeOut] = fade;

        // Slide
        SlideAnimation slide = gameObject.AddComponent<SlideAnimation>();
        animations[UIAnimationType.SlideInRight] = slide;
        animations[UIAnimationType.SlideInLeft] = slide;
        animations[UIAnimationType.SlideInUp] = slide;
        animations[UIAnimationType.SlideInDown] = slide;
        animations[UIAnimationType.SlideOutRight] = slide;
        animations[UIAnimationType.SlideOutLeft] = slide;
        animations[UIAnimationType.SlideOutUp] = slide;
        animations[UIAnimationType.SlideOutDown] = slide;

        // Scale
        ScaleAnimation scale = gameObject.AddComponent<ScaleAnimation>();
        animations[UIAnimationType.ScaleIn] = scale;
        animations[UIAnimationType.ScaleOut] = scale;

        // Shake
        ShakeAnimation shake = gameObject.AddComponent<ShakeAnimation>();
        animations[UIAnimationType.Shake] = shake;

        // Punch
        PunchAnimation punch = gameObject.AddComponent<PunchAnimation>();
        animations[UIAnimationType.Punch] = punch;

        // Rotate
        RotateAnimation rotate = gameObject.AddComponent<RotateAnimation>();
        animations[UIAnimationType.Rotate] = rotate;

        // Loop
        LoopAnimation loop = gameObject.AddComponent<LoopAnimation>();
        animations[UIAnimationType.Loop] = loop;

        // Color
        ColorTweenAnimation colorTween = gameObject.AddComponent<ColorTweenAnimation>();
        animations[UIAnimationType.ColorTween] = colorTween;

        //Fill Amount
        FillAmountAnimation fillAmount = gameObject.AddComponent<FillAmountAnimation>();
        animations[UIAnimationType.FillAmount] = fillAmount;

        //Stagger Animation
        StaggerAnimation stagger = gameObject.AddComponent<StaggerAnimation>();
        animations[UIAnimationType.Stagger] = stagger;
        // NOTa: Poner nuevas animaciones aqui
    }

    // --- PLAY CON TIPO --- //
    public void Play(RectTransform target, UIAnimationType type,
        System.Action onComplete = null)
    {
        UIAnimationPreset preset = GetDefaultPreset(type);
        if (preset == null)
        {
            Debug.LogWarning($"UIAnimationManager: No hay preset por defecto para {type}");
            return;
        }

        // Sobreescribir el tipo en el preset
        preset.animationType = type;
        Play(target, preset, onComplete);
    }

    // --- PLAY CON PRESET --- //
    public void Play(RectTransform target, UIAnimationPreset preset,
        System.Action onComplete = null)
    {
        if (target == null || preset == null) return;

        // Detener animacion anterior si existe
        Stop(target);

        if (!animations.ContainsKey(preset.animationType))
        {
            Debug.LogWarning($"UIAnimationManager: Animacion {preset.animationType} no registrada");
            return;
        }

        UIAnimationBase animation = animations[preset.animationType];
        Coroutine coroutine = StartCoroutine(
            animation.Animate(target, preset, onComplete));

        activeAnimations[target] = coroutine;
    }

    // --- PLAY MULTIPLE AL MISMO TIEMPO --- //
    public void PlayMultiple(RectTransform target, System.Action onComplete = null,
        params UIAnimationPreset[] presets)
    {
        if (target == null || presets.Length == 0) return;

        Stop(target);
        StartCoroutine(PlayMultipleCoroutine(target, presets, onComplete));
    }

    // --- STAGGER --- //
    public void PlayStagger(List<RectTransform> targets, UIAnimationPreset preset, float staggerDelay = 0.1f,
    System.Action onComplete = null)
    {
        if (targets == null || targets.Count == 0) return;

        StaggerAnimation stagger = GetComponent<StaggerAnimation>();
        if (stagger == null)
        {
            Debug.LogWarning("UIAnimationManager: No hay StaggerAnimation registrado");
            return;
        }

        StartCoroutine(stagger.AnimateStagger(
            targets, preset, staggerDelay, onComplete));
    }

    private IEnumerator PlayMultipleCoroutine(RectTransform target,
        UIAnimationPreset[] presets, System.Action onComplete)
    {
        // Calcular duracion total (la mas larga)
        float totalDuration = 0f;
        foreach (var preset in presets)
            totalDuration = Mathf.Max(totalDuration, preset.duration + preset.delay);

        // Lanzar todas las animaciones al mismo tiempo
        foreach (var preset in presets)
        {
            if (animations.ContainsKey(preset.animationType))
                StartCoroutine(animations[preset.animationType]
                    .Animate(target, preset, null));
        }

        // Esperar a que todas terminen
        yield return new WaitForSeconds(totalDuration);
        onComplete?.Invoke();
    }

    // --- STOP --- //
    public void Stop(RectTransform target)
    {
        if (activeAnimations.ContainsKey(target))
        {
            if (activeAnimations[target] != null)
                StopCoroutine(activeAnimations[target]);
            activeAnimations.Remove(target);
        }
    }

    public void StopAll()
    {
        StopAllCoroutines();
        activeAnimations.Clear();
    }

    // --- HELPERS --- //
    private UIAnimationPreset GetDefaultPreset(UIAnimationType type)
    {
        switch (type)
        {
            case UIAnimationType.FadeIn: return defaultFadeIn;
            case UIAnimationType.FadeOut: return defaultFadeOut;
            case UIAnimationType.SlideInRight:
            case UIAnimationType.SlideInLeft:
            case UIAnimationType.SlideInUp:
            case UIAnimationType.SlideInDown: return defaultSlideIn;
            case UIAnimationType.SlideOutRight:
            case UIAnimationType.SlideOutLeft:
            case UIAnimationType.SlideOutUp:
            case UIAnimationType.SlideOutDown: return defaultSlideOut;
            case UIAnimationType.ScaleIn: return defaultScaleIn;
            case UIAnimationType.ScaleOut: return defaultScaleOut;
            default: return null;
        }
    }
}