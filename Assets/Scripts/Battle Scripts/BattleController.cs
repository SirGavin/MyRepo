using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class BattleController : MonoBehaviour {

    public Text results;

    public MapController mapController;
    public float baseDamage = 0.7f;
    public float damageRange = 0.15f;
    public float baseHealth = 10f;
    public int combatRounds = 10;
    public Army attacker;
    public Army defender;

    public StrategyPanel attackerStratPanel;
    public StrategyPanel defenderStratPanel;

    private ArmyMap attackerMap;
    private ArmyMap defenderMap;
    private Strategy attackerStrat;
    private Strategy defenderStrat;

    private MatchUpData matchUps;
    private string matchDataFileName = "strategies.json";

    private void Awake() {
        LoadMatchData();
    }

    private void LoadMatchData() {
        string filePath = Path.Combine(Application.streamingAssetsPath, matchDataFileName);

        if (File.Exists(filePath)) {
            string dataAsJson = File.ReadAllText(filePath);
            matchUps = JsonUtility.FromJson<MatchUpData>(dataAsJson);
        } else {
            throw new Exception("Match up data not configured!");
        }
    }

    public void FightV2() {
        if (attackerStratPanel.GetSelected() != null && defenderStratPanel.GetSelected() != null) {
            float attackerArmySize = attackerMap.armySize;
            float defenderArmySize = defenderMap.armySize;
            float attackerDamage = 0f;
            float defenderDamage = 0f;

            MatchUpStats matchUp = matchUps.GetMatchUp(attackerStratPanel.GetSelected(), defenderStratPanel.GetSelected());

            for (int i = 1; i <= combatRounds; i++) {
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

            // % of enemy army killed
            float attackerPerformance = 1 - defenderArmySize / defenderMap.armySize;
            float defenderPerformance = 1 - attackerArmySize / attackerMap.armySize;
            Debug.Log("attackerPerformance: " + attackerPerformance);
            Debug.Log("defenderPerformance: " + defenderPerformance);

            attackerMap.UpdateArmySize((int)Mathf.Ceil(attackerArmySize));
            defenderMap.UpdateArmySize((int)Mathf.Ceil(defenderArmySize));

            gameObject.SetActive(false);
            mapController.UpdateArmies(attackerMap, defenderMap, attackerPerformance > defenderPerformance);
        }
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

    public void StartBattle(ArmyMap attacker, ArmyMap defender) {
        gameObject.SetActive(true);

        attackerMap = attacker;
        defenderMap = defender;

        GenerateStrategies(attacker, attackerStratPanel);
        GenerateStrategies(defender, defenderStratPanel);
    }

    private void GenerateStrategies(ArmyMap army, StrategyPanel stratPane) {
        stratPane.Clear();
        foreach (Strategy strat in army.strategies) {
            stratPane.AddStrategy(strat);
        }
    }

    private float GetDamage(float damageModifier, float armySize)
    {
        return UnityEngine.Random.Range(baseDamage - damageRange, baseDamage + damageRange) * damageModifier * armySize;
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

