using UnityEngine;

public class FusePuzzle : PuzzleBehaviour
{
    LineRenderer lineRend;
    [SerializeField] Transform startPos, endPos;
    private void Awake()
    {
        lineRend = GetComponent<LineRenderer>();
    }

}
