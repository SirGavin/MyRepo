using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StrategySO))]
public class StrategyEditor : EditorWithSubEditors<MatchUpStatsEditor, MatchUpStats>
{
    //StrategySO Fields
    //public int id;
    //public string strategyName;
    //public GameObject gameObject;
    //public MatchUpStats[] matchUps;

    public SerializedProperty strategies;

    private StrategySO strategy;
    private SerializedProperty idProp;
    private SerializedProperty strategyNameProp;
    private SerializedProperty gameObjectProp;
    private SerializedProperty matchUpsProp;

    private const string idPropName = "id";
    private const string strategyNamePropName = "strategyName";
    private const string gameObjectPropName = "gameObject";
    private const string matchUpsPropName = "matchUps";
    
    private const float collectionButtonWidth = 125f;

    private void OnEnable()
    {
        strategy = (StrategySO)target;

        if (target == null)
        {
            DestroyImmediate(this);
            return;
        }

        idProp = serializedObject.FindProperty(idPropName);
        strategyNameProp = serializedObject.FindProperty(strategyNamePropName);
        gameObjectProp = serializedObject.FindProperty(gameObjectPropName);
        matchUpsProp = serializedObject.FindProperty(matchUpsPropName);

        CheckAndCreateSubEditors(strategy.matchUps);
    }

    private void OnDisable()
    {
        CleanupEditors();
    }

    protected override void SubEditorSetup(MatchUpStatsEditor editor)
    {
        editor.strategies = strategies;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        CheckAndCreateSubEditors(strategy.matchUps);

        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUI.indentLevel++;

        EditorGUILayout.BeginHorizontal();

        strategyNameProp.isExpanded = EditorGUILayout.Foldout(strategyNameProp.isExpanded, strategyNameProp.stringValue);

        if (GUILayout.Button("Remove Collection", GUILayout.Width(collectionButtonWidth)))
        {
            strategies.RemoveFromObjectArray(strategy);
        }

        EditorGUILayout.EndHorizontal();

        if (strategyNameProp.isExpanded)
        {
            ExpandedGUI();
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }


    private void ExpandedGUI()
    {
        EditorGUILayout.LabelField("ID: ", idProp.intValue.ToString());
        EditorGUILayout.PropertyField(strategyNameProp);
        EditorGUILayout.PropertyField(gameObjectProp);

        EditorGUILayout.Space();

        float space = EditorGUIUtility.currentViewWidth / 3f;
        
        for (int i = 0; i < subEditors.Length; i++)
        {
            subEditors[i].OnInspectorGUI();
        }
    }

    public void AddMatchUp(MatchUpStats newMatchUp)
    {
        matchUpsProp.AddToObjectArray(newMatchUp);
    }
    
    public static StrategySO CreateStrategy(SerializedProperty existingStrategies)
    {
        StrategySO newStrat = CreateInstance<StrategySO>();
        newStrat.id = existingStrategies.arraySize + 1;
        newStrat.strategyName = "New Strategy";
        newStrat.matchUps = new MatchUpStats[0];
        return newStrat;
    }
}
