using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSaveData", menuName = "ScriptableObjects/Inventory")]
public class Inventory : ScriptableObject
{
    public int fishingLevel = 1; // keeping track of license level, 1,2,3
    public float money = 0f; //keeping track of money whenever selling fish
    [SerializeField] private int fishingRodLevel = 1; //goes from 1-4, add 1 each upgrade
    [SerializeField] private float mashingWeight = 3f;
    [SerializeField] private float qteWeight = 24f;
    private List<Fish> fishList = new List<Fish>();
    private int inventorySlots = 30;

    public void Reset()
    {
        fishingLevel = 1;
        money = 0f;
        fishingRodLevel = 1;
        mashingWeight = 3f;
        qteWeight = 24f;
        fishList = new List<Fish>();
        inventorySlots = 30;
    }



    public void AddFish(Fish newFish)
    {
        if(fishList.Count < inventorySlots) //this should never happen but is a failsafe just in case
        {
            fishList.Add(newFish);
            Debug.Log("Adding fish: " + newFish.GetFishName() + " worth: $" + newFish.GetFishPrice());
        }   
    }

    public void RemoveFish(Fish fish)
    {
        fishList.Remove(fish);
    }

    public void SellFish(Fish fish)
    {
        money += fish.GetFishPrice();
        RemoveFish(fish);
    }

    public bool CanCatchFish()
    {
        return fishList.Count < inventorySlots;
    }

    public void LogFish()
    {
        if (fishList == null || fishList.Count < 1) { Debug.Log("Inventory is empty"); return; }

        for(int i = 0; i < fishList.Count; i++)
        {
            Debug.Log("Fish in position " + i + " is a " + fishList[i].GetFishName() + " worth $" + fishList[i].GetFishPrice());
        }
    }    

    public List<Fish> GetFishList()
    {
        return fishList;
    }

    public void IncreaseFishingRod()
    {
        fishingRodLevel++;

        UpdateWeights();
    }

    public void UpdateWeights()
    {
        switch (fishingRodLevel)
        {
            case 1: //defaults
                mashingWeight = 3;
                qteWeight = 24;
                break;
            case 2:
                mashingWeight = 5;
                qteWeight = 40;
                break;
            case 3:
                mashingWeight = 7;
                qteWeight = 64;
                break;
            case 4:
                mashingWeight = 10;
                qteWeight = 96;
                break;
        }
    }

    public float GetMashingPower()
    {
        return mashingWeight;
    }

    public float GetQTEPower()
    {
        return qteWeight;
    }
}
