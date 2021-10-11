using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour
{
    public FighterAI fighterA;
    public GameObject gameOverUI;
    public GameObject HUD;
    public TextMeshProUGUI finalTime;
    public TimeControl timeController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (fighterA != null && (fighterA.CurrentState == FighterAI.AISTATE.VICTORY || fighterA.CurrentState == FighterAI.AISTATE.DEFEAT))
        {
            setFinalTime();
            HUD.SetActive(false);
            gameOverUI.SetActive(true);
        }
    }

    void setFinalTime()
    {
        int d = (int)(timeController.getTime() * 100.0f);

        int minutes = d / (60 * 100);
        int seconds = (d % (60 * 100)) / 100;

        finalTime.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);

    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void Resume()
    {
        Time.timeScale = 1;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetTimeScale(float newValue)
    {
        Time.timeScale = newValue;
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}
