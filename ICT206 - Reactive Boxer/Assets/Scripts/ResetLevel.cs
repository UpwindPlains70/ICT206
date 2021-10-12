using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetLevel : MonoBehaviour
{
    public GameObject fighterA;
    public GameObject fighterB;

    class Fighter
    {
        public GameObject fighterObj { get; set; }

        public Fighter()
        {
            AI = fighterObj.GetComponent<FighterAI>();
            Stats = fighterObj.GetComponent<FighterHealth>();
        }

        public FighterAI AI { get; set; }
        public FighterHealth Stats { get; set; }
    }

    [SerializeField]
    private List<Fighter> fighters = new List<Fighter>();

    private FighterAI fighterA_AI;
    private FighterHealth fighterAStats;

    private FighterAI fighterB_AI;
    private FighterHealth fighterBStats;

    private Transform fighterATrans;
    private Transform fighterBTrans;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void ResetFighter()
    {
        foreach(Fighter f in fighters)
        {
            f.AI.ResetAI();
            f.Stats.ResetStats();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
