using UnityEngine;

[CreateAssetMenu(fileName = "NewPuzzleData", menuName = "Events/PuzzleData")]
public class PuzzleData : ScriptableObject
{
    [SerializeReference, SubclassSelector]public PuzzleBehaviour adas;
    public string puzzleName;
    public string description;
}
