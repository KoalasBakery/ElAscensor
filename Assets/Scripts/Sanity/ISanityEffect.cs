/*
 * ---------------------------------------------------------------
 *                      I SANITY EFFECT
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * Interfaz que deben implementar todos los efectos de cordura.
 * Para agregar un nuevo efecto, crear una clase que implemente
 * esta interfaz y agregarla al SanityEffects.
 *
 * COMO AGREGAR UN NUEVO EFECTO:
 *   1. Crear una clase que herede de SanityEffectBase
 *   2. Implementar OnSanityChanged y OnLevelChanged
 *   3. Agregar el componente al GameObject de SanityEffects
 * ---------------------------------------------------------------
 */

public interface ISanityEffect
{
    // Se llama cada vez que cambia el valor de cordura
    void OnSanityChanged(float currentSanity, float maxSanity);

    // Se llama cuando cambia el nivel de cordura (1-4)
    void OnLevelChanged(int newLevel);

    // Se llama cuando el efecto debe activarse
    void Enable();

    // Se llama cuando el efecto debe desactivarse
    void Disable();
}