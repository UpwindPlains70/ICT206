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
    private int healthRecoveryChance = 4;

    [SerializeField]
    private float _MaxStamina = 100f;
    private float _MaxStaminaDefault;

    [SerializeField]
    private float _MaxStrength = 10f;

    public float highAttackCost = 15;
    public float lowAttackCost = 5;

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

    //Percentage that can be lost/used before requiring a change of action
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
        while (Stamina <= Random.Range(Stamina, _MaxStaminaDefault * Resilience) || HealthPoints <= Random.Range(HealthPoints, _MaxHealthDefault * Resilience))
        {
            Stamina += StaminaRecovery;

            //chance to recover health
            if (Random.Range(0, healthRecoveryChance) == 1)
                HealthPoints += HealthRecovery;

            yield return new WaitForSeconds(RecoveryRate);
        }
    }

    void Update()
    {
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

    public float getMaxStamina()
    {
        return _MaxStaminaDefault;
    }

    public float getMaxHealth()
    {
        return _MaxHealthDefault;
    }

    public void ResetStats()
    {
        HealthPoints = _MaxHealthDefault;
        Stamina = _MaxStaminaDefault;
    }

}

