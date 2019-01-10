using UnityEngine;
using UnityEngine.UI;

public class BattleController : MonoBehaviour {

    public Text results;

    public float baseDamage = 0.7f;
    public float damageRange = 0.15f;
    public float baseHealth = 10f;
    public int combatRounds = 10;
    public Army attacker;
    public Army defender;

    public void Awake() {
        gameObject.SetActive(false);
    }

    public void Fight()
    {
        float attackerArmySize = attacker.armySize;
        float defenderArmySize = defender.armySize;
        float attackerDamage = 0f;
        float defenderDamage = 0f;

        MatchUpStats matchUp = attacker.selectedStrategy.GetMatchUp(defender.selectedStrategy.id);

        for (int i = 1; i <= combatRounds; i++)
        {
            attackerDamage += GetDamage(matchUp.attackerDmgModifier, attackerArmySize);
            defenderDamage += GetDamage(matchUp.defenderDmgModifier, defenderArmySize);

            Debug.Log("attackerDamage1: " + attackerDamage);
            Debug.Log("defenderDamage1: " + defenderDamage);

            attackerArmySize -= GetDead(ref defenderDamage);
            defenderArmySize -= GetDead(ref attackerDamage);

            Debug.Log("attackerDamage2: " + attackerDamage);
            Debug.Log("defenderDamage2: " + defenderDamage);
        }

        results.text = "Results: \n" +
            "\tAttacker: " + attackerArmySize + "\n" +
            "\tDefender: " + defenderArmySize;
    }

    private float GetDamage(float damageModifier, float armySize)
    {
        return Random.Range(baseDamage - damageRange, baseDamage + damageRange) * damageModifier * armySize;
    }

    private int GetDead(ref float damage)
    {
        int dead = (int)(damage / baseHealth);
        Debug.Log("dead: " + dead);
        damage = damage % baseHealth;
        Debug.Log("damage: " + damage);
        return dead;
    }
}
