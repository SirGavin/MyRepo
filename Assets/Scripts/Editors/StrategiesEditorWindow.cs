using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class StrategiesEditorWindow : EditorWindow {
    //TODO: review strat setup process
    
    public MatchUpData strategies;

    private string strategiesProjectFilePath = "/StreamingAssets/strategies.json";

    [MenuItem("Window/Strategies Editor")]
    static void Init()
    {
        StrategiesEditorWindow window = (StrategiesEditorWindow)EditorWindow.GetWindow(typeof(StrategiesEditorWindow));
        window.Show();
    }

    private void OnGUI()
    {
        if (strategies != null)
        {
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty serializedProperty = serializedObject.FindProperty("strategies");

            EditorGUILayout.PropertyField(serializedProperty, true);

            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Save Data"))
            {
                SaveGameData();
            }
        }

        if (GUILayout.Button("Load Data"))
        {
            LoadGameData();
        }
    }

    private void LoadGameData()
    {
        string filePath = Application.dataPath + strategiesProjectFilePath;

        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            strategies = JsonUtility.FromJson<MatchUpData>(dataAsJson);
        }
        else
        {
            strategies = new MatchUpData();
        }
    }

    private void SaveGameData()
    {
        try {
            string dataAsJson = JsonUtility.ToJson(strategies);
            string filePath = Application.dataPath + strategiesProjectFilePath;
            File.WriteAllText(filePath, dataAsJson);
        } catch (Exception ex) {
            Debug.Log("ex: " + ex);
        }
    }
}
