using System.Collections;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] float timeToLoadScene = 2f;
    private void Start()
    {
        StartCoroutine(LoadLevelScene());
    }
    IEnumerator LoadLevelScene()
    {
        yield return Helpers.GetWait(timeToLoadScene);
        string sceneName = SaveManager.GetSceneName();
        if (sceneName != "" || sceneName != null)
        {
            Helpers.LoadScene(SaveManager.GetSceneName());
        }
        else
        { 
            Debug.LogWarning("No scene name saved, loading default scene");
        }

    }
    [ContextMenu("DeleteSave")]
    public void LoadScene()
    {
        SaveManager.DeleteSaved();
    }   
}
