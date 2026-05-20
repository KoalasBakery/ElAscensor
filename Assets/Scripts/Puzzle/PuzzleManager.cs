using UnityEngine;
using UnityEngine.InputSystem;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField] PuzzleData puzzle;
    public static PuzzleManager Instance;
    public bool activePuzzle;
    public PuzzleBehaviour currentPuzzle;
    [field: SerializeField] public GameObject codePuzzleHolder { get; private set; }
    [field: SerializeField] public GameObject fusePuzzleHolder { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
   
    public GameObject InstantiatePrefab(GameObject _prefab)
    {
        return Instantiate(_prefab);
    }
    public void DestroyGameObject(GameObject _objectToDestroy)
    { 
        Destroy(_objectToDestroy);
    }
    public void StartPuzzle(PuzzleData _data)
    {
        transform.position = Vector3.zero;
       //currentPuzzle = FindAnyObjectByType<PuzzleBehaviour >();
        activePuzzle = true;
    }
  
    [ContextMenu("Puzzle in Game")]
    public void StartPuzzle()
    {
        puzzle.behaviour.Init(puzzle, Instance);


        // transform.position = Vector3.zero;
        //currentPuzzle = FindAnyObjectByType<PuzzleBehaviour >();
        activePuzzle = true;
    }
    public void OnInteract(InputAction.CallbackContext context) => puzzle.behaviour.OnInteract(context);

    public void OnRelease() => puzzle.behaviour.OnRelease();
}
