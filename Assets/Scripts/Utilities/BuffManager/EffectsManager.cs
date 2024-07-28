using Assets.Scripts.Utilities.BuffManager;
using Model;
using Model.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class EffectsManager 
{

     internal void addEffect(Unit unit, AbstractEffect effect)
    {
        switch (unit.Config.UnitType) 
        {
            case 2:
                unit.doubleshot = true;
                break;
            case 3:
                unit.attackRangeMod = 3;
                break;
            default:
                break;
        }

}
