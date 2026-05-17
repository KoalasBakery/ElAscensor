using UnityEngine;

[CreateAssetMenu(fileName = "NewPuzzleData", menuName = "Events/PuzzleData")]
public class PuzzleData : ScriptableObject
{
    [field: SerializeField, SerializeReference, SubclassSelector] public PuzzleBehaviour puzzleBehaviour { get; private set; }
    public string puzzleName;
    public string description;
}
