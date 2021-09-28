using System.Collections;
using System.Collections.Generic;
//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(FighterHealth))]
public class FighterAI : MonoBehaviour
{
    public enum AISTATE { CHASE = 0, ATTACKLOW = 1, ATTACKHIGH = 2, BLOCK = 3, DODGEHIGH = 4, DODGELOW = 5, RETREAT = 6, DEFEAT = 7, VICTORY = 8};
    public AISTATE CurrentState
    {
        get { return _CurrentState; }
        set
        {
            //StopAllCoroutines();
            _CurrentState = value;
            switch (_CurrentState)
            {
                case AISTATE.CHASE:
                    StartCoroutine(StateChase());
                    break;
                case AISTATE.ATTACKHIGH:
                    StartCoroutine(StateAttackHigh());
                    break;
                case AISTATE.ATTACKLOW:
                    StartCoroutine(StateAttackLow());
                    break;
                case AISTATE.BLOCK:
                    StartCoroutine(StateBlock());
                    break;
                case AISTATE.DODGEHIGH:
                    StartCoroutine(StateDodgeHigh());
                    break;
                case AISTATE.DODGELOW:
                    StartCoroutine(StateDodgeLow());
                    break;
                case AISTATE.RETREAT:
                    StartCoroutine(StateRetreat());
                    break;
                case AISTATE.DEFEAT:
                    //StartCoroutine(StateDefeat());
                    break;
                case AISTATE.VICTORY:
                    StartCoroutine(Oponent.GetComponent<FighterHealth>().Victory());
                    break;
            }
        }
    }
    
    [SerializeField]
    private AISTATE _CurrentState = AISTATE.CHASE;

    private NavMeshAgent ThisAgent = null;
    [SerializeField]
    private Transform Oponent; 
    [SerializeField]
    private Transform OponentLeftGlove; 
    [SerializeField]
    private Transform OponentRightGlove;

    private Transform ThisTransform = null;
    private float attackRange = 0.8f;
    public float oponentDistanceOnRetreat = 2.0f;

    private FighterHealth FHealthScript;
    private Animator anim;

    private float highAttackCost = 15;
    private float lowAttackCost = 5;

    //Stores local refences to Fighter health values
    private int fitnessLevel, reactionLevel, nonBlockingChance;

    private void OnValidate()
    {
            //Restrict customization values
        //highAttackCost = Mathf.Clamp(highAttackCost, 5, FHealthScript.Stamina);
        //lowAttackCost = Mathf.Clamp(lowAttackCost, 2, FHealthScript.Stamina);
    }

    private void Awake()
    {
            //store reference to health/stamina so effects can be applied
        FHealthScript = GetComponent<FighterHealth>();
            //Store animator
        anim = GetComponent<Animator>();
        ThisAgent = GetComponent<NavMeshAgent>();

        //GetComponent<Transform>();
        ThisTransform = GetComponent<Transform>();

        fitnessLevel = FHealthScript.fitnessLevel;
        reactionLevel = FHealthScript.reactionLevel;
        nonBlockingChance = FHealthScript.nonBlockingChance;
    }

    private void Start()
    {
        CurrentState = _CurrentState;
    }
    private void OnEnable()
    {
        CurrentState = AISTATE.CHASE;
        //Debug.Log("enabled");
    }
    private void OnDisable()
    {
        StopAllCoroutines();
        //Debug.Log("disabled");
    }

    public void FixedUpdate()
    {
            //Keep track of moving target
        ThisAgent.SetDestination(Oponent.position);
        Vector3 dir = Oponent.position - transform.position;
        Quaternion lookRot = Quaternion.LookRotation(dir);
        lookRot.x = 0; lookRot.z = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Mathf.Clamp01(3.0f * Time.maximumDeltaTime));

        if (FHealthScript.HealthPoints <= 0 && CurrentState != AISTATE.DEFEAT)
        {
            //Debug.Log("END");
            //CurrentState = _CurrentState = AISTATE.DEFEAT;
        }

