using System;
using UnityEngine;

[Serializable]
public class MatchUpStats : ScriptableObject {

    public int attackerStratId = 1;
    public int defenderStratId = 1;
    public float attackerDmgModifier = 1;
    public float defenderDmgModifier = 1;
}
