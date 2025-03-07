using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;
using UnityEngine;
using System;

public class Shop : MonoBehaviour
{
    [SerializeField] private GameObject shopCanvas;
    [SerializeField] private InputManager IM;
    [SerializeField] private Inventory inventory; //to display inventory and money 
    [SerializeField] private GameObject[] inventorySlots;
    [SerializeField] private ShopTrigger trigger;

    [Space(10)]
    [SerializeField] private CinemachineBrain cmBrain;
    [SerializeField] private ThirdPersonCam cameraMove;
    [SerializeField] private SoundManager SM;

    [Header("Shop Data")]
    [SerializeField] private float ticketPrice = 500f;
    [SerializeField] private bool boughtThisLevelTicket = false;
    [SerializeField] private float rodUpgradePrice = 170f;
    [SerializeField] private bool boughtThisLevelRod = false;
    [SerializeField] private string upgradedFishingTitle;

    [Space(10)]
    public GameObject ticketPriceText;
    public GameObject rodPriceText;

    [Space(10)]
    [SerializeField] private TMP_Text moneyDisplay;

    [Header("Dialogue")]
    public TMP_Text dialogueText;
    
    [TextArea(3, 10)]
    [SerializeField] private string[] talkDialogue;

    [TextArea(3, 10)]
    [SerializeField] private string[] sellDialogue;

    public event Action CheckTriggerText;

    private void Awake()
    {
        shopCanvas.SetActive(true);
        SM = GetComponent<SoundManager>();
    }
    void Start()
    {
        inventorySlots = GameObject.FindGameObjectsWithTag("ShopInventorySlot");
        Debug.Log(inventorySlots.Count());
        foreach (GameObject slot in inventorySlots)
        {
            slot.SetActive(false);
        }
        shopCanvas.SetActive(false);

        //trigger.EnteringShop += EnteringShop;
    }

    private void OnEnable()
    {
        trigger.EnteringShop += EnteringShop;
        IM.UIEscEvent += ExitShopButton;
    }

    private void OnDisable()
    {
        trigger.EnteringShop -= EnteringShop;
        IM.UIEscEvent -= ExitShopButton;
    }

    private void EnteringShop()
    {
        UpdateShopInfo();
        dialogueText.text = "Welcome to the Shack!";
    }

    public void ExitShopButton()
    {
        shopCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        cmBrain.enabled = true;
        cameraMove.enabled = true;
        IM.SwitchToGameplay();
        GameObject CamStateMachine = GameObject.FindWithTag("CamStateMachine");
        CamStateMachine.GetComponent<CinemachineSwitcher>().SwitchState("Overworld");
        GameObject cam = GameObject.FindWithTag("MainCamera");
        cam.GetComponent<ThirdPersonCam>().EnableLookMove();
        CheckTriggerText?.Invoke();
        SM.SwitchTheme("Overworld", 0.5f);
    }

    public void UpdateShopInfo()
    {
        //probably split this into multiple functions, 
        //use for updating display whenever selling fish or buying upgrades
        UpdateShownInventory();
        UpdateMoneyDisplay();
        UpdatePricesDisplay();
    }

    public void Talk()
    {
        dialogueText.text = talkDialogue[UnityEngine.Random.Range(0, 3)];
    }

    public void SellAll()
    {
        if(inventory.GetFishList() == null || inventory.GetFishList().Count() < 1)
        {
            dialogueText.text = "You don't have any fish to sell.";
        }
        else
        {
            SM.PlaySFX("Sell");
        }

        List<Fish> fishList = inventory.GetFishList();
        for (int i = fishList.Count() - 1; i >=0; i--)
        {
            inventory.SellFish(fishList[i]);
            dialogueText.text = sellDialogue[UnityEngine.Random.Range(0, 3)]; // this is so not supposed to go here but it is for now
        }
        Debug.Log("Fish list count after sell all: " + inventory.GetFishList().Count());
        UpdateShopInfo();
    }

    public void BuyTicket()
    {
        if(inventory.money >= ticketPrice)
        {
            if(!boughtThisLevelTicket)
            {
                SM.PlaySFX("Buy");
                inventory.money -= ticketPrice;
                inventory.fishingLevel++;
                boughtThisLevelTicket = true;
                ticketPriceText.GetComponent<TextMeshProUGUI>().text = "BOUGHT!";
                dialogueText.text = "Congrats! You are now a " + upgradedFishingTitle + " Fisher!";

                if (SceneManager.GetActiveScene().buildIndex == 3)
                {
                    GameObject GameManager = GameObject.FindWithTag("GameManager");
                    GameManager.GetComponent<LevelLoader>().FadeToLevel(5);
                }
            }
            else
            {
                dialogueText.text = "Already bought this ticket!";
            }
            
        }
        else
        {
            dialogueText.text = "Not enough money for ticket!";
        }
        UpdateShopInfo();
    }

    public void BuyRodUpgrade()
    {
        if (inventory.money >= rodUpgradePrice)
        {
            if (!boughtThisLevelRod)
            {
                SM.PlaySFX("Buy");
                inventory.money -= rodUpgradePrice;
                inventory.IncreaseFishingRod();
                boughtThisLevelRod = true;
                rodPriceText.GetComponent<TextMeshProUGUI>().text = "BOUGHT!";
                dialogueText.text = "Make good use of that new rod.";
            }
            else
            {
                dialogueText.text = "Already bought this rod upgrade!";
            }

        }
        else
        {
            dialogueText.text = "Not enough money for rod upgrade!";
        }
        UpdateShopInfo();
    }

    public void AutoBuyTicket()
    {
        if (!boughtThisLevelTicket)
        {
            SM.PlaySFX("Buy");
            inventory.fishingLevel++;
            boughtThisLevelTicket = true;
            Debug.Log("auto bought ticket");
        }
        else
        {
            Debug.Log("already bought the ticket!");
        }
        UpdateShopInfo();
    }

    private void UpdateShownInventory()
    {
        List<Fish> fishList = inventory.GetFishList();
        if (fishList == null)
        {
            Debug.Log("fishlist was null");
            return;
        }

        if(fishList.Count() == 0)
        {
            Debug.Log("disabling text for all shop slots!");
            for (int i = 0; i < inventorySlots.Count(); i++)
            {
                Debug.Log("disabling text for shop slot: " + i);
                inventorySlots[i].SetActive(false);
            }
            return;
        }

        //displays info for every slot taken up
        for (int i = 0; i < fishList.Count(); i++)
        {
            Debug.Log("updating text for shop slot: " + i);
            inventorySlots[i].GetComponent<TextMeshProUGUI>().text = fishList[i].GetFishName() + "<br>" + fishList[i].GetFishPrice().ToString("C2");
            inventorySlots[i].SetActive(true);
        }

        //sets the rest to disabled
        for (int i = fishList.Count(); i < inventorySlots.Count(); i++)
        {
            Debug.Log("disabling text for shop slot: " + i);
            inventorySlots[i].SetActive(false);
        }
    }

    private void UpdateMoneyDisplay()
    {
        Debug.Log("Money: " + inventory.money);
        moneyDisplay.text = inventory.money.ToString("C2");
    }

    private void UpdatePricesDisplay()
    {
        if(!boughtThisLevelRod)
        {
            rodPriceText.GetComponent<TextMeshProUGUI>().text = rodUpgradePrice.ToString("C2");
        }
        if(!boughtThisLevelTicket)
        {
            ticketPriceText.GetComponent<TextMeshProUGUI>().text = ticketPrice.ToString("C2");
        }
    }
}
