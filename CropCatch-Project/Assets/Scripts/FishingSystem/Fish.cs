using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish
{
    private string fishName;
    private float price;

    private ScriptableFish.Rarity rarity;
    private bool isRareVersion;

    private float fishingPointsRequired;
    private float startFishingPoints;
    private float catchDrainRate;

    private float qteFrequency;
    private float rngQTE;
    private float qtePass;
    private float qteFail;
    private float qteDrain;
    private float size;

    private int difficulty;

    public GameObject model;
    public string quip;
    public Sprite icon;

    public Fish(ScriptableFish fishData)
    {
        //this sets everything in case there's an error with randomization, or if you just want the base fish
        this.fishName = fishData.fishName;
        this.price = fishData.basePrice;
        this.rarity = fishData.rarity;
        this.isRareVersion = fishData.isRareVersion;
        this.fishingPointsRequired = fishData.maxFishingPoints;
        this.startFishingPoints = fishData.startFishingPoints;
        this.catchDrainRate = fishData.catchDrainRate;
        this.qteFrequency = fishData.qteFrequency;
        this.rngQTE = fishData.rngQTE;

        this.qteDrain = fishData.qteDrain;

        this.size = fishData.baseSize;
        this.difficulty = fishData.baseDifficulty;

        this.model = fishData.modelPrefab;
        this.quip = fishData.pun;
        this.icon = fishData.fishImage;
    }

    public void InitializeStats(ScriptableFish fishData)
    {
        //this sets everything in case there's an error with randomization, or if you just want the base fish
        this.fishName = fishData.fishName;
        this.price = fishData.basePrice;
        this.rarity = fishData.rarity;
        this.isRareVersion = fishData.isRareVersion;
        this.fishingPointsRequired = fishData.maxFishingPoints;
        this.startFishingPoints = fishData.startFishingPoints;
        this.catchDrainRate = fishData.catchDrainRate;
        this.qteFrequency = fishData.qteFrequency;
        this.rngQTE = fishData.rngQTE;
        this.size = fishData.baseSize;
        this.difficulty = fishData.baseDifficulty;
    }

    public void RandomizeStats(ScriptableFish fishData)
    {
        //create random difficulty between min and max
        difficulty = Random.Range(fishData.minDifficulty, fishData.maxDifficulty + 1);
        Debug.Log("RNG fish difficulty: " + difficulty);

        float difficultyChange;
        bool difficultyIncrease;
        if(difficulty > fishData.baseDifficulty)
        {
            difficultyIncrease = true;
            //this should get the % of difficulty change between base and max
            difficultyChange = (float)((difficulty - fishData.baseDifficulty)) / (fishData.maxDifficulty - fishData.baseDifficulty);
            Debug.Log("RNG fish difficulty change = " + difficultyChange);
        }
        else if (difficulty < fishData.baseDifficulty)
        {
            difficultyIncrease = false;
            //this should get the % of difficulty change between base and min (have to subtract from 100 for proper value)
            difficultyChange = 1f - (float)((((difficulty - fishData.minDifficulty)) / (fishData.baseDifficulty - fishData.minDifficulty)));
            Debug.Log("RNG fish difficulty change = " + difficultyChange);
        }
        else
        {
            //difficulty is the same as base
            Debug.Log("RNG fish difficulty same as default");
            return;
        }

        //take new difficulty and augment stats to match
        if(difficultyIncrease)
        {
            float priceChange = fishData.maxPrice - fishData.basePrice;
            price += (priceChange * difficultyChange);

            fishingPointsRequired += (fishData.fishingPointsModifier * difficultyChange);
            startFishingPoints += (fishData.startPointsModifier * difficultyChange);
            catchDrainRate += (fishData.drainRateModifier * difficultyChange);
            qteFrequency += (fishData.qteModifier * difficultyChange);
            size += (fishData.sizeModifier * difficultyChange);
        }
        else
        {
            float priceChange = fishData.basePrice - fishData.minPrice;
            price -= (priceChange * difficultyChange);

            fishingPointsRequired -= (fishData.fishingPointsModifier * difficultyChange);
            startFishingPoints -= (fishData.startPointsModifier * difficultyChange);
            catchDrainRate -= (fishData.drainRateModifier * difficultyChange);
            qteFrequency -= (fishData.qteModifier * difficultyChange);
            size -= (fishData.sizeModifier * difficultyChange);
        }
    }

    public string GetFishName()
    {
        return fishName;
    }

    public float GetFishPrice()
    {
        return price;
    }
    public float GetPointsRequired()
    {
        return fishingPointsRequired;
    }
    public float GetStartingPoints()
    {
        return startFishingPoints;
    }

    public float GetDrainRate()
    {
        return catchDrainRate;
    }
    public float GetQTEDrainRate()
    {
        return qteDrain;
    }

    public float GetQTEFrequency()
    {
        return qteFrequency;
    }

    public float GetQTERNG()
    {
        return rngQTE;
    }

    public Sprite GetIcon()
    {
        return icon;
    }
}
