using System;
using UnityEngine;

namespace Project.Scripts.Game
{
    public class GameController : MonoBehaviour
    {
        [Header("Field generator settings")]
        public float cellSize = 0.5f;
        [Range(0.01f, 1f)]
        public float cellsPadding = 0.1f;
        public int rows = 12;
        public int columns = 10;
        
        [Header("Field objects")] 
        public GameObject itemPrefab;
        public GameObject spawnerPrefab;
        public GameObject fieldGameObject;
        
        [Header("Game settings")]
        public int lineLength = 3;
        
        private Field _field;
        private Vector2 _fieldSize;

        private void Awake()
        {
            var cells = new FieldGenerator().Generate(rows, columns);
            _fieldSize = new Vector2(
                columns * (cellSize + cellsPadding) + cellsPadding,
                rows * (cellSize + cellsPadding) + cellsPadding);
            _field = new Field(cells, _fieldSize, cellSize, cellsPadding, fieldGameObject);
            _field.InstantiateItems(itemPrefab, spawnerPrefab);
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            if (_field.ContainsAnyEmpty()) return;
            var figures = _field.GetAllFigures(lineLength);
            
            if (figures.Count == 0) return;
            foreach (var figure in figures)
            {
                foreach (var coord in figure.Coords)
                {
                    _field[coord].Explode();
                }
            }
        }
    }
}