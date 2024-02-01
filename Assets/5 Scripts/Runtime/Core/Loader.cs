using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Udar.SceneManager;
using System;

public class Loader : MonoBehaviour
{
    private static Loader instance;

    public SceneField[] persistentScenes;    

    private static int currentSceneIndex = -1;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DoLoadPersistent());
    }
    

    public static void LoadScene(int index, Action<GameScene> onComplete)
    {
        instance.StartCoroutine(DoLoad(index, onComplete));
    }

    IEnumerator DoLoadPersistent()
    {
        for (int i = 0; i < persistentScenes.Length; i++)
        {
            yield return SceneManager.LoadSceneAsync(persistentScenes[i].BuildIndex, LoadSceneMode.Additive);
        }
        Debug.Log("Persistent scenes loaded");
    }

    static IEnumerator DoLoad(int index, Action<GameScene> onComplete)
    {
        if(currentSceneIndex == index)
        {
            Debug.Log("Scene is already loaded");

            foreach (GameObject go in SceneManager.GetSceneAt(currentSceneIndex).GetRootGameObjects())
            {
                var gameScene = go.GetComponent<GameScene>();
                if (gameScene != null)
                {
                    gameScene.OnReload();
                    onComplete?.Invoke(gameScene);
                    break;
                }
            }

            yield break;
        }

        //TODO show loading screen

        if(currentSceneIndex >= 0)
        {
            foreach(GameObject go in SceneManager.GetSceneAt(currentSceneIndex).GetRootGameObjects())
            {
                var gameScene = go.GetComponent<GameScene>();
                if (gameScene != null)
                {
                    gameScene.OnUnload();
                    break;
                }
            }

            yield return SceneManager.UnloadSceneAsync(currentSceneIndex);
        }

        currentSceneIndex = index;
        yield return SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);

        SceneManager.SetActiveScene(SceneManager.GetSceneAt(index));

        foreach (GameObject go in SceneManager.GetSceneAt(currentSceneIndex).GetRootGameObjects())
        {
            var gameScene = go.GetComponent<GameScene>();
            if (gameScene != null)
            {
                gameScene.OnPreLoad();
                break;
            }
        }

        //TODO hide loading screen

        Debug.Log("Scene loading complete");
        foreach (GameObject go in SceneManager.GetSceneAt(currentSceneIndex).GetRootGameObjects())
        {
            var gameScene = go.GetComponent<GameScene>();
            if (gameScene != null)
            {
                gameScene.OnPostLoad();
                onComplete?.Invoke(gameScene);
                break;
            }
        }
    }
}
