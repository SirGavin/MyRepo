using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Army))]
public class ArmyEditor : EditorWithSubEditors<StrategyEditor, StrategySO>
{
    private Army army;
    private SerializedProperty stratImgsProperty;
    private SerializedProperty stratsProperty;

    private const float collectionButtonWidth = 125f;
    private const string strategyPropStratImgsName = "stratImgs";
    private const string strategyPropStrategiesName = "strategies";

    private void OnEnable()
    {
        army = (Army)target;

        stratImgsProperty = serializedObject.FindProperty(strategyPropStratImgsName);
        stratsProperty = serializedObject.FindProperty(strategyPropStrategiesName);

        CheckAndCreateSubEditors(army.strategies);
    }

    private void OnDisable()
    {
        CleanupEditors();
    }

    protected override void SubEditorSetup(StrategyEditor editor)
    {
        editor.strategies = stratsProperty;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        CheckAndCreateSubEditors(army.strategies);

        for (int i = 0; i < subEditors.Length; i++)
        {
            subEditors[i].OnInspectorGUI();
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Add Strategy", GUILayout.Width(collectionButtonWidth)))
        {
            StrategySO newStrategy = StrategyEditor.CreateStrategy(stratsProperty);
            stratsProperty.AddToObjectArray(newStrategy);
        }

        if (GUILayout.Button("Update Match Ups", GUILayout.Width(collectionButtonWidth)))
        {
            for (int i = 0; i < army.strategies.Length; i++)
            {
                StrategySO strategy = army.strategies[i];

                for (int j = strategy.matchUps.Length; j < army.strategies.Length; j++)
                {
                    MatchUpStats newMatchUp = MatchUpStatsEditor.CreateMatchUp(strategy, army.strategies[j]);
                    subEditors[i].AddMatchUp(newMatchUp);
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}
