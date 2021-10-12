using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Randomize : MonoBehaviour
{
    public Slider Health;
    public Slider Stamina;
    public Slider Strength;
    public Slider ReactionLevel;
    public Slider BlockChance;
    public Slider RecoveryRate;
    public Slider StaminaRecovery;
    public Slider HealthRecovery;
    public Slider FitnessLevel;
    public Slider Resilience;

    public FighterHealth fighter;

    public Randomize otherRandomizer;

    public void RandomizeFighter()
    {
        Health.value = Random.Range(Health.minValue, Health.maxValue);
        Stamina.value = Random.Range(Stamina.minValue, Stamina.maxValue);
        Strength.value = Random.Range(Strength.minValue, Strength.maxValue);
        ReactionLevel.value = Random.Range(ReactionLevel.minValue, ReactionLevel.maxValue);
        BlockChance.value = Random.Range(BlockChance.minValue, BlockChance.maxValue);
        RecoveryRate.value = Random.Range(RecoveryRate.minValue, RecoveryRate.maxValue);
        StaminaRecovery.value = Random.Range(StaminaRecovery.minValue, StaminaRecovery.maxValue);
        HealthRecovery.value = Random.Range(HealthRecovery.minValue, HealthRecovery.maxValue);
        FitnessLevel.value = Random.Range(FitnessLevel.minValue, FitnessLevel.maxValue);
        Resilience.value = Random.Range(Resilience.minValue, Resilience.maxValue);
    }

    public void Copy_A_to_B()
    {
        Health.value = otherRandomizer.Health.value;
        Stamina.value = otherRandomizer.Stamina.value;
        Strength.value = otherRandomizer.Strength.value;
        ReactionLevel.value = otherRandomizer.ReactionLevel.value;
        BlockChance.value = otherRandomizer.BlockChance.value;
        RecoveryRate.value = otherRandomizer.RecoveryRate.value;
        StaminaRecovery.value = otherRandomizer.StaminaRecovery.value;
        HealthRecovery.value = otherRandomizer.HealthRecovery.value;
        FitnessLevel.value = otherRandomizer.FitnessLevel.value;
        Resilience.value = otherRandomizer.Resilience.value;
    }
}
