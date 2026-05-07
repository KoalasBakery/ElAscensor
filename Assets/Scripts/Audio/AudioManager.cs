using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * MonoBehaviour para controlar AudioSource en escena
 */
public class AudioManager : MonoBehaviour
{
    #region Parameters
    public static AudioManager instance;
    [SerializeField, Range(1,25), Tooltip("Cantidad de AudioSources que se agregaran a la escena al inciar")] int initialPoolSize = 10;
    Queue<AudioSource> audioSourcesPool= new Queue<AudioSource>();
    #endregion


    #region MonoBehaviour Methods
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        InitPool();
    }
    #endregion


    #region Pool Methods
    void InitPool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            audioSourcesPool.Enqueue(CreacteAudioSource());
        }
    }
    AudioSource CreacteAudioSource()
    {
        GameObject audioSourceGameObject = new GameObject("AudioSource");
        AudioSource audioSource = audioSourceGameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSourceGameObject.SetActive(false);
        return audioSource;
    }
    #endregion


    #region Audio execution methods
    AudioSource PlayAudioSource(AudioData _audioData)
    {
        AudioSource source = audioSourcesPool.Count > 0 ? audioSourcesPool.Dequeue() : CreacteAudioSource();
        source.gameObject.SetActive(true);
        ClipWithVolume clip = _audioData.GetRandomClip;
        source.clip = clip.clip;
        source.volume = clip.volume;
        source.outputAudioMixerGroup = _audioData.mixerGroup;
        source.pitch = _audioData.GetPitch;
        source.loop = _audioData.loop;
        source.priority = _audioData.priority;
        source.spatialBlend = _audioData.spatialBlend;
        source.reverbZoneMix = _audioData.reverbZoneMix;
        source.dopplerLevel = _audioData.dopplerLevel;
        source.spread = _audioData.spread;
        source.rolloffMode = _audioData.rolloffMode;
        source.minDistance = _audioData.minDistance;
        source.maxDistance = _audioData.maxDistance;

        source.Play();
        if (!_audioData.loop)
            StartCoroutine(StopAudioSource(clip.clip.length / source.pitch, source));

        return source;
    }
    IEnumerator StopAudioSource(float time, AudioSource source)
    {
        yield return Helpers.GetWait(time);
        audioSourcesPool.Enqueue(source);
        source.transform.parent = transform;
        source.Stop();
        source.clip = null;
        source.gameObject.SetActive(false);
    }
    #endregion
    
    
    #region Play Methods
    public void Play(AudioData _audioData, Transform _objectRef = null)
    {
        AudioSource source = PlayAudioSource(_audioData);
        if (_objectRef == null) return;
        source.transform.position= _objectRef.position;
        source.transform.parent= _objectRef;
        
    }
    public void Play(AudioData _audioData, Vector3 _position = default)
    {
        AudioSource source = PlayAudioSource(_audioData);
        source.transform.position = _position;
    }
    public void Play(AudioData _audioData)
    {
        PlayAudioSource(_audioData);
    }
    #endregion
}
