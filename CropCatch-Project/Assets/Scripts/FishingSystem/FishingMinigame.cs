using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using TMPro;

public class FishingMinigame : MonoBehaviour
{
    [SerializeField] private InputManager IM;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Encyclopedia encyclopedia;
    [SerializeField] private SoundManager SM;
    [SerializeField] private PauseMenu pauseMenu;

    [Header("UI")]
    public Slider slider;
    public GameObject fishPointsMeter;
    public GameObject qteCanvas;
    public GameObject biteScreen;
    public GameObject warning;
    public GameObject failScreen;
    //public GameObject winScreen;

    [Header("QTE Objects")]
    public GameObject qteOne;
    public GameObject qteTwo;
    public GameObject qteThree;
    private List<QTEv2> qteOrder;
    private int onQTENum = -1;
    private int QTELeftToDo = 0; //connect this to text to show how many QTE's are left

    [Header("Settings")]
    //update this to check player upgrades
    [SerializeField] private float playerMashingWeight = 5;
    [SerializeField] private float playerQTEWeight = 10;

    //these are all base settings
    [SerializeField] private float decrementAmount = 1f; //this is per second, so * time.deltatime
    [SerializeField] private float startingPoints = 30;
    [SerializeField] private float pointsNeeded = 100;
    [SerializeField] private float nextQTETime = 5f;
    //public List<ScriptableFish> availableFish;
    [SerializeField] private Fish selectedFish = null;
    [SerializeField] private List<ScriptableFish> tempAvailablefish = null;

    [HideInInspector] public float currentPoints;
    private bool isBit = false;
    private bool isFishing = false;
    private int MashLorR; //0 is A, 1 is D
    public GameObject LImage;
    public GameObject RImage;
    private bool stopMashingAndDrain = false;

    private bool isInQTE = false;
    private bool fishingComplete = false;

    public event Action CheckTriggerText;

    public TextMeshProUGUI qteLeftText;


    public GameObject winCanvas;
    public TextMeshProUGUI fishDialogue;
    public TextMeshProUGUI quipText;
    public GameObject winContinueTexts;
    public GameObject failContinueTexts;

    private bool waitingForFinish = false;


    private void Awake()
    {
        SM = GetComponent<SoundManager>();
    }
    private void OnEnable()
    {
        //set up input manager events
        IM.FishingInteractEvent += LeftClickEvent;
        IM.FishingRightClickEvent += RightClickEvent;
        IM.FishingWEvent += WPress;
        IM.FishingAEvent += APress;
        IM.FishingSEvent += SPress;
        IM.FishingDEvent += DPress;
        IM.FishingEscEvent += EscEvent;
    }

    private void OnDisable()
    {
        IM.FishingInteractEvent -= LeftClickEvent;
        IM.FishingRightClickEvent -= RightClickEvent;
        IM.FishingWEvent -= WPress;
        IM.FishingAEvent -= APress;
        IM.FishingSEvent -= SPress;
        IM.FishingDEvent -= DPress;
        IM.FishingEscEvent -= EscEvent;
    }

    private void LeftClickEvent()
    {
        //if not fishing, check for bite
        if (!isFishing)
        {
            if (fishingComplete)
            {
                if(!waitingForFinish)
                {
                    //restart minigame
                    Debug.Log("Restart minigame!");
                    if(renderedFish != null)
                    {
                       GameObject.Destroy(renderedFish);
                        renderedFish = null;
                    }
                    SelectFish(tempAvailablefish);
                    InitializeMinigame();
                    EnterFishing();
                }
                
            }
            else if (isBit)
            {
                //start minigame
                StopCoroutine(WaitForBite());
                Debug.Log("Player starting minigame!");
                if (renderedFish != null)
                {
                    GameObject.Destroy(renderedFish);
                    renderedFish = null;
                }
                StartFishing();
            }
        }
        else
        {
            if (stopMashingAndDrain || isInQTE)
            {
                return;
            }
            if (MashLorR == 0)
            {
                //add to counter and update ui
                currentPoints += playerMashingWeight;
                MashLorR = 1;
                UpdateMashingDisplay();
                //Debug.Log("Player hit leftclick, must now hit rightclick!");
            }
        }
    }

