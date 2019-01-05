using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MatchUpStats))]
public class MatchUpStatsEditor : Editor
{
    //MatchUpStats Fields
    //public int attackerStratId = 1;
    //public int defenderStratId = 1;
    //public int attackerDmgModifier = 1;
    //public int defenderDmgModifier = 1;

    public SerializedProperty strategies;

    private MatchUpStats matchUpStats;
    private SerializedProperty attackerStratIdProp;
    private SerializedProperty defenderStratIdProp;
    private SerializedProperty attackDmgModifierProp;
    private SerializedProperty defenderDmgModifierProp;

    private const string attackerStratIdPropName = "attackerStratId";
    private const string defenderStratIdPropName = "defenderStratId";
    private const string attackDmgModifierPropName = "attackerDmgModifier";
    private const string defenderDmgModifierPropName = "defenderDmgModifier";
    
    private void OnEnable()
    {
        matchUpStats = (MatchUpStats)target;

        attackerStratIdProp = serializedObject.FindProperty(attackerStratIdPropName);
        defenderStratIdProp = serializedObject.FindProperty(defenderStratIdPropName);
        attackDmgModifierProp = serializedObject.FindProperty(attackDmgModifierPropName);
        defenderDmgModifierProp = serializedObject.FindProperty(defenderDmgModifierPropName);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        string expanderText = "Att Id: " + attackerStratIdProp.intValue.ToString() + "  Def Id: " + defenderStratIdProp.intValue.ToString();
        defenderStratIdProp.isExpanded = EditorGUILayout.Foldout(defenderStratIdProp.isExpanded, expanderText);

        if (defenderStratIdProp.isExpanded)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(attackDmgModifierProp);
            EditorGUILayout.PropertyField(defenderDmgModifierProp);
            if (EditorGUI.EndChangeCheck())
            {
                //UpdateOtherMatch();
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void UpdateOtherMatch()
    {
        for (int i = 0; i < strategies.arraySize; i++)
        {
            SerializedProperty strategy = strategies.GetArrayElementAtIndex(i);
            StrategySO strat = (StrategySO)strategy.objectReferenceValue;
            if (strat.id == defenderStratIdProp.intValue)
            {
                for (int j = 0; j < strat.matchUps.Length; j++)
                {
                    if (strat.matchUps[j].defenderStratId == attackerStratIdProp.intValue)
                    {
                        strat.matchUps[j].attackerDmgModifier = defenderDmgModifierProp.floatValue;
                        strat.matchUps[j].defenderDmgModifier = attackDmgModifierProp.floatValue;
                    }
                }
            }
        }
    }

    public static MatchUpStats CreateMatchUp(StrategySO attacker, StrategySO defender)
    {
        MatchUpStats newMatchUp = CreateInstance<MatchUpStats>();
        newMatchUp.attackerStratId = attacker.id;
        newMatchUp.defenderStratId = defender.id;
        return newMatchUp;
    }
}
