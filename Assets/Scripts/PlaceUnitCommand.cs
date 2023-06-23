using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceUnitCommand : Command
{
    private UnitPlacement unitPlacement;
    private GameObject unitPrefab;

    public PlaceUnitCommand(UnitPlacement unitPlacement, GameObject unitPrefab)
    {
        this.unitPlacement = unitPlacement;
        this.unitPrefab = unitPrefab;
    }

    public override void Execute()
    {
        unitPlacement.PlaceUnit(unitPrefab);
    }
}
