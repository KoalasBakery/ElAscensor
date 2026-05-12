using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
/*
 * MonoBehaviour para controlar AudioSource en escena, toda la informacion 
 * del AudioSource se setea a partir de un AudioData, si el AuidoData esta
 * asociado a un AudioMixerGroup de musica, el AudioManager se encargara de 
 * hacer una transicion suave entre la musica activa y la nueva musica a reproducir.
 * Los AudioSoure que esten reproduciendo musica no se desactivaran al cargar otra escena
 * ya que se establecen como hijo del AudioManager, lo que permite mantener la musica activa entre escenas.
 */
public class AudioManager : MonoBehaviour
{
    #region Parameters
    public static AudioManager instance;
    [SerializeField, Range(1,25), Tooltip("Cantidad de AudioSources que se agregaran a la escena al inciar")] int initialPoolSize = 10;

    [Header("Music Transition")]
    [SerializeField, Range(0.01f, 10), Tooltip("Velocidad de transicion entre la musica activa y la que se quiere activar")] float musicTransitionSpeed = 1;
    [SerializeField, Tooltip("Grupo de musica del AuidoMixer")] AudioMixerGroup musicAudioGroup;
    
    Queue<AudioSource> audioSourcesPool= new Queue<AudioSource>();
    AudioSource currentMusicAudioSource;
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
        audioSourceGameObject.transform.parent = transform;
        return audioSource;
    }
    #endregion


    #region Audio execution methods
    AudioSource PlayAudioSource(AudioData _audioData)
    {
        AudioSource source = audioSourcesPool.Count > 0 ? audioSourcesPool.Dequeue() : CreacteAudioSource();
        ClipWithVolume clip = _audioData.GetRandomClip;
        
        source.gameObject.SetActive(true);

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

        if (source.outputAudioMixerGroup== musicAudioGroup)
            StartCoroutine(TransitionMusicAudioSource(source));
        else
            source.Play();
        
        if (!_audioData.loop)
            StartCoroutine(StopAudioSource(clip.clip.length / source.pitch, source));

        return source;
    }
    IEnumerator StopAudioSource(float _time, AudioSource _source)
    {
        yield return Helpers.GetWait(_time);
        StopAudioSource(_source);
    }
    void StopAudioSource(AudioSource _source)
    {
        audioSourcesPool.Enqueue(_source);
        _source.transform.parent = transform;
        _source.Stop();
        _source.gameObject.SetActive(false);
    }
    IEnumerator TransitionMusicAudioSource(AudioSource _newMusicAudioSource)
    {
        if (currentMusicAudioSource!=null)
        {
            while (currentMusicAudioSource.volume > 0.01)
            {
                currentMusicAudioSource.volume= Mathf.Lerp(currentMusicAudioSource.volume, 0, Time.deltaTime * musicTransitionSpeed);
                //currentMusicAudioSource.volume -= Time.deltaTime* musicTransitionSpeed;
                yield return Helpers.GetWaitForEndOfFrame();
            }
            StopAudioSource(currentMusicAudioSource);
            currentMusicAudioSource.volume = 0;
        }

        currentMusicAudioSource = _newMusicAudioSource;
        currentMusicAudioSource.volume = 0;
        currentMusicAudioSource.Play();


        while (currentMusicAudioSource.volume < .99)
        {
            currentMusicAudioSource.volume=Mathf.Lerp(currentMusicAudioSource.volume, 1, Time.deltaTime * musicTransitionSpeed);
           // currentMusicAudioSource.volume += Time.deltaTime * musicTransitionSpeed;
            yield return Helpers.GetWaitForEndOfFrame();
        }


        currentMusicAudioSource.volume = 1;
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
