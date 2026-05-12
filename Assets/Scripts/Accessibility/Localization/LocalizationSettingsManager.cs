using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
/*
 * Monobehaviour que crea un boton por cada idioma disponible en el proyecto, asignandole 
 * la bandera correspondiente y un evento para cambiar el idioma al ser pulsado.
*/

public class LocalizationSettingsManager : MonoBehaviour
{
    #region Parameters
    [SerializeField] string localizationFolderPath = "Localization/";
    [SerializeField] GameObject languajeButtonPrefab;
    [SerializeField] Transform buttonsHolder;
    Sprite[] flags;
    #endregion


    #region MonoBehaviour methods
    private IEnumerator Start()
    {
        LoadLocalizationSprites();

        yield return LocalizationSettings.InitializationOperation;

        List<Locale> locales = LocalizationSettings.AvailableLocales.Locales;

        for (int i = 0; i < locales.Count; ++i)
             CreateLanguageButton(locales[i].Formatter.ToString(), i);
    }
    #endregion


    #region Localization Methods
    [ContextMenu("Load Sprites")]
    void LoadLocalizationSprites()
    { 
        flags= Resources.LoadAll<Sprite>(localizationFolderPath+"Sprites");
    }

    void CreateLanguageButton(string _languaje, int _idx)
    { 
        GameObject languajeButtonObject = Instantiate(languajeButtonPrefab, buttonsHolder);
        Image buttonImage = languajeButtonObject.GetComponent<Image>();
        Button button = languajeButtonObject.GetComponent<Button>();

        foreach (var flag in flags)
        {
            if (flag.name != _languaje) continue;
            
            buttonImage.sprite = flag;
            break;
        }

        button.onClick.AddListener(() =>LocaleSelected(_idx));
    }

    static void LocaleSelected(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }
    #endregion
}
