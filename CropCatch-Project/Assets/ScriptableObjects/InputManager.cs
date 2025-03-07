using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "GameSaveData", menuName = "ScriptableObjects/InputManager")]
public class InputManager : ScriptableObject, GameInputs.IPlayerActions, GameInputs.IFishingActions, GameInputs.IUIActions, GameInputs.ICheatcodesActions
{
    private GameInputs gameInputs;
    [SerializeField] private bool devCheatsEnabled = true;

    private void OnEnable()
    {
        if (gameInputs == null)
        {
            gameInputs = new GameInputs();

            gameInputs.Player.SetCallbacks(this);
            gameInputs.UI.SetCallbacks(this);
            gameInputs.Fishing.SetCallbacks(this);
            gameInputs.Cheatcodes.SetCallbacks(this);

            SwitchToGameplay();          
        }
        Debug.Log("InputManager Instantialized");
    }

    public void Reset()
    {
        OnEnable();
    }

    public enum InputMap
    {
        Player,
        UI,
        Fishing,
        Unset
    }
    public InputMap currentMap = InputMap.Unset;

    public event Action<Vector2> MovementEvent;
    public event Action PlayerInteractEvent;
    public event Action UIInteractEvent;
    public event Action FishingInteractEvent;
    public event Action PauseToggleEvent;
    public event Action InventoryToggleEvent;
    public event Action UIEscEvent;

    public event Action FishingRightClickEvent;
    public event Action FishingWEvent;
    public event Action FishingAEvent;
    public event Action FishingSEvent;
    public event Action FishingDEvent;
    public event Action FishingEscEvent;

    //dev cheats
    public event Action LoadLevel1Event;
    public event Action LoadLevel2Event;
    public event Action LoadLevel3Event;
    public event Action GainMoneyEvent;
    public event Action LoseMoneyEvent;
    public event Action CompleteFishingEvent;
    public event Action BuyTicketEvent;
    public event Action SellAllFishEvent;


    public void SwitchToGameplay()
    {
        DisableMaps();
        gameInputs.Player.Enable();
        currentMap = InputMap.Player;
        Debug.Log("Switching to Gameplay Inputs");
    }

    public void SwitchToUI()
    {
        DisableMaps();
        gameInputs.UI.Enable();
        currentMap = InputMap.UI;
        Debug.Log("Switching to UI Inputs");
    }

    public void SwitchToFishing()
    {
        DisableMaps();
        gameInputs.Fishing.Enable();
        currentMap = InputMap.Fishing;
        Debug.Log("Switching to Fishing Inputs");
    }

    private void DisableMaps()
    {
        gameInputs.Player.Disable();
        gameInputs.UI.Disable();
        gameInputs.Fishing.Disable();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        MovementEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            switch(currentMap)
            {
                case InputMap.Player:
                    PlayerInteractEvent?.Invoke();
                    break;
                case InputMap.UI:
                    UIInteractEvent?.Invoke();
                    break;
                case InputMap.Fishing:
                    FishingInteractEvent?.Invoke();
                    break;
            }
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        //demo for events, doesnt actually work with pausing since there's multiple UI layers lol
        if(context.phase == InputActionPhase.Started)
        {
            switch (currentMap)
            {
                case InputMap.Player:
                    PauseToggleEvent?.Invoke();
                    break;
                case InputMap.UI:
                    UIEscEvent?.Invoke();
                    break;
                case InputMap.Fishing:
                    FishingEscEvent?.Invoke();
                    break;
            }
        }
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            InventoryToggleEvent?.Invoke();
        }
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (currentMap == InputMap.Fishing && context.phase == InputActionPhase.Started)
        {
            FishingRightClickEvent?.Invoke();
        }
    }

    public void OnW(InputAction.CallbackContext context)
    {
        if(currentMap == InputMap.Fishing && context.phase == InputActionPhase.Started)
        {
            FishingWEvent?.Invoke();
        }
    }

    public void OnA(InputAction.CallbackContext context)
    {
        if (currentMap == InputMap.Fishing && context.phase == InputActionPhase.Started)
        {
            FishingAEvent?.Invoke();
        }
    }

    public void OnS(InputAction.CallbackContext context)
    {
        if (currentMap == InputMap.Fishing && context.phase == InputActionPhase.Started)
        {
            FishingSEvent?.Invoke();
        }
    }

    public void OnD(InputAction.CallbackContext context)
    {
        if (currentMap == InputMap.Fishing && context.phase == InputActionPhase.Started)
        {
            FishingDEvent?.Invoke();
        }
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        
    }

    public void OnPrevious(InputAction.CallbackContext context)
    {
        
    }

    public void OnNext(InputAction.CallbackContext context)
    {
        
    }


    //dev cheats
    public void OnLoadLevel1(InputAction.CallbackContext context)
    {
        Debug.Log("LoadLevel1 detected!");
        if(context.phase == InputActionPhase.Started && SceneManager.GetActiveScene().name != "Level_1")
        {
            Debug.Log("LoadLevel1 started!");
            LoadLevel1Event?.Invoke();
        }
    }

    public void OnLoadLevel2(InputAction.CallbackContext context)
    {
        Debug.Log("LoadLevel2 detected!");
        if (context.phase == InputActionPhase.Started && SceneManager.GetActiveScene().name != "Level_2")
        {
            LoadLevel2Event?.Invoke();
        }
    }

    public void OnLoadLevel3(InputAction.CallbackContext context)
    {
        Debug.Log("LoadLevel3 detected!");
        if (context.phase == InputActionPhase.Started && SceneManager.GetActiveScene().name != "Level_3")
        {
            LoadLevel3Event?.Invoke();
        }
    }

    public void OnGainMoney(InputAction.CallbackContext context)
    {
        Debug.Log("GainMoney detected!");
        if (context.phase == InputActionPhase.Started)
        {
            GainMoneyEvent?.Invoke();
        }
    }

    public void OnLoseMoney(InputAction.CallbackContext context)
    {
        Debug.Log("LoseMoney detected!");
        if (context.phase == InputActionPhase.Started)
        {
            LoseMoneyEvent?.Invoke();
        }
    }

    public void OnCompleteFishing(InputAction.CallbackContext context)
    {
        Debug.Log("complete fishing detected!");
        if (context.phase == InputActionPhase.Started)
        {
            CompleteFishingEvent?.Invoke();
        }
    }

    public void OnBuyTicket(InputAction.CallbackContext context)
    {
        Debug.Log("BuyTicket detected!");
        if (context.phase == InputActionPhase.Started)
        {
            BuyTicketEvent?.Invoke();
        }
    }

    public void OnSellAllFish(InputAction.CallbackContext context)
    {
        Debug.Log("Sellallfish detected!");
        if (context.phase == InputActionPhase.Started)
        {
            SellAllFishEvent?.Invoke();
        }
    }
}
