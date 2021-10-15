using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Used by User interface HUD (heads up display)
//Displays the fighters health, stamina, & current state
public class HealthStamBar : MonoBehaviour
{
    public FighterHealth playerData;
    private Slider mySlider;
    public bool Health = true;
    public bool Stamina = false;

    // Start is called before the first frame update
    void Start()
    {
        mySlider = GetComponent<Slider>();
        if(Health)
            setHealthBar();
        else if(Stamina)
            setStaminaBar();
    }

    // Update is called once per frame
    void Update()
    {
        if(Health)
            updateHealthBar();
        else if(Stamina)
            updateStaminaBar();
    }

        //update health bar to fighters health value
    public void updateHealthBar()
    {
        mySlider.value = playerData.HealthPoints;
    }

        //Set health bars max value
    public void setHealthBar()
    {
        mySlider.maxValue = playerData.HealthPoints;
    }

        //Set stamina bars max value
    public void setStaminaBar()
    {
        mySlider.maxValue = playerData.Stamina;
    }

        //update stamina bar to fighters stamina value
    public void updateStaminaBar()
    {
        mySlider.value = playerData.Stamina;
    }
}