    private void RightClickEvent()
    {
        //if fishing, check mashing
        if (isFishing)
        {
            if (stopMashingAndDrain || isInQTE)
            {
                return;
            }

            if (MashLorR == 1)
            {
                //add to counter and update ui
                currentPoints += playerMashingWeight;
                MashLorR = 0;
                UpdateMashingDisplay();
                //Debug.Log("Player hit rightclick, must now hit leftclick!");
            }
        }
        else if (fishingComplete)//if not fishing, check if at end of fishing. if so, right click goes back to overworld
        {
            if(!waitingForFinish)
            {
                if (renderedFish != null)
                {
                    GameObject.Destroy(renderedFish);
                    renderedFish = null;
                }
                ExitFishing();
                Debug.Log("leave minigame!");
            }
            
        }
        else
        {
            if(isBit)
            {
                return;
            }
            if (renderedFish != null)
            {
                GameObject.Destroy(renderedFish);
                renderedFish = null;
            }
            CompleteFishing();
            ExitFishing();
            Debug.Log("leave minigame!");
        }
        
    }

    private void APress()
    {
        if (isFishing && isInQTE)
        {
            //compare to qte button
            if(qteOrder != null && onQTENum != -1)
            { 
                qteOrder[onQTENum].CheckKey("A");
            }
        }
    }

    private void DPress()
    {
        if (isFishing && isInQTE)
        {
            //compare to qte button
            if (qteOrder != null && onQTENum != -1)
            {
                qteOrder[onQTENum].CheckKey("D");
            }
        }
    }

    private void WPress()
    {
        if (isFishing && isInQTE)
        {
            //compare to qte button
            if (qteOrder != null && onQTENum != -1)
            {
                qteOrder[onQTENum].CheckKey("W");
            }
        }
    }

    private void SPress()
    {
        if (isFishing && isInQTE)
        {
            //compare to qte button
            if (qteOrder != null && onQTENum != -1)
            {
                qteOrder[onQTENum].CheckKey("S");
            }
        }
    }

    private void EscEvent()
    {
        if(!isFishing)
        {
            if(fishingComplete)
            {
                if(!waitingForFinish)
                {
                    ExitFishing();
                }
                
            }
            else
            {
                CompleteFishing();
                ExitFishing();
            }
        }
        else
        {
            //pause
            pauseMenu.PublicTogglePause();
        }
    }

    private void Update()
    {
        if (isFishing && !isInQTE)
        {
            CheckStatus();
            if(!stopMashingAndDrain)
            {
                currentPoints -= (decrementAmount * Time.deltaTime);
            }
            //Debug.Log("Fishing Points: " + currentPoints);
            SetMeter();
        }
        else if(isFishing && isInQTE)
        {
            //Debug.Log("PLAYER IN QTE");
        }
    }

    private void CheckStatus()
    {
        SetMeter();
        //Debug.Log("Fishing Points: " + currentPoints);
        if (currentPoints >= pointsNeeded)
        {
            CompleteFishing();

            //add fish to inventory here?

            Debug.Log("Player caught fish!");
            inventory.AddFish(selectedFish);
            ShowFishCatch();
        }
        else if (currentPoints <= 0)
        {
            CompleteFishing();
            Debug.Log("Player lost fish!");
            ShowFishLoss();
        }
    }

    public void SelectFish(List<ScriptableFish> availableFish)
    {
        //select a fish from available fish and use its stats
        if (availableFish != null && availableFish.Count > 0)
        {
            tempAvailablefish = availableFish;
            int rng = Random.Range(0, availableFish.Count); //maybe augment this to make rarer fish rarer
            if (availableFish[rng] != null)
            {
                Debug.Log("Available Fish: " + availableFish);
                Debug.Log("Selected Fish from available: " + availableFish[rng]);
                selectedFish = new Fish(availableFish[rng]);
                //Debug.Log(selectedFish.GetFishName());
                //print(selectedFish);
                //selectedFish.InitializeStats(availableFish[rng]);
                selectedFish.RandomizeStats(availableFish[rng]); //off for testing //i believe the randomization is broke lmao
                Debug.Log("Selected Fish: " + selectedFish);
                InitializeMinigame();
            }
            else
            {
                Debug.Log("selected available fish was null!");
            }
        }
        else
        {
            Debug.Log("Fish unable to be selected from improper list");
        }
    }

