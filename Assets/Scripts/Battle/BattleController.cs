using System;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleController : MonoBehaviour {
    //TODO: review battle code?

    [Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public GameObject armyPanePrefab;
    public VerticalLayoutGroup layoutGroup;

    public float baseDamage = 0.7f;
    public float damageRange = 0.15f;
    public float baseHealth = 10f;
    public int combatRounds = 10;

    private Army attacker;
    private Army defender;
    private Strategy attackerStrat;
    private Strategy defenderStrat;
    private BoolEvent resolveBattle = new BoolEvent();

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

    public void StartBattle(Player attackingPlayer, Army attacker, Player defendingPlayer, Army defender, UnityAction<Boolean> resolveBattle) {
        gameObject.SetActive(true);
        this.resolveBattle.AddListener(resolveBattle);

        this.attacker = attacker;
        this.defender = defender;

        //Remove old strategy panels
        for (int i = 0; i < transform.childCount - 1; i++) {
            Destroy(transform.GetChild(i).gameObject);
        }

        if (defendingPlayer is AIPlayer) {
            defenderStrat = ((AIPlayer)defendingPlayer).GetRandomStrategy();
        } else {
            GenerateStrategiesUI(defender, delegate (Strategy strategy) {
                defenderStrat = strategy;
            });
        }

        if (attackingPlayer is AIPlayer) {
            attackerStrat = ((AIPlayer)attackingPlayer).GetRandomStrategy();
        } else {
            GenerateStrategiesUI(attacker, delegate (Strategy strategy) {
                attackerStrat = strategy;
            });
        }

        if (attackerStrat != null && defenderStrat != null) {
            Fight();
        }
    }

    public void Fight() {
        if (attackerStrat != null && defenderStrat != null) {
            float attackerArmySize = attacker.armySize;
            float defenderArmySize = defender.armySize;
            float attackerDamage = 0f;
            float defenderDamage = 0f;

            MatchUpStats matchUp = matchUps.GetMatchUp(attackerStrat, defenderStrat);

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

            // % of enemy army killed
            float attackerPerformance = 1 - defenderArmySize / defender.armySize;
            float defenderPerformance = 1 - attackerArmySize / attacker.armySize;
            Debug.Log("attackerPerformance: " + attackerPerformance);
            Debug.Log("defenderPerformance: " + defenderPerformance);

            attacker.SetArmySize((int)Mathf.Ceil(attackerArmySize));
            defender.SetArmySize((int)Mathf.Ceil(defenderArmySize));

            gameObject.SetActive(false);
            Debug.Log("attackerPerformance > defenderPerformance: " + (attackerPerformance > defenderPerformance));

            resolveBattle.Invoke(attackerPerformance > defenderPerformance);
            resolveBattle.RemoveAllListeners();
        }
    }

    private void GenerateStrategiesUI(Army army, UnityAction<Strategy> setStrat) {
        GameObject armyObj = Instantiate(armyPanePrefab);
        armyObj.transform.SetParent(layoutGroup.transform);
        armyObj.transform.SetAsFirstSibling();

        StrategyPanel stratPane = armyObj.GetComponentInChildren<StrategyPanel>();
        stratPane.GenerateUI(army, setStrat);
    }

    private void SetStrategy(ref Strategy stratToSet, Strategy strat) {
        stratToSet = strat;
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

