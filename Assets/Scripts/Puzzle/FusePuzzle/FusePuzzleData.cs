using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPuzzleData", menuName = "Events/Puzzle/Fuse")]
public class FusePuzzleData : PuzzleData
{
    [field: SerializeField] public FuseData[] fuses { get; private set; }
    [field: SerializeField] public Fuse fusePrefab { get; private set; }
    [field: SerializeField,Tooltip("Offset respecto al centro del objeto padre")] public Vector2 offset { get; private set; }
    [field: SerializeField, Tooltip("Espacio entre los fusibles")] public Vector2 spacing { get; private set; }
    [field: SerializeField, Tooltip("Escala de todos los fusibles")] public Vector2 scale { get; private set; } =Vector2.one;
}
[Serializable]
public class FuseData
{
    [Tooltip("Grid Position (n, 2)")]public Vector2Int fuseStart, fuseEnd;
    [Tooltip("Color de fusbiles y line renderer")]public Gradient color;
    [Tooltip("Sprite por fusible")]public Sprite sprite;
    [Tooltip("Escala local de cada fusible")]public Vector2 scale=Vector2.one;
}
