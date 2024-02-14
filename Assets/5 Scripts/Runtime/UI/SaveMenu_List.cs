using log4net.DateFormatter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveMenu_List : Page
{
    public TMPro.TextMeshProUGUI header;
    public RectTransform container;
    public SaveDisplay saveButtonPrefab;

    [Header("Name Entry")]
    public GameObject nameEntry;
    public TMPro.TMP_InputField inputField;

    private SaveMenu _saveMenu;

    public void Setup(SaveMenu saveMenu, bool isSaving)
    {
        _saveMenu = saveMenu;
        nameEntry.SetActive(false);

        // Remove old buttons
        foreach (Transform child in container)
            Destroy(child.gameObject);

        // Set header
        header.text = isSaving ? "Save Game" : "Load Game";

        // Add empty save
        if(isSaving)
        {
            SaveDisplay saveButton = Instantiate(saveButtonPrefab, container);
            saveButton.Setup(null);
            saveButton.onClick = () =>
            {
                ShowSavePopup(string.Empty);                
            };
            saveButton.onHover = () =>
            {
                _saveMenu.dataPage.Setup(null);
            };
        }

        // Show list of saves
        foreach(string save in SaveManager.GetSaveFiles())
        {
            string saveName = save;
            SaveDisplay saveButton = Instantiate(saveButtonPrefab, container);
            saveButton.Setup(SaveManager.GetSaveInfo(save));
            saveButton.onClick = () =>
            {
                if (isSaving) ShowSavePopup(save);
                else DoLoad(save);
            };
            saveButton.onHover = () =>
            {
                _saveMenu.dataPage.Setup(SaveManager.GetSaveInfo(save));
            };
        }
    }

    public void ShowSavePopup(string fileName)
    {
        if(string.IsNullOrEmpty(fileName))
        {
            nameEntry.SetActive(true);
        } else
        {
            ConfirmationPopup.Show($"Are you sure you want to save over {fileName}", "Save", () =>
            {
                DoSave(fileName);
            });            
        }
    }

    public void ClickSaveNew()
    {
        DoSave(inputField.text);
    }

    public void DoSave(string filename)
    {
        nameEntry.SetActive(false);

        SaveManager.Save(filename);
        _saveMenu.Hide();
    }

    public void DoLoad(string saveName)
    {
        if(SaveManager.CurrentSaveGame != null)
        {
            ConfirmationPopup.Show($"Warning, unsaved progress will be lost", "Load", () =>
            {
                _saveMenu.Hide();
                SaveManager.Load(SaveManager.GetSaveInfo(saveName));
            });            
            return;
        }

        _saveMenu.Hide();
        SaveManager.Load(SaveManager.GetSaveInfo(saveName));
    }
}
