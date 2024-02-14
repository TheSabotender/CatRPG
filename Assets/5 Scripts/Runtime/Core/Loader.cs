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

    public static void LoadScene(string scene, Action<GameScene> onComplete)
    {
        int index = SceneManager.GetSceneByName(scene).buildIndex;
        instance.StartCoroutine(DoLoad(index, onComplete));
    }

    public static void LoadScene(int index, Action<GameScene> onComplete)
    {
        instance.StartCoroutine(DoLoad(index, onComplete));
    }

    IEnumerator DoLoadPersistent()
    {
        for (int i = 0; i < persistentScenes.Length; i++)
        {
            Debug.Log("Loading " + persistentScenes[i].Name);
            yield return SceneManager.LoadSceneAsync(persistentScenes[i].BuildIndex, LoadSceneMode.Additive);
        }
        Debug.Log("Persistent scenes loaded");
    }

    static IEnumerator DoLoad(int index, Action<GameScene> onComplete)
    {
        if(currentSceneIndex >= 0 && currentSceneIndex == index)
        {
            Debug.Log("Scene is already loaded");

            foreach (GameObject go in SceneManager.GetSceneByBuildIndex(currentSceneIndex).GetRootGameObjects())
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

        if (currentSceneIndex >= 0)
        {
            var prevScene = SceneManager.GetSceneByBuildIndex(currentSceneIndex);
            foreach (GameObject go in prevScene.GetRootGameObjects())
            {
                var gameScene = go.GetComponent<GameScene>();
                if (gameScene != null)
                {
                    Debug.Log("Unloading " + prevScene.name);
                    gameScene.OnUnload();
                    break;
                }
            }

            yield return SceneManager.UnloadSceneAsync(prevScene);
        }

        currentSceneIndex = index;        
        yield return SceneManager.LoadSceneAsync(currentSceneIndex, LoadSceneMode.Additive);
        var scene = SceneManager.GetSceneByBuildIndex(currentSceneIndex);

        foreach (GameObject go in scene.GetRootGameObjects())
        {
            var gameScene = go.GetComponent<GameScene>();
            if (gameScene != null)
            {
                gameScene.OnPreLoad();
                break;
            }
        }

        //TODO Apply save stuff

        SceneManager.SetActiveScene(scene);

        //TODO hide loading screen

        Debug.Log("Scene loading complete " + scene.name);
        foreach (GameObject go in scene.GetRootGameObjects())
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
