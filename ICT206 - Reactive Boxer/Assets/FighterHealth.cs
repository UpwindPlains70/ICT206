using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class FighterHealth : MonoBehaviour
{
    public UnityEvent OnHealthChanged;
    public UnityEvent OnStaminaChanged;

    [SerializeField]
    private float _HealthPoints = 100f;
    
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
    public int blockChance = 20;

    //rate of which stamina is recovered (in seconds)
    public int recoveryRate = 2;
    //Amount stamina is recovered
    public int staminaRecovery = 10;

    private void OnValidate()
    {
        _HealthPoints = Mathf.Clamp(_HealthPoints, 50, 1000);
        _MaxStamina = Mathf.Clamp(_MaxStamina, 50, 1000);
        _MaxStrength = Mathf.Clamp(_MaxStrength, 1, 10);
    }

    public float HealthPoints
    {
        get { return _HealthPoints; }
        set
        {
            _HealthPoints = value;
            OnHealthChanged.Invoke();
            if (_HealthPoints <= 0f) Die();
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
                StartCoroutine(recoverStamina());

            if (_MaxStamina > _MaxStaminaDefault)
                _MaxStamina = _MaxStaminaDefault;
        }
    }
    private void Start()
    {
        _MaxStaminaDefault = _MaxStamina;
    }

    private void Die()
    {        
        Debug.Log("dead");
    }
    private void Retreat()
    {        
        Debug.Log("out of stamina");
    }
    public IEnumerator recoverStamina()
    {
        Stamina += staminaRecovery;

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

        if (Input.GetKeyDown(KeyCode.P))
            increaseStamina(10);
    }

    public void increaseStamina(float newStam)
    {
            Stamina += newStam;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts[0].thisCollider.CompareTag("Head"))
            highHit();
        else if (collision.contacts[0].thisCollider.CompareTag("Torso"))
            lowHit();
    }

    private void highHit()
    {
        Debug.Log("High Hit");
    }

    private void lowHit()
    {
        Debug.Log("Low Hit");
    }
}

