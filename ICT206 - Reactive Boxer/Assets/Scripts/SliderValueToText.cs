using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SliderValueToText : MonoBehaviour
{
    public Slider sliderUI;
    private Text textSliderValue;

    void Start()
    {
        textSliderValue = GetComponent<Text>();
        ShowSliderValue();
    }

        //Assign text field to show the value a the slider
    public void ShowSliderValue()
    {
            //Output for whole number
        if(sliderUI.wholeNumbers)
            textSliderValue.text = "(" + sliderUI.value + ")";
        else //Output for floating number (decimal point)
            textSliderValue.text = "(" + sliderUI.value.ToString("F2") + ")";
    }
}