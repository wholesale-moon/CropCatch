using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpin : MonoBehaviour
{
    private float angleChange = 60f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float currentAngle = gameObject.transform.localEulerAngles.y;
        float newAngle = currentAngle + (angleChange * Time.deltaTime);
        gameObject.transform.localEulerAngles = new Vector3(0, newAngle, 0);
    }
}
