using System.Collections;
using UnityEngine;

/*
 * ---------------------------------------------------------------
 *                       I UI ANIMATION
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * Interfaz que deben implementar todas las animaciones de UI.
 * Para agregar una nueva animacion, crear una clase que herede
 * de UIAnimationBase e implemente este metodo.
 *
 * COMO AGREGAR UNA NUEVA ANIMACION:
 *   1. Crear clase que herede de UIAnimationBase
 *   2. Implementar el metodo Animate
 *   3. Registrarla en UIAnimationManager
 *   4. Agregar su tipo en el enum UIAnimationType
 * ---------------------------------------------------------------
 */

public interface IUIAnimation
{
    IEnumerator Animate(RectTransform target,
        UIAnimationPreset preset,
        System.Action onComplete);
}