//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class FighterHealth : MonoBehaviour
{
    public UnityEvent OnHealthChanged;
    public UnityEvent OnStaminaChanged;

    [SerializeField]
    private float _HealthPoints = 100f;
    private float _MaxHealthDefault;
    private float healthRecoveryChance = 4;

    [SerializeField]
    private float _MaxStamina = 100f;
    private float _MaxStaminaDefault;

    [SerializeField]
    private float _MaxStrength = 10f;

    public float highAttackCost = 15;
    public float lowAttackCost = 5;

    //Max stamina to remain before stopping attack
    public int fitnessLevel = 50;
    //used to determine block or retreat on low stamina
    public int reactionLevel = 50;
    //chance to block (linked to reationLevel)
    public int nonBlockingChance = 20;

    //rate of which stamina is recovered (in seconds)
    public int recoveryRate = 2;
    //Amount stamina is recovered
    public int staminaRecovery = 10;
    //Amount health is recovered
    public int healthRecovery = 5;

    private Animator anim;
    private void OnValidate()
    {
            //Min Max health information
        _HealthPoints = Mathf.Clamp(_HealthPoints, 50, 1000);
        healthRecoveryChance = Mathf.Clamp(healthRecoveryChance, 1, 10);
        healthRecovery = Mathf.Clamp(healthRecovery, 50, 1000);
        
            //Min Max stamina information
        _MaxStamina = Mathf.Clamp(_MaxStamina, 50, 1000);
        recoveryRate = Mathf.Clamp(recoveryRate, 50, 1000);
        staminaRecovery = Mathf.Clamp(staminaRecovery, 50, 1000);

            //Min Max strength information
        _MaxStrength = Mathf.Clamp(_MaxStrength, 1, 10);

        nonBlockingChance = Mathf.Clamp(nonBlockingChance, 0, 100);
        reactionLevel = Mathf.Clamp(reactionLevel, 0, 100);
        fitnessLevel = Mathf.Clamp(fitnessLevel, 0, 100);

    }

    //Only recoveres when fighter is retreating (maybe blocking)
    public float HealthPoints
    {
        get { return _HealthPoints; }
        set
        {
            _HealthPoints = value;
            OnHealthChanged.Invoke();
            //Prevent health exceding maximum
            if (_HealthPoints > _MaxHealthDefault)
                _HealthPoints = _MaxHealthDefault;

            //Handled in AI
            if (_HealthPoints <= 0)
            {
                //StopAllCoroutines();
                StartCoroutine(StateDefeat());
            }
        }
    }
    public float Stamina
    {
        get { return _MaxStamina; }
        set
        {
            _MaxStamina = value;
            OnStaminaChanged.Invoke();
            if (_MaxStamina <= 0) 
                StartCoroutine(Recover());

            if (_MaxStamina > _MaxStaminaDefault)
                _MaxStamina = _MaxStaminaDefault;
        }
    }

    public float Strength
    {
        get { return _MaxStrength; }
        set
        {
            _MaxStrength = value;
        }
    }
    private void Start()
    {
        _MaxStaminaDefault = _MaxStamina;
        _MaxHealthDefault = _HealthPoints;
        anim = GetComponent<Animator>();
    }
    
    private IEnumerator StateDefeat()
    {
        anim.SetBool("Defeat", true);
        //anim.SetFloat("DefeatType", 0);
        Debug.Log("def.");
        GetComponent<NavMeshAgent>().enabled = false;

        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
    }

    public IEnumerator Victory()
    {
        //Debug.Log("Vic");
        anim.Play("Victory");
        //anim.SetBool("Victory", true);

        GetComponent<NavMeshAgent>().enabled = false;

        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
    }

    public IEnumerator Recover()
    {
        Stamina += staminaRecovery;

        //chance to recover health
        if (Random.Range(1, healthRecoveryChance) == 1)
            HealthPoints += healthRecovery;

        yield return new WaitForSeconds(recoveryRate);
    }


    void Update()
    {
        HealthCheck();
        StaminaCheck();
    }

    void HealthCheck()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HealthPoints = 0;
        }
    }

    void StaminaCheck()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Stamina = 0;
        }
    }

    public void increaseStamina(float newStam)
    {
            Stamina += newStam;
    }

}

