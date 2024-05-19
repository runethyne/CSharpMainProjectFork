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
            List<Tile> ReachableTiles = new List<Tile> { new Tile(startPoint, endPoint, null) }; //��������� ������. �� ������ ��� �������� ������ ��������� �����
            List<Vector2Int> ReachedleCoords = new List<Vector2Int> { startPoint }; //�������� ����������
             
            Tile endTile = null;


            //���� � ��� ���� ������ ��� ������������ - ���������� ��������� ������ ���������)
            while (ReachableTiles.Count > 0)
            {
                //������ ��������� �� �������� ������ 
                Tile closedTile = getClosedTile(ReachableTiles);

                ReachableTiles.Remove(closedTile); //������ �� ��������� ������ closedTile

                //������� � ������ �������� ���������. ������������ ���� ��� �� �������� ���������� �������� ������
                ReachedleCoords.Add(closedTile.coord);

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

                    if (runtimeModel.TileIsNotWall(newCoord) && !ReachedleCoords.Contains(newCoord))
                    {
                        ReachableTiles.Add(new Tile(newCoord, endPoint, closedTile));
                        ReachedleCoords.Add(newCoord);
                    }

                }

            }

            //�� ��������� ����� ����� ��� ��������. ���� endTile � ��� null (������ ���� �� ������) ���� � �� �������� Tile � ���������� endPoint
            //���� null �� ������ ������ ������� ������ � ����� �� �����.. 
            if (endTile == null)
            {
                path = new Vector2Int[] { startPoint };
                return; //��������� ����� �� ��������� ���������, ����� ���� �������� ����������
            }

            //� endTile �� .prevTile ����� ��������� ���� �� �������� �� ��������� �����
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
