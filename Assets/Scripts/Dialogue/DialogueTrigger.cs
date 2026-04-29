using UnityEngine;

public class DialogueTrigger : Interactable
{
    [SerializeField] private DialogueData dialogueData;

    public override void Interact()
    {
        if (dialogueData != null)
            DialogueManager.Instance.StartDialogue(dialogueData);
    }
}