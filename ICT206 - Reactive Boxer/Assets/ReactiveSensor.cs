using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(FighterHealth))]
public class ReactiveSensor : MonoBehaviour
{
    private FighterHealth MyHealthScript;
    private FighterAI MyAI;
    private Animator myAnim;

    public GameObject Oponent;
    private FighterHealth OponentHealthScript;
    private FighterAI OponentAI;
    private Animator OponentAnim;

    // Start is called before the first frame update
    void Start()
    {
        MyAI = GetComponent<FighterAI>();
        MyHealthScript = GetComponent<FighterHealth>();
        myAnim = GetComponent<Animator>();

        OponentHealthScript = Oponent.GetComponent<FighterHealth>();
        OponentAnim = Oponent.GetComponent<Animator>();
        OponentAI = Oponent.GetComponent<FighterAI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool Defeat = false;
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.IsChildOf(transform))
        {
           

            if (collision.contacts[0].thisCollider.CompareTag("Head"))
                highHit();
            else if (collision.contacts[0].thisCollider.CompareTag("Torso"))
                lowHit();


            if (OponentHealthScript.HealthPoints > 0)
            {
                OponentAnim.Play("Boxing_Idle");
                //StartCoroutine(OponentAI.StateDefeat());
                //Debug.Log("Defeat...");
                //StartCoroutine(MyHealthScript.Victory());
            }
                
        }
    }

    private void highHit()
    {
        MyHealthScript.HealthPoints -= 10 * OponentHealthScript.Strength;
        //Debug.Log("High Hit");
    }

    private void lowHit()
    {
        MyHealthScript.HealthPoints -= 2 * OponentHealthScript.Strength;
        //Debug.Log("Low Hit");
    }
}