    private void InitializeMinigame()
    {
        if (selectedFish != null)
        {
            UpdatePlayerStats();
            pointsNeeded = selectedFish.GetPointsRequired();
            startingPoints = selectedFish.GetStartingPoints();
            decrementAmount = selectedFish.GetDrainRate();

            qteOne.GetComponent<QTEv2>().UpdateStats(decrement: selectedFish.GetQTEDrainRate());
            qteTwo.GetComponent<QTEv2>().UpdateStats(decrement: selectedFish.GetQTEDrainRate());
            qteThree.GetComponent<QTEv2>().UpdateStats(decrement: selectedFish.GetQTEDrainRate());

            nextQTETime = GetNextQTETime();

            //StartFishing();
            Debug.Log("Minigame Instantialized!");
            Debug.Log("Game Info -- PointsNeeded: " + pointsNeeded + ", StartingPoints: " + startingPoints + ", DecrementAmount: " + decrementAmount);
        }
        else
        {
            Debug.Log("selected fish was null, can't initialize minigame");
        }
    }

    private void UpdatePlayerStats()
    {
        inventory.UpdateWeights(); //just in case

        playerMashingWeight = inventory.GetMashingPower();
        playerQTEWeight = inventory.GetQTEPower();
        Debug.Log("mash: " + playerMashingWeight + ", qte: " + playerQTEWeight);
    }

    private float GetNextQTETime()
    {
        if (selectedFish != null)
        {
            float rng = selectedFish.GetQTERNG();
            return selectedFish.GetQTEFrequency() + Random.Range(-rng, rng);
        }
        else
        {
            Debug.Log("Selected fish was null, next QTE time is 5"); //failsafe
            return 5;
        }
    }

    public void EnterFishing()
    {
        SM.PlaySFX("WaterSplash");
        fishPointsMeter.SetActive(false);
        failScreen.SetActive(false);
        winCanvas.SetActive(false);
        failContinueTexts.SetActive(false);
        winContinueTexts.SetActive(false);
        LImage.SetActive(false);
        RImage.SetActive(false);
        biteScreen.SetActive(false);
        fishingComplete = false;
        waitingForFinish = false;

        GameObject player = GameObject.FindWithTag("Player");
        player.GetComponent<PlayerMovement>().PlayAnimation("Casting");

        StartCoroutine(WaitForBite());
    }

    private IEnumerator WaitForBite()
    {
        Debug.Log("Waiting for Bite...");
        float biteTime = Random.Range(2, 6);
        yield return new WaitForSeconds(biteTime);
        isBit = true;
        Debug.Log("Bite!");
        biteScreen.SetActive(true);
        SM.PlaySFX("Bite");
        yield return new WaitForSeconds(2);
        if(isFishing)
        {
            yield break;
        }
        biteScreen.SetActive(false);
        SM.PlaySFX("BiteLost");

        if (!isFishing)
        {
            isBit = false;
            Debug.Log("Bite Lost.");
            StartCoroutine(WaitForBite());
        }
    }

    public void StartFishing()
    {
        fishingComplete = false;
        isFishing = true;
        stopMashingAndDrain = false;
        waitingForFinish = false;
        currentPoints = startingPoints;
        fishPointsMeter.SetActive(true);
        biteScreen.SetActive(false);

        // Animation
        GameObject player = GameObject.FindWithTag("Player");
        player.GetComponent<PlayerMovement>().PlayAnimation("Fishing");

        MashLorR = Random.Range(0,2);
        UpdateMashingDisplay();
        onQTENum = -1;
        //StartCoroutine(decrementMeter());
        StartCoroutine(qteCoolDown());
        SM.SwitchTheme("Fishing", 0.05f);
    }

