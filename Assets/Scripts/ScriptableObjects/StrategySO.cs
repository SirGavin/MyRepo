using System;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class StrategySO : ScriptableObject
{
    public int id;
    public string strategyName;
    public GameObject gameObject;
    public MatchUpStats[] matchUps;

    public MatchUpStats GetMatchUp(int otherId)
    {
        for (int i = 0; i < matchUps.Length; i++)
        {
            if (matchUps[i].defenderStratId == otherId)
            {
                return matchUps[i];
            }
        }

        Debug.LogError("Match Up not found");
        return null;
    }
}
