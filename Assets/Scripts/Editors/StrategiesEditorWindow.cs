using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class StrategiesEditorWindow : EditorWindow {

    [Serializable]
    public class Strategies {
        public Strategy[] strategies;
    }
    public Strategies strategies;

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
            //strategies = JsonUtility.FromJson<Strategy[]>(dataAsJson);
        }
        else
        {
           // strategies = new Strategy[1];
        }
    }

    private void SaveGameData()
    {
        try {
            Debug.Log("strategies: " + strategies.strategies.Length);
            Debug.Log("strategies: " + strategies.strategies[0].strategyName);
            string dataAsJson = JsonUtility.ToJson(strategies);
            string filePath = Application.dataPath + strategiesProjectFilePath;
            Debug.Log("filePath: " + filePath);
            Debug.Log("dataAsJson: " + dataAsJson);
            File.WriteAllText(filePath, dataAsJson);
        } catch (Exception ex) {
            Debug.Log("ex: " + ex);
        }
    }
}
