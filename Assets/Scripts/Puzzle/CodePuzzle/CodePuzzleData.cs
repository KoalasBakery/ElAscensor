using UnityEngine;

[CreateAssetMenu(fileName = "NewPuzzleData", menuName = "Events/Puzzle/Code")]
public class CodePuzzleData : PuzzleData
{
    [field: SerializeField] public string code { get; private set; }

}
