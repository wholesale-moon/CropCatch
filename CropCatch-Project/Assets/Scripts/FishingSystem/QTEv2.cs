using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QTEv2 : MonoBehaviour
{
    public GameObject circle;
    public TMP_Text keyDisplay;
    public Image QteDisplay;

    public Color startColor;
    public Color startDisplayColor;
    public Color passColor;
    public Color failColor;

    private float currentScale = 1;
    public float decrementSize = 0.35f; //this could probably be altered via fish data, also per second so * time.deltatime
    private float passScale = .14f;
    private float failScale = .06f;

    private bool isQTE = false; //get fishminigame to change this and allow for proper use
    private bool inPassZone;

    [SerializeField] private FishingMinigame fishingMinigame;

    private string qteKey;
    private string[] keyChoices = { "W", "A", "S", "D" };

    void Start()
    {
        //fishMeter = GameObject.FindWithTag("FishMeter");
    }

    void OnEnable()
    {
        circle.SetActive(false);
        qteKey = keyChoices[Random.Range(0, 4)];
        keyDisplay.text = qteKey;
    }

    public void CheckKey(string key)
    {
        if(!isQTE)
        {
            return;
        }
        //print("Check QTE Info: " + "input:" + key + ", chosen key:" + qteKey + ", in pass = " + CheckPassZone());
        if(key == qteKey && CheckPassZone())
        {
            circle.GetComponent<Image>().color = passColor;
            QteDisplay.color = passColor;
            fishingMinigame.PassQTE();
            Debug.Log("Pass.");
        }
        else
        {
            circle.GetComponent<Image>().color = failColor;
            QteDisplay.color = failColor;
            fishingMinigame.FailQTE();
            Debug.Log("Fail.");
        }
        isQTE = false;
    }

    private bool CheckPassZone()
    {
        return inPassZone;
    }

    private void Update()
    {
        if(isQTE)
        {
            currentScale -= decrementSize * Time.deltaTime;
            circle.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            ChangeOpacity();


            if (currentScale <= passScale && currentScale > failScale)
            {
                inPassZone = true;
            }
            else
            {
                inPassZone = false;
            }
            CheckFail();
        }
    }

    public void StartQTE()
    {
        isQTE = true;
        inPassZone = false;
        circle.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        currentScale = circle.transform.localScale.x;
        QteDisplay.color = startDisplayColor;
        circle.GetComponent<Image>().color = startColor;
        circle.SetActive(true);
    }

    private void EndQTE()
    {
        isQTE = false;
        circle.GetComponent<Image>().color = failColor;
        QteDisplay.color = failColor;
        fishingMinigame.FailQTE();
        //call back to minigame to wait and continue next QTE
        //fishingMinigame;
    }

    private void CheckFail()
    {
        if (currentScale < failScale) // if the circle is within the fail zone, qte is failed
        {
            if (!CheckPassZone())
            {
                Debug.Log("Fail");
            }
            EndQTE();
        }
    }

    public void UpdateStats(float pass = 14, float fail = 6, float decrement = 35)
    {
        passScale = Mathf.Clamp(pass / 100, 0, 1);
        failScale = Mathf.Clamp(fail / 100, 0, 1);
        decrementSize = Mathf.Clamp(decrement / 100, 0, 1);
    }

    private void ChangeOpacity()
    {
        //something here isnt working properly, gotta fix math


        //float scaleNormalized = 1f - (Mathf.Clamp(currentScale, passScale, 1) - passScale) / (1f - passScale);
        //float alphaNum = Mathf.Clamp(scaleNormalized * 255, 20, 148);
        //float alphaNumNormalized = (alphaNum - 20)/(148/20);

        ////Debug.Log("scale normal: " + scaleNormalized + ", alpha num: " + alphaNum);
        //Color currentColor = circle.GetComponent<Image>().color;
        //Color newColor = new Color(currentColor.r, currentColor.g, currentColor.b, alphaNumNormalized);
        //circle.GetComponent<Image>().color = newColor;
    }
}
