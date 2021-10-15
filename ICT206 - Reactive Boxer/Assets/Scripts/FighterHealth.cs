//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

//Defines & stores all stats for a fighter
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
    public float HealthRecovery { get; set; }

    //Amount stamina is recovered
    public float StaminaRecovery { get; set; }

    //rate of which stamina is recovered (in seconds)
    public float RecoveryRate { get; set; }

    //chance to block
    public float BlockChance { get; set; }
    
    //Max stamina to remain before stopping attack
    public float FitnessLevel { get; set; }

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
        BlockChance = 50;

        RecoveryRate = 2;
        StaminaRecovery = 10;
        HealthRecovery = 5;
        
        _MaxStaminaDefault = _MaxStamina;
        _MaxHealthDefault = _HealthPoints;
    }

        //Controls how the fighter recovers health/stamina
    public IEnumerator Recover()
    {
        do
        {
            //Always recover stamina
            Stamina += StaminaRecovery;

            //chance to recover health
            if (Random.Range(0, healthRecoveryChance) == 1)
                HealthPoints += HealthRecovery;

            yield return new WaitForSeconds(RecoveryRate);
        } while (Stamina <= Random.Range(Stamina, _MaxStaminaDefault * Resilience) || HealthPoints <= Random.Range(HealthPoints, _MaxHealthDefault * Resilience));
    }

    public float getMaxStamina()
    {
        return _MaxStaminaDefault;
    }

    public float getMaxHealth()
    {
        return _MaxHealthDefault;
    }

        //Used on restart & main menu buttons in GameOver
        //These stats are the only ones changed
    public void ResetStats()
    {
        HealthPoints = _MaxHealthDefault;
        Stamina = _MaxStaminaDefault;
    }

}

