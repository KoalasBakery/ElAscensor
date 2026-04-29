using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speakerName;
        [TextArea(2, 5)]
        public string text;
        public Sprite speakerPortrait; // Para después, el retrato del personaje si es que lo piden
    }

    public DialogueLine[] lines;
}
