using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Define the state machine for a fighter
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(FighterHealth))]
public class FighterAI : MonoBehaviour
{
    public enum AISTATE { CHASE = 0, ATTACKLOW = 1, ATTACKHIGH = 2, BLOCK = 3, DODGE = 4, RETREAT = 5, DEFEAT = 6, VICTORY = 7, ESCAPE = 8};
    
        //Defines handling of state matchine (executes state functions)
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
                case AISTATE.ESCAPE:
                    StartCoroutine(StateEscape());
                    break;
            }
        }
    }
    
    [SerializeField]
    private AISTATE _CurrentState = AISTATE.CHASE;

    private NavMeshAgent ThisAgent = null;
    [SerializeField]
    private Transform Oponent; 

    private Transform ThisTransform = null;
    private float attackRange = 1.0f;
    public float oponentDistanceOnRetreat = 2.0f;

    private FighterHealth FHealthScript;
    private Animator anim;
    private ReactiveSensor myRSensor;

    public GameObject victory;
    private Vector3 initialPos;

    private void Start()
    {
        initialPos = GetComponent<Transform>().position;
            //store reference to health/stamina so effects can be applied
        FHealthScript = GetComponent<FighterHealth>();
            //Store animator
        anim = GetComponent<Animator>();
        ThisAgent = GetComponent<NavMeshAgent>();
        myRSensor = GetComponent<ReactiveSensor>();

        ThisTransform = GetComponent<Transform>();

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

        //Called every physics cycle
    public void FixedUpdate()
    {
        if (CurrentState != AISTATE.DEFEAT && CurrentState != AISTATE.VICTORY)
        {
                //Initiates CHASE state
            if (!checkDistance() && CurrentState != AISTATE.CHASE)
                CurrentState = _CurrentState = AISTATE.CHASE;

                //Initiates the dodge state (results in a jittery fight)
            //if (CurrentState != AISTATE.CHASE && CurrentState != AISTATE.RETREAT && CurrentState != AISTATE.DODGE && Random.Range(0, 100) < reactionLevel)
            //    CurrentState = _CurrentState = AISTATE.DODGE;

                //Ensures the fighter looks at their oponent
            if (CurrentState == AISTATE.CHASE || Random.Range(0,2) == 1)
                LookAtOponent();

                //trigger DEFEAT/VICTROY state on 0 health & no prior call
            if (FHealthScript.HealthPoints <= 0 && CurrentState != AISTATE.DEFEAT)
            {
                CurrentState = _CurrentState = AISTATE.DEFEAT;
                Oponent.GetComponent<FighterAI>().CurrentState = AISTATE.VICTORY;
            }
            
                //Controles retreat movement of navMeshAgent
            if (CurrentState == AISTATE.RETREAT)
            {
                EvadeOponent();
            }

                //Controles subtle movement of fighter
            if (CurrentState != AISTATE.CHASE && CurrentState != AISTATE.RETREAT && CurrentState != AISTATE.ESCAPE && Random.Range(0, 4) == 1)
                subtleMovement();
        }
    }

        //Keeps the fighter a curtain distance from oponent to recover
    private void EvadeOponent()
    {
            //Distance from fighter to oponent
        float distance = Vector3.Distance(transform.position, Oponent.position);

            //If not far enough away move oposite direction
        if (distance < oponentDistanceOnRetreat)
        {
            Vector3 dirToOponent = transform.position - Oponent.position;

            Vector3 newPos = transform.position + dirToOponent;

            ThisAgent.SetDestination(newPos);
        }
    }

        //Keeps fighter looking at oponent & targets oponent
    private void LookAtOponent()
    {
            //Only target oponent if not ESCAPE or RETREAT state
        if(CurrentState != AISTATE.ESCAPE && CurrentState != AISTATE.RETREAT)
            ThisAgent.SetDestination(Oponent.position);

        //Keep track of moving target
        Vector3 dir = Oponent.position - transform.position;
        Quaternion lookRot = Quaternion.LookRotation(dir);
        lookRot.x = 0; lookRot.z = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Mathf.Clamp01(3.0f * Time.maximumDeltaTime));
    }

        //Gives fighter subtle movement (prevents them staying in one position)
        //Results in slide movement (minimal animation)
    private void subtleMovement()
    {
        float distance = Vector3.Distance(transform.position, Oponent.position);
        Vector3 dirToOponent = Vector3.zero;

            //If within attack range (1) move around oponent 
        if (distance <= attackRange)
        {
            anim.SetBool("move", true);
                //50/50 chance to move left or right
            if (Random.Range(0, 2) == 1)
                dirToOponent = transform.position + Oponent.position;
            else
                dirToOponent = transform.position - Oponent.position;

            Vector3 newPos = transform.position + dirToOponent;

            ThisAgent.SetDestination(newPos);
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

    //Defines behaviour for CHASE state
    private IEnumerator StateChase()
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

        //Calc random point on navMesh
        //Used by ESCAPE state
        //source: https://answers.unity.com/questions/475066/how-to-get-a-random-point-on-navmesh.html
    private Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 finalPosition = Vector3.zero;
        bool posFound = false;
        while (posFound == false)
        {
                //Set random point
            Vector3 randomDirection = Random.insideUnitSphere * radius;
                //Offset by object position
            randomDirection += transform.position * 2;
            NavMeshHit hit;
            NavMeshHit ropeHit;
                //When point is on navMesh
            if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
            {
                    //Find nearest navMesh border
                if (NavMesh.FindClosestEdge(finalPosition, out ropeHit, NavMesh.AllAreas))
                {
                    float DistancetoDest = Vector3.Distance(ropeHit.position, randomDirection);

                    //If random pos is far enough away from border Then position is valid
                    if (DistancetoDest > ThisAgent.stoppingDistance)
                    {
                        posFound = true;
                        finalPosition = hit.position;
                    }
                }
            }
        }
        return finalPosition;
    }

    //Defines behaviour for CHESCAPEASE state
    private IEnumerator StateEscape()
    {
        Vector3 dest = RandomNavmeshLocation(5f);
        while (CurrentState == AISTATE.ESCAPE)
        {         
                //Set escape point
            ThisAgent.SetDestination(dest);
            Debug.DrawRay(dest, Vector3.up, Color.green);

            float DistancetoDest = Vector3.Distance(ThisTransform.position, dest);

            //Check distance
            if (Mathf.Approximately(DistancetoDest, ThisAgent.stoppingDistance))
            {
                //Change state due to met condition (within range)
                if (Random.Range(0, 2) == 1)
                    CurrentState = _CurrentState = AISTATE.ATTACKHIGH;
                else
                    CurrentState = _CurrentState = AISTATE.ATTACKLOW;


                yield break;
            }
                //wait for 2seconds then try again
            yield return new WaitForSeconds(2);
            dest = RandomNavmeshLocation(5f);
        }
    }

    //Defines behaviour for ATTACKHIGH state
    private IEnumerator StateAttackHigh()
    {
        while (CurrentState == AISTATE.ATTACKHIGH)
        {
                //Choose attack type (one of three)
            anim.SetFloat("AttackType", Random.Range(0.0f, 1.0f));
            anim.SetBool("attackHigh", true);

            //Wait for animation to finish until applying stamina penalty
            yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

                //Reduce stamina
            FHealthScript.Stamina -= FHealthScript.highAttackCost;

                //Enter rocovery if found true
            if (ResilienceCheck() || FHealthScript.Stamina < FHealthScript.highAttackCost || FHealthScript.Stamina <= Random.Range(0, FHealthScript.FitnessLevel))
            {
                anim.SetBool("attackHigh", false);
                BlockOrRetreat();
            }
            yield return null;
        }
    }

        //Returns true if stamina/health is less than a random range based on Resilience value
        //other returns false
    private bool ResilienceCheck()
    {
        if (FHealthScript.Stamina >= FHealthScript.Stamina * Random.Range(0.1f, FHealthScript.Resilience) ||
            FHealthScript.HealthPoints >= FHealthScript.HealthPoints * Random.Range(0.1f, FHealthScript.Resilience))
        {
            return true;
        }

        return false;
    }

        //Defines which recovery state to enter
        //Dependant on BlockChance variable (higher = more likey to block)
    private void BlockOrRetreat()
    {
        //chance to block
        if (Random.Range(1, 101) < FHealthScript.BlockChance)
            CurrentState = _CurrentState = AISTATE.BLOCK;
        else
            CurrentState = _CurrentState = AISTATE.RETREAT;

    }

        //Defines behaviour for ATTACKLOW state
    private IEnumerator StateAttackLow()
    {
        while (CurrentState == AISTATE.ATTACKLOW)
        {
            //Choose attack type (one of three)
            anim.SetFloat("AttackType", Random.Range(0.0f, 1.0f));
            anim.SetBool("attackLow", true);
            
            //Wait for animation to finish until applying stamina penalty
            yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

                //Reduce stamina
            FHealthScript.Stamina -= FHealthScript.lowAttackCost;

                //Enter rocovery if found true
            if (ResilienceCheck() || FHealthScript.Stamina < FHealthScript.lowAttackCost || FHealthScript.Stamina <= Random.Range(0, FHealthScript.FitnessLevel))
            {
                anim.SetBool("attackLow", false);
                BlockOrRetreat();
            }
            yield return null;
        }
    }

        //Defines behaviour for BLOCK state
    private IEnumerator StateBlock()
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

                //Start recovery
            StartCoroutine(FHealthScript.Recover());

            yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

                //Keep action until resilience has replenished
            if (ResilienceCheck() == false)
            {
                ActionAfterRecovery();
                anim.SetBool("block", false);
            }
        }
    }

        //Defines behaviour for CHASE state
    private IEnumerator StateDodge()
    {
        while (CurrentState == AISTATE.DODGE)
        {
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

            anim.SetBool("dodge", false);
            ActionAfterRecovery();
        }
    }

        //Defines behaviour for CHASE state
    private IEnumerator StateRetreat()
    {
        while (CurrentState == AISTATE.RETREAT)
        {
            anim.SetBool("move", true);

            //Start recovery
            StartCoroutine(FHealthScript.Recover());

            //Wait for animation to finish until applying stamina penalty
            yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
                
                //if recovered enough continue fighting
            if (ResilienceCheck() == false)
            {
                ActionAfterRecovery();
                anim.SetBool("move", false);
            }
        }
    }

        //Allows fighter to continue fighting after recovering
    private void ActionAfterRecovery()
    {
        //Use random range to prevent AI from attacking with minimal stamina
        //Check distance
        if(checkDistance())
            CurrentState = _CurrentState = AISTATE.CHASE;
        else if (ResilienceCheck() || FHealthScript.Stamina >= (FHealthScript.highAttackCost + Random.Range(0, (int)FHealthScript.FitnessLevel)))
            CurrentState = _CurrentState = AISTATE.ATTACKHIGH;
        else if (ResilienceCheck() || FHealthScript.Stamina >= (FHealthScript.lowAttackCost + Random.Range(0, (int)FHealthScript.FitnessLevel)))
            CurrentState = _CurrentState = AISTATE.ATTACKLOW;
    }

        //Defines behaviour for CHASE state
    private IEnumerator StateDefeat()
    {
        victory.SetActive(false);
        anim.StopPlayback();//Check punch direction
        if (myRSensor.leftHit)
            anim.SetFloat("DefeatType", 1); //Left
        else if (myRSensor.centreHit)
            anim.SetFloat("DefeatType", 0.5f); //centre
        else if (myRSensor.rightHit)
            anim.SetFloat("DefeatType", 0); //Right

        anim.Play("Defeat");
        //anim.SetBool("Defeat", true);
        
        ThisAgent.enabled = false;
            yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
    }

        //Defines behaviour for CHASE state
    private IEnumerator StateVictory()
    {
        anim.StopPlayback();
        victory.SetActive(true);
        anim.Play("Victory");
        //anim.SetBool("Victory", true);

        ThisAgent.enabled = false;

        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
    }

        //Reset animation Parameters & CurrentState & position
    public void ResetAI()
    {
        anim.SetBool("Defeat", false);
        anim.SetBool("Victory", false);
        anim.SetBool("block", false);
        anim.SetBool("attackHigh", false);
        anim.SetBool("attackLow", false);

        anim.Play("Boxing_Idle");
        this.transform.position = initialPos;

        CurrentState = _CurrentState = AISTATE.CHASE;
    }

        //Turn off navMeshAgent script
    public void disableAgent()
    {
        ThisAgent.enabled = false;
    }

        //Turn on navMeshAgent script
    public void enableAgent()
    {
        ThisAgent.enabled = true;
    }
}
