using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SaveSlot
{
    public string slotName;
    public Button slotButton;
    public TMP_Text slotText;
}

public class MainMenuController : MonoBehaviour
{
    [SerializeField,Tooltip("Nombre de la escena donde se precargan cosas")] string sceneLoaderName;
    [SerializeField,Tooltip("Nombre del primer nivel")] string firstLevelName;
    [SerializeField] string newGameString, continueString;
    [SerializeField] SaveSlot[] slots;

    private void Start()
    {
        foreach (SaveSlot slot in slots)
        {
            slot.slotButton.onClick.AddListener(() => StartGame(slot.slotName)); 

            if (SaveManager.SaveExist(slot.slotName))
                slot.slotText.text = "Continue";

            else
            { 
                slot.slotText.text = "New Game";
                slot.slotButton.onClick.AddListener(() => SaveManager.SaveSceneName(firstLevelName));
            }
        }
    }
    public void StartGame(string _slotName)
    {
        PlayerPrefs.SetString(SaveManager.saveSlotConst, _slotName);
        FadeController.Instance.FadeIn(() => Helpers.LoadScene(sceneLoaderName));
    }
}
