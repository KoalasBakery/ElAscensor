using System;
using UnityEngine;
using UnityEngine.Audio;
/*
 * ScriptableObject que contiene la informacion necesaria para reproducir un evento de audio específico.
 * Los nuevos AudioData se deben de crear en el siguiente path Assets/Resources/Audio/Data para que el 
 * AudioManagerData los pueda cargar automáticamente y estén disponibles en el editor.
 * 
 * NOTA: El array de SoundCustomVolume permite asignar diferentes clips con volúmenes personalizados dentro del mismo evento de audio, 
 * lo que es útil para variar el sonido sin necesidad de crear múltiples AudioData para el mismo tipo de evento (ejemplo: pasos, golpes, etc.). 
 * El AudioManager eligira entre los clips disponibles y aplicara el volumen personalizado.
 */

[CreateAssetMenu(fileName = "AudioData", menuName = "Audio/AudioData")]
public class AudioData : ScriptableObject
{
    [field: SerializeField, Tooltip("Reproducira un clip random de este Array")] public ClipWithVolume[] clips { get; private set;  }
    [field: SerializeField] public AudioMixerGroup mixerGroup { get; private set; }
    [field: SerializeField,Range(0.8f, 1.5f), Tooltip("Minimo valor random del pitch")] public float pitchMin { get; private set; } = 0.9f;
    [field: SerializeField,Range(0.8f, 1.5f), Tooltip("Maximo valor random del pitch")] public float  pitchMax { get; private set; } = 1.1f;
    [field: SerializeField] public bool loop { get; private set; } = false;
    [field: SerializeField, Range(0, 256), Tooltip("Menos es mas prioridad")] public int priority { get; private set; } =150;
    [field: SerializeField, Range(0, 1), Tooltip("0 -> 2D (Ui o musica), 1 -> 3D (posicion en el mundo)")] public float spatialBlend { get; private set; } =0;
    [Header("3D settings")]
    [field: SerializeField, Range(0,1.1f), Tooltip("Cuanto afecta el reverb del entorno al sonido")] public float reverbZoneMix { get; private set; } =1;
    [field: SerializeField, Range(0, 5), Tooltip("Effecto doppler, ej: sirenas de ambulancia")] public int dopplerLevel { get; private set; } =1;
    [field: SerializeField, Range(0, 360), Tooltip("Anchura del espacio 3D, ej: explosion spread alto")] public int spread { get; private set; } =0;
    [field: SerializeField, Tooltip("Logarithmic -> Realista, Linear -> Uniforme, Custom -> Curva Personalizada")] public AudioRolloffMode rolloffMode { get; private set; } =AudioRolloffMode.Logarithmic;
    [field: SerializeField, Tooltip("No baja volumen"), Min(0)] public float minDistance { get; private set; } =1;
    [field: SerializeField, Tooltip("Ya no se escucha"), Min(0.01f)] public float maxDistance { get; private set; } =500;


    public float GetPitch=> (float)UnityEngine.Random.Range(pitchMin, pitchMax);
    public ClipWithVolume GetRandomClip => (clips != null && clips.Length > 0) ? clips[UnityEngine.Random.Range(0, clips.Length)]: null;
    public ClipWithVolume GetClipAtIndex(int idx) => (clips != null && clips.Length > 0) ? clips[Mathf.Clamp(idx, 0, clips.Length - 1)] : null;
}
/*
 * Clase publica que contiene el clip con volumen Custom
*/
[Serializable]
public class ClipWithVolume
{
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1;
}