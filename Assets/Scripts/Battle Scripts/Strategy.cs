using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Strategy {

    public int strategyId;
    public string strategyName;
    public Sprite strategyImg;
    //public MatchUpStats[] matchUps;

    [Serializable]
    public class MatchUps {
        public MatchUpStats matchUp;
    }
    public MatchUps[] matchUps;

    public MatchUpStats GetMatchUp(int otherId) {
        for (int i = 0; i < matchUps.Length; i++) {
            //if (matchUps[i].defenderStratId == otherId) {
            //    return matchUps[i];
            //}
        }

        Debug.LogError("Match Up not found");
        return null;
    }
}
