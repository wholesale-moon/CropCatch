using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicMusic : MonoBehaviour
{
    [SerializeField] private Sound overworldTheme;
    [SerializeField] private float overworldTargetVol = 0.5f;
    [SerializeField] private Sound fishingTheme;
    [SerializeField] private float fishingTargetVol = 0.5f;
    [SerializeField] private Sound shopTheme;
    [SerializeField] private float shopTargetVol = 0.5f;

    private Sound currentSound;

    private void Awake()
    {
        //initialize each song to start and 0 volume
        if(overworldTheme == null)
        {
            Debug.Log("overworld theme is null!");
        }
        if (fishingTheme == null)
        {
            Debug.Log("fishing theme is null!");
        }
        if (shopTheme == null)
        {
            Debug.Log("shop theme is null!");
        }
    }

    public void SwitchTheme(string name, float fadeTime = 1f)
    {
        switch(name)
        {
            case "Overworld":
                currentSound = overworldTheme;
                break;
            case "Fishing":
                currentSound = fishingTheme;
                break;
            case "Shop":
                currentSound = shopTheme;
                break;
        }
    }

    private IEnumerator FadeMusic(string name, float fadeTime)
    {
        float currentTime = 0f;
        while(currentTime < fadeTime) // add "or volume reaches target"
        {
            //getting selected theme to target vol
            if(false)
            {

            }
            //lowering other 2 themes to 0
            if (false)
            {

            }
        }
        yield return null;
    }
}
