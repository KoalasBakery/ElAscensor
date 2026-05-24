/*
 * ---------------------------------------------------------------
 *                     UI ANIMATION TYPE
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * Enum con todos los tipos de animacion disponibles.
 * Al agregar una nueva animacion, agregar su tipo aqui.
 * ---------------------------------------------------------------
 */

public enum UIAnimationType
{
    // Fade
    FadeIn,
    FadeOut,

    // Slide
    SlideInRight,
    SlideInLeft,
    SlideInUp,
    SlideInDown,
    SlideOutRight,
    SlideOutLeft,
    SlideOutUp,
    SlideOutDown,

    // Scale
    ScaleIn,
    ScaleOut,

    Shake,
    Punch,
    Rotate,
    Loop,

    // Tier 2
    ColorTween,
    FillAmount,
    Stagger
}

public enum LoopType
{
    Restart,
    PingPong
}