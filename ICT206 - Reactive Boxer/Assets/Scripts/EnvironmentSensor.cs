using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Gives the AI an ESCAPE state
//sensors when they are close to the rope, thus try to escape
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
            //Only check for rope if AI is not in CHASE, RETREAT, VICTORY and DEFEAT
        if(myAI.CurrentState != FighterAI.AISTATE.CHASE && myAI.CurrentState != FighterAI.AISTATE.ESCAPE 
            && myAI.CurrentState != FighterAI.AISTATE.VICTORY && myAI.CurrentState != FighterAI.AISTATE.DEFEAT)
            nearestRope();
    }

    //Find closest edge of navMesh & if in range of fighter trigger ESCAPE state
    //Source: https://docs.unity3d.com/ScriptReference/AI.NavMesh.FindClosestEdge.html
    void nearestRope()
    {
        NavMeshHit hit;
            //Find closest navMesh edge
        if (NavMesh.FindClosestEdge(transform.position, out hit, NavMesh.AllAreas))
        {
            float DistancetoDest = Vector3.Distance(hit.position, transform.position);
                //Check distance between edge and stopDistance
            if (DistancetoDest <= myAgent.stoppingDistance / 2f)
                myAI.CurrentState = FighterAI.AISTATE.ESCAPE;

                //Debug line showing nearest edge (only in scene view)
            Debug.DrawRay(hit.position, Vector3.up, Color.red);
        }
    }
}
