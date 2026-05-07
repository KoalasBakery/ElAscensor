using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
/*
 * MonoBehaviour para controlar los ajustes de audio del juego, creando sliders dinamicamente segun los canales del AudioMixer asignado. 
 * Hace el guardado de los ajustes usando PlayerPrefs, lo que permite que se mantengan entre sesiones de juego.
 */


public class AudioSettingsManager : MonoBehaviour
{
    [SerializeField] int defaultVolume = 10;
    [SerializeField] GameObject volumeSliderPrefab;
    [SerializeField] Transform slidersHolder;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioData sliderAudioData;
    [SerializeField] AudioData testAudioData;
    const string vol = "Volume";
    private void Start()
    {
        SetAudioMixer();
        InvokeRepeating(nameof(TestSfx), 2, 3);
        Invoke(nameof(TestMusic), 2);
    }
    void TestSfx()
    {
        AudioManager.instance.Play(testAudioData);
    }
    void TestMusic()
    {
        AudioManager.instance.Play(sliderAudioData);
    }
    void SetAudioMixer()
    {
        foreach (var channel in audioMixer.FindMatchingGroups("Master"))
        {
            CreateSlider(channel.name);
        }
    }

    void CreateSlider(string _channelName)
    {
        GameObject sliderObject = Instantiate(volumeSliderPrefab, slidersHolder);
        Slider slider = sliderObject.GetComponent<Slider>();
        TMP_Text labelChannelName = sliderObject.GetComponentsInChildren<TMP_Text>()[0];
        TMP_Text labelValue = sliderObject.GetComponentsInChildren<TMP_Text>()[1];
        int volume;


        if (!PlayerPrefs.HasKey(_channelName + vol))
            volume = defaultVolume;
        else
            volume = PlayerPrefs.GetInt(_channelName + vol);


        sliderObject.name = _channelName + "VolumeSlider";
        labelChannelName.text = _channelName;
        slider.value = volume;
        labelValue.text = volume.ToString();
        SetVolumeInChannel(_channelName, volume);

        slider.onValueChanged.AddListener((value) =>
        {
            SetVolumeInChannel(_channelName, (int)value);
            labelValue.text = ((int)value).ToString();
        });

    }

    void SetVolumeInChannel(string _channelName, int _value)
    {
        audioMixer.SetFloat(_channelName, _value < 1 ? -80 : Mathf.Log10(_value / 10f) * 20);
        PlayerPrefs.SetInt(_channelName + vol, _value);
    }
}
