using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class fishMeter : MonoBehaviour
{
    [SerializeField] private InputManager IM;

    [Header("UI")]
    public Slider slider;
    public GameObject qteCanvas;
    public GameObject warning;
    public GameObject failScreen;
    public GameObject winScreen;

    [Header("QTE Objects")]
    public GameObject qteOne;
    public GameObject qteTwo;
    public GameObject qteThree;

    [Header("Settings")]
    public int playerWeight = 25;
    public int decrementAmount = 1;
    public int startingLevel = 250;
    public int maxLevel = 500;
    
    [HideInInspector] public int fishLevel;
    private bool isFishing;
    private bool isBit;

    public event Action CheckTriggerText;

    void Start()
    {
        slider.maxValue = maxLevel;
    }

    void Update()
    {
        if (isFishing)
        {
            if(Input.GetButtonDown("A") || Input.GetButtonDown("D"))
            {
                fishLevel += playerWeight;
            }
        }

        if (isBit)
        {
            if(Input.GetMouseButtonDown(0))
            {
                StartFishing();
            }
        }
    }

    void SetMeter()
    {
        slider.value = fishLevel;
    }
    
    public void EnterFishing()
    {
        fishLevel = startingLevel;
        failScreen.SetActive(false);
        winScreen.SetActive(false);
        SetMeter();
        StartCoroutine(WaitForBite());
    }

    public void StartFishing()
    {
        isFishing = true;
        StartCoroutine(decrementMeter());
        StartCoroutine(qteCoolDown());
    }

    public void ResumeFishing()
    {
        qteCanvas.SetActive(false);
        StartCoroutine(decrementMeter());
        StartCoroutine(qteCoolDown());
    }

    public void ExitFishing()
    {
        // Switch to gameplay controls
        IM.SwitchToGameplay();
        // Hide canvas
        GameObject canvas = GameObject.FindWithTag("ButtonMashCanvas");
        canvas.SetActive(false);

        // Switch camera
        GameObject CamStateMachine = GameObject.FindWithTag("CamStateMachine");
        CamStateMachine.GetComponent<CinemachineSwitcher>().SwitchState("Overworld");
        CheckTriggerText?.Invoke();
    }

    private IEnumerator decrementMeter()
    {
        while (isFishing)
        {
            fishLevel -= decrementAmount;

            if (fishLevel <= 0)
            {
                StartCoroutine(CompleteFishing());
                winScreen.SetActive(true);
            } else if (fishLevel >= maxLevel)
            {
                StartCoroutine(CompleteFishing());
                winScreen.SetActive(true);
            }
            
            SetMeter();
            yield return new WaitForSeconds(0.01f);
        }

        yield return null;
    }

    private IEnumerator WaitForBite()
    {
        Debug.Log("Waiting for Bite...");
        float biteTime = Random.Range(5,8);
        yield return new WaitForSeconds(biteTime);
        isBit = true;
        Debug.Log("Bite!");
        yield return new WaitForSeconds(2);
        isBit = false;
        Debug.Log("Bite Lost.");

        if(!isFishing)
        {
            StartCoroutine(WaitForBite());
        }
    }

    private IEnumerator qteCoolDown()
    {
        float cdTime = Random.Range(5,8);
        yield return new WaitForSeconds(cdTime);

        warning.GetComponent<Animator>().SetTrigger("Warning");
        yield return new WaitForSeconds(1.5f);

        StopAllCoroutines();

        qteCanvas.SetActive(true);
        qteOne.GetComponent<QTESys>().StartQTE();
        qteTwo.GetComponent<QTESys>().StartQTE();
        qteThree.GetComponent<QTESys>().StartQTE();

        yield return null;
    }

    private IEnumerator CompleteFishing()
    {
        isFishing = false;
        warning.GetComponent<Animator>().SetTrigger("Deactivated");
        StopAllCoroutines();
        
        yield return new WaitForSeconds(1);
        ExitFishing();
        yield return null;
    }
}