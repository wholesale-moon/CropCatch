using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FishingTrigger : MonoBehaviour
{
    [SerializeField] private InputManager IM;
    [SerializeField] private SoundManager SM;
    [SerializeField] private Inventory inventory;
    bool playerInTrigger;
    [SerializeField] private FishingMinigame minigame;
    [SerializeField] private List<ScriptableFish> availablefish;

    [SerializeField] private GameObject interactText;

    private GameObject CamStateMachine;
    [SerializeField] private GameObject ButtonMashCanvas;

    [Header("Camera Switching")]
    [SerializeField] private string CameraName;

    private void Awake()
    {
        CamStateMachine = GameObject.FindWithTag("CamStateMachine");
    }

    private void OnEnable()
    {
        IM.PlayerInteractEvent += StartFishing;
        minigame.CheckTriggerText += CheckTextVisibility;
    }

    private void OnDisable()
    {
        IM.PlayerInteractEvent -= StartFishing;
        minigame.CheckTriggerText -= CheckTextVisibility;
    }

    private void Start()
    {
        //minigame.CheckTriggerText += CheckTextVisibility;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInTrigger = true;
            interactText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            interactText.SetActive(false);
        }
    }

    private void StartFishing()
    {
        if(playerInTrigger)
        {
            if(availablefish == null || availablefish.Count < 1)
            {
                Debug.Log("Available fish in trigger not set up properly!");
                return;
            }
            if(!inventory.CanCatchFish())
            {
                Debug.Log("Inventory Full! Can't catch more fish");
                return;
            }
            //switch to fishing controls
            IM.SwitchToFishing();

            //move camera over
            CamStateMachine.GetComponent<CinemachineSwitcher>().SwitchState(CameraName);

            //do fishing minigame
            Debug.Log("Player is trying to start fishing");
            interactText.SetActive(false);
            ButtonMashCanvas.SetActive(true);
            minigame.SelectFish(availablefish);
            minigame.EnterFishing();
            SM.FadeOutAllMusic(2f);
            
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<PlayerMovement>().PlayAnimation("Casting");
        }
    }

    public void CheckTextVisibility()
    {
        if (playerInTrigger)
        {
            interactText.SetActive(true);
        }
    }
}
