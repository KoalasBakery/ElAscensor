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
        Helpers.LoadScene(SaveManager.GetSceneName());

    }
    [ContextMenu("DeleteSave")]
    public void LoadScene()
    {
        SaveManager.DeleteSaved();
    }   
}
