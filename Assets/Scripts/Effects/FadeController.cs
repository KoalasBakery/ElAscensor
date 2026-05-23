using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/*
 * Monobehavoour que controla un SpriteRenderer para fadear la pantalla al entrar o salir de una escena. 
 * Tiene eventos para avisar cuando el fade in o fade out han terminado.
 */
public class FadeController : MonoBehaviour
{

    #region Parameters
    public static FadeController Instance;
    public event Action OnFadeInComplete;
    public event Action OnFadeOutComplete;
    Image sprRend;
    [SerializeField] Color fadeColor= Color.black;
    #endregion


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
       // DontDestroyOnLoad(gameObject);
        sprRend = GetComponent<Image>();
        sprRend.color = fadeColor;
        FadeOut();
    }

    #region Fade In Methods
    public void FadeIn()
    {
        fadeColor.a = 0;
        sprRend.color = fadeColor;
        StartCoroutine(Fading(0, 1));
    }
    public void FadeIn(float _time)
    {
        fadeColor.a = 0;
        sprRend.color = fadeColor;
        StartCoroutine(Fading(0, 1, _time));
    } 
    public void FadeIn(float _time, Action _event)
    {
        OnFadeInComplete += _event;
        fadeColor.a = 0;
        sprRend.color = fadeColor;
        StartCoroutine(Fading(0, 1, _time));
    }
    public void FadeIn(Action _event)
    {
        OnFadeInComplete += _event;
        fadeColor.a = 0;
        sprRend.color = fadeColor;
        StartCoroutine(Fading(0, 1));
    }
    #endregion


    #region Fade Out Methods
    public void FadeOut()
    {
        fadeColor.a = 1;
        sprRend.color = fadeColor;
        StartCoroutine(Fading(1, 0));
    }
    public void FadeOut(float _time)
    {
        fadeColor.a = 1;
        sprRend.color = fadeColor;
        StartCoroutine(Fading(1, 0, _time));
    }
    public void FadeOut(float _time, Action _event)
    {
        OnFadeOutComplete += _event;
        fadeColor.a = 1;
        sprRend.color = fadeColor;
        StartCoroutine(Fading(1, 0, _time));
    }
    public void FadeOut(Action _event)
    {
        OnFadeOutComplete += _event;
        fadeColor.a = 1;
        sprRend.color = fadeColor;
        StartCoroutine(Fading(1, 0));
    }
    #endregion

    IEnumerator Fading(float _alphaStart, float _alphaEnd, float _fadeTime = 1)
    {

        float elapsedTime = 0;
        Color color = sprRend.color;

        while (elapsedTime < _fadeTime)
        {
            float alpha = Mathf.Lerp(_alphaStart, _alphaEnd, elapsedTime / _fadeTime);
            color.a = alpha;
            sprRend.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        color.a = _alphaEnd;
        sprRend.color = color;

        if (_alphaEnd == 1)
            OnFadeInComplete?.Invoke();
        else
            OnFadeOutComplete?.Invoke();
        OnFadeInComplete = null;
        OnFadeOutComplete = null;
    }
}
