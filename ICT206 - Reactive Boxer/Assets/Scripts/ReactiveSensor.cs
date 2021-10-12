using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
        Vector3 deltaR = (rightGlove.transform.position - transform.position).normalized;
        Vector3 crossR = Vector3.Cross(deltaR, rightGlove.transform.forward);
        
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

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.IsChildOf(transform))
        {   
            if (collision.contacts[0].thisCollider.CompareTag("Head") && !hitLowRunning)
                highHit();
            else if (collision.contacts[0].thisCollider.CompareTag("Torso") && !hitHighRunning)
                lowHit();

            if (OponentHealthScript.HealthPoints > 0)
                OponentAnim.Play("Boxing_Idle");
        }
    }

    private void highHit()
    {
            hitSound.Play();
        if (!hitHighRunning && !hitLowRunning)
        {
            MyHealthScript.HealthPoints -= 10 * OponentHealthScript.Strength;
        }

        if(MyHealthScript.HealthPoints > 0)
            StartCoroutine(HighHit());
    }

    private IEnumerator HighHit()
    {
        hitHighRunning = true;
        myAnim.StopPlayback();
        if (rightHit)
        {
            myAnim.SetFloat("HitType", 0.75f);
            myAnim.Play("Hit");
        }
        else if (leftHit)
        {
            myAnim.SetFloat("HitType", 0);
            myAnim.Play("Hit");
        }
        else if (centreHit)
        {
            myAnim.SetFloat("HitType", 0.5f);
            myAnim.Play("Hit");
        }

        yield return new WaitForSeconds(myAnim.GetCurrentAnimatorStateInfo(0).length);

        hitHighRunning = false;
    }

    private void lowHit()
    {
        hitSound.Play();
        if (!hitHighRunning && !hitLowRunning)
        {
            MyHealthScript.HealthPoints -= 2 * OponentHealthScript.Strength;
        }
        if (MyHealthScript.HealthPoints > 0)
            StartCoroutine(LowHit());
    }

    private IEnumerator LowHit()
    {
        hitLowRunning = true;
        myAnim.StopPlayback();
        if (rightHit)
        {
            myAnim.SetFloat("HitType", 1);
            myAnim.Play("Hit");
        }
        else if (leftHit)
        {
            myAnim.SetFloat("HitType", 0.25f);
            myAnim.Play("Hit");
        }
        else if (centreHit)
        {
            myAnim.SetFloat("HitType", 0.5f);
            myAnim.Play("Hit");
        }

        yield return new WaitForSeconds(myAnim.GetCurrentAnimatorStateInfo(0).length);
        hitLowRunning = false;
    }
}
