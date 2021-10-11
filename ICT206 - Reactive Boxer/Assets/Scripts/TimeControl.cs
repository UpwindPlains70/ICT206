using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeControl : MonoBehaviour
{
    private TextMeshProUGUI timeText;
    float time = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        timeText = GetComponent<TextMeshProUGUI>();    
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        int d = (int)(time * 100.0f);
        int minutes = d / (60 * 100);
        int seconds = (d % (60 * 100)) / 100;
        timeText.text = string.Format("Time:\n{0:00}:{1:00}", minutes, seconds);
    }

    public float getTime()
    {
        return time;
    }
}
