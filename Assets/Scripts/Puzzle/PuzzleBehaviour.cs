
using System;
using UnityEngine;

public abstract class PuzzleBehaviour: MonoBehaviour
{
    protected event Action onPuzzleCompleted;
    protected event Action onPuzzleFailed;
    protected event Action<string> onPuzzleInputReceived;
    protected event Action<AudioData, Transform> soundReproductor;

    public PuzzleData data;
    public virtual void PuzzleComplete()
    { 
        onPuzzleCompleted?.Invoke();
    }
    public virtual void Init()
    {

    }
    public virtual void Start()
    { 
        soundReproductor+= AudioManager.instance.Play;

    }
    public virtual void End()
    {
    }
    public virtual void Input(string input)
    {
        onPuzzleInputReceived?.Invoke(input);
    }   

    public virtual void Update()
    {
        
    }
}
