using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public AudioMixer audioMixer;
    public GameSaveData _GameSaveData;

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject xMark;

    [SerializeField] private Slider mouseSensitivity;
    [SerializeField] private Slider masterVolume;
    [SerializeField] private Slider musicVolume;
    [SerializeField] private Slider sfxVolume;
    
    public void SetMouseSensitivity(float mouseVal)
    {
        PlayerPrefs.SetFloat("Mouse Sensitivity", mouseSensitivity.value);
        _GameSaveData._mouseSensitivity = mouseVal;

        GameObject cam = GameObject.FindWithTag("MainCamera");
        if(cam != null )
        {
            cam.GetComponent<ThirdPersonCam>().UpdateMouseSensitivity();
        }
    }
    
    public void SetMasterVolume(float volume)
    {
        PlayerPrefs.SetFloat("Master", masterVolume.value);
        audioMixer.SetFloat("Master", volume);
        _GameSaveData._masterVolume = volume;
    }

    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("Music", musicVolume.value);
        audioMixer.SetFloat("Music", volume);
        _GameSaveData._musicVolume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFX", sfxVolume.value);
        audioMixer.SetFloat("SFX", volume);
        _GameSaveData._sfxVolume = volume;
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
        xMark.SetActive(!isFullScreen);
        GetComponent<SoundManager>().PlaySFX("Button Click");
    }

    public void ClosePanel()
    {
        settingsPanel.SetActive(false);
    }
}
