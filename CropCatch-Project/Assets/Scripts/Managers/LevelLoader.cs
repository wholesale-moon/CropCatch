using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public GameSaveData _GameSaveData;
    public GameObject transition;

    public GameObject[] playerAppearance;

    void Awake()
    {
        if(SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().buildIndex == 2 || SceneManager.GetActiveScene().buildIndex == 3)
        {
            playerAppearance[_GameSaveData._playerAppearance].SetActive(true);
        }
    }
    
    public void FadeToLevel(int level)
    {
        StartCoroutine(LoadLevel(level));
    }

    private IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetActive(true);
        transition.GetComponent<Animator>().SetTrigger("Start");
        GetComponent<SoundManager>().FadeSongOut(3);

        yield return new WaitForSeconds(3);

        SceneManager.LoadScene(levelIndex);
    }
}
