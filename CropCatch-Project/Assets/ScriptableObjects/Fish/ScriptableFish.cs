using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "GameSaveData", menuName = "ScriptableObjects/Fish")]
public class ScriptableFish : ScriptableObject
{
    public string fishName = "Base Fish";
    public float basePrice = 10f;
    public float minPrice = 5f;
    public float maxPrice = 20f;
  
    [System.Serializable]
    public enum Rarity
    {
        common,
        uncommon,
        rare
    }
    public Rarity rarity = Rarity.common;
    public bool isRareVersion = false;

    public float maxFishingPoints = 100f;  //how many "points" needed to fill the fishing bar
    public float fishingPointsModifier = 50f; //changes required "points" to catch the fish 
    public float startFishingPoints = 30f; //in case you want the fishing bar to start at different values
    public float startPointsModifier = 20f; //modifies starting points based on difficulty
    public float catchDrainRate = 10f; //how many points you want drained each second, multiply by time.deltatime for each frame value
    public float drainRateModifier = 5f; //modifies drain rate based on difficulty

    public float qteFrequency = 10f; //how often the fish likes to initiate the QTE event
    public float rngQTE = 2f; //this is if you want to randomize the timing of each QTE event
    public float qteModifier = 3f; //how much the QTE frequency is altered based on difficulty

    //these are on scale of 0-100
    public float qtePass = 20;
    public float qteFail = 6;
    public float qteDrain = 35;

    public float baseSize = 10f;
    public float sizeModifier = 5f; //how much the size is modified based on difficulty (i.e. max difficulty -> size = 15 and min difficulty -> size = 5)

    public int baseDifficulty = 5; //this helps determine price and could be shown in encyclopedia

    //min and max difficulty are used for the range of how the stats are changed when rng difficulty is decided 
    public int minDifficulty = 1;  
    public int maxDifficulty = 10;


    public GameObject modelPrefab;
    public Sprite fishImage;
    public string pun = "Test pun.";
    public string location = "Test Location";
}
