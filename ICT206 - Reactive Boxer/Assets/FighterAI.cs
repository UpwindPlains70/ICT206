using System.Collections;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FighterAI : MonoBehaviour
{
    public enum AISTATE { CHASE = 0, ATTACKLOW = 1, ATTACKHIGH = 2, BLOCK = 3, DODGEHIGH = 4, DODGELOW = 5, RETREAT = 6 };
    public AISTATE CurrentState
    {
        get { return _CurrentState; }
        set
        {
            StopAllCoroutines();
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
            }
        }
    }
    [SerializeField]
    private AISTATE _CurrentState = AISTATE.CHASE;
    private NavMeshAgent ThisAgent = null;
    [SerializeField]
    private Transform Oponent;
    private Transform ThisTransform = null;
    //public ParticleSystem WeaponPS = null;
    private void Awake()
    {
        ThisAgent = GetComponent<NavMeshAgent>();
        //ThisPlayer = GameObject.FindWithTag("Player").
        GetComponent<Transform>();
        ThisTransform = GetComponent<Transform>();
    }
    private void Start()
    {
        CurrentState = _CurrentState;
    }
    private void OnEnable()
    {
        CurrentState = AISTATE.CHASE;
        Debug.Log("enabled");
    }
    private void OnDisable()
    {
        StopAllCoroutines();
        Debug.Log("disabled");
    }

    public void FixedUpdate()
    {
            //Keep track of moving target
        ThisAgent.SetDestination(Oponent.position);
        Vector3 dir = Oponent.position - transform.position;
        Quaternion lookRot = Quaternion.LookRotation(dir);
        lookRot.x = 0; lookRot.z = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Mathf.Clamp01(3.0f * Time.maximumDeltaTime));
    }
    public IEnumerator StateChase()
    {
        Debug.Log("chase");
        while (CurrentState == AISTATE.CHASE)
        {
            //Check distance
            float DistancetoDest = Vector3.Distance(ThisTransform.position,
            Oponent.position);
            if (Mathf.Approximately(DistancetoDest, ThisAgent.stoppingDistance) ||
            DistancetoDest <= ThisAgent.stoppingDistance)
            {
                //CurrentState = AISTATE.ATTACK;
                yield break;
            }
            yield return null;
        }
    }
    public IEnumerator StateAttackHigh()
    {
        while (CurrentState == AISTATE.ATTACKHIGH)
        {
            Vector3 Dir = (Oponent.position - ThisTransform.position).
            normalized;
            Dir.y = 0;
            ThisTransform.rotation = Quaternion.LookRotation(Dir, Vector3.up);
            yield return null;
        }
    }
    public IEnumerator StateAttackLow()
    {
        while (CurrentState == AISTATE.ATTACKLOW)
        {
            Vector3 Dir = (Oponent.position - ThisTransform.position).
            normalized;
            Dir.y = 0;
            ThisTransform.rotation = Quaternion.LookRotation(Dir, Vector3.up);
            yield return null;
        }
    }

    public IEnumerator StateBlock()
    {
        while (CurrentState == AISTATE.BLOCK)
        {
            Vector3 Dir = (Oponent.position - ThisTransform.position).
            normalized;
            Dir.y = 0;
            ThisTransform.rotation = Quaternion.LookRotation(Dir, Vector3.up);
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
            Vector3 Dir = (Oponent.position - ThisTransform.position).
            normalized;
            Dir.y = 0;
            ThisTransform.rotation = Quaternion.LookRotation(Dir, Vector3.up);
            yield return null;
        }
    }
}
