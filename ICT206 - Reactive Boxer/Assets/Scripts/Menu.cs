using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

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
    public GameObject PauseMenu;
    public TextMeshProUGUI finalTime;
    public TimeControl timeController;

    public Text fighterA_State;
    public Text fighterB_State;

    private bool paused = false;
    private bool gameStarted = false;
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

        FighterA_State();
        FighterB_State();

        if(Input.GetKeyDown(KeyCode.Escape) && !paused && gameStarted)
        {
            paused = true;
            PauseGame();
        }else if(Input.GetKeyDown(KeyCode.Escape) && paused && gameStarted)
        {
            paused = false;
            ResumeGame();
        }

        if (fighterA_AI != null && (fighterA_AI.CurrentState == FighterAI.AISTATE.VICTORY || fighterA_AI.CurrentState == FighterAI.AISTATE.DEFEAT))
        {
            gameStarted = false;
            setFinalTime();
            HUD.SetActive(false);
            gameOverUI.SetActive(true);
        }
    }

    public void StartGame()
    {
        gameStarted = true;
    }

    public void GameStopped()
    {
        gameStarted = false;
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
        fighterA_AI.disableAgent();
        fighterB_AI.disableAgent();
        Time.timeScale = 0;
        HUD.SetActive(false);
        PauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        fighterA_AI.enableAgent();
        fighterB_AI.enableAgent();
        Time.timeScale = 1;
        HUD.SetActive(true);
        PauseMenu.SetActive(false);
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
        Time.timeScale = 1;
        fighterA_AI.ResetAI();
        fighterA_Stats.ResetStats();

        fighterB_AI.ResetAI();
        fighterB_Stats.ResetStats();

        timeController.ResetTime();
    }

    public void FighterA_State()
    {
        fighterA_State.text = "" + fighterA_AI.CurrentState;
    }

    public void FighterB_State()
    {
        fighterB_State.text = "" + fighterB_AI.CurrentState;
    }
}
