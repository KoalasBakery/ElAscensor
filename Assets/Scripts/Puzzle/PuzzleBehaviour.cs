
using System;
using UnityEngine;
using UnityEngine.InputSystem;
[Serializable]
public abstract class PuzzleBehaviour
{
    protected event Action onPuzzleCompleted;
    protected event Action onPuzzleFailed;
    protected event Action<string> onPuzzleInputReceived;
    protected event Action<AudioData, Transform> soundReproductor;

  //  public PuzzleData data;

    public virtual void OnInteract(InputAction.CallbackContext context)
    { 
    }
    public virtual void OnRelease()
    {
    }


    public virtual void PuzzleComplete()
    { 
        onPuzzleCompleted?.Invoke();
    }
    public virtual void Init(PuzzleData _newPuzzleData, PuzzleManager _manager)
    {
       // data = _newPuzzleData;
    }
  
    public virtual void Start()
    { 
        soundReproductor+= AudioManager.instance.Play;

    }
    public virtual void End()
    {
    }
    public virtual void OnClick(string input)
    {
        onPuzzleInputReceived?.Invoke(input);
    }   

    public virtual void Update()
    {
        
    }
}
