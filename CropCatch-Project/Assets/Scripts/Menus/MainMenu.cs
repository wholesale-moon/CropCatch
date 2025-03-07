using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject SettingsPanel;
    public GameObject HelpPanel;

    [Header("Help")]
    [SerializeField] private GameObject buttonMashPanel;
    [SerializeField] private GameObject qtePanel;
    [SerializeField] private GameObject mode1Selected;
    [SerializeField] private GameObject mode2Selected;

    [SerializeField] private InputManager IM;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Encyclopedia encyclopedia;

    public void Play()
    {
        //reset all scriptable objects that can change
        IM.Reset();
        inventory.Reset();
        encyclopedia.Reset();

        GetComponent<LevelLoader>().FadeToLevel(4);
    }

    public void Settings()
    {
        SettingsPanel.SetActive(true);
    }

    public void Help()
    {
        HelpPanel.SetActive(true);
    }
    public void Mode1()
    {
        buttonMashPanel.SetActive(true);
        mode1Selected.SetActive(true);
        qtePanel.SetActive(false);
        mode2Selected.SetActive(false);
    }

    public void Mode2()
    {
        buttonMashPanel.SetActive(false);
        mode1Selected.SetActive(false);
        qtePanel.SetActive(true);
        mode2Selected.SetActive(true);
    }

    public void CloseHelp()
    {
        HelpPanel.SetActive(false);
    }

    public void Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        
        Application.Quit();
    }

    public void OnButtonClick()
    {
        GetComponent<SoundManager>().PlaySFX("Button Click");
    }

    public void OnHighlight()
    {
        GetComponent<SoundManager>().PlaySFX("Button Highlight");
    }
}
