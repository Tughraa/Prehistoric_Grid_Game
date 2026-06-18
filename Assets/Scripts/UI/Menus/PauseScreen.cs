using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PauseScreen : MonoBehaviour
{
    public AllSystems allSystems;
    public bool paused = false;
    [SerializeField] GameObject screenUI;
    [SerializeField] TMP_Text seedText;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                UnPauseGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    
    public void PauseGame()
    {
        screenUI.SetActive(true);
        paused = true;

        seedText.text = allSystems.randomSystem.seed;

        allSystems.timeSystem.ChangeTimeScale(0f);
    }
    public void UnPauseGame()
    {
        screenUI.SetActive(false);
        paused = false;
        allSystems.timeSystem.ChangeTimeScale(1f);
    }
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
