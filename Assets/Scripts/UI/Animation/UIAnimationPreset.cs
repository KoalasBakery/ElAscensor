using UnityEngine;

/*
 * ---------------------------------------------------------------
 *                    UI ANIMATION PRESET
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * ScriptableObject que define la configuracion de una animacion.
 * Se puede crear desde el Inspector y reutilizar en cualquier
 * elemento de UI del juego.
 *
 * COMO CREAR UN PRESET:
 * Clic derecho en Assets -> UI -> Animation Preset
 *
 * EJEMPLO DE USO:
 *   UIAnimationManager.Instance.Play(miPanel, miPreset);
 * ---------------------------------------------------------------
 */

[CreateAssetMenu(fileName = "NewAnimationPreset", menuName = "UI/Animation Preset")]

public class UIAnimationPreset : ScriptableObject
{
    public enum EaseType
    {
        Linear,
        EaseIn,
        EaseOut,
        EaseInOut,
        Bounce,
        Elastic,
        BackIn,
        BackOut
    }
    //Curve
    [Header("Configuracion General")]
    [Tooltip("Duracion de la animacion en segundos")]
    public float duration = 0.3f;
    [Tooltip("Delay antes de iniciar la animacion")]
    public float delay = 0f;
    [Tooltip("Tipo de ease — controla la velocidad en cada punto")]
    public EaseType easeType = EaseType.EaseInOut;
    [Tooltip("Curva personalizada — solo se usa si EaseType es Custom")]
    public AnimationCurve customCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Tooltip("Tipo de animacion")]
    public UIAnimationType animationType = UIAnimationType.FadeIn;

    //Fade
    [Header("Fade")]
    [Tooltip("Alpha inicial del fade")]
    [Range(0f, 1f)]
    public float fadeFrom = 0f;

    [Tooltip("Alpha final del fade")]
    [Range(0f, 1f)]
    public float fadeTo = 1f;

    [Header("Slide")]
    [Tooltip("Distancia del slide en pixeles")]
    public float slideDistance = 100f;

    [Header("Scale")]
    [Tooltip("Scale inicial")]
    public float scaleFrom = 0f;

    [Tooltip("Scale final")]
    public float scaleTo = 1f;

    [Header("Shake")]
    [Tooltip("Intensidad del shake")]
    public float shakeStrength = 10f;
    [Tooltip("Vibraciones por segundo")]
    public int shakeVibrato = 10;
    [Tooltip("Si el shake se suaviza al final")]
    public bool shakeFadeOut = true;

    [Header("Punch")]
    [Tooltip("Fuerza del punch")]
    public float punchStrength = 0.5f;
    [Tooltip("Vibraciones del punch")]
    public int punchVibrato = 3;

    [Header("Rotate")]
    [Tooltip("Angulo inicial en grados")]
    public float rotateFrom = 0f;
    [Tooltip("Angulo final en grados")]
    public float rotateTo = 360f;
    [Tooltip("Si la rotacion hace loop")]
    public bool rotateLoop = false;

    [Header("Loop")]
    [Tooltip("Tipo de loop")]
    public LoopType loopType = LoopType.Restart;
    [Tooltip("Repeticiones. -1 = infinito")]
    public int loopCount = -1;

    [Header("Color Tween")]
    [Tooltip("Color inicial")]
    public Color colorFrom = Color.white;

    [Tooltip("Color final")]
    public Color colorTo = Color.red;

    [Header("Fill Amount")]
    [Tooltip("Fill inicial. 0 = vacio, 1 = lleno")]
    [Range(0f, 1f)]
    public float fillFrom = 0f;

    [Tooltip("Fill final. 0 = vacio, 1 = lleno")]
    [Range(0f, 1f)]
    public float fillTo = 1f;

    [Header("Stagger")]
    [Tooltip("Delay entre cada elemento de la lista")]
    public float staggerDelay = 0.1f;
}