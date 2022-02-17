using UnityEngine;

namespace Project.Scripts.Game
{
    public enum CellType
    {
        Spawner,
        Empty,
        Red,
        Blue,
        Yellow,
        Green,
        Magenta
    }

    public class Field
    {
        public static CellType[] ItemsTypes { get; } =
            {CellType.Blue, CellType.Green, CellType.Magenta, CellType.Red, CellType.Yellow};

        private CellType[,] cells;

        public int Rows => cells.GetLength(0);
        public int Columns => cells.GetLength(1);
        
        public Field(CellType[,] cells, Vector2 fieldSize, Vector2 cellSize, float padding)
        {
            this.cells = cells;
        }

        public CellType this[int x, int y]
        {
            get => cells[y, x];
            set => cells[y, x] = value;
        }
    }
}