using System.Collections;
using System.Collections.Generic;
using Udar.SceneManager;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public SceneField introScene;

    public UIAnimator menuAnimator;
    public AnimationClip showMenuSlow;
    public AnimationClip showMenuQuick;
    public AnimationClip hideMenu;

    private GameScene gameScene;

    private void Start()
    {
        Loader.LoadScene(introScene.BuildIndex, (gs) => gameScene = gs);
        menuAnimator.Play(showMenuSlow);
    }

    public void NewGame()
    {
        SaveManager.NewGame("");
        menuAnimator.Play(hideMenu, () =>
        {
            gameScene.GetComponent<CinematicPlayer>().Play(() =>
            {
                //TODO load first tutorial level
            });
        });
    }
}
