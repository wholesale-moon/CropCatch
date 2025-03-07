using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Map : MonoBehaviour
{
    [SerializeField] GameObject mapCanvas;
    [SerializeField] private InputManager IM;
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject interactText;
    public CinemachineBrain cmBrain;

    private bool playerInTrigger = false;
    //public event Action OpeningMap;

    [Header("UI")]
    [SerializeField] private TMP_Text fishingTitle;
    [SerializeField] private GameObject fishLevel2;
    [SerializeField] private GameObject fishLevel3;

    [Space(10)]
    [SerializeField] private GameObject SwampButton;
    [SerializeField] private GameObject SpaceButton;

    private void OnEnable()
    {
        IM.PlayerInteractEvent += OpenMap;
        IM.UIEscEvent += EscEvent;
    }

    private void OnDisable()
    {
        IM.PlayerInteractEvent -= OpenMap;
        IM.UIEscEvent -= EscEvent;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
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

    public void OpenMap()
    {
        if (playerInTrigger)
        {
            mapCanvas.SetActive(true);
            IM.SwitchToUI();
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            cmBrain.enabled = false;
            CheckFishingLevel();
        } 
    }

    private void EscEvent()
    {
        if (playerInTrigger)
        {
            CloseMap();
        }
    }

    public void CloseMap()
    {
        mapCanvas.SetActive(false);
        IM.SwitchToGameplay();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cmBrain.enabled = true;
        CheckTextVisibility();
    }

    public void CheckFishingLevel()
    {
        if (inventory.fishingLevel == 2)
        {
            SwampButton.SetActive(true);
            fishLevel2.SetActive(true);
            fishingTitle.text = "Novice";
        } 
        else if (inventory.fishingLevel == 3)
        {
            SpaceButton.SetActive(true);
            fishLevel3.SetActive(true);
            fishingTitle.text = "Competent";
        } 
        else if (inventory.fishingLevel > 3)
        {
            fishingTitle.text = "Pro";
        }
    }

    public void PotatoPierButton()
    {
        if (inventory.fishingLevel > 0)
        {
            if(SceneManager.GetActiveScene().name == "Level_1")
            {
                Debug.Log("Already in that level!");
            }
            else
            {
                IM.SwitchToGameplay();
                //load level 1
                GameObject GameManager = GameObject.FindWithTag("GameManager");
                GameManager.GetComponent<LevelLoader>().FadeToLevel(1);
            }
        }
        else
        {
            Debug.Log("Fishing level not high enough to go there!");
        }
    }
    public void SquashSwampButton()
    {
        if (inventory.fishingLevel > 1)
        {
            if (SceneManager.GetActiveScene().name == "Level_2")
            {
                Debug.Log("Already in that level!");
            }
            else
            {
                IM.SwitchToGameplay();
                //load level 2
                GameObject GameManager = GameObject.FindWithTag("GameManager");
                GameManager.GetComponent<LevelLoader>().FadeToLevel(2);
            }
        }
        else
        {
            Debug.Log("Fishing level not high enough to go there!");
        }
    }
    public void CosmicCarrotButton()
    {
        if (inventory.fishingLevel > 2)
        {
            if (SceneManager.GetActiveScene().name == "Level_3")
            {
                Debug.Log("Already in that level!");
            }
            else
            {
                IM.SwitchToGameplay();
                //load level 3
                GameObject GameManager = GameObject.FindWithTag("GameManager");
                GameManager.GetComponent<LevelLoader>().FadeToLevel(3);
            }
        }
        else
        {
            Debug.Log("Fishing level not high enough to go there!");
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
