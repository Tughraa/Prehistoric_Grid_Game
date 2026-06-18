using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StartMenuUI : MonoBehaviour
{
    public TMP_InputField seedTextInput;
    public Toggle randomSeedToggle;

    public void StartGame()
    {
        //Have more things
        Debug.Log(seedTextInput.text+" is the given seed");
        RandomSeedToggle();
        SceneManager.LoadScene(1);
    }
    public void RandomSeedToggle()
    {
        seedTextInput.interactable = !randomSeedToggle.isOn;
        if (randomSeedToggle.isOn)
        {
            if (PlayerPrefs.HasKey("givenSeedKey"))
            {
                PlayerPrefs.DeleteKey("givenSeedKey");
            }
        }
        else
        {
            PlayerPrefs.SetString("givenSeedKey",seedTextInput.text);
        }
    }
}