    public void ResumeFishing()
    {
        qteCanvas.SetActive(false);
        //StartCoroutine(decrementMeter());
        StartCoroutine(qteCoolDown());
        stopMashingAndDrain = false;
        UpdateMashingDisplay();

        isInQTE = false;
    }

    private void CompleteFishing()
    {
        SM.FadeOutAllMusic(2f);
        isBit = false;
        isInQTE = false;
        isFishing = false;
        stopMashingAndDrain = false;
        //fishingComplete = true;
        //selectedFish = null;
        onQTENum = -1;

        warning.GetComponent<Animator>().SetTrigger("Deactivated");
        qteCanvas.SetActive(false); // just in case
        fishPointsMeter.SetActive(false);
        LImage.SetActive(false);
        RImage.SetActive(false);
        StopAllCoroutines();

        encyclopedia.discovery[selectedFish.GetFishName()] += 1;

        fishingComplete = true;
    }

    public void ExitFishing()
    {
        fishingComplete = false;
        waitingForFinish = false;
        tempAvailablefish = null;
        //StopAllCoroutines(); //this should fix bite issue
        // Switch to gameplay controls
        IM.SwitchToGameplay();
        // Hide canvas
        GameObject canvas = GameObject.FindWithTag("ButtonMashCanvas");
        canvas.SetActive(false);


        winCanvas.SetActive(false);
        // Switch camera
        GameObject CamStateMachine = GameObject.FindWithTag("CamStateMachine");
        CamStateMachine.GetComponent<CinemachineSwitcher>().SwitchState("Overworld");
        CheckTriggerText?.Invoke();
        SM.SwitchTheme("Overworld", 1f);

        GameObject player = GameObject.FindWithTag("Player");
        player.GetComponent<PlayerMovement>().PlayAnimation("Idle");
    }

    private IEnumerator qteCoolDown()
    {
        float cdTime = GetNextQTETime();
        yield return new WaitForSeconds(cdTime);

        warning.GetComponent<Animator>().SetTrigger("Warning");
        LImage.SetActive(false);
        RImage.SetActive(false);
        //stopMashingAndDrain = true;
        yield return new WaitForSeconds(1.5f);

        StopAllCoroutines();
        qteOne.gameObject.SetActive(false);
        qteTwo.gameObject.SetActive(false);
        qteThree.gameObject.SetActive(false);
        qteCanvas.SetActive(true);

        LImage.SetActive(false);
        RImage.SetActive(false);

        qteOrder = new List<QTEv2>();
        int qteNum = Random.Range(1, 4); //1-3 qte to do
        QTELeftToDo = qteNum;
        List<int> qteAvailable = new List<int>(); //helps with qhich QTEs are chosen in which order
        qteAvailable.Add(1);
        qteAvailable.Add(2);
        qteAvailable.Add(3);
        for (int i = 0; i < qteNum; i++)
        {
            //sets which  QTE happen randomly
            int rngQTE = Random.Range(0, qteAvailable.Count - 1);
            int chosenQTE = qteAvailable[rngQTE];
            switch (chosenQTE)
            {
                case 1:
                    Debug.Log("QTE 1 ADDED");
                    qteOrder.Add(qteOne.GetComponent<QTEv2>());
                    break;
                case 2:
                    Debug.Log("QTE 2 ADDED");
                    qteOrder.Add(qteTwo.GetComponent<QTEv2>());
                    break;
                case 3:
                    Debug.Log("QTE 3 ADDED");
                    qteOrder.Add(qteThree.GetComponent<QTEv2>());
                    break;
            }
            qteAvailable.RemoveAt(rngQTE);
        }
        onQTENum = 0;
        Debug.Log("qteOrder " + qteOrder + " and first qte " + qteOrder[0]);
        qteOrder[onQTENum].gameObject.SetActive(true);
        qteOrder[onQTENum].StartQTE();

        isInQTE = true;
        UpdateQTELeft();

        yield return null;
    }



    private IEnumerator WaitForNextQTE(float time)
    {
        Debug.Log("QAITING FOR NEXT QTE: " + time);
        yield return new WaitForSeconds(time);
        IncrementQTE();
    }

