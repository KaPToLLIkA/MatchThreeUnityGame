using UnityEngine;

namespace Project.Scripts.Game
{
    public class SpawnerController : MonoBehaviour, IFieldItem
    {
        public GameObject item;
        
        private Field _field;
        private int _x;
        private int _y;
        private CellSelector _selector;

        public void Init(int x, int y, Field field, CellSelector selector)
        {
            _x = x;
            _y = y;
            _field = field;
            _selector = selector;
        }

        private void SpawnAt(int x, int y)
        {
            var pos = _field.GetCellWorldPos(x, y);
            var itemGO = 
                Instantiate(item, pos, Quaternion.identity, _field.FieldGameObject.transform);
            _field[x, y] = itemGO.GetComponent<ItemController>();
            _field[x, y].Init(x, y, _field, Field.GetRandomType(), _selector);
        }
        
        private void Update()
        {
            if (_field == null) return;

            if (_y - 1 < _field.Rows && _field[_x, _y - 1] == null)
            {
                SpawnAt(_x, _y - 1);
            }
        }
    }
}