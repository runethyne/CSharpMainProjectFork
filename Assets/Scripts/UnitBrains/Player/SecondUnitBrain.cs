using System;
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
            List<Vector2Int> Targets = SelectTargets();
            List<Vector2Int> ReachableTargets = GetReachableTargets();
            if (Targets.Count > 0 ) 
            {
                if (ReachableTargets.Contains(Targets[0]))
                {
                    Vector2Int position = unit.Pos;
                    return position;
                }
                else
                {
                    Vector2Int position = unit.Pos;
                    Vector2Int nextPosition = Targets[0];
                    return position.CalcNextStepTowards(nextPosition);
                }
            }
            else
                {
                    Vector2Int position = unit.Pos;
                    return position;
                }
        }

        protected override List<Vector2Int> SelectTargets()
        {
            IEnumerable<Vector2Int> AllTargets = GetAllTargets();
            List<Vector2Int> ReachableTargets = GetReachableTargets();
            List<Vector2Int> UnreachableTargets = new List<Vector2Int>();
            List<Vector2Int> result = new List<Vector2Int>();

            if (AllTargets.Count() == 0)
            {
                if (IsPlayerUnitBrain)
                {
                    result.Add(runtimeModel.RoMap.Bases[RuntimeModel.BotPlayerId]);
                }
                else
                {
                    result.Add(runtimeModel.RoMap.Bases[RuntimeModel.PlayerId]);
                }
            }
            else
            {
                float closestDistance = float.MaxValue;

                Vector2Int closestTarget = AllTargets.ElementAt(0);
                foreach (Vector2Int target in AllTargets)
                {
                    float distance = DistanceToOwnBase(target);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestTarget = target;
                    }
                }

                if (ReachableTargets.Contains(closestTarget))
                {
                    result.Add(closestTarget);
                }
                else
                {
                    UnreachableTargets.Add(closestTarget);
                }

                //result.Clear();
                result.Add(closestTarget);
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