﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public static SceneHandler Instance;
    // Start is called before the first frame update
    public static void OnLoad()
    {
        if (Instance != null)
        {
            return;
        }

        GameObject go = new GameObject("Scene Handler");
        SceneHandler newManager = go.AddComponent<SceneHandler>();
        Instance = newManager;
    }
    public void SwitchToWorldScene()
    {
        if (IsSceneLoaded("Auth"))
            StartCoroutine(UnloadScene("Auth"));

        if (!IsSceneLoaded("World"))
            StartCoroutine(LoadScene("World", LoadSceneMode.Additive));
    }
    public void SwitchToAuthScene()
    {
        if (IsSceneLoaded("World"))
            StartCoroutine(UnloadScene("World"));

        if (!IsSceneLoaded("Auth"))
            StartCoroutine(LoadScene("Auth", LoadSceneMode.Additive));
    }
    internal IEnumerator LoadScene(string sceneName, LoadSceneMode mode)
    {
        yield return SceneManager.LoadSceneAsync(sceneName, mode);
    }

    internal IEnumerator UnloadScene(string sceneName)
    {
        yield return SceneManager.UnloadSceneAsync(sceneName);
    }

    internal bool IsSceneLoaded(string sceneName)
    {
        var scene = SceneManager.GetSceneByName(sceneName);

        if (scene.name == null)
        {
            return false;
        }

        return true;
    }
    public static GameObject GetChildByName(GameObject parent, string childName)
    {
        Transform[] _Children = parent.transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform _child in _Children)
        {
            if (_child.gameObject.name == childName)
            {
                return _child.gameObject;
            }
        }

        return null;
    }
}
