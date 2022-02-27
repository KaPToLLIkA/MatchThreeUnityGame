using UnityEngine;

namespace Project.Scripts.Game
{
    public class CellSelector
    {
        private ItemController _firstSelected = null;
        private ItemController _secondSelected = null;

        public bool CanSelect { get; set; } = true;

        public void Select(ItemController selectable)
        {
            if (!CanSelect) return;
            if (_firstSelected is null)
            {
                _firstSelected = selectable;
                _firstSelected.Select();
                return;
            }

            _secondSelected = selectable;

            var dx = Mathf.Abs(_firstSelected.Coords.x - _secondSelected.Coords.x);
            var dy = Mathf.Abs(_firstSelected.Coords.y - _secondSelected.Coords.y);

            try
            {
                _firstSelected.Deselect();
            }
            catch (UnityException e)
            {
                // suppress exception
            }
            
            if (dx == 1 && dy == 0 || dx == 0 && dy == 1)
            {
                _firstSelected.Swap(_secondSelected);
                _firstSelected = null;
                _secondSelected = null;
            }
            else
            {
                _firstSelected = _secondSelected;
                _secondSelected = null;
                _firstSelected.Select();
            }
        }
    }
}