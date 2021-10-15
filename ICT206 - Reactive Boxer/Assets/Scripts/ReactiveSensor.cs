using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Reactive agent that effects the state machine through reacting
//to being hit byt the oponent
[RequireComponent(typeof(FighterHealth))]
public class ReactiveSensor : MonoBehaviour
{
    private FighterHealth MyHealthScript;

    public GameObject Oponent;
    private FighterHealth OponentHealthScript;
    private Animator OponentAnim;
    private Animator myAnim;
    public GameObject leftGlove;
    public GameObject rightGlove;
    public GameObject torso;

    private AudioSource hitSound;

    private int highHitDamage = 10;
    private int lowHitDamage = 4;

    public bool rightHit {get; private set; }
    public bool leftHit { get; private set; }
    public bool centreHit { get; private set; }

    bool hitHighRunning, hitLowRunning = false;
    
    // Start is called before the first frame update
    void Start()
    {
        MyHealthScript = GetComponent<FighterHealth>();
        myAnim = GetComponent<Animator>();
        hitSound = GetComponent<AudioSource>();

        OponentHealthScript = Oponent.GetComponent<FighterHealth>();
        OponentAnim = Oponent.GetComponent<Animator>();

    }
    // Update is called once per frame
    void Update()
    {
            //Calc direction of right Glove
        Vector3 deltaR = (rightGlove.transform.position - transform.position).normalized;
        Vector3 crossR = Vector3.Cross(deltaR, rightGlove.transform.forward);

            //Calc direction of leftt Glove
        Vector3 deltaL = (leftGlove.transform.position - transform.position).normalized;
        Vector3 crossL = Vector3.Cross(deltaL, leftGlove.transform.forward);

        if (crossR.y > -0.2f && crossR.y < 0.2f)
        {
            // Target is straight ahead
            //Debug.Log("centre");
            centreHit = true;
            rightHit = false;
            leftHit = false;
        }
        else if (crossR.y > 0.2f)
        {
            // Target is to the right
            //Debug.Log("Right");
            centreHit = false;
            rightHit = true;
            leftHit = false;
        }
        else if(crossR.y < -0.2f)
        {
            // Target is to the left
            //Debug.Log("Left");
            centreHit = false;
            rightHit = false;
            leftHit = true;
        }
    }

        //Hit by oponent check
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.IsChildOf(transform))
        {   
                //Hit restricted to Head & Torso
            if (collision.contacts[0].thisCollider.CompareTag("Head") && !hitLowRunning)
                highHit();
            else if (collision.contacts[0].thisCollider.CompareTag("Torso") && !hitHighRunning)
                lowHit();
                
                //When hit stop animation, prevent object clipping
                //Results in jittery combat
            if (OponentHealthScript.HealthPoints > 0)
                OponentAnim.Play("Boxing_Idle");
        }
    }

        //Define behavior for high hit (head)
    private void highHit()
    {
            //Only decrease health if the have stop reacting to prior hit
        if (!hitHighRunning && !hitLowRunning)
        {
            hitSound.Play();
            MyHealthScript.HealthPoints -= highHitDamage * OponentHealthScript.Strength;
        }
            //play hit animation
        if(MyHealthScript.HealthPoints > 0)
            StartCoroutine(HighHit());
    }

        //Controls the High hit animation type
        //HitType is a parameter in the animator
    private IEnumerator HighHit()
    {
        hitHighRunning = true;
        myAnim.StopPlayback();
        if (rightHit)
        {
            myAnim.SetFloat("HitType", 0.75f);
        }
        else if (leftHit)
        {
            myAnim.SetFloat("HitType", 0);
            //myAnim.Play("Hit");
        }
        else if (centreHit)
        {
            myAnim.SetFloat("HitType", 0.5f);
            //myAnim.Play("Hit");
        }

        myAnim.Play("Hit");
        yield return new WaitForSeconds(myAnim.GetCurrentAnimatorStateInfo(0).length);

        hitHighRunning = false;
    }

    private void lowHit()
    {
        //Only decrease health if the have stop reacting to prior hit
        if (!hitHighRunning && !hitLowRunning)
        {
            hitSound.Play();
            MyHealthScript.HealthPoints -= lowHitDamage * OponentHealthScript.Strength;
        }
            //Play hit animation
        if (MyHealthScript.HealthPoints > 0)
            StartCoroutine(LowHit());
    }

    //Controls the Low hit animation type
    //HitType is a parameter in the animator
    private IEnumerator LowHit()
    {
        hitLowRunning = true;
        myAnim.StopPlayback();
        if (rightHit)
        {
            myAnim.SetFloat("HitType", 1);
            //myAnim.Play("Hit");
        }
        else if (leftHit)
        {
            myAnim.SetFloat("HitType", 0.25f);
            //myAnim.Play("Hit");
        }
        else if (centreHit)
        {
            myAnim.SetFloat("HitType", 0.5f);
        }
        myAnim.Play("Hit");

        yield return new WaitForSeconds(myAnim.GetCurrentAnimatorStateInfo(0).length);
        hitLowRunning = false;
    }
}
