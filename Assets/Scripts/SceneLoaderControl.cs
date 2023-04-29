using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderControl : MonoBehaviour
{
    public Animator loadTransition;
    private float transitionTime = 1f;
    public void LoadSceneWithIndex(int index)
    {
        StartCoroutine(LoadWithIndex(index));
    }
    public void LoadSceneWithName(string name)
    {
        StartCoroutine(LoadWithName(name));
    }
    IEnumerator LoadWithIndex(int SceneIndex)
    {
        loadTransition.SetTrigger("start");
        yield return new WaitForSecondsRealtime(transitionTime);
        Debug.Log("Load Scene:" + SceneIndex);
        SceneManager.LoadScene(SceneIndex);
    }
    IEnumerator LoadWithName(string SceneName)
    {
        loadTransition.SetTrigger("start");
        yield return new WaitForSecondsRealtime(transitionTime);
        Debug.Log("Load Scene:" + SceneName);
        SceneManager.LoadScene(SceneName);
    }
}
