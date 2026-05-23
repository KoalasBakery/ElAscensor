using TMPro;
using UnityEngine;

public class ElevatorButtonsController : MonoBehaviour
{
    public static ElevatorButtonsController Instance { get; private set; }
    [SerializeField] TMP_Text floorText;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
      

        //gameObject.SetActive(false);
    }
    private void Start()
    {
        string startScene = SaveManager.GetSceneName();

        int lastIndex = startScene.LastIndexOfAny(new char[] { 'F' });
        string fString = "";

        for (int i = lastIndex + 1; i < startScene.Length; i++)
            fString += startScene[i];
        floorText.text = fString;

    }
    public void LoadScene(string _sceneName)
    {
        int lastIndex = _sceneName.LastIndexOfAny(new char[] { 'F' });
        string fString = "";

        for (int i = lastIndex + 1; i < _sceneName.Length; i++)
            fString += _sceneName[i];

        floorText.text = fString;

        SaveManager.SaveSceneName(_sceneName);
        Helpers.LoadScene(_sceneName);
        FadeController.Instance.FadeOut();


    }

    public void OnTouchButton(string _value)
    {
      
        FadeController.Instance.FadeIn(()=> LoadScene(_value));
    }
}
