using System.Collections;
using TMPro;
using UnityEngine;
using static DialogueData;

public class DialogueTextEffects : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    private Coroutine activeEffect;

    public bool IsPlayingEffect { get; private set; }

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    public void PlayEffect(TextEffect effect)
    {
        StopCurrentEffect();

        switch (effect)
        {
            case TextEffect.Wave:
                activeEffect = StartCoroutine(WaveEffect());
                break;
            case TextEffect.Shake:
                activeEffect = StartCoroutine(ShakeEffect());
                break;
            case TextEffect.FadeIn:
                activeEffect = StartCoroutine(FadeInEffect());
                break;
            case TextEffect.None:
            default:
                ResetVertices();
                break;
        }
    }

    public void StopCurrentEffect()
    {
        if (activeEffect != null)
        {
            StopCoroutine(activeEffect);
            activeEffect = null;
        }
        IsPlayingEffect = false;
        ResetVertices();
    }

    // --- WAVE --- //
    private IEnumerator WaveEffect()
    {
        IsPlayingEffect = true;

        // Esperar un frame para que TMP procese el mesh
        yield return null;

        while (true)
        {
            textMesh.ForceMeshUpdate();
            TMP_TextInfo textInfo = textMesh.textInfo;

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                int vertexIndex = charInfo.vertexIndex;
                int materialIndex = charInfo.materialReferenceIndex;
                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                float wave = Mathf.Sin(Time.time * 2f + i * 0.5f) * 3f;

                for (int v = 0; v < 4; v++)
                    vertices[vertexIndex + v] += new Vector3(0, wave, 0);
            }

            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
                meshInfo.mesh.vertices = meshInfo.vertices;
                textMesh.UpdateGeometry(meshInfo.mesh, i);
            }

            yield return null;
        }
    }

    // --- SHAKE --- //
    private IEnumerator ShakeEffect()
    {
        IsPlayingEffect = true;

        while (true)
        {
            textMesh.ForceMeshUpdate();
            TMP_TextInfo textInfo = textMesh.textInfo;

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                int vertexIndex = charInfo.vertexIndex;
                int materialIndex = charInfo.materialReferenceIndex;
                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                Vector3 shake = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f),
                    0) * 0.5f;

                for (int v = 0; v < 4; v++)
                    vertices[vertexIndex + v] += shake;
            }

            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
                meshInfo.mesh.vertices = meshInfo.vertices;
                textMesh.UpdateGeometry(meshInfo.mesh, i);
            }

            yield return new WaitForSeconds(0.05f);
        }
    }

    // --- FADE IN --- //
    private IEnumerator FadeInEffect()
    {
        IsPlayingEffect = true;

        textMesh.ForceMeshUpdate();
        TMP_TextInfo textInfo = textMesh.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int vertexIndex = charInfo.vertexIndex;
            int materialIndex = charInfo.materialReferenceIndex;
            Color32[] colors = textInfo.meshInfo[materialIndex].colors32;

            float elapsed = 0f;
            float duration = 0.15f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                byte alpha = (byte)Mathf.Lerp(0, 255, elapsed / duration);

                for (int v = 0; v < 4; v++)
                    colors[vertexIndex + v].a = alpha;

                textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                yield return null;
            }

            for (int v = 0; v < 4; v++)
                colors[vertexIndex + v].a = 255;

            textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }

        IsPlayingEffect = false;
    }

    // --- RESET --- //
    private void ResetVertices()
    {
        if (textMesh == null) return;

        textMesh.ForceMeshUpdate();
        TMP_TextInfo textInfo = textMesh.textInfo;

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
            if (meshInfo.mesh != null)
            {
                meshInfo.mesh.vertices = meshInfo.vertices;
                textMesh.UpdateGeometry(meshInfo.mesh, i);
            }
        }
    }
}