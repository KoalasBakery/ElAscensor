using UnityEngine;

public class TestInteractable : Interactable
{
    public override void Interact()
    {
        Debug.Log("¡Interacción funcionando! Objeto: " + gameObject.name);
    }

    public override void OnPlayerEnter()
    {
        Debug.Log("Jugador cerca de: " + gameObject.name);
    }

    public override void OnPlayerExit()
    {
        Debug.Log("Jugador se alejó de: " + gameObject.name);
    }
}