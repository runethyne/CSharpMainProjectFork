using Model;
using Model.Runtime;
using Model.Runtime.ReadOnly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utilities;
using static UnityEngine.UI.CanvasScaler;
using Unit = Model.Runtime.Unit;

namespace Assets.Scripts.Utilities
{
    public class PlayerUnitsTargetManager
    {
        static PlayerUnitsTargetManager instance;

        private IReadOnlyRuntimeModel _runtimeModel;
        private TimeUtil _timeUtil;
        public IReadOnlyUnit recomendTarget = null;

        public PlayerUnitsTargetManager()
        {
            _runtimeModel = ServiceLocator.Get<IReadOnlyRuntimeModel>();
            _timeUtil = ServiceLocator.Get<TimeUtil>();

            _timeUtil.AddUpdateAction(updateTarget);
        }

        public static PlayerUnitsTargetManager getInstance()
        {
            if (instance == null)
                instance = new PlayerUnitsTargetManager();
            return instance;
        }

        private void updateTarget(float deltaTime)
        {
            //eto vserovno ni kto ne prochtet, potomu napishu zdes'.... IA GEY (SHUTKA) ((NET)) (((AZAZAZAZAZ LALKA)))
            //vsegda hotel napisat' memuari v kode translitom, nu i huli vi mne sdelaete?
            //ladno... pora delat' zadachu....

            /* Рекомендуемая цель: если на нашей половине карты есть враги, то юнитам
                 рекомендуется атаковать ближайшего к нашей базе. В противном случае 
                 целью становится враг с наименьшим количеством здоровья.

             Рекомендуемая точка: если на нашей половине карты есть враги, то рекомендуемая точка
                 устанавливается перед базой. Иначе, рекомендуемая точка находится на 
                 расстоянии выстрела от ближайшего к базе врага.

             Рекомендуемая точка - по сути это путь к таргету, который обнволяется постоянно каждый update в DefaultPlayerUnitBrain.GetNextStep
                Там происходит поиск пути либо до врага, если он есть, либо до базы противника если все мертвы. Приоритетная цель всегда выбирается если есть. 
                Если потребуется - юниты повернут назад что бы защитить базу
            */
            recomendTarget = null;

            //Ищем на нашей половине карты ближайшего врага к нашей базе
            int firstHalfMapX = _runtimeModel.RoMap.Width / 2;
            float closedDist = float.MaxValue;
            List<IReadOnlyUnit> Targets = _runtimeModel.RoBotUnits.ToList();

            foreach (IReadOnlyUnit target in Targets)
            {
                if (target.Pos.x < firstHalfMapX && closedDist > (target.Pos - _runtimeModel.RoMap.Bases[RuntimeModel.PlayerId]).magnitude)
                {
                    closedDist = (target.Pos - _runtimeModel.RoMap.Bases[RuntimeModel.PlayerId]).magnitude;
                    recomendTarget = target;
                }
                    
            }
            //если найден - завершаем поиски
            if (recomendTarget != null) { 
                return; 
            }

            //ищем врага с минимальным HP
            int minHP = int.MaxValue;
            foreach (IReadOnlyUnit target in Targets)
            {
                if (minHP > target.Health)
                {
                    minHP = target.Health;
                    recomendTarget = target;
                }
            }

            

            //ps: ah da, spisivai, mne ne zhalko

            
        }

    }
}
