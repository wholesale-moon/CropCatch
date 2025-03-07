using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QTESys : MonoBehaviour
{
    public GameObject circle;
    public TMP_Text keyDisplay;

    private Vector3 currentScale = new Vector3(1, 1, 1);
    private Vector3 decrementSize = new Vector3(0.01f, 0.01f, 0.01f);
    private Vector3 passScale = new Vector3(0.14f, 0.14f, 0.14f);
    private Vector3 failScale = new Vector3(0.06f, 0.06f, 0.06f);

    private bool isQTE;
    private bool inPassZone;
    private bool passedQTE;
    private bool canQTECheck;

    [SerializeField] private bool isSecondQTE;
    [SerializeField] private bool isThirdQTE;

    private GameObject fishMeter;

    private string qteKey;
    private string[] keyChoices = {"W", "A", "S", "D"};

    void Start()
    {
        // Hardcoded for now, but the randomization can be put in an OnEnable
        // if (isSecondQTE)
        // {
        //     qteKey = "A";
        // }
        // else if (isThirdQTE)
        // {
        //     qteKey = "S";
        // }
        // else
        // {
        //     qteKey = "D";
        // }

        fishMeter = GameObject.FindWithTag("FishMeter");
    }

    void OnEnable()
    {
        circle.SetActive(false);
        qteKey = keyChoices[Random.Range(0,3)];
        keyDisplay.text = qteKey;
    }

    void Update()
    {
        if (Input.GetButtonDown(qteKey))
        {
            if (isQTE && canQTECheck)
            {
                if (inPassZone) // if the circle is within the passing zone, qte is passed
                {
                    passedQTE = true;
                    isQTE = false;
                    fishMeter.GetComponent<fishMeter>().fishLevel += 10;

                    if (isThirdQTE)
                        fishMeter.GetComponent<fishMeter>().ResumeFishing();
                }
                else // if the circle is not within the passing zone, qte is failed
                {
                    isQTE = false;
                    if (isThirdQTE)
                        fishMeter.GetComponent<fishMeter>().ResumeFishing();
                }
            }
        }
    }

    public void StartQTE()
    {
        StartCoroutine(QTE());
    }

    private IEnumerator QTE()
    {
        if (isSecondQTE)
        {
            yield return new WaitForSeconds(1f);
            canQTECheck = false;
            StartCoroutine(canQTEWait(0.2f));
        }
        else if (isThirdQTE)
        {
            yield return new WaitForSeconds(2f);
            canQTECheck = false;
            StartCoroutine(canQTEWait(0.2f));
        }
        else 
        {
            canQTECheck = false;
            StartCoroutine(canQTEWait(0.2f));
        }
        
        // initial value setting
        isQTE = true;
        passedQTE = false;
        circle.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        currentScale = circle.transform.localScale;
        circle.SetActive(true);

        while (isQTE)
        {
            currentScale -= decrementSize;
            circle.transform.localScale = currentScale;
            yield return new WaitForSeconds(0.015f);

            if (currentScale.x <= passScale.x && currentScale.x > failScale.x)
            {
                inPassZone = true;
            }
            else
            {
                inPassZone = false;
            }

            CheckFail();
        }

        yield return null;
    }

    void CheckFail()
    {
        if (currentScale.x < failScale.x) // if the circle is within the fail zone, qte is failed
        {
            isQTE = false;
            if (!passedQTE)
            {
                Debug.Log("Fail");
                if (isThirdQTE)
                    fishMeter.GetComponent<fishMeter>().ResumeFishing();
            }
        }
    }

    private IEnumerator canQTEWait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        canQTECheck = true;
    }
}
