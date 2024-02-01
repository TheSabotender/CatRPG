using UnityEngine;
using UnityEngine.Events;

public class GameScene : MonoBehaviour
{
    public UnityEvent onPreLoad;
    public UnityEvent onPostLoad;
    public UnityEvent onReload;
    public UnityEvent onUnload;

    /// <summary>
    /// Called after the scene is loaded, but before it is activated
    /// </summary>
    public virtual void OnPreLoad()
    {
        onPreLoad?.Invoke();
    }

    /// <summary>
    /// Called after the scene is loaded and activated
    /// </summary>
    public virtual void OnPostLoad()
    {
        onPostLoad?.Invoke();
    }

    /// <summary>
    /// Called before the scene is unloaded
    /// </summary>
    public virtual void OnUnload()
    {
        onUnload?.Invoke();
    }

    /// <summary>
    /// Called if the scene is loaded but is already active
    /// </summary>
    public virtual void OnReload()
    {
        onReload?.Invoke();
    }
}
