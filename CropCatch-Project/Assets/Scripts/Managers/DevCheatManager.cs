using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DevCheatManager : MonoBehaviour
{
    [SerializeField] private InputManager IM;
    [SerializeField] private Inventory inventory;
    [SerializeField] private FishingMinigame minigame;
    [SerializeField] private Shop shop;

    private void OnEnable()
    {
        IM.LoadLevel1Event += LoadLvl1;
        IM.LoadLevel2Event += LoadLvl2;
        IM.LoadLevel3Event += LoadLvl3;
        IM.GainMoneyEvent += GainMoney;
        IM.LoseMoneyEvent += LoseMoney;
        IM.CompleteFishingEvent += AutoFish;
        IM.BuyTicketEvent += BuyTicket;
        IM.SellAllFishEvent += SellAllFish;
    }

    private void OnDisable()
    {
        IM.LoadLevel1Event -= LoadLvl1;
        IM.LoadLevel2Event -= LoadLvl2;
        IM.LoadLevel3Event -= LoadLvl3;
        IM.GainMoneyEvent -= GainMoney;
        IM.LoseMoneyEvent -= LoseMoney;
        IM.CompleteFishingEvent -= AutoFish;
        IM.BuyTicketEvent -= BuyTicket;
        IM.SellAllFishEvent -= SellAllFish;
    }

    private void LoadLvl1()
    {
        SceneManager.LoadScene("Level_1");
    }

    private void LoadLvl2()
    {
        SceneManager.LoadScene("Level_2");
    }

    private void LoadLvl3()
    {
        SceneManager.LoadScene("Level_3");
    }

    private void GainMoney()
    {
        inventory.money += 100f;
    }

    private void LoseMoney()
    {
        inventory.money = Mathf.Clamp(inventory.money - 100, 0, Mathf.Infinity);
    }

    private void AutoFish()
    {
        minigame.AutoFish();
    }

    private void BuyTicket()
    {
        shop.AutoBuyTicket();
    }

    private void SellAllFish()
    {
        shop.SellAll();
    }
}