        if (CurrentState == AISTATE.RETREAT)
        {
            float distance = Vector3.Distance(transform.position, Oponent.position);

            if (distance < oponentDistanceOnRetreat)
            {
                Vector3 dirToOponent = transform.position - Oponent.position;

                Vector3 newPos = transform.position + dirToOponent;

                ThisAgent.SetDestination(newPos);
            }
        }
    }

    public IEnumerator StateChase()
    {
        while (CurrentState == AISTATE.CHASE)
        {
            //Check distance
            float DistancetoDest = Vector3.Distance(ThisTransform.position,
            Oponent.position);

            if (Mathf.Approximately(DistancetoDest, ThisAgent.stoppingDistance) ||
            DistancetoDest <= attackRange)
            {
                //update current state
                CurrentState = _CurrentState = AISTATE.ATTACKHIGH;
                yield break;
            }
            yield return null;
        }
    }
    public IEnumerator StateAttackHigh()
    {
        while (CurrentState == AISTATE.ATTACKHIGH)
        {
            anim.SetBool("attackHigh", true);

            if (FHealthScript.HealthPoints <= 0)
            {
                //Debug.Log("END");
                CurrentState = _CurrentState = AISTATE.DEFEAT;
                break;
            }

            //Wait for animation to finish until applying stamina penalty
            yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

            FHealthScript.Stamina -= highAttackCost;
            if (FHealthScript.Stamina < highAttackCost || FHealthScript.Stamina <= Random.Range(0, fitnessLevel))
            {
                anim.SetBool("attackHigh", false);
                //80% chance to block
                if (Random.Range(0, reactionLevel) > nonBlockingChance)
                    CurrentState = _CurrentState = AISTATE.BLOCK;
                else
                    CurrentState = _CurrentState = AISTATE.RETREAT;
            }
        yield return null;
        }
    }

    public IEnumerator StateAttackLow()
    {
        while (CurrentState == AISTATE.ATTACKLOW)
        {
            anim.SetBool("attackLow", true);

            //Wait for animation to finish until applying stamina penalty
            yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

            FHealthScript.Stamina -= lowAttackCost;

            if (FHealthScript.Stamina < lowAttackCost || FHealthScript.Stamina <= Random.Range(0, fitnessLevel))
            {
                anim.SetBool("attackLow", false);
                //80% chance to block
                if (Random.Range(0, reactionLevel) > nonBlockingChance)
                    CurrentState = _CurrentState = AISTATE.BLOCK;
                else
                    CurrentState = _CurrentState = AISTATE.RETREAT;
            }
            yield return null;
        }
    }

    public IEnumerator StateBlock()
    {
        while (CurrentState == AISTATE.BLOCK)
        {
            StartCoroutine(FHealthScript.Recover());

            ActionAfterRecovery();

            yield return null;
        }
    }

    public IEnumerator StateDodgeHigh()
    {
        while (CurrentState == AISTATE.DODGEHIGH)
        {
            Vector3 Dir = (Oponent.position - ThisTransform.position).
            normalized;
            Dir.y = 0;
            ThisTransform.rotation = Quaternion.LookRotation(Dir, Vector3.up);
            yield return null;
        }
    }
    public IEnumerator StateDodgeLow()
    {
        while (CurrentState == AISTATE.DODGELOW)
        {
            Vector3 Dir = (Oponent.position - ThisTransform.position).
            normalized;
            Dir.y = 0;
            ThisTransform.rotation = Quaternion.LookRotation(Dir, Vector3.up);
            yield return null;
        }
    }
    public IEnumerator StateRetreat()
    {
        while (CurrentState == AISTATE.RETREAT)
        {
            anim.SetBool("move", true);

            StartCoroutine(FHealthScript.Recover());

            //Wait for animation to finish until applying stamina penalty
            yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
            ActionAfterRecovery();

            yield return null;
        }
    }

    private void ActionAfterRecovery()
    {
        //Use random range to prevent AI from attacking with minimal stamina
        if (FHealthScript.Stamina >= (highAttackCost + Random.Range(0, fitnessLevel)))
            CurrentState = _CurrentState = AISTATE.ATTACKHIGH;
        else if (FHealthScript.Stamina >= (lowAttackCost + Random.Range(0, fitnessLevel)))
            CurrentState = _CurrentState = AISTATE.ATTACKLOW;
    }

    public IEnumerator StateDefeat()
    {
        //Debug.Log("Defeat............");
            anim.SetBool("Defeat", true);
            anim.SetFloat("DefeatType", 0);
            
        ThisAgent.enabled = false;
            yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
    }
}
