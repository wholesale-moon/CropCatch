using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterSelect : MonoBehaviour
{
    public GameSaveData _GameSaveData;
    
    [Header("UI")]
    public TMP_Text nameText;
    public TMP_Text personalityText;
    public TMP_Text culinaryText;
    public TMP_Text bestByText;

    [Space(10)]
    public GameObject onionSelect;
    public GameObject daikonSelect;
    public GameObject mushSelect;

    [Header("Character Models")]
    public GameObject onionCharacter;
    public GameObject daikonCharacter;
    public GameObject mushCharacter;

    void Start()
    {
        Onion(); // Default look
    }
    
    public void Onion()
    {
        onionCharacter.SetActive(true);
        daikonCharacter.SetActive(false);
        mushCharacter.SetActive(false);

        onionSelect.SetActive(true);
        daikonSelect.SetActive(false);
        mushSelect.SetActive(false);

        nameText.text = "Onion";
        personalityText.text = "Personality:\n<size=40><color=#B5C3F1>Cries easily, has too many layers.";
        culinaryText.text = "Culinary:\n<size=40><color=#B5C3F1>Great fried, caramelized, and grilled";
        bestByText.text = "Best By: 3 Months";

        _GameSaveData._playerAppearance = 0;
    }

    public void Daikon()
    {
        onionCharacter.SetActive(false);
        daikonCharacter.SetActive(true);
        mushCharacter.SetActive(false);

        onionSelect.SetActive(false);
        daikonSelect.SetActive(true);
        mushSelect.SetActive(false);

        nameText.text = "Daikon";
        personalityText.text = "Personality:\n<size=40><color=#B5C3F1>Stays cool under pressure.";
        culinaryText.text = "Culinary:\n<size=40><color=#B5C3F1>Great baked or boiled";
        bestByText.text = "Best By: 2 Weeks";

        _GameSaveData._playerAppearance = 1;
    }

    public void Mush()
    {
        onionCharacter.SetActive(false);
        daikonCharacter.SetActive(false);
        mushCharacter.SetActive(true);

        onionSelect.SetActive(false);
        daikonSelect.SetActive(false);
        mushSelect.SetActive(true);

        nameText.text = "Mushroom";
        personalityText.text = "Personality:\n<size=40><color=#B5C3F1>Firm and directive.";
        culinaryText.text = "Culinary:\n<size=40><color=#B5C3F1>Best grilled, stuffed, or baked";
        bestByText.text = "Best By: 10 Days";

        _GameSaveData._playerAppearance = 2;
    }

    public void Confirm()
    {
        GetComponent<LevelLoader>().FadeToLevel(1);
    }
}
