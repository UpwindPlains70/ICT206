using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour
{
    private FighterAI fighterA_AI;
    private FighterHealth fighterA_Stats;
    public GameObject fighterA;

    private FighterAI fighterB_AI;
    private FighterHealth fighterB_Stats;
    public GameObject fighterB;

    public GameObject gameOverUI;
    public GameObject HUD;
    public TextMeshProUGUI finalTime;
    public TimeControl timeController;

    // Start is called before the first frame update
    void Start()
    {
        fighterA_AI = fighterA.GetComponent<FighterAI>();
        fighterA_Stats = fighterA.GetComponent<FighterHealth>();
        
        fighterB_AI = fighterB.GetComponent<FighterAI>();
        fighterB_Stats = fighterB.GetComponent<FighterHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        if (fighterA_AI != null && (fighterA_AI.CurrentState == FighterAI.AISTATE.VICTORY || fighterA_AI.CurrentState == FighterAI.AISTATE.DEFEAT))
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
        fighterA_AI.ResetAI();
        fighterA_Stats.ResetStats();

        fighterB_AI.ResetAI();
        fighterB_Stats.ResetStats();

        timeController.ResetTime();
    }
}
