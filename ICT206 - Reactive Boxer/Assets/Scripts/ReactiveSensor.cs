using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(FighterHealth))]
public class ReactiveSensor : MonoBehaviour
{
    private FighterHealth MyHealthScript;

    public GameObject Oponent;
    private FighterHealth OponentHealthScript;
    private Animator OponentAnim;
    public GameObject glove;
    public GameObject torso;

    Vector3 prevPos, currPos;
    float prevDist = 0;
    // Start is called before the first frame update
    void Start()
    {
        MyHealthScript = GetComponent<FighterHealth>();

        OponentHealthScript = Oponent.GetComponent<FighterHealth>();
        OponentAnim = Oponent.GetComponent<Animator>();

        prevPos = glove.transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        /*currPos = glove.transform.position;
        float dist = Vector3.Distance(prevPos, currPos);

        Debug.Log("dis: " + dist + "   prevDist: " + prevDist);
        if (dist > prevDist)
            Debug.Log("Left punch");
        else
            Debug.Log("Right punch");

        prevDist = dist;
        prevPos = glove.transform.position;*/
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.IsChildOf(transform))
        {   
            if (collision.contacts[0].thisCollider.CompareTag("Head"))
                highHit();
            else if (collision.contacts[0].thisCollider.CompareTag("Torso"))
                lowHit();

            if (OponentHealthScript.HealthPoints > 0)
                OponentAnim.Play("Boxing_Idle");
        }
    }

    private void highHit()
    {
        MyHealthScript.HealthPoints -= 10 * OponentHealthScript.Strength;
    }

    private void lowHit()
    {
        MyHealthScript.HealthPoints -= 2 * OponentHealthScript.Strength;
    }
}
