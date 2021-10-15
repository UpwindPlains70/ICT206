using UnityEngine;
using UnityEngine.UI;
using System.Collections;
 
//Allows a button to trigger another
public class InvokeButton : MonoBehaviour
{
 
    public Button button;
 
    void Awake ()
    {
        if (button == null)
            button = GetComponent<Button>();
    }
 
        //Triggers the given button
    public void Invoke()
    {
        if (button != null && button.onClick != null)
            button.onClick.Invoke();
    }
}