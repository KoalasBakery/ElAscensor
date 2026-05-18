using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField] PuzzleData puzzle;
    public static PuzzleManager Instance { get; private set; }
    public bool activePuzzle;
    public PuzzleBehaviour currentPuzzle;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Update()
    {
    }
    public void StartPuzzle(PuzzleData _data)
    {
        transform.position = Vector3.zero;
        currentPuzzle = FindAnyObjectByType<PuzzleBehaviour >();
        activePuzzle = true;
    }
    public void OnInteract(InputAction.CallbackContext context) => currentPuzzle.OnInteract(context);

    public void OnRelease() => currentPuzzle.OnRelease();

}
