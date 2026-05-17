using UnityEngine;

/*
 * ---------------------------------------------------------------
 *                   AUDIO SANITY EFFECT
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * Efecto de audio que cambia segun la cordura.
 * Por el momento solo contiene el hook para conectar
 * con el sistema de audio cuando este listo.
 *
 * TODO: Conectar con Audio de Evan creo
 * tenga el sistema de audio listo. El SanityManager ya
 * tiene onSanityChanged y onSanityLevelChanged listos
 * para suscribirse desde cualquier AudioManager.
 *
 * EJEMPLO DE USO FUTURO:
 *   SanityManager.Instance.onSanityChanged.AddListener(
 *       audioManager.OnSanityChanged);
 *
 * EFECTOS PLANEADOS (Por ahora es lo que se me ocurrio):
 *   - Bajar pitch de la musica conforme baja cordura
 *   - Agregar reverb/distorsion
 *   - Reproducir susurros en nivel 1-2
 *   - Reproducir pasos en nivel 1
 * ---------------------------------------------------------------
 */

public class AudioSanityEffect : SanityEffectBase
{
    public override void OnSanityChanged(float currentSanity, float maxSanity)
    {
        // NOTA: Implementar cuando el sistema de audio este listo
    }

    public override void OnLevelChanged(int newLevel)
    {
        // NOTA: Implementar cuando el sistema de audio este listo
        // Nivel 2: comenzar susurros suaves
        // Nivel 1: pasos, voces, distorsion maxima
    }
}