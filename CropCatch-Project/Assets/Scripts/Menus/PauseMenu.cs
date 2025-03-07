using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using Cinemachine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameSaveData _GameSaveData;
    [SerializeField] private InputManager IM;
    [SerializeField] private GameObject PauseCanvas;
    [SerializeField] private GameObject SettingsCanvas;
    [SerializeField] private GameObject HelpCanvas;
    public CinemachineBrain cmBrain;
    [SerializeField] private SoundManager SM;

    [Header("Help")]
    [SerializeField] private GameObject buttonMashPanel;
    [SerializeField] private GameObject qtePanel;
    [SerializeField] private GameObject mode1Selected;
    [SerializeField] private GameObject mode2Selected;

    private bool isPauseActive = false;
    private InputManager.InputMap mapBeforePause;
    
    void Awake()
    {
        SM = GetComponent<SoundManager>();
        // PauseCanvas = GameObject.FindWithTag("PauseCanvas");
        // SettingsCanvas = GameObject.FindWithTag("SettingsCanvas");
        // PauseCanvas.SetActive(false);
        // SettingsCanvas.SetActive(false);
        TogglePauseOff();
    }

    private void OnEnable()
    {
        IM.PauseToggleEvent += TogglePause;
        IM.UIEscEvent += UITogglePause;
    }

    private void OnDisable()
    {
        IM.PauseToggleEvent -= TogglePause;
        IM.UIEscEvent -= UITogglePause;
    }

    private void TogglePause()
    {
        if(IM.currentMap != InputManager.InputMap.Player)
        {
            return; //wont open menu unless in basic gameplay
        }

        TogglePauseOn();
    }

    public void PublicTogglePause()
    {
        if (isPauseActive)
        {
            TogglePauseOff();
        }
        else
        {
            TogglePauseOn();
        }
    }

    private void UITogglePause()
    {
        if(isPauseActive)
        {
            TogglePauseOff();
        }
    }

    private void TogglePauseOn()
    {
        //perhaps when this starts we can store which type of gameplay we were in, 
        //that way when we want to unpause we can go back to that specific input map
        Time.timeScale = 0.0f;
        PauseCanvas.SetActive(true);
        _GameSaveData.isGamePaused = true;
        cmBrain.enabled = false;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        mapBeforePause = IM.currentMap;
        if (IM.currentMap != InputManager.InputMap.UI)
        {
            IM.SwitchToUI();
        }  
        isPauseActive = true;
        SM.MusicLowPassOn();
    }

    public void TogglePauseOff()
    {
        Time.timeScale = 1.0f;
        PauseCanvas.SetActive(false);
        SettingsCanvas.SetActive(false);
        HelpCanvas.SetActive(false);
        _GameSaveData.isGamePaused = false;
        if(mapBeforePause != InputManager.InputMap.UI)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            cmBrain.enabled = true;
        }
       
        //this is here since the button can be pressed without the event, and the event is what changes back to gameplay inputs
        //also this will have to be updated later since the pause can occur during shops and fishing, so more checks are needed
        switch(mapBeforePause)
        {
            case InputManager.InputMap.Player:
                IM.SwitchToGameplay();
                break;
            case InputManager.InputMap.UI:
                //IM.SwitchToUI(); //this is unneeded since the pause map is already UI
                break;
            case InputManager.InputMap.Fishing:
                IM.SwitchToFishing();
                break;
        }
        isPauseActive = false;
        SM.MusicLowPassOff();
    }

    public void Settings()
    {
        SettingsCanvas.SetActive(true);
    }

    public void Help()
    {
        HelpCanvas.SetActive(true);
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
        HelpCanvas.SetActive(false);
    }

    public void MainMenu()
    {
        Time.timeScale = 1.0f;
        _GameSaveData.isGamePaused = false;
        GetComponent<LevelLoader>().FadeToLevel(0);
    }
}