    public void PassQTE()
    {
        SM.PlaySFX("QTEPass");
        currentPoints += playerQTEWeight;
        StartCoroutine(WaitForNextQTE(Random.Range(0.2f, 0.8f)));
        CheckStatus();
    }

    public void FailQTE()
    {
        SM.PlaySFX("QTEFail");
        //Debug.Log("Check for when this is called: FailQTE");
        currentPoints -= playerQTEWeight;
        StartCoroutine(WaitForNextQTE(Random.Range(0.5f, 1f)));
        CheckStatus();
    }


    private void IncrementQTE()
    {
        Debug.Log("MADE IT TO INCREMENT QTE");
        onQTENum++;
        if(onQTENum < qteOrder.Count)
        {
            Debug.Log("Doing next QTE");
            qteOrder[onQTENum].gameObject.SetActive(true);
            qteOrder[onQTENum].StartQTE();
            UpdateQTELeft();
        }
        else
        {
            onQTENum = -1;
            Debug.Log("Resuming Fishing");
            ResumeFishing();
        }
    }

    private void SetMeter()
    {
        slider.value = Math.Clamp((currentPoints/pointsNeeded) * 100f,0,100);
    }

    private void UpdateMashingDisplay()
    {
        if (MashLorR == 0)
        {
            LImage.SetActive(true);
            RImage.SetActive(false);
        }
        else if(MashLorR == 1)
        {
            LImage.SetActive(false);
            RImage.SetActive(true);
        }
        else
        {
            Debug.Log("Mashing num is invalid");
        }
    }


    private void UpdateQTELeft()
    {
        qteLeftText.text = "QTE Left: " + (qteOrder.Count - onQTENum);
    }


    private GameObject renderedFish;
    private void ShowFishCatch()
    {
        SM.PlaySFX("CatchFish");
        waitingForFinish = true;
        winContinueTexts.SetActive(false);
        //show fish finish canvas
        winCanvas.SetActive(true);

        // Animation
        GameObject player = GameObject.FindWithTag("Player");
        player.GetComponent<PlayerMovement>().PlayAnimation("Success");

        //set text on diaglogue as "you caught "fish name"! "pun here"
        fishDialogue.text = "You Caught A <color=#4865f4>" + selectedFish.GetFishName() + "</color>!";
        quipText.text = selectedFish.quip;
        //spawn fish model prefab in the middle of the screen 
        renderedFish = Instantiate(selectedFish.model);
        renderedFish.SetActive(true);
        renderedFish.transform.parent = Camera.main.transform;
        renderedFish.transform.localPosition = new Vector3(0,.05f, 1f);
        renderedFish.transform.localEulerAngles = new Vector3(0,-90,0);
        renderedFish.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        renderedFish.AddComponent<FishSpin>();
        renderedFish.layer = 7;
        //start continue coroutine 
        StartCoroutine(ShowContinueTexts());
    }


    private void ShowFishLoss()
    {
        SM.PlaySFX("LostFish");
        waitingForFinish = true;
        failContinueTexts.SetActive(false);
        //show fish finish canvas

        // Animation
        GameObject player = GameObject.FindWithTag("Player");
        player.GetComponent<PlayerMovement>().PlayAnimation("Fail");

        //set text on diaglogue as "You didn't catch the fish :("
        failScreen.SetActive(true);
        //start continue coroutine 
        StartCoroutine(ShowContinueTexts());
    }

    private IEnumerator ShowContinueTexts()
    {
        //wait 1 second
        yield return new WaitForSeconds(1.5f);
        //show "left click to continue, right click to exit" and allow inputs again by making fishimgComplete true
        if(winCanvas.activeSelf)
        {
            winContinueTexts.SetActive(true);
        }
        else
        {
            failContinueTexts.SetActive(true);
        }
        //continueTexts.SetActive(true);
        waitingForFinish = false;
        yield break;
    }


    public void AutoFish()
    {
        if(isFishing)
        {
            Debug.Log("autofish complete");
            currentPoints = pointsNeeded;
            CheckStatus();
        }
        else
        {
            Debug.Log("not fishing right now");
        }
    }
}
