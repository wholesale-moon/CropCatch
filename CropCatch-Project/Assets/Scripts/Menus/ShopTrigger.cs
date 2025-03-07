using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
    [SerializeField] GameObject shopCanvas;
    [SerializeField] Shop shopRef;
    [SerializeField] private InputManager IM;
    [SerializeField] private GameObject interactText;
    public ThirdPersonCam cameraMove;
    [SerializeField] private SoundManager SM;

    private bool playerInTrigger = false;
    public event Action EnteringShop;

    private void Start()
    {
        //shopRef.CheckTriggerText += CheckTextVisibility;
    }

    private void OnEnable()
    {
        IM.PlayerInteractEvent += EnableShop;
        shopRef.CheckTriggerText += CheckTextVisibility;
    }

    private void OnDisable()
    {
        IM.PlayerInteractEvent -= EnableShop;
        shopRef.CheckTriggerText -= CheckTextVisibility;
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

    private void EnableShop()
    {
        if(playerInTrigger)
        {
            interactText.SetActive(false);
            EnteringShop?.Invoke();
            shopCanvas.SetActive(true);
            GameObject CamStateMachine = GameObject.FindWithTag("CamStateMachine");
            CamStateMachine.GetComponent<CinemachineSwitcher>().SwitchState("Shop");
            GameObject cam = GameObject.FindWithTag("MainCamera");
            cam.GetComponent<ThirdPersonCam>().DisableLookMove();
            cameraMove.enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            IM.SwitchToUI();
            SM.SwitchTheme("Shop", 0.5f);
        }
    }

    public void CheckTextVisibility()
    {
        if(playerInTrigger)
        {
            interactText.SetActive(true);
        }
    }
}
