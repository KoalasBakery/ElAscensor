using UnityEngine;

public class PauseController : MonoBehaviour
{
    public static PauseController Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        //ElevatorButtonsController.Instance.transform.SetParent(transform);
       // PuzzleManager.Instance.codePuzzleHolder.transform.SetParent(transform); 
        //FadeController.Instance.transform.SetParent(transform);
        //gameObject.SetActive(false);
    }
  
}
