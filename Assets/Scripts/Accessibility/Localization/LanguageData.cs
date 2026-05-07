using UnityEngine;

[CreateAssetMenu(fileName = "NewLanguageData", menuName = "Localization/LanguageData")]
public class LanguageData : ScriptableObject
{
    [field: SerializeField] public Sprite sprite { get; private set; }
    [field: SerializeField] public string languajeName { get; private set; }
}
