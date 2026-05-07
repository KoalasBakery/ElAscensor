using UnityEngine;
using UnityEngine.UI;

public class LocalizationSettingsManager : MonoBehaviour
{
    [SerializeField] GameObject languajeButtonPrefab;
    [SerializeField] Transform buttonsHolder;
    LanguageData[] languages;

    private void Awake()
    {
        languages = Resources.LoadAll<LanguageData>("");
        foreach (LanguageData languaje in languages)
        {
            CreateLanguageButton(languaje);
        }
    }

    void CreateLanguageButton(LanguageData _languaje)
    { 
        GameObject languajeButtonObject = Instantiate(languajeButtonPrefab, buttonsHolder);
        Image buttonImage = languajeButtonObject.GetComponent<Image>();
        Button button = languajeButtonObject.GetComponent<Button>();
        buttonImage.sprite = _languaje.sprite;
        //button.onClick.AddListener(() => LocalizationManager.Instance.SetLanguage(_languaje));




    }

}
