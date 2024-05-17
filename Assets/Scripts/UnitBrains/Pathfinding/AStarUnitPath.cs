using System.Collections.Generic;
using System.Linq;
using Model;
using UnityEngine;

namespace UnitBrains.Pathfinding
{
    public class AStarUnitPath : BaseUnitPath
    {
        Vector2Int[] directions = new Vector2Int[] {new Vector2Int(1,0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) };//направлени€ движени€

        public AStarUnitPath(IReadOnlyRuntimeModel runtimeModel, Vector2Int startPoint, Vector2Int endPoint) : base(runtimeModel, startPoint, endPoint)
        {
        }

        protected override void Calculate()
        { 
            List<Tile> ReachableTiles = new List<Tile> { new Tile(startPoint, endPoint, null) }; //»звестные клетки. Ќа старте нам известна только стартова€ точка
            List<Vector2Int> ReachedleCoords = new List<Vector2Int> { startPoint }; //изученые координаты
             
            Tile endTile = null;


            //ѕока у нас есть клетки дл€ исследовани€ - бесконечно провер€ем список доступных)
            while (ReachableTiles.Count > 0)
            {
                //найдем ближайшую не изученую клетку 
                Tile closedTile = getClosedTile(ReachableTiles);

                ReachableTiles.Remove(closedTile); //уберем из известных клеток closedTile

                /*//≈сли этот тайл наша конечна€ точка - то установим endTile и выйдем из цикла
                if (closedTile.coord == endPoint) 
                {
                    endTile = new Tile(endPoint, endPoint, closedTile); 
                    break; 
                }*/
                //добавим в список изученых координат. ѕонадобитсь€ ниже что бы избежать повторного изучени€ тайлов
                ReachedleCoords.Add(closedTile.coord);

                //исследуем наш endTile и добавим соседние клеки в ReachableTiles
                //провер€ем каждую клетку на существование и дсотупность, перед добавлением. (ѕо 4 направлени€м)
                foreach (Vector2Int dir in directions) {
                    Vector2Int newCoord = closedTile.coord + dir;

                    if (newCoord == endPoint)
                    {
                        endTile = new Tile(newCoord, newCoord, closedTile);
                        ReachableTiles.Clear();
                        break;
                    }

                    if (runtimeModel.IsTileWalkable(newCoord) && !ReachedleCoords.Contains(newCoord))
                    {
                        ReachableTiles.Add(new Tile(newCoord, endPoint, closedTile));
                        ReachedleCoords.Add(newCoord);
                    }
                }

            }

            //по окончанию цикла имеем две ситуации. Ћибо endTile у нас null (значит путь Ќ≈ найден) либо в нЄм конечный Tile с координами endPoint
            //если null то просто вернем текущую клетку и стоим на месте.. 
            if (endTile == null)
            {
                path = new Vector2Int[] { startPoint };
                return; //попробуем снова на следующей прокрутке, может быть ситуаци€ помен€етс€
            }

            //¬ endTile по .prevTile можем выстроить путь от конечной до начальной точки
            var result = new List<Vector2Int> { }; //наш путь
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
