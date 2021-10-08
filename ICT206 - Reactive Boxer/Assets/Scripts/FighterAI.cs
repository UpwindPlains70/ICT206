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
    public enum AISTATE { CHASE = 0, ATTACKLOW = 1, ATTACKHIGH = 2, BLOCK = 3, DODGE = 4, RETREAT = 5, DEFEAT = 6, VICTORY = 7};
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
                case AISTATE.DODGE:
                    StartCoroutine(StateDodge());
                    break;
                case AISTATE.RETREAT:
                    StartCoroutine(StateRetreat());
                    break;
                case AISTATE.DEFEAT:
                    StopAllCoroutines();
                    StartCoroutine(StateDefeat());
                    break;
                case AISTATE.VICTORY:
                    StopAllCoroutines();
                    StartCoroutine(StateVictory());
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
    private float attackRange = 1.0f;
    public float oponentDistanceOnRetreat = 2.0f;

    private FighterHealth FHealthScript;
    private Animator anim;
    private ReactiveSensor myRSensor;

    //Stores local refences to Fighter health values
    private int fitnessLevel, reactionLevel, blockingChance;
    private float maxResilience, highAttackCost, lowAttackCost;

    private void Start()
    {
            //store reference to health/stamina so effects can be applied
        FHealthScript = GetComponent<FighterHealth>();
            //Store animator
        anim = GetComponent<Animator>();
        ThisAgent = GetComponent<NavMeshAgent>();
        myRSensor = GetComponent<ReactiveSensor>();

        //GetComponent<Transform>();
        ThisTransform = GetComponent<Transform>();

        fitnessLevel = FHealthScript.FitnessLevel;
        reactionLevel = FHealthScript.ReactionLevel;
        blockingChance = FHealthScript.BlockChance;
        highAttackCost = FHealthScript.highAttackCost;
        lowAttackCost = FHealthScript.lowAttackCost;
        maxResilience = FHealthScript.Resilience;
        CurrentState = _CurrentState;
    }

    private void OnEnable()
    {
        CurrentState = AISTATE.CHASE;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void FixedUpdate()
    {
        if (CurrentState != AISTATE.DEFEAT && CurrentState != AISTATE.VICTORY)
        {
            if (!checkDistance() && CurrentState != AISTATE.CHASE)
                CurrentState = _CurrentState = AISTATE.CHASE;

            if (CurrentState != AISTATE.CHASE && CurrentState != AISTATE.RETREAT && CurrentState != AISTATE.DODGE && Random.Range(0, 100) < reactionLevel)
                CurrentState = _CurrentState = AISTATE.DODGE;

            if(CurrentState == AISTATE.CHASE || Random.Range(0,2) == 1)
                LookAtOponent();

            if (FHealthScript.HealthPoints <= 0 && CurrentState != AISTATE.DEFEAT)
            {
                CurrentState = _CurrentState = AISTATE.DEFEAT;
                Oponent.GetComponent<FighterAI>().CurrentState = AISTATE.VICTORY;
            }

            
            if (CurrentState == AISTATE.RETREAT)
            {
                EvadeOponent();
            }

            if (CurrentState != AISTATE.CHASE && CurrentState != AISTATE.RETREAT && Random.Range(0, 4) == 1)
                subtleMovement();
        }
    }

    private void EvadeOponent()
    {
        float distance = Vector3.Distance(transform.position, Oponent.position);

        if (distance < oponentDistanceOnRetreat)
        {
            Vector3 dirToOponent = transform.position - Oponent.position;

            Vector3 newPos = transform.position + dirToOponent;

            ThisAgent.SetDestination(newPos);
        }
    }

    private void LookAtOponent()
    {
        ThisAgent.SetDestination(Oponent.position);

        //Keep track of moving target
        Vector3 dir = Oponent.position - transform.position;
        Quaternion lookRot = Quaternion.LookRotation(dir);
        lookRot.x = 0; lookRot.z = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Mathf.Clamp01(3.0f * Time.maximumDeltaTime));
    }

    private void subtleMovement()
    {
        float distance = Vector3.Distance(transform.position, Oponent.position);
        Vector3 dirToOponent = Vector3.zero;

        if (distance <= attackRange)
        {
            anim.SetBool("move", true);
            if (Random.Range(0, 2) == 1)
                dirToOponent = transform.position + Oponent.position;
            else
                dirToOponent = transform.position - Oponent.position;

            Vector3 newPos = transform.position + dirToOponent;

            ThisAgent.SetDestination(newPos);
        }
    }

    public IEnumerator StateChase()
    {
        while (CurrentState == AISTATE.CHASE)
        {
            //Check distance
            if(checkDistance())
            {
                    //Change state due to met condition (within range)
                if(Random.Range(0,2) == 1)
                    CurrentState = _CurrentState = AISTATE.ATTACKHIGH;
                else
                    CurrentState = _CurrentState = AISTATE.ATTACKLOW;

                yield break;
            }
            yield return null;
        }
    }
    public IEnumerator StateAttackHigh()
    {
        while (CurrentState == AISTATE.ATTACKHIGH)
        {
                //Choose attack type (one of three)
            anim.SetFloat("AttackType", Random.Range(0.0f, 1.0f));
            anim.SetBool("attackHigh", true);

            //Wait for animation to finish until applying stamina penalty
            yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

            FHealthScript.Stamina -= highAttackCost;
            if (ResilienceCheck() || FHealthScript.Stamina < highAttackCost || FHealthScript.Stamina <= Random.Range(0, fitnessLevel))
            {
                anim.SetBool("attackHigh", false);
                BlockOrRetreat();
            }
        yield return null;
        }
    }

    private bool ResilienceCheck()
    {
        if (FHealthScript.Stamina <= FHealthScript.Stamina * Random.Range(0.1f, maxResilience) ||
            FHealthScript.HealthPoints <= FHealthScript.HealthPoints * Random.Range(0.1f, maxResilience))
        {
            return true;
        }

        return false;
    }

    private void BlockOrRetreat()
    {
        //chance to block
        if (Random.Range(0, reactionLevel) < blockingChance)
            CurrentState = _CurrentState = AISTATE.BLOCK;
        else
            CurrentState = _CurrentState = AISTATE.RETREAT;
    }

    public IEnumerator StateAttackLow()
    {
        while (CurrentState == AISTATE.ATTACKLOW)
        {
            //Choose attack type (one of three)
            anim.SetFloat("AttackType", Random.Range(0.0f, 1.0f));
            anim.SetBool("attackLow", true);
            
            //Wait for animation to finish until applying stamina penalty
            yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

            FHealthScript.Stamina -= lowAttackCost;

            if (ResilienceCheck() || FHealthScript.Stamina < lowAttackCost || FHealthScript.Stamina <= Random.Range(0, fitnessLevel))
            {
                anim.SetBool("attackLow", false);
                BlockOrRetreat();
            }
            yield return null;
        }
    }

    public IEnumerator StateBlock()
    {
        while (CurrentState == AISTATE.BLOCK)
        {
            //Check punch direction
            if(myRSensor.leftHit)
                anim.SetFloat("BlockType", 1); //Left
            else if(myRSensor.centreHit)
                anim.SetFloat("BlockType", 0.5f); //centre
            else if(myRSensor.rightHit)
                anim.SetFloat("BlockType", 0); //Right

            anim.SetBool("block", true);

            StartCoroutine(FHealthScript.Recover());

            ActionAfterRecovery();

            yield return null;
        }
    }

    public IEnumerator StateDodge()
    {
        while (CurrentState == AISTATE.DODGE)
        {
            Debug.Log("Didging");
            //anim.StopPlayback();
            //Check punch direction
            if (myRSensor.leftHit)
                anim.SetFloat("DodgeType", 1); //Left
            else if (myRSensor.centreHit)
                anim.SetFloat("DodgeType", 0.5f); //centre
            else if (myRSensor.rightHit)
                anim.SetFloat("DodgeType", 0); //Right

            anim.SetBool("dodge", true);

            yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
            
            ActionAfterRecovery();
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

    //returns true of distance is within range
    private bool checkDistance()
    {
        float DistancetoDest = Vector3.Distance(ThisTransform.position,
        Oponent.position);

        if (Mathf.Approximately(DistancetoDest, ThisAgent.stoppingDistance) ||
            DistancetoDest <= attackRange)
            return true;

        return false;
    }
    private void ActionAfterRecovery()
    {
        //Use random range to prevent AI from attacking with minimal stamina
        //UPDATE: add distance to gloves check
        //Check distance
        if(checkDistance())
            CurrentState = _CurrentState = AISTATE.CHASE;
        else if (ResilienceCheck() || FHealthScript.Stamina >= (highAttackCost + Random.Range(0, fitnessLevel)))
            CurrentState = _CurrentState = AISTATE.ATTACKHIGH;
        else if (ResilienceCheck() || FHealthScript.Stamina >= (lowAttackCost + Random.Range(0, fitnessLevel)))
            CurrentState = _CurrentState = AISTATE.ATTACKLOW;
    }

    public IEnumerator StateDefeat()
    {
        anim.StopPlayback();//Check punch direction
        if (myRSensor.leftHit)
            anim.SetFloat("DefeatType", 1); //Left
        else if (myRSensor.centreHit)
            anim.SetFloat("DefeatType", 0.5f); //centre
        else if (myRSensor.rightHit)
            anim.SetFloat("DefeatType", 0); //Right

        anim.SetBool("Defeat", true);
            //Turn 'off' movement layer
        anim.SetLayerWeight(1, 0);
        ThisAgent.enabled = false;
            yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
    }

    public IEnumerator StateVictory()
    {
        anim.StopPlayback();
        anim.SetBool("Victory", true);
            //Turn 'off' movement layer
        anim.SetLayerWeight(1, 0);

        ThisAgent.enabled = false;

        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
    }
}
