using System.Collections.Generic;
using UnityEngine;

/*
 * ---------------------------------------------------------------
 *                        MISSION DATA
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * ScriptableObject que define una mision del juego.
 * Las misiones tienen pasos que se van completando
 * y se muestran en el Journal del jugador.
 *
 * TIPOS:
 *   - Principal: necesaria para progresar
 *   - Secundaria: opcional (tapoco se si se usara pero bueno)
 *
 * SETUP:
 * Clic derecho en Assets -> Journal -> Mission
 * ---------------------------------------------------------------
 */

[CreateAssetMenu(fileName = "NewMission", menuName = "Journal/Mission")]
public class MissionData : ScriptableObject
{
    [System.Serializable]
    public class MissionStep
    {
        [Tooltip("Key de localizacion del paso")]
        public string stepDescriptionKey;

        [Tooltip("Flag que marca este paso como completado")]
        public string completionFlagKey;

        [HideInInspector]
        public bool isCompleted = false;
    }

    [Header("Informacion")]
    [Tooltip("Key de localizacion del titulo")]
    public string titleKey;

    [Tooltip("Key de localizacion de la descripcion")]
    public string descriptionKey;

    [Tooltip("Tipo de mision")]
    public MissionType missionType;

    [Header("Pasos")]
    [Tooltip("Pasos necesarios para completar la mision")]
    public List<MissionStep> steps;

    [Header("Flags")]
    [Tooltip("Flag que desbloquea esta mision")]
    public string unlockFlagKey;

    [Tooltip("Flag que se activa al completar la mision")]
    public string completionFlagKey;

    [Header("Recompensa")]
    [Tooltip("Cuanta cordura da completar esta mision")]
    public float sanityReward = 5f;
}

public enum MissionType
{
    Main,      // Mision principal
    Secondary  // Mision secundaria
}