using UnityEngine;

public class PuzzleData : ScriptableObject
{
    [SerializeReference, SubclassSelector]public PuzzleBehaviour behaviour;
    public string puzzleKey;
    public string description;
}
