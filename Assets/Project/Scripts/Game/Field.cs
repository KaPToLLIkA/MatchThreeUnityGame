using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.Scripts.Game
{
    public enum CellType
    {
        Spawner,
        Red,
        Blue,
        Yellow,
        Green,
        Magenta
    }

    public class Figure
    {
        public List<Vector2Int> Coords { get; } = new List<Vector2Int>();

        public Vector2Int this[int i]
        {
            get => Coords[i];
            set => Coords[i] = value;
        }

        public Figure(List<Vector2Int> coords)
        {
            Coords = coords;
        }
    }
    
    public class Field
    {
        public static CellType[] ItemsTypes { get; } =
            {CellType.Blue, CellType.Green, CellType.Magenta, CellType.Red, CellType.Yellow};

        public static CellType GetRandomType()
        {
            return ItemsTypes[Random.Range(0, ItemsTypes.Length)];
        }

        public int Rows => cells.GetLength(0);
        public int Columns => cells.GetLength(1);
        public float CellSize => worldCellSize;
        public GameObject FieldGameObject => fieldGameObject;
        
        // static in game objects
        private CellType[,] cells;
        // dynamic in game objects
        private ItemController[,] items;

        private Vector2 worldFieldSize;
        private float worldCellSize;
        private float worldCellsPadding;

        private GameObject fieldGameObject;
        
        public Field(CellType[,] cells, Vector2 fieldSize, float cellSize, float padding, GameObject field)
        {
            this.cells = cells;
            this.worldCellSize = cellSize;
            this.worldFieldSize = fieldSize;
            this.worldCellsPadding = padding;
            this.fieldGameObject = field;
        }

        public ItemController this[int x, int y]
        {
            get => items[y, x];
            set => items[y, x] = value;
        }

        public ItemController this[Vector2Int coord]
        {
            get => items[coord.y, coord.x];
            set => items[coord.y, coord.x] = value;
        }

        public Vector2 GetCellWorldPos(int x, int y)
        {
            return new Vector2(
                -worldFieldSize.x / 2 + worldCellsPadding + x * (worldCellSize + worldCellsPadding) + worldCellSize / 2,
                -worldFieldSize.y / 2 + worldCellsPadding + y * (worldCellSize + worldCellsPadding) + worldCellSize / 2
            );
        }


        public bool ContainsAnyMovable()
        {
            for (int y = 0; y < Rows; ++y)
            {
                for (int x = 0; x < Columns; ++x)
                {
                    if (items[y, x] != null && items[y, x].Movable)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool ContainsAnyEmpty()
        {
            for (int y = 0; y < Rows; ++y)
            {
                for (int x = 0; x < Columns; ++x)
                {
                    if (cells[y, x] != CellType.Spawner && items[y, x] == null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void InstantiateItems(GameObject item, GameObject spawner)
        {
            items = new ItemController[Rows, Columns];
            
            for (int y = 0; y < Rows; ++y)
            {
                for (int x = 0; x < Columns; ++x)
                {
                    var pos = GetCellWorldPos(x, y);
                    if (cells[y, x] == CellType.Spawner)
                    {
                        var spawnerGO = 
                            Object.Instantiate(spawner, pos, Quaternion.identity, fieldGameObject.transform);
                        spawnerGO.GetComponent<SpawnerController>().Init(x, y, this);
                        items[y, x] = null;
                    }
                    else
                    {
                        var itemGO = 
                            Object.Instantiate(item, pos, Quaternion.identity, fieldGameObject.transform);
                        items[y, x] = itemGO.GetComponent<ItemController>();
                        items[y, x].Init(x, y, this, cells[y, x]);
                    }
                }
            }
        }

        public Figure GetAnyFigureAt(int x, int y, int length)
        {
            var coords = new List<Vector2Int>();
            var visited = new bool[Rows, Columns];
            LineFigureSearch(x, y, length, coords, visited);
            return new Figure(coords);
        }
        
        public List<Figure> GetAllFigures(int minLength)
        {
            var figures = new List<Figure>();
            var isFigure = new bool[Rows, Columns];
            
            for (int y = 0; y < Rows; ++y)
            {
                var visited = new bool[Rows, Columns];
                
                for (int x = 0; x < Columns; ++x)
                {
                    var coords = new List<Vector2Int>();
                    if (items[y, x] != null && !isFigure[y, x])
                    {
                        LineFigureSearch(x, y, minLength, coords, visited);
                        if (coords.Count >= minLength)
                        {
                            figures.Add(new Figure(coords));
                            coords.ForEach(c => isFigure[c.y, c.x] = true);
                        }
                    }
                }
            }

            return figures;
        }

        private bool CheckBounds(ref Vector2Int coord)
        {
            return coord.x < Columns && coord.x >= 0 && coord.y < Rows && coord.y >= 0;
        }
        
        private void LineFigureSearch(
            int x, int y,
            int lenght,
            List<Vector2Int> coords,
            bool[,] visited)
        {
            CellType target = items[y, x].Type;
            
            for (int v = 0; v < lenght; ++v)
            {
                var newCoords = new List<Vector2Int>();
                for (int ix = 0; ix < lenght; ++ix)
                {
                    var newX = x + ix - v;
                    if (newX >= 0 && newX < Columns && items[y, newX] != null && items[y, newX].Type == target)
                    {
                        newCoords.Add(new Vector2Int(newX, y));
                    }
                }

                if (newCoords.Count >= lenght)
                {
                    coords.AddRange(newCoords);
                    break;
                }
            }
            
            for (int v = 0; v < lenght; ++v)
            {
                var newCoords = new List<Vector2Int>();
                for (int iy = 0; iy < lenght; ++iy)
                {
                    var newY = y + iy - v;
                    if (newY >= 0 && newY < Rows && items[newY, x] != null && items[newY, x].Type == target)
                    {
                        newCoords.Add(new Vector2Int(x, newY));
                    }
                }

                if (newCoords.Count >= lenght)
                {
                    coords.AddRange(newCoords);
                    break;
                }
            }

            var neighbours = new HashSet<Vector2Int>();

            foreach (var coord in coords)
            {
                visited[coord.y, coord.x] = true;
                var points = new []
                {
                    new Vector2Int(coord.x - 1, coord.y),
                    new Vector2Int(coord.x + 1, coord.y),
                    new Vector2Int(coord.x, coord.y + 1),
                    new Vector2Int(coord.x, coord.y - 1)
                };
                for (int i = 0; i < points.Length; ++i)
                {
                    if (CheckBounds(ref points[i]) 
                        && !visited[points[i].y, points[i].x]
                        && this[points[i]] != null
                        && this[points[i]].Type == target)
                    {
                        neighbours.Add(points[i]);
                    }
                }
            }
            
            FindAllNeighbours(target, coords, neighbours, visited);

            coords = (new HashSet<Vector2Int>(coords.ToList())).ToList();
        }

        private void FindAllNeighbours(
            CellType target, 
            List<Vector2Int> coords, 
            HashSet<Vector2Int> neighbours, 
            bool[,] visited)
        {
            var queue = new Queue<Vector2Int>(neighbours.ToList());
            while (queue.Count > 0)
            {
                var coord = queue.Dequeue();
                
                coords.Add(coord);
                visited[coord.y, coord.x] = true;
                
                var points = new []
                {
                    new Vector2Int(coord.x - 1, coord.y),
                    new Vector2Int(coord.x + 1, coord.y),
                    new Vector2Int(coord.x, coord.y + 1),
                    new Vector2Int(coord.x, coord.y - 1)
                };
                
                for (int i = 0; i < points.Length; ++i)
                {
                    if (CheckBounds(ref points[i]) 
                        && !visited[points[i].y, points[i].x]
                        && this[points[i]] != null
                        && this[points[i]].Type == target)
                    {
                        queue.Enqueue(points[i]);
                    }
                }
            }
        }
    }
}