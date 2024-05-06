using Model;
using Model.Runtime.Projectiles;
using System.Collections;
using System.Collections.Generic;
using UnitBrains.Pathfinding;
using UnitBrains.Player;
using UnityEngine;
using Utilities;

public class ThirdUnitBrain : DefaultPlayerUnitBrain
{
    private bool modeMoving = true;
    private float chageModeTimer;
    public override string TargetUnitName => "Ironclad Behemoth";

    protected override void GenerateProjectiles(Vector2Int forTarget, List<BaseProjectile> intoList)
    {
        //���� ������ ����� ��������, �� �� ��������, ��������� �������� � �������� ������ ������������
        if (modeMoving)
        {
            modeMoving = false;
            chageModeTimer = 1f;
            return;
        }
        else if(chageModeTimer > 0) //���� ������ �� ������� - �� �����������, ����� ����� ��������
        {
            chageModeTimer -= Time.deltaTime;
        }
        else
        {
            base.GenerateProjectiles(forTarget, intoList);
        }
    }
    public override Vector2Int GetNextStep()
    {
        if (HasTargetsInRange()) //���� ���� � ���� �������� - �� ������������ ������� �����
        {
            return base.GetNextStep();
        }
        else //�����, ���� ��������� � ������ �������� �� ������ ���� �� ���� � ������ �����
        {
            if(!modeMoving) { //���� �������� ���� ��������� - �� ����� �� ����� 1000 ��
                modeMoving =true;
                chageModeTimer = 1f;
                return unit.Pos;
            }
            else if (chageModeTimer > 0) //���� 1000 ��
            {
                chageModeTimer -= Time.deltaTime;
                return unit.Pos;
            }
            else //���� ��� �������� ������� � ������ ������������ = 0 - �� ����� �����
            {
                return base.GetNextStep();
            }

        }

    }

  

}
