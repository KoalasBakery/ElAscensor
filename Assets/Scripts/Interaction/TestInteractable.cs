using UnityEngine;

public class TestInteractable : Interactable
{
    public override void Interact()
    {
        Debug.Log("Objeto: " + gameObject.name);
    }

    public override void OnPlayerEnter()
    {
        Debug.Log("Se puede interactuar con: " + gameObject.name);
    }

    public override void OnPlayerExit()
    {
        Debug.Log("Jugador se alejó de: " + gameObject.name);
    }
}