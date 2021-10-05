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

    public void ShowSliderValue()
    {
        if(sliderUI.wholeNumbers)
            textSliderValue.text = "(" + sliderUI.value + ")";
        else
            textSliderValue.text = "(" + sliderUI.value.ToString("F2") + ")";
    }
}