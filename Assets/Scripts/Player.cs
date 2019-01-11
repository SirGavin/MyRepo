using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player {

    private List<ArmyMap> armies { get; set; }

    public void AddArmy(ArmyMap newArmy) {
        if (armies == null) armies = new List<ArmyMap>();

        armies.Add(newArmy);
    }
}
