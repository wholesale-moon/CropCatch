using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;

public class SoundManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public GameSaveData _GameSaveData;
    // GameSaveData is a scriptable object that holds all of the important
    // settings and player data. I usually have it hold volume levels
    // so it persists between loads.
    
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup soundEffectsMixerGroup;

    [SerializeField] private float fadeDuration = 1;

    // [Header("UI")]
    // [SerializeField] private Slider masterVolume;
    // [SerializeField] private Slider musicVolume;
    // [SerializeField] private Slider sfxVolume;
    
    [Space(10)]
    [SerializeField] private Sound[] sounds;

    [SerializeField] private List<Sound> musicList = new List<Sound>();
    [Range(0, 1)] [SerializeField] private float overworldTargetVol = 0.5f;
    [Range(0, 1)] [SerializeField] private float fishingTargetVol = 0.5f;
    [Range(0, 1)] [SerializeField] private float shopTargetVol = 0.5f;
    private List<Coroutine> musicCoroutines = new List<Coroutine>();

    private void Awake()
    {
        
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.isLoop;
            s.source.playOnAwake = s.playOnAwake;
            s.source.volume = s.volume;

            switch (s.audioType)
            {
                case Sound.AudioTypes.soundEffect:
                    s.source.outputAudioMixerGroup = soundEffectsMixerGroup;
                    break;
                
                case Sound.AudioTypes.music:
                    s.source.outputAudioMixerGroup = musicMixerGroup;
                    musicList.Add(s);
                    break;
            }

            if (s.playOnAwake)
                s.source.Play();
        }

        //music as well
        foreach (Sound m in musicList)
        {
            m.source = gameObject.AddComponent<AudioSource>();
            m.source.clip = m.clip;
            m.source.loop = m.isLoop;
            m.source.playOnAwake = m.playOnAwake;
            m.source.volume = m.volume;

            switch (m.audioType)
            {
                case Sound.AudioTypes.soundEffect:
                    m.source.outputAudioMixerGroup = soundEffectsMixerGroup;
                    break;

                case Sound.AudioTypes.music:
                    m.source.outputAudioMixerGroup = musicMixerGroup;
                    //musicList.Add(m);
                    break;
            }

            if (m.playOnAwake)
                m.source.Play();
        }
    }

    public void Start()
    {
        // This is where the volume loading/setting come in play
        audioMixer.SetFloat("Master", _GameSaveData._masterVolume);
        audioMixer.SetFloat("Music", _GameSaveData._musicVolume);
        audioMixer.SetFloat("SFX", _GameSaveData._sfxVolume);
        // masterVolume.value = PlayerPrefs.GetFloat("Master");
        // musicVolume.value = PlayerPrefs.GetFloat("Music");
        // sfxVolume.value = PlayerPrefs.GetFloat("SFX");
        SwitchTheme("Overworld", 2f);
    }

    #region Music

    public void SwitchTheme(string songName, float fadeTime)
    {
        Debug.Log("Switching to theme: " + songName);
        
        for(int i = musicCoroutines.Count - 1; i >= 0; i--)
        {
            StopCoroutine(musicCoroutines[i]);
            musicCoroutines.Remove(musicCoroutines[i]);
        }

        foreach (Sound m in musicList)
        {
            if(m.clipName == songName)
            {
                musicCoroutines.Add(StartCoroutine(FadeInMusic(fadeTime, m)));
            }
            else if(m.source.volume > 0)
            {
                musicCoroutines.Add(StartCoroutine(FadeOutMusic(fadeTime, m)));
            }
        }
    }

    private IEnumerator FadeInMusic(float duration, Sound music)
    {
        //music.source.Play();

        float currentTime = 0;
        float targetVol = 0;
        switch(music.clipName)
        {
            case "Overworld":
                targetVol = overworldTargetVol;
                break;
            case "Fishing":
                targetVol = fishingTargetVol;
                break;
            case "Shop":
                targetVol = shopTargetVol;
                break;
        }

        float currentVol = music.source.volume;
        while (currentTime < duration || currentVol >= targetVol)
        {
            float newVol = Mathf.Lerp(currentVol, targetVol, currentTime/duration);
            music.source.volume = newVol;
            yield return null;
            currentTime += Time.deltaTime;
        }
        music.source.volume = targetVol;
        yield break;
    }

    private IEnumerator FadeOutMusic(float duration, Sound music)
    {
        //music.source.Play();

        float currentTime = 0;
        float targetVol = 0;

        float currentVol = music.source.volume;
        while (currentTime < duration || currentVol >= targetVol)
        {
            float newVol = Mathf.Lerp(currentVol, targetVol, currentTime / duration);
            music.source.volume = newVol;
            yield return null;
            currentTime += Time.deltaTime;
        }
        music.source.volume = targetVol;
        yield break;
    }

    public void FadeOutAllMusic(float duration)
    {
        foreach (Sound music in musicList) 
        {
            musicCoroutines.Add(StartCoroutine(FadeOutMusic(duration, music)));
        }
    }

    public void MusicLowPassOn()
    {
        //GetComponent<AudioLowPassFilter>().cutoffFrequency = 1000f;
        //musicMixerGroup.audioMixer.GetComponent<AudioLowPassFilter>().cutoffFrequency = 1000f;
    }

    public void MusicLowPassOff()
    {
        //GetComponent<AudioLowPassFilter>().cutoffFrequency = 20000f;
        //musicMixerGroup.audioMixer.GetComponent<AudioLowPassFilter>().cutoffFrequency = 20000f;
    }

    public void PlaySong(string _clipName) // use when nothing is playing
    {
        // Finds sound clip in array with matching name
        Sound soundToPlay = Array.Find(sounds, dummySound => dummySound.clipName == _clipName);

        if (soundToPlay != null)
        {
            StartCoroutine(FadeInMusic(fadeDuration, soundToPlay));
        }
    }

    public void PlayNewSong(string _clipName) // use when a clip is playing before this one
    {
        // Finds sound clip in array with matching name
        Sound soundToPlay = Array.Find(sounds, dummySound => dummySound.clipName == _clipName);

        if (soundToPlay != null)
        {
            StartCoroutine(FadeOutMusic(fadeDuration, soundToPlay));
        }
    }

    // untested
    public void PauseSong(string _clipName)
    {
        // Finds sound clip in array with matching name
        Sound soundToPlay = Array.Find(sounds, dummySound => dummySound.clipName == _clipName);

        if (soundToPlay != null)
        {
            if (soundToPlay.source.isPlaying)
                soundToPlay.source.Pause();
        }
    }

    // untested
    public void ResumeSong(string _clipName)
    {
        // Finds sound clip in array with matching name
        Sound soundToPlay = Array.Find(sounds, dummySound => dummySound.clipName == _clipName);

        if (soundToPlay != null)
        {
            if (soundToPlay.source.isPlaying)
                soundToPlay.source.UnPause();
        }
    }

    //public IEnumerator FadeInMusic(float duration, Sound nextSong)
    //{
    //    nextSong.source.Play();

    //    float currentTime = 0;
    //    float currentVol;

    //    audioMixer.GetFloat("MusicFade", out currentVol);
    //    currentVol = Mathf.Pow(10, currentVol / 20);
    //    float targetValue = Mathf.Clamp(1, 0.0001f, 1);
    //    while (currentTime < duration)
    //    {
    //        currentTime += Time.deltaTime;
    //        float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
    //        audioMixer.SetFloat("MusicFade", Mathf.Log10(newVol) * 20);
    //        yield return null;
    //    }
    //    yield break;
    //}
    
    //public IEnumerator FadeOutMusic(float duration, Sound nextSong)
    //{
    //    float currentTime = 0;
    //    float currentVol;

    //    audioMixer.GetFloat("MusicFade", out currentVol);
    //    currentVol = Mathf.Pow(10, currentVol / 20);
    //    float targetValue = Mathf.Clamp(0.001f, 0.0001f, 1);
    //    while (currentTime < duration)
    //    {
    //        currentTime += Time.deltaTime;
    //        float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
    //        audioMixer.SetFloat("MusicFade", Mathf.Log10(newVol) * 20);
    //        yield return null;
    //    }

    //    StopAllMusic();
    //    yield return new WaitForSeconds(0.5f);
    //    StartCoroutine(FadeInMusic(duration, nextSong));
    //    yield break;
    //}

    public IEnumerator FadeSongOut(float duration)
    {
        float currentTime = 0;
        float currentVol;

        audioMixer.GetFloat("MusicFade", out currentVol);
        currentVol = Mathf.Pow(10, currentVol / 20);
        float targetValue = Mathf.Clamp(0.001f, 0.0001f, 1);
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
            audioMixer.SetFloat("MusicFade", Mathf.Log10(newVol) * 20);
            yield return null;
        }

        StopAllMusic();
        yield break;
    }

    public void StopAllMusic()
    {
        foreach (Sound m in musicList)
        {
            if (m.source.isPlaying)
                m.source.Stop();
        }
    }
    #endregion

    #region SFX
    public void PlaySFX(string _clipName)
    {
        // Finds sound clip in array with matching name
        Sound soundToPlay = Array.Find(sounds, dummySound => dummySound.clipName == _clipName);

        if (soundToPlay != null)
        {
            soundToPlay.source.Play();
        }
    }

    public void StopSFX(string _clipName)
    {
        // Finds sound clip in array with matching name
        Sound soundToStop = Array.Find(sounds, dummySound => dummySound.clipName == _clipName);
        
        if (soundToStop != null)
            soundToStop.source.Stop();
    }

    public void ButtonHoverOn()
    {
        PlaySFX("ButtonHover");
    }

    public void ButtonClick()
    {
        PlaySFX("ButtonClick");
    }
    #endregion
}
