using System.Collections.Generic;
using UnityEngine;

public class UIAnimationTester : MonoBehaviour
{
    [SerializeField] private RectTransform testPanel;
    [SerializeField] private UIAnimationPreset fadeInPreset;
    [SerializeField] private UIAnimationPreset fadeOutPreset;
    [SerializeField] private UIAnimationPreset slideInPreset;
    [SerializeField] private UIAnimationPreset slideOutPreset;
    [SerializeField] private UIAnimationPreset scaleInPreset;
    [SerializeField] private UIAnimationPreset scaleOutPreset;
    [SerializeField] private UIAnimationPreset shakePreset;
    [SerializeField] private UIAnimationPreset punchPreset;
    [SerializeField] private UIAnimationPreset rotatePreset;
    [SerializeField] private UIAnimationPreset loopPreset;
    [SerializeField] private UIAnimationPreset colorTweenPreset;
    [SerializeField] private UIAnimationPreset fillAmountPreset;
    [SerializeField] private UIAnimationPreset staggerPreset;
    [SerializeField] private List<RectTransform> staggerTargets;

    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Alpha1))
            UIAnimationManager.Instance.Play(testPanel, fadeInPreset,
                () => Debug.Log("FadeIn completo"));

        if (Input.GetKeyDown(KeyCode.Alpha2))
            UIAnimationManager.Instance.Play(testPanel, fadeOutPreset,
                () => Debug.Log("FadeOut completo"));
        */
        if (Input.GetKeyDown(KeyCode.Alpha3))
            UIAnimationManager.Instance.Play(testPanel, slideInPreset,
                () => Debug.Log("SlideIn completo"));

        if (Input.GetKeyDown(KeyCode.Alpha4))
            UIAnimationManager.Instance.Play(testPanel, slideOutPreset,
                () => Debug.Log("SlideOut completo"));

        if (Input.GetKeyDown(KeyCode.Alpha5))
            UIAnimationManager.Instance.Play(testPanel, scaleInPreset,
                () => Debug.Log("ScaleIn completo"));

        if (Input.GetKeyDown(KeyCode.Alpha6))
            UIAnimationManager.Instance.Play(testPanel, scaleOutPreset,
                () => Debug.Log("ScaleOut completo"));

        if (Input.GetKeyDown(KeyCode.Alpha7))
            UIAnimationManager.Instance.Play(testPanel, shakePreset,
                () => Debug.Log("Shake completo"));

        if (Input.GetKeyDown(KeyCode.Alpha8))
            UIAnimationManager.Instance.Play(testPanel, punchPreset,
                () => Debug.Log("Punch completo"));

        if (Input.GetKeyDown(KeyCode.Alpha9))
            UIAnimationManager.Instance.Play(testPanel, rotatePreset,
                () => Debug.Log("Rotate completo"));

        if (Input.GetKeyDown(KeyCode.Alpha0))
            UIAnimationManager.Instance.Play(testPanel, loopPreset,
                () => Debug.Log("Loop completo"));

        if (Input.GetKeyDown(KeyCode.Alpha1))
            UIAnimationManager.Instance.Play(testPanel, colorTweenPreset,
                () => Debug.Log("ColorTween completo"));

        if (Input.GetKeyDown(KeyCode.Alpha2))
            UIAnimationManager.Instance.Play(testPanel, fillAmountPreset,
                () => Debug.Log("FillAmount completo"));

        if (Input.GetKeyDown(KeyCode.R))
            UIAnimationManager.Instance.PlayStagger(staggerTargets, staggerPreset,
                staggerPreset.staggerDelay,
                () => Debug.Log("Stagger completo"));
    }
}