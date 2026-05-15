using UnityEngine;
using UnityEngine.Events;

/*
 * ---------------------------------------------------------------
 *                      SANITY MANAGER
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * Singleton que controla el sistema de cordura del jugador.
 * Rango: 0-50, dividido en 4 niveles cada 15 puntos.
 *
 * NIVELES:
 *   Nivel 4: 50-36 - Completamente cuerdo
 *   Nivel 3: 35-21 - Algo estresado
 *   Nivel 2: 20-06 - Perdiendo cordura
 *   Nivel 1: 05-00 - Al limite (Game Over si llega a 0)
 *
 * COMO USARLO:
 *   SanityManager.Instance.ModifySanity(-10); // bajar cordura
 *   SanityManager.Instance.ModifySanity(5);   // subir cordura
 *   SanityManager.Instance.CurrentLevel;       // nivel actual
 *
 * DEPENDENCIAS:
 *   - SanityEffects (efectos visuales)
 *   - SanityUI      (barra visual)
 *
 * TODO: Conectar con sistema de guardado cuando este listo
 * ---------------------------------------------------------------
 */

public class SanityManager : MonoBehaviour
{
    public static SanityManager Instance { get; private set; }

    [Header("Configuracion")]
    [SerializeField] private float maxSanity = 50f;
    [SerializeField] private float startingSanity = 50f;

    [Header("Niveles")]
    [SerializeField] private float level4Threshold = 36f; // 50-36 nivel 4
    [SerializeField] private float level3Threshold = 21f; // 35-21 nivel 3
    [SerializeField] private float level2Threshold = 6f;  // 20-06 nivel 2
                                                          // 05-00 nivel 1

    // Eventos
    public UnityEvent<float> onSanityChanged;     // manda el valor actual
    public UnityEvent<int> onSanityLevelChanged;  // manda el nivel nuevo
    public UnityEvent onGameOver;                 // cordura llego a 0

    // Estado
    public float CurrentSanity { get; private set; }
    public int CurrentLevel { get; private set; }
    public float MaxSanity => maxSanity;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        CurrentSanity = startingSanity;
        CurrentLevel = CalculateLevel(CurrentSanity);
        onSanityChanged?.Invoke(CurrentSanity);
    }

    // --- MODIFICAR CORDURA --- //
    public void ModifySanity(float amount)
    {
        float previousSanity = CurrentSanity;
        int previousLevel = CurrentLevel;

        // Aplicar cambio con limites
        CurrentSanity = Mathf.Clamp(CurrentSanity + amount, 0f, maxSanity);

        // Calcular nuevo nivel
        int newLevel = CalculateLevel(CurrentSanity);

        // Avisar del cambio de valor
        onSanityChanged?.Invoke(CurrentSanity);

        // Avisar si cambio de nivel
        if (newLevel != previousLevel)
        {
            CurrentLevel = newLevel;
            onSanityLevelChanged?.Invoke(CurrentLevel);
            Debug.Log($"Nivel de cordura cambio: {previousLevel} -> {CurrentLevel}");
        }

        // Game Over si llego a 0
        if (CurrentSanity <= 0f)
        {
            Debug.Log("GAME OVER - Cordura perdida");
            onGameOver?.Invoke();
        }

        Debug.Log($"Cordura: {CurrentSanity}/{maxSanity} - Nivel: {CurrentLevel}");
    }

    // --- CALCULAR NIVEL --- //
    private int CalculateLevel(float sanity)
    {
        if (sanity >= level4Threshold) return 4;
        if (sanity >= level3Threshold) return 3;
        if (sanity >= level2Threshold) return 2;
        return 1;
    }

    // --- HELPERS --- //
    public float GetSanityPercentage()
    {
        return CurrentSanity / maxSanity;
    }

    public bool IsAtLevel(int level)
    {
        return CurrentLevel == level;
    }
}