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

    private Dictionary<Unit, List<AbstractEffect>> UnitsEffectStatus = new Dictionary<Unit, List<AbstractEffect>>();
   
    public EffectsManager()
    {
        ServiceLocator.Get<TimeUtil>().AddFixedUpdateAction(updateEffects);
    }

    internal void addEffect(Unit unit, AbstractEffect effect)
    {
        List<AbstractEffect> unitsEffect;

        if (UnitsEffectStatus.ContainsKey(unit))
        {
            unitsEffect = UnitsEffectStatus[unit];
        }
        else
        {
            unitsEffect = new List<AbstractEffect>();
            UnitsEffectStatus.Add(unit, unitsEffect);
        }

        unitsEffect.Add(effect);

        Debug.Log(unit + " Стартовал эффект " + effect);
    }

    internal void RemoveEffect(AbstractEffect effect, Unit unit)
    {

        if (UnitsEffectStatus.ContainsKey(unit))
        {
            List<AbstractEffect> unitsEffect = UnitsEffectStatus[unit];
            unitsEffect.Remove(effect);
            Debug.Log(unit + " Удален эффект " + effect);
        }

    }

    public void updateEffects(float deltaTime)
    {
        foreach (KeyValuePair<Unit, List<AbstractEffect>> pare in UnitsEffectStatus)
        {
            foreach (AbstractEffect effect in pare.Value.ToArray())
            {
                effect.duration -= deltaTime;
                if (effect.duration <= 0)
                    RemoveEffect(effect, pare.Key);
                    
            }
        }
    }

    internal float getAttackDelayMod(Unit unit)
    {
        float mod = 1f;

        List<AbstractEffect> effectsList;
        if (UnitsEffectStatus.TryGetValue(unit, out effectsList))
        {
            foreach (AbstractEffect effect in effectsList)
            {
                mod *= effect.AttackDelayMod;
            }
        }

        return mod;


    }

}
