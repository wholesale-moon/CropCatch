using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class EncyclopediaDisplay : MonoBehaviour
{
    [SerializeField] private Encyclopedia encyclopedia;
    [SerializeField] private TabMenu tabMenu;

    [Header("UI")]
    [SerializeField] private TMP_Text fishName;
    [SerializeField] private TMP_Text rarity;
    [SerializeField] private TMP_Text difficulty;
    [SerializeField] private TMP_Text avgSize;
    [SerializeField] private TMP_Text avgPrice;
    [SerializeField] private TMP_Text location;
    [SerializeField] private RawImage fishGraphic;
    
    [Space(10)]
    public ScriptableFish[] fishies;
    private ScriptableFish selectedFish;

    public void OnEnable()
    {
        tabMenu.closeTabMenu += RemoveRenderedFish;
    }

    public void OnDisable()
    {
        tabMenu.closeTabMenu += RemoveRenderedFish;
    }


    public void DisplayFish(string desiredFish)
    {
        //find so with corresponding fish name
        selectedFish = null;
        foreach (ScriptableFish fish in fishies)
        {
            if (fish.fishName == desiredFish)
            {
                selectedFish = fish;
                break;
            }
        }

        if (selectedFish == null)
        {
            Debug.Log("Fish not found.");
            return;
        }

        Debug.Log("The selected fish is " + selectedFish.fishName);
        if (isFishDiscovered(selectedFish.fishName))
        {
            // display fish info into the encyclopedia spots
            fishName.text = selectedFish.fishName;
            rarity.text = selectedFish.rarity.ToString();
            difficulty.text = selectedFish.baseDifficulty.ToString();
            avgSize.text = selectedFish.baseSize.ToString() + " lbs";
            avgPrice.text = selectedFish.basePrice.ToString("C2");
            location.text = selectedFish.location;
            //location.text = selectedFish.location.ToString();
            //fishGraphic.text = selectedFish.fishGraphic;
            RenderFish();
            fishGraphic.gameObject.SetActive(true);
        } else {
            // display fish info into the encyclopedia spots
            fishName.text = "???";
            rarity.text = "???";
            difficulty.text = "???";
            avgSize.text = "??? lbs";
            avgPrice.text = "???";
            location.text = selectedFish.location;
            RenderFish(true);
            fishGraphic.gameObject.SetActive(true);
        }
    }

    private GameObject renderedFish;
    public Material silhouetteMat;
    private void RenderFish(bool silhouette = false)
    {
        if(renderedFish != null)
        {
            GameObject.Destroy(renderedFish);
        }
        //spawn fish model prefab in the middle of the screen 
        renderedFish = Instantiate(selectedFish.modelPrefab);
        renderedFish.SetActive(true);
        renderedFish.transform.parent = Camera.main.transform;
        renderedFish.transform.localPosition = new Vector3(0, .05f, 1f);
        renderedFish.transform.localEulerAngles = new Vector3(0, -90, 0);
        renderedFish.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        renderedFish.AddComponent<FishSpin>();
        renderedFish.layer = 7;
        if(silhouette)
        {
            renderedFish.GetComponent<Renderer>().material = silhouetteMat;
        }
    }

    private bool isFishDiscovered(string fishName)
    {
        bool check = false;

        if(encyclopedia.discovery[fishName] > 0)
        {
            check = true;
        }
        
        return check;
    }

    private void RemoveRenderedFish()
    {
        if (renderedFish != null)
        {
            GameObject.Destroy(renderedFish);
        }
    }
}
