using UnityEngine;
using System.Collections;

//Allows for camera movement
public class CameraClass : MonoBehaviour
{

    public GameObject target;//the target object
    public float menuSpeedMod = 1.0f;//a speed modifier
    public float gameSpeedMod = 6.0f;//a speed modifier
    private Vector3 point;//the coord to the point where the camera looks at
    private Vector3 initPos;

    public bool MainMenu { get; set; }
    
    void Start()
    {//Set up things on the start method
        point = target.transform.position;//get target's coords
        transform.LookAt(point);//makes the camera look to it
        initPos = transform.position;
        MainMenu = true;
    }

    void Update()
    {
            //makes the camera rotate around "point" coords, rotating around its Y axis, 10 degrees per second times the speed modifier
        if(MainMenu) //Pan camera in Main menu
            transform.RotateAround(point, new Vector3(0.0f, 1.0f, 0.0f), 10 * Time.deltaTime * menuSpeedMod);
        else if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) //Move camera left
            transform.RotateAround(point, new Vector3(0.0f, 1.0f, 0.0f), 10 * Time.deltaTime * gameSpeedMod);
        else if(!MainMenu && (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))) //Move camera right
            transform.RotateAround(point, new Vector3(0.0f, 1.0f, 0.0f), 10 * Time.deltaTime * -gameSpeedMod);

            //camera pan (same as Main menu)
        if (Input.GetKeyDown(KeyCode.P) && MainMenu)
            MainMenu = false;
        else if (Input.GetKeyDown(KeyCode.P) && !MainMenu)
            MainMenu = true;
    }

        //Used by Menu's Play, Replay, MainMenu buttons
        //Sets the camera to the original position
    public void ResetCamPos()
    {
        transform.position = initPos;
        transform.LookAt(point);//makes the camera look to it
    }
}