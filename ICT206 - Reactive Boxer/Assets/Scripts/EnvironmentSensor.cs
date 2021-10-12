using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnvironmentSensor : MonoBehaviour
{
    private NavMeshAgent myAgent;
    private FighterAI myAI;

    // Start is called before the first frame update
    void Start()
    {
        myAgent = GetComponent<NavMeshAgent>();
        myAI = GetComponent<FighterAI>();
    }

    // Update is called once per frame
    void Update()
    {
        if(myAI.CurrentState != FighterAI.AISTATE.CHASE && myAI.CurrentState != FighterAI.AISTATE.ESCAPE 
            && myAI.CurrentState != FighterAI.AISTATE.VICTORY && myAI.CurrentState != FighterAI.AISTATE.DEFEAT)
            nearestRope();
    }

    //Source: https://docs.unity3d.com/ScriptReference/AI.NavMesh.FindClosestEdge.html
    void nearestRope()
    {
        NavMeshHit hit;
        if (NavMesh.FindClosestEdge(transform.position, out hit, NavMesh.AllAreas))
        {
            float DistancetoDest = Vector3.Distance(hit.position, transform.position);

            if (DistancetoDest <= myAgent.stoppingDistance / 2f)
                myAI.CurrentState = FighterAI.AISTATE.ESCAPE;

            Debug.DrawRay(hit.position, Vector3.up, Color.red);
        }
    }
}
