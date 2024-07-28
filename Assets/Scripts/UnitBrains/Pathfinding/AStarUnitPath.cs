using System.Collections.Generic;
using System.Linq;
using Model;
using UnityEngine;

namespace UnitBrains.Pathfinding
{
    public class AStarUnitPath : BaseUnitPath
    {
        private Vector2Int[] _directions = new Vector2Int[] {new Vector2Int(1,0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) };//����������� ��������

        public AStarUnitPath(IReadOnlyRuntimeModel runtimeModel, Vector2Int startPoint, Vector2Int endPoint) : base(runtimeModel, startPoint, endPoint)
        {
        }

        protected override void Calculate()
        {
            CalcPath(false);
            if (path == null)
                CalcPath(true);

            if (path == null)
            {
                path = new Vector2Int[] { startPoint, startPoint };
                return; //��������� ����� �� ��������� ���������, ����� ���� �������� ����������
            }
        }

        private void CalcPath(bool ignoreUnits)
        { 
            List<Tile> ReachableTiles = new List<Tile> { new Tile(startPoint, endPoint, null) }; //��������� ������. �� ������ ��� �������� ������ ��������� �����
            List<Tile> ReachedTiles = new List<Tile> {}; //�������� ����������
             
            Tile endTile = null;


            //���� � ��� ���� ������ ��� ������������ - ���������� ��������� ������ ���������)
            while (ReachableTiles.Count > 0)
            {
                //������ ��������� �� �������� ������ 
                Tile closedTile = getClosedTile(ReachableTiles);

                ReachableTiles.Remove(closedTile); //������ �� ��������� ������ closedTile

                //������� � ������ �������� ���������. ������������ ���� ��� �� �������� ���������� �������� ������
                ReachedTiles.Add(closedTile);

                //��������� ��� endTile � ������� �������� ����� � ReachableTiles
                //��������� ������ ������ �� ������������� � �����������, ����� �����������. (�� 4 ������������)
                foreach (Vector2Int dir in _directions) {
                    Vector2Int newCoord = closedTile.coord + dir;

                    if (newCoord == endPoint)  
                    {
                        endTile = new Tile(newCoord, newCoord, closedTile);
                        ReachableTiles.Clear(); 
                        break;
                    }
                    
                    bool exist = !
                        (
                       newCoord.x < 0 || newCoord.x >= runtimeModel.RoMap.Width ||
                       newCoord.y < 0 || newCoord.y >= runtimeModel.RoMap.Height
                       );
                    if (!exist)
                    {
                        continue; 
                    }

                    if (runtimeModel.RoMap[newCoord] || (runtimeModel.RoUnits.Any(u => u.Pos == newCoord) && !ignoreUnits))
                    {
                        continue;
                    }
                                        
                    //���� ������ �� ������� � ���������� �� ����� - �� ��������� � ������ ��������� � ��������
                    
                    if (!ReachableTiles.Any(pos => pos.coord.x == newCoord.x && pos.coord.y == newCoord.y)
                        && !ReachedTiles.Any(pos => pos.coord.x == newCoord.x && pos.coord.y == newCoord.y)) 
                        
                    {
                        ReachableTiles.Add(new Tile(newCoord, endPoint, closedTile));
                    }

                }

            }



            //� endTile �� .prevTile ����� ��������� ���� �� �������� �� ��������� �����
            if (endTile != null)
            {
                var result = new List<Vector2Int> { }; //��� ����
                while (endTile.prevTile != null)
                {
                    result.Add(endTile.coord);
                    endTile = endTile.prevTile;
                }
                result.Add(startPoint);
                result.Reverse();
                path = result.ToArray();

            }

        }

        private Tile getClosedTile(List<Tile> reachableTiles)
        {
            Tile ClosedTile = reachableTiles[0];
            foreach (Tile tile in reachableTiles)
            {
                if (tile.distToTarget < ClosedTile.distToTarget)
                    ClosedTile = tile;
            }

            return ClosedTile;
        }
    }
}
