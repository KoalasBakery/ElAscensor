using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * ---------------------------------------------------------------
 *                   OBJECT SANITY EFFECT
 * ---------------------------------------------------------------
 * DESCRIPCION:
 * Controla objetos que aparecen o desaparecen segun el nivel
 * de cordura con efecto de fade suave.
 * ---------------------------------------------------------------
 */

public class ObjectSanityEffect : SanityEffectBase
{
    [System.Serializable]
    public class SanityObject
    {
        [Tooltip("Objeto a controlar")]
        public GameObject targetObject;

        [Tooltip("Niveles de cordura en los que este objeto es visible")]
        public List<int> visibleAtLevels;

        [Tooltip("Desactivar el collider ademas de ocultarlo")]
        public bool disableCollider = false;

        [HideInInspector]
        public bool isCurrentlyVisible = false;

        [HideInInspector]
        public Coroutine fadeCoroutine = null;
    }

    [Header("Objetos controlados por cordura")]
    [SerializeField] private List<SanityObject> sanityObjects;

    [Header("Fade")]
    [SerializeField] private float fadeDuration = 1f;

    private void Start()
    {
        StartCoroutine(InitializeDelayed());
    }

    private IEnumerator InitializeDelayed()
    {
        yield return null;
        UpdateObjects(SanityManager.Instance.CurrentLevel);
    }

    public override void OnSanityChanged(float currentSanity, float maxSanity) { }

    public override void OnLevelChanged(int newLevel)
    {
        if (!isEnabled) return;
        UpdateObjects(newLevel);
    }

    private void UpdateObjects(int currentLevel)
    {
        foreach (var sanityObject in sanityObjects)
        {
            if (sanityObject.targetObject == null) continue;

            bool shouldBeVisible = sanityObject.visibleAtLevels.Contains(currentLevel);

            if (shouldBeVisible != sanityObject.isCurrentlyVisible)
            {
                // Cancelar fade anterior si existe
                if (sanityObject.fadeCoroutine != null)
                    StopCoroutine(sanityObject.fadeCoroutine);

                sanityObject.fadeCoroutine = StartCoroutine(
                    FadeObject(sanityObject, shouldBeVisible));

                sanityObject.isCurrentlyVisible = shouldBeVisible;
            }
        }
    }

    private IEnumerator FadeObject(SanityObject sanityObject, bool fadeIn)
    {
        GameObject obj = sanityObject.targetObject;

        // Obtener todos los SpriteRenderers del objeto y sus hijos
        SpriteRenderer[] renderers = obj.GetComponentsInChildren<SpriteRenderer>();

        if (renderers.Length == 0)
        {
            // Si no tiene SpriteRenderer usar SetActive directo, obvio asi no hay fade
            obj.SetActive(fadeIn);
            yield break;
        }

        // Activar el objeto antes del fade in
        if (fadeIn)
        {
            obj.SetActive(true);

            // Empezar transparente
            foreach (var r in renderers)
            {
                Color c = r.color;
                c.a = 0f;
                r.color = c;
            }
        }

        // Desactivar collider si fade out
        if (!fadeIn && sanityObject.disableCollider)
        {
            Collider2D col = obj.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
        }

        float elapsed = 0f;
        float startA = fadeIn ? 0f : 1f;
        float targetA = fadeIn ? 1f : 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            float alpha = Mathf.Lerp(startA, targetA, t);

            foreach (var r in renderers)
            {
                Color c = r.color;
                c.a = alpha;
                r.color = c;
            }

            yield return null;
        }

        // Asegurar valor final
        foreach (var r in renderers)
        {
            Color c = r.color;
            c.a = targetA;
            r.color = c;
        }

        // Desactivar objeto al terminar fade out
        if (!fadeIn)
            obj.SetActive(false);

        // Activar collider si fade in
        if (fadeIn && sanityObject.disableCollider)
        {
            Collider2D col = obj.GetComponent<Collider2D>();
            if (col != null) col.enabled = true;
        }

        sanityObject.fadeCoroutine = null;
        Debug.Log($"'{obj.name}' -> {(fadeIn ? "aparecio" : "desaparecio")}");
    }

    public void RegisterObject(GameObject obj, List<int> levels, bool disableCollider = false)
    {
        sanityObjects.Add(new SanityObject
        {
            targetObject = obj,
            visibleAtLevels = levels,
            disableCollider = disableCollider
        });
        StartCoroutine(InitializeDelayed());
    }
}