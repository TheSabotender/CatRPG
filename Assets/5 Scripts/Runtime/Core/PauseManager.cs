using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PauseManager : MonoBehaviour
{
    private static List<Object> pausers = new List<Object>();
    public Volume blurVolume;
    public CanvasGroup pauseMenu;

    public static PauseManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        pausers = new List<Object>();

        pauseMenu.alpha = 0f;
        pauseMenu.interactable = false;
        pauseMenu.blocksRaycasts = false;
    }

    private void Update()
    {
        Time.timeScale = pausers.Count == 0 ? 1 : 0;

        blurVolume.weight = Mathf.MoveTowards(blurVolume.weight, 1 - Time.timeScale, Time.unscaledDeltaTime);
        pauseMenu.alpha = Mathf.MoveTowards(pauseMenu.alpha, (pausers.Count == 0 || !pausers.Contains(gameObject)) ? 0 : 1, Time.unscaledDeltaTime);

        if (Input.GetKeyDown(KeyCode.Escape) && !IsPaused)
        {
            if (!IsPaused)
            {
                if (CinematicPlayer.IsInBlockingCutscene)
                    Pause(gameObject);
                else
                    MainMenu.Show();
            } else
            {
                if (pausers.Contains(gameObject))
                    Unpause(gameObject);
                else if(SaveManager.CurrentSaveGame != null)
                    MainMenu.Hide();
            }
        }
    }

    public static bool IsPaused => pausers.Count > 0;

    public static void Pause(Object source)
    {
        if(source == null)
        {
            Debug.LogWarning("Cannot pause without source");
            return;
        }

        if(!pausers.Contains(source))
            pausers.Add(source);

        if(instance != null && source == instance.gameObject)
        {            
            instance.pauseMenu.interactable = true;
            instance.pauseMenu.blocksRaycasts = true;
        }
    }

    public static void Unpause(Object source)
    {
        if(pausers.Contains(source))
        {
            pausers.Remove(source);
        }

        if (instance != null && source == instance.gameObject)
        {
            instance.pauseMenu.interactable = false;
            instance.pauseMenu.blocksRaycasts = false;
        }
    }
}
