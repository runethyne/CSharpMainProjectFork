﻿using System;
using System.Collections.Generic;
using System.Linq;
using Model;
using Model.Runtime.Projectiles;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities;

namespace UnitBrains.Player
{
    public class SecondUnitBrain : DefaultPlayerUnitBrain
    {
        public override string TargetUnitName => "Cobra Commando";
        private const float OverheatTemperature = 3f;
        private const float OverheatCooldown = 2f;
        private float _temperature = 0f;
        private float _cooldownTime = 0f;
        private bool _overheated;
        private List<Vector2Int> _currentTargets = new List<Vector2Int>();

        protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
        {
            float overheatTemperature = OverheatTemperature;
            float currentTemperature = GetTemperature();
            float numProjectiles = currentTemperature;

            if (currentTemperature >= overheatTemperature)
            {
                _overheated = true;
                return;
            }
            IncreaseTemperature();
            for (int i = 0; i < numProjectiles; i++)
            {
                var projectile = CreateProjectile(forTarget);
                AddProjectileToList(projectile, intoList);
            }
        }
        public override Vector2Int GetNextStep()
        {
            if (_currentTargets.Count > 0 ) 
            {
                if (IsTargetInRange(_currentTargets[0]))
                {
                    return unit.Pos;
                }
                else
                {
                    return unit.Pos.CalcNextStepTowards(_currentTargets[0]);
                }
            }
            else
                {
                return unit.Pos;
            }
        }

        protected override List<Vector2Int> SelectTargets()
        {
            List<Vector2Int> result = new List<Vector2Int>();
            float closestDistance = float.MaxValue;
            Vector2Int closestTarget = Vector2Int.zero;
            foreach (Vector2Int target in GetAllTargets())
            {
                float distance  = DistanceToOwnBase(target);
                if (distance < closestDistance)
                {
                    closestDistance = distance; 
                    closestTarget = target;
                }
            }

            _currentTargets.Clear();
            if (closestDistance < float.MaxValue)
            {
                _currentTargets.Add(closestTarget);
                if (IsTargetInRange(closestTarget))
                {
                    result.Add(closestTarget);
                }
            }
            else
            {
                if (IsPlayerUnitBrain)
                {
                    _currentTargets.Add(runtimeModel.RoMap.Bases[RuntimeModel.BotPlayerId]);
                }
                else
                {
                    _currentTargets.Add(runtimeModel.RoMap.Bases[RuntimeModel.PlayerId]);
                }
            }

            return result;
        }

        public override void Update(float deltaTime, float time)
        {
            if (_overheated)
            {
                _cooldownTime += Time.deltaTime;
                float t = _cooldownTime / (OverheatCooldown / 10);
                _temperature = Mathf.Lerp(OverheatTemperature, 0, t);
                if (t >= 1)
                {
                    _cooldownTime = 0;
                    _overheated = false;
                }
            }
        }

        private int GetTemperature()
        {
            if (_overheated) return (int)OverheatTemperature;
            else return (int)_temperature;
        }

        private void IncreaseTemperature()
        {
            _temperature += 1f;
            if (_temperature >= OverheatTemperature) _overheated = true;
        }
    }
}