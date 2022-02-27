using System;
using UnityEngine;

namespace Project.Scripts.Game.Git
{
    [Serializable]
    public class GitLastState
    {
        [SerializeField] private CellType[,] _items;
        [SerializeField] private int _score;

        public CellType this[int x, int y] => _items[y, x];

        public int Score => _score;

        public CellType[,] Cells => _items;

        public GitLastState(Field field)
        {
            _items = new CellType[field.Rows, field.Columns];
            _score = PlayerPrefs.GetInt("player_score", 0);
            
            for (int y = 0; y < field.Rows; ++y)
            {
                for (int x = 0; x < field.Columns; ++x)
                {
                    if (field[x, y] == null)
                    {
                        _items[y, x] = CellType.Spawner;
                    }
                    else
                    {
                        _items[y, x] = field[x, y].Type;
                    }
                }
            }
        }
    }
}