using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * ---------------------------------------------------------------
 *                      MISSION MANAGER
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * Singleton que controla todas las misiones del juego.
 * Las misiones se desbloquean por flags y se completan
 * cuando todos sus pasos estan completados.
 *
 * COMO USARLO:
 *   // Desbloquear una mision
 *   MissionManager.Instance.UnlockMission(missionData);
 *
 *   // Completar un paso
 *   MissionManager.Instance.CompleteStep(missionData, stepIndex);
 *
 *   // Verificar si una mision esta activa
 *   MissionManager.Instance.IsMissionActive(missionData);
 *
 * DEPENDENCIAS:
 *   - FlagManager
 *   - SanityManager (recompensa al completar)
 *
 * ---------------------------------------------------------------
 */

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    private List<MissionData> activeMissions = new List<MissionData>();
    private List<MissionData> completedMissions = new List<MissionData>();

    // Eventos
    public UnityEvent<MissionData> onMissionUnlocked;
    public UnityEvent<MissionData> onMissionCompleted;
    public UnityEvent<MissionData, int> onStepCompleted;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // --- DESBLOQUEAR MISION --- //
    // En MissionManager.cs agrega esto en UnlockMission()
    public void UnlockMission(MissionData mission)
    {
        if (mission == null) return;
        if (activeMissions.Contains(mission)) return;
        if (completedMissions.Contains(mission)) return;

        // Resetear pasos al desbloquear
        foreach (var step in mission.steps)
            step.isCompleted = false;

        activeMissions.Add(mission);
        onMissionUnlocked?.Invoke(mission);
        Debug.Log($"Mision desbloqueada: {mission.name}");
    }

    // --- COMPLETAR PASO --- //
    public void CompleteStep(MissionData mission, int stepIndex)
    {
        if (!activeMissions.Contains(mission)) return;
        if (stepIndex >= mission.steps.Count) return;

        MissionData.MissionStep step = mission.steps[stepIndex];
        if (step.isCompleted) return;

        step.isCompleted = true;

        // Activar flag del paso
        if (!string.IsNullOrEmpty(step.completionFlagKey))
            FlagManager.Instance.SetFlag(step.completionFlagKey, true);

        onStepCompleted?.Invoke(mission, stepIndex);
        Debug.Log($"Paso completado: {mission.name} - Paso {stepIndex + 1}");

        // Verificar si la mision esta completa
        CheckMissionCompletion(mission);
    }

    // --- VERIFICAR COMPLETADO --- //
    private void CheckMissionCompletion(MissionData mission)
    {
        foreach (var step in mission.steps)
            if (!step.isCompleted) return;

        CompleteMission(mission);
    }

    private void CompleteMission(MissionData mission)
    {
        activeMissions.Remove(mission);
        completedMissions.Add(mission);

        // Activar flag de completado
        if (!string.IsNullOrEmpty(mission.completionFlagKey))
            FlagManager.Instance.SetFlag(mission.completionFlagKey, true);

        // Dar recompensa de cordura
        if (mission.sanityReward > 0)
            SanityManager.Instance.ModifySanity(mission.sanityReward);

        onMissionCompleted?.Invoke(mission);
        Debug.Log($"Mision completada: {mission.name}");
    }

    // --- GETTERS --- //
    public List<MissionData> GetActiveMissions() => activeMissions;
    public List<MissionData> GetCompletedMissions() => completedMissions;

    public MissionData GetMainMission()
    {
        return activeMissions.Find(m => m.missionType == MissionType.Main);
    }

    public bool IsMissionActive(MissionData mission)
    {
        return activeMissions.Contains(mission);
    }

    public bool IsMissionCompleted(MissionData mission)
    {
        return completedMissions.Contains(mission);
    }

    // --- VERIFICAR FLAGS --- //
    // Llamar esto periodicamente o cuando cambie una flag
    public void CheckMissionFlags(List<MissionData> allMissions)
    {
        foreach (var mission in allMissions)
        {
            if (activeMissions.Contains(mission)) continue;
            if (completedMissions.Contains(mission)) continue;

            // Verificar si se debe desbloquear
            if (!string.IsNullOrEmpty(mission.unlockFlagKey))
            {
                if (FlagManager.Instance.GetFlag(mission.unlockFlagKey))
                    UnlockMission(mission);
            }
        }
    }
}