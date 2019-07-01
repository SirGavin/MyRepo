using System;

[Serializable]
public class MatchUpData {

    public MatchUpStats[] matchUpStats;

    public MatchUpStats GetMatchUp(Strategy attacker, Strategy defender) {
        foreach (MatchUpStats matchUp in matchUpStats) {
            if (matchUp.attackerStratId == attacker.strategyId && matchUp.defenderStratId == defender.strategyId) {
                return matchUp;
            }

            //Currently all match ups are attacker/defender symmetrical
            if (matchUp.attackerStratId == defender.strategyId && matchUp.defenderStratId == attacker.strategyId) {
                return SwitchSides(matchUp);
            }
        }

        throw new Exception("Match up not configured!");
    }

    private MatchUpStats SwitchSides(MatchUpStats matchUp) {
        MatchUpStats switchedMatchUp = new MatchUpStats();
        switchedMatchUp.attackerDmgModifier = matchUp.defenderDmgModifier;
        switchedMatchUp.defenderDmgModifier = matchUp.attackerDmgModifier;
        return switchedMatchUp;
    }
}
