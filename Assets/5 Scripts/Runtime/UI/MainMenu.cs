using Udar.SceneManager;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private static MainMenu instance;
    public SceneField introScene;

    public Button continueButton;
    public Button saveButton;
    public Button loadButton;

    public UIAnimator menuAnimator;
    public AnimationClip showMenuSlow;
    public AnimationClip showMenuQuick;
    public AnimationClip hideMenu;
    public SaveMenu saveMenu;

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
        continueButton.interactable = SaveManager.CurrentSaveGame != null || !string.IsNullOrEmpty(SaveManager.LastSaveFile());
        saveButton.interactable = SaveManager.CurrentSaveGame != null;
        loadButton.interactable = SaveManager.GetSaveFiles().Length > 0;

        Loader.LoadScene(introScene.BuildIndex, (gs) => gameScene = gs);
        menuAnimator.Play(showMenuSlow);
        PauseManager.Pause(this);
    }

    public static void Show()
    {
        Refresh();
        instance.menuAnimator.Play(instance.showMenuQuick);
        PauseManager.Pause(instance);
    }

    public static void Hide()
    {
        PauseManager.Unpause(instance);
        Refresh();

        instance.menuAnimator.Play(instance.hideMenu);
    }

    public static void Refresh()
    {
        instance.continueButton.interactable = SaveManager.CurrentSaveGame != null || !string.IsNullOrEmpty(SaveManager.LastSaveFile());
        instance.saveButton.interactable = SaveManager.CurrentSaveGame != null;
        instance.loadButton.interactable = SaveManager.GetSaveFiles().Length > 0;
    }

    public void Btn_NewGame()
    {
        //TODO clear cutscenes and other persistent data

        SaveManager.NewGame("");
        PauseManager.Unpause(instance);
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
        //Already in game
        if(SaveManager.CurrentSaveGame != null)
        {
            Hide();
            return;
        }

        //TODO clear cutscenes and other persistent data

        var save = SaveManager.GetSaveInfo(SaveManager.LastSaveFile());
        SaveManager.Load(SaveManager.GetSaveInfo(SaveManager.LastSaveFile()));
        PauseManager.Unpause(instance);

        menuAnimator.Play(hideMenu, () =>
        {
            gameScene.GetComponent<CinematicPlayer>().Play(() =>
            {
                LocationData location = GuidDatabase.Find<LocationData>(save.location.scene);
                Loader.LoadScene(location.scene.BuildIndex, null);
            });
        });
    }

    public void Btn_Load()
    {
        saveMenu.Show(false);
    }

    public void Btn_Save()
    {
        saveMenu.Show(true);
    }

    public void Btn_Encyclo()
    {
        Encyclopedia.Show(1);
    }

    public void Btn_Settings()
    {

    }

    public void Btn_Quit()
    {

    }
}
