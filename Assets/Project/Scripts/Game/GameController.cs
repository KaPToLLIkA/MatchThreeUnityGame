using System;
using UnityEngine;

namespace Project.Scripts.Game
{
    public class GameController : MonoBehaviour
    {
        [Header("Field generator settings")]
        public Vector2 cellSize = new Vector2(0.5f, 0.5f);
        [Range(0.01f, 1f)]
        public float cellsPadding = 0.1f;
        public int rows = 12;
        public int columns = 10;

        private Field _field;
        private Vector2 _fieldSize;

        private void Awake()
        {
            var cells = new FieldGenerator().Generate(rows, columns);
            _fieldSize = new Vector2(
                columns * (cellSize.x + cellsPadding) + cellsPadding,
                rows * (cellSize.y + cellsPadding) + cellsPadding);
            _field = new Field(cells, _fieldSize, cellSize, cellsPadding);
        }

        private void Start()
        {
            throw new NotImplementedException();
        }
    }
}