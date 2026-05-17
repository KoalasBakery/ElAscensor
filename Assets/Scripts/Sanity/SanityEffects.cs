using System.Collections.Generic;
using UnityEngine;

/*
 * ---------------------------------------------------------------
 *                      SANITY EFFECTS
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * Coordinador central de todos los efectos de cordura.
 * No implementa efectos directamente, solo los coordina.
 * Busca automaticamente todos los SanityEffectBase en sus hijos.
 *
 * COMO AGREGAR UN NUEVO EFECTO:
 *   1. Crear script que herede de SanityEffectBase
 *   2. Agregarlo como componente hijo de este GameObject
 *   3. Se conecta automaticamente, no hay que tocar este script
 * ---------------------------------------------------------------
 */

public class SanityEffects : MonoBehaviour
{
    private List<ISanityEffect> effects = new List<ISanityEffect>();

    private void Awake()
    {
        // Buscar todos los efectos en hijos automaticamente
        var foundEffects = GetComponentsInChildren<SanityEffectBase>();
        foreach (var effect in foundEffects)
            effects.Add(effect);

        Debug.Log($"SanityEffects: {effects.Count} efectos encontrados"); //
    }

    private void Start()
    {
        // Suscribirse a eventos del SanityManager
        SanityManager.Instance.onSanityChanged.AddListener(OnSanityChanged);
        SanityManager.Instance.onSanityLevelChanged.AddListener(OnLevelChanged);
    }

    private void OnSanityChanged(float currentSanity)
    {
        foreach (var effect in effects)
            effect.OnSanityChanged(currentSanity, SanityManager.Instance.MaxSanity);
    }

    private void OnLevelChanged(int newLevel)
    {
        foreach (var effect in effects)
            effect.OnLevelChanged(newLevel);
    }

    // --- API PUBLICA --- //
    public void EnableEffect<T>() where T : SanityEffectBase
    {
        foreach (var effect in effects)
            if (effect is T) effect.Enable();
    }

    public void DisableEffect<T>() where T : SanityEffectBase
    {
        foreach (var effect in effects)
            if (effect is T) effect.Disable();
    }
}