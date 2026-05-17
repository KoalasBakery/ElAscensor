using UnityEngine;

/*
 * ---------------------------------------------------------------
 *                    SANITY EFFECT BASE
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * Clase base abstracta para todos los efectos de cordura.
 * Hereda de MonoBehaviour para poder agregarse como componente
 * y implementa ISanityEffect para garantizar la interfaz.
 *
 * COMO USARLA:
 *   public class MiEfecto : SanityEffectBase
 *   {
 *       public override void OnSanityChanged(float current, float max) { }
 *       public override void OnLevelChanged(int level) { }
 *   }
 * ---------------------------------------------------------------
 */

public abstract class SanityEffectBase : MonoBehaviour, ISanityEffect
{
    [Header("Configuracion Base")]
    [Tooltip("Si este efecto esta activo")]
    [SerializeField] protected bool isEnabled = true;

    public virtual void Enable() => isEnabled = true;
    public virtual void Disable() => isEnabled = false;

    public abstract void OnSanityChanged(float currentSanity, float maxSanity);
    public abstract void OnLevelChanged(int newLevel);
}