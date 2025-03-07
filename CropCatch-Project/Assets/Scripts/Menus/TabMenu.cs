using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class TabMenu : MonoBehaviour
{
    [SerializeField] GameSaveData _GameSaveData;
    [SerializeField] Inventory inventory; //to display inventory and money 
    [SerializeField] private InputManager IM;
    public GameObject canvas;
    public CinemachineBrain cmBrain;

    private bool tabMenuActive = false;

    [Header("UI")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject encyclopediaPanel;
    [SerializeField] private TextMeshProUGUI moneyDisplay;
    [SerializeField] private TextMeshProUGUI slotsDisplay;
    [SerializeField] private GameObject[] inventorySlots;

    public event Action closeTabMenu;

    private void Awake()
    {
        canvas.SetActive(true);
    }
    void Start()
    {
        inventorySlots = GameObject.FindGameObjectsWithTag("InventorySlot");
        Debug.Log(inventorySlots.Count());
        //inventorySlots.Reverse();
        foreach (GameObject slot in inventorySlots)
        {
            slot.SetActive(false);
        }
        canvas.SetActive(false);
    }

    private void OnEnable()
    {
        IM.InventoryToggleEvent += ToggleTabMenu;
        IM.UIEscEvent += EscEvent;
    }

    private void OnDisable()
    {
        IM.InventoryToggleEvent -= ToggleTabMenu;
        IM.UIEscEvent -= EscEvent;
    }

    private void ToggleTabMenu()
    {
        if(tabMenuActive)
        {
            ToggleTabMenuOff();
        }
        else
        {
            ToggleTabMenuOn();
        }
    }

    private void EscEvent()
    {
        if (tabMenuActive)
        {
            ToggleTabMenuOff();
        }
    }

    private void ToggleTabMenuOn()
    {
        // Make sure pause not active and not in shop or fishing
        if(IM.currentMap != InputManager.InputMap.Player)
        {
            Debug.Log("Unable to open inventory");
            return; //this should work to make sure you can't open inventory unless in overworld       
        }
        UpdateInventoryDisplay();
        canvas.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        cmBrain.enabled = false;
        IM.SwitchToUI();
        tabMenuActive = true;

        //for testing
        inventory.LogFish();
    }

    private void ToggleTabMenuOff()
    {
        closeTabMenu?.Invoke();
        canvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        cmBrain.enabled = true;
        IM.SwitchToGameplay();
        tabMenuActive = false;
    }

    public void Encyclopedia()
    {
        inventoryPanel.SetActive(false);
        encyclopediaPanel.SetActive(true);
    }

    public void Inventory()
    {
        inventoryPanel.SetActive(true);
        encyclopediaPanel.SetActive(false);
    }

    public void UpdateInventoryDisplay()
    {
        UpdateFishDisplay();
        UpdateMoneyDisplay();
        UpdateSlotDisplay();
    }

    private void UpdateFishDisplay()
    {
        List<Fish> fishList = inventory.GetFishList();
        if(fishList == null)
        {
            Debug.Log("fishlist is null");
            return;
        }
        if(inventorySlots == null || inventorySlots.Count() < 1)
        {
            Debug.Log("no inventory slots to display/ not set");
            return;
        }

        if(fishList.Count() == 0)
        {
            Debug.Log("fishlist has no fish!");
        }


        for (int i = 0; i < fishList.Count(); i++)
        {
            inventorySlots[i].SetActive(true);
        }


        //displays info for every slot taken up
        for (int i = 0; i < fishList.Count(); i++)
        {
            TextMeshProUGUI[] texts = inventorySlots[i].GetComponentsInChildren<TextMeshProUGUI>(true);
            Image[] images = inventorySlots[i].GetComponentsInChildren<Image>(true);

            foreach (TextMeshProUGUI text in texts)
            {
                if(text.gameObject.name == "Name")
                {
                    text.text = fishList[i].GetFishName();
                }
                else if (text.gameObject.name == "Price")
                {
                    text.text = fishList[i].GetFishPrice().ToString("C2");
                }
            }

            foreach (Image image in images)
            {
                if (image.gameObject.name == "Icon")
                {
                    //image.enabled = false;  
                    //image.sprite = fish.getimage();
                    image.sprite = fishList[i].GetIcon();
                }
            }
            inventorySlots[i].SetActive(true);
        }

        //sets the rest to disabled
        for(int i = fishList.Count(); i < inventorySlots.Count(); i++)
        {
            inventorySlots[i].SetActive(false);
        }
    }
    private void UpdateSlotDisplay()
    {
        slotsDisplay.text = inventory.GetFishList().Count() + "/30";
    }

    private void UpdateMoneyDisplay()
    {
        Debug.Log("Money: " + inventory.money);
        moneyDisplay.text = inventory.money.ToString("C2");
    }
}
