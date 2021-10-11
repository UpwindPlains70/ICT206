using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public void updateHealthBar()
    {
        mySlider.value = playerData.HealthPoints;
    }

    public void setHealthBar()
    {
        mySlider.maxValue = playerData.HealthPoints;
    }

    public void setStaminaBar()
    {
        mySlider.maxValue = playerData.Stamina;
    }

    public void updateStaminaBar()
    {
        mySlider.value = playerData.Stamina;
    }
}
