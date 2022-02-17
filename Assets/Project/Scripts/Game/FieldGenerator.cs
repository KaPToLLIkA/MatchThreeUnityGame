﻿using UnityEngine;

namespace Project.Scripts.Game
{
    public class FieldGenerator
    {
        public CellType[,] Generate(int rows, int columns)
        {
            var field = new CellType[rows, columns];

            for (int y = 0; y < rows; ++y)
            {
                for (int x = 0; x < columns; ++x)
                {
                    if (y == 0)
                    {
                        field[x, y] = CellType.Spawner;
                    }
                    else
                    {
                        field[x, y] = Field.ItemsTypes[Random.Range(0, Field.ItemsTypes.Length)];
                    }
                }
            }

            return field;
        }
    }
}