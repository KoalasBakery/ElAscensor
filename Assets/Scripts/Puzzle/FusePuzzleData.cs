using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPuzzleData", menuName = "Events/Puzzle/Fuse")]
public class FusePuzzleData : PuzzleData
{
    [field: SerializeField] public FuseData[] fuses { get; private set; }
    [field: SerializeField] public Sprite fuseSprite { get; private set; }
    [field: SerializeField] public Vector2 offset { get; private set; }
    [field: SerializeField] public Vector2 spacing { get; private set; }
    [field: SerializeField] public GameObject lineRendPrefab { get; private set; }
}
[Serializable]
public class FuseData
{
    [Tooltip("Grid Position (n, 2)")]public Vector2Int fuseStart, fuseEnd;
    public Gradient color;
}
