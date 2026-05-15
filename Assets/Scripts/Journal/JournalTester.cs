using UnityEngine;

public class JournalTester : MonoBehaviour
{
    [SerializeField] private MissionData testMission;
    [SerializeField] private NoteData testNote;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
            MissionManager.Instance.UnlockMission(testMission);

        if (Input.GetKeyDown(KeyCode.B))
            JournalUI.Instance.AddNote(testNote);

        if (Input.GetKeyDown(KeyCode.N))
        {
            // Solo completar si la mision esta activa
            if (MissionManager.Instance.IsMissionActive(testMission))
                MissionManager.Instance.CompleteStep(testMission, 0);
            else
                Debug.Log("Mision no activa, presiona M primero");
        }
    }
}