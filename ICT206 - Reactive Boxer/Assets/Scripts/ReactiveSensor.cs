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

    bool rightHit, leftHit, centreHit = false;

    bool hitHigh, hitLow = false;
    
    // Start is called before the first frame update
    void Start()
    {
        MyHealthScript = GetComponent<FighterHealth>();
        myAnim = GetComponent<Animator>();

        OponentHealthScript = Oponent.GetComponent<FighterHealth>();
        OponentAnim = Oponent.GetComponent<Animator>();

    }
    // Update is called once per frame
    void Update()
    {
        Vector3 delta = (rightGlove.transform.position - transform.position).normalized;
        Vector3 cross = Vector3.Cross(delta, rightGlove.transform.forward);

        if (cross.y > -0.2f && cross.y < 0.2f)
        {
            // Target is straight ahead
            Debug.Log("centre");
            centreHit = true;
            rightHit = false;
            leftHit = false;
        }
        else if (cross.y > 0.2f)
        {
            // Target is to the right
            Debug.Log("Right");
            centreHit = false;
            rightHit = true;
            leftHit = false;
        }
        else
        {
            // Target is to the left
            Debug.Log("Left");
            centreHit = false;
            rightHit = false;
            leftHit = true;
        }
        /*float leftDir = Vector3.Dot(torso.transform.position, leftGlove.transform.right);
        float rightDir = Vector3.Dot(torso.transform.position, rightGlove.transform.right);
        
        Debug.Log(rightDir);

        if (rightDir > 0.2f)
            Debug.Log("Right");
        else if (leftDir < -0.4f && leftDir > -0.8f)
            Debug.Log("Left");
        else
            Debug.Log("Centre");*/
    }

    //Source: https://docs.unity3d.com/ScriptReference/AI.NavMesh.FindClosestEdge.html
    void nearestRope()
    {
        NavMeshHit hit;
        if (NavMesh.FindClosestEdge(transform.position, out hit, NavMesh.AllAreas))
        {
            //DrawCircle(transform.position, hit.distance, Color.red);
            Debug.DrawRay(hit.position, Vector3.up, Color.red);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.IsChildOf(transform))
        {   
            if (collision.contacts[0].thisCollider.CompareTag("Head") && !hitLow)
                highHit();
            else if (collision.contacts[0].thisCollider.CompareTag("Torso") && !hitHigh)
                lowHit();

            if (OponentHealthScript.HealthPoints > 0)
                OponentAnim.Play("Boxing_Idle");
        }
        hitHigh = hitLow = false;
    }

    private void highHit()
    {
        hitHigh = true;
        MyHealthScript.HealthPoints -= 10 * OponentHealthScript.Strength;
        //StopAllCoroutines();
        if(MyHealthScript.HealthPoints > 0)
            StartCoroutine(HighHit());
    }

    private IEnumerator HighHit()
    {
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
    }

    private void lowHit()
    {
        hitLow = true;
        MyHealthScript.HealthPoints -= 2 * OponentHealthScript.Strength;
        //StopAllCoroutines();

        if (MyHealthScript.HealthPoints > 0)
            StartCoroutine(LowHit());
    }

    private IEnumerator LowHit()
    {
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
    }
}
