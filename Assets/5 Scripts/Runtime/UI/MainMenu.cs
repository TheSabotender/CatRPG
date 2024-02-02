using Udar.SceneManager;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private static MainMenu instance;
    public SceneField introScene;

    public Button continueButton;

    public UIAnimator menuAnimator;
    public AnimationClip showMenuSlow;
    public AnimationClip showMenuQuick;
    public AnimationClip hideMenu;

    private GameScene gameScene;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        continueButton.interactable = !string.IsNullOrEmpty(SaveManager.LastSaveFile());

        Loader.LoadScene(introScene.BuildIndex, (gs) => gameScene = gs);
        menuAnimator.Play(showMenuSlow);
    }

    public static void Show()
    {
        instance.continueButton.interactable = !string.IsNullOrEmpty(SaveManager.LastSaveFile());
        instance.menuAnimator.Play(instance.showMenuQuick);
    }

    public static void Hide()
    {
        instance.continueButton.interactable = !string.IsNullOrEmpty(SaveManager.LastSaveFile());
        instance.menuAnimator.Play(instance.hideMenu);
    }

    public void Btn_NewGame()
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

    public void Btn_Continue()
    {
        var save = SaveManager.GetSaveInfo(SaveManager.LastSaveFile());
        SaveManager.Load(SaveManager.LastSaveFile());

        menuAnimator.Play(hideMenu, () =>
        {
            gameScene.GetComponent<CinematicPlayer>().Play(() =>
            {
                LocationData location = GuidDatabase.Find<LocationData>(save.playerLocation);
                Loader.LoadScene(location.scene.BuildIndex, null);
            });
        });
    }

    public void Btn_Load()
    {

    }

    public void Btn_Save()
    {

    }

    public void Btn_Encyclo()
    {

    }

    public void Btn_Settings()
    {

    }

    public void Btn_Quit()
    {

    }
}
