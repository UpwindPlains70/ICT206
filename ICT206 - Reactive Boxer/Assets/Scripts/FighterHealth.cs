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
        //Could be an editable value
    private float healthRecoveryChance = 4;

    [SerializeField]
    private float _MaxStamina = 100f;
    private float _MaxStaminaDefault;

    [SerializeField]
    private float _MaxStrength = 10f;

    public float highAttackCost = 15;
    public float lowAttackCost = 5;

    //Max stamina to remain before stopping attack
    //public int fitnessLevel = 50;
    //used to determine block or retreat on low stamina
    //public int reactionLevel = 50;
    //chance to block (linked to reationLevel)
    //public int nonBlockingChance = 20;

    //rate of which stamina is recovered (in seconds)
    //public int recoveryRate = 2;
    //public int staminaRecovery = 10;

    //public int healthRecovery = 5;

    //public float maxResilience = 0.40f;

   /* private void OnValidate()
    {
            //Min Max health information
        _HealthPoints = Mathf.Clamp(_HealthPoints, 10, 1000);
        healthRecoveryChance = Mathf.Clamp(healthRecoveryChance, 1, 10);
        healthRecovery = Mathf.Clamp(healthRecovery, 50, 1000);
        
            //Min Max stamina information
        _MaxStamina = Mathf.Clamp(_MaxStamina, 50, 1000);
        recoveryRate = Mathf.Clamp(recoveryRate, 1, 10);
        staminaRecovery = Mathf.Clamp(staminaRecovery, 50, 1000);

            //Min Max strength information
        _MaxStrength = Mathf.Clamp(_MaxStrength, 1, 10);

        nonBlockingChance = Mathf.Clamp(nonBlockingChance, 0, 100);
        reactionLevel = Mathf.Clamp(reactionLevel, 0, 100);
        fitnessLevel = Mathf.Clamp(fitnessLevel, 0, 100);

    }*/

    //Amount health is recovered
    public int HealthRecovery { get; set; }

    //Amount stamina is recovered
    public int StaminaRecovery { get; set; }

    //rate of which stamina is recovered (in seconds)
    public int RecoveryRate { get; set; }

    //chance to block (linked to reationLevel)
    public int BlockChance { get; set; }

    //used to determine block or retreat on low stamina
    public int ReactionLevel { get; set; }
    
    //Max stamina to remain before stopping attack
    public int FitnessLevel { get; set; }

    //Percentage that con be lost/used before requiring a change of action
    public float Resilience
    {
        get; set;
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
            /*if (_HealthPoints <= 0)
            {
                //StopAllCoroutines();
                //StartCoroutine(myAI.StateDefeat());
            }*/
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

    private void Awake()
    {
        Resilience = 0.4f;
        FitnessLevel = 50;
        ReactionLevel = 50;
        BlockChance = 50;

        RecoveryRate = 2;
        StaminaRecovery = 10;
        HealthRecovery = 5;
        
        _MaxStaminaDefault = _MaxStamina;
        _MaxHealthDefault = _HealthPoints;
    }

    public IEnumerator Recover()
    {
        Stamina += StaminaRecovery;

        //chance to recover health
        if (Random.Range(1, healthRecoveryChance) == 1)
            HealthPoints += HealthRecovery;

        yield return new WaitForSeconds(RecoveryRate);
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

