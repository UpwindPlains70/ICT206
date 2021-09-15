﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class FighterHealth : MonoBehaviour
{
    public UnityEvent OnHealthChanged;
    public string SpawnPoolTag = string.Empty;
    //private ObjectPool Pool = null;
    private Transform ThisTransform = null; public float HealthPoints
    {
        get { return _HealthPoints; }
        set
        {
            _HealthPoints = value;
            OnHealthChanged.Invoke();
            if (_HealthPoints <= 0f) Die();
        }
    }
    [SerializeField]
    private float _HealthPoints = 100f;
    /*private void Awake()
    {
        ThisTransform = GetComponent<Transform>();
        if (SpawnPoolTag.Length > 0)
            Pool = GameObject.FindWithTag(SpawnPoolTag).
            GetComponent<ObjectPool>();
    }*/
    private void Die()
    {
        /*if (Pool != null)
        {
            Pool.DeSpawn(ThisTransform);
            HealthPoints = 100f;
        }*/
        Debug.Log("dead");
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HealthPoints = 0;
        }
    }
}

